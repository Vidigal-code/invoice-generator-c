using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using InvoiceGenerator.Api.Domain.Interfaces;
using InvoiceGenerator.Api.Infrastructure.Data.Context;
using InvoiceGenerator.Api.Infrastructure.Repositories;
using InvoiceGenerator.Api.API.Middlewares;
using InvoiceGenerator.Api.Application.Commands;
using InvoiceGenerator.Api.Application.Abstractions;
using InvoiceGenerator.Api.Application.Authorization;
using InvoiceGenerator.Api.Application.Services.Admin;
using InvoiceGenerator.Api.Application.Services.Auth;
using InvoiceGenerator.Api.Application.Services.Billets;
using InvoiceGenerator.Api.Application.Services.Contracts;
using InvoiceGenerator.Api.API.Services;
using InvoiceGenerator.Api.Application.Services.Billets.Barcode;
using InvoiceGenerator.Api.Domain.Authorization;
using Serilog;
using Serilog.Sinks.Elasticsearch;
using MassTransit;
using InvoiceGenerator.Api.Infrastructure.Configuration;
using InvoiceGenerator.Api.Application.Constants;
using InvoiceGenerator.Api.Application.Consumers;
using InvoiceGenerator.Api.Application.Debt;
using InvoiceGenerator.Api.Infrastructure.Redis;
using InvoiceGenerator.Api.Infrastructure.Security;
using InvoiceGenerator.Api.Infrastructure.Data;
using FluentValidation;
using System.Threading.RateLimiting;
using System.Reflection;
using MediatR;
using InvoiceGenerator.Api.API.OpenApi;
using Swashbuckle.AspNetCore.ReDoc;

var builder = WebApplication.CreateBuilder(args);


// Bind strongly-typed settings (Enterprise Options Pattern)
var appSettings = new AppSettings();
builder.Configuration.Bind(appSettings);
builder.Services.Configure<AppSettings>(builder.Configuration);
builder.Services.AddSingleton(appSettings); 


// 1. Serilog — mesmo ficheiro que AdminPanelController (Logging:FilePath). Elasticsearch opcional.
var logFilePath = appSettings.Logging.FilePath;
if (string.IsNullOrWhiteSpace(logFilePath))
    throw new InvalidOperationException("Logging:FilePath é obrigatório na configuração (Serilog + painel admin).");

var loggerConfiguration = new LoggerConfiguration()
    .Enrich.FromLogContext()
    .Enrich.WithProperty("Application", "InvoiceGenerator.Api")
    .WriteTo.Console()
    .WriteTo.File(
        path: logFilePath,
        rollingInterval: RollingInterval.Day,
        retainedFileCountLimit: 7,
        outputTemplate: "[{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} {CorrelationId} {Level:u3}] {Message:lj}{NewLine}{Exception}"
    );

if (!string.IsNullOrWhiteSpace(appSettings.ElasticSearch.Uri))
{
    loggerConfiguration.WriteTo.Elasticsearch(new ElasticsearchSinkOptions(new Uri(appSettings.ElasticSearch.Uri))
    {
        AutoRegisterTemplate = true,
        IndexFormat = "invoice-generator-logs-{0:yyyy.MM.dd}"
    });
}

Log.Logger = loggerConfiguration.CreateLogger();

builder.Host.UseSerilog();


// Banking Defense configuration from Typed AppSettings
var maxPayloadMb = appSettings.Security.MaxPayloadMb > 0 ? appSettings.Security.MaxPayloadMb : 10;
var maxPayloadBytes = maxPayloadMb * 1024 * 1024;
var requestTimeoutSecs = appSettings.Security.RequestTimeoutSeconds > 0 ? appSettings.Security.RequestTimeoutSeconds : 10;


// Maximum Banking API Defense: Restrict Payload Size at Edge
builder.WebHost.ConfigureKestrel(options =>
{
    options.Limits.MaxRequestBodySize = maxPayloadBytes; // Hard prevention of Heap Exhaustion
    options.Limits.RequestHeadersTimeout = TimeSpan.FromSeconds(requestTimeoutSecs); 
});


// Maximum Banking API Defense: Strict Request Timeouts
builder.Services.AddRequestTimeouts(options =>
{
    options.DefaultPolicy = new Microsoft.AspNetCore.Http.Timeouts.RequestTimeoutPolicy 
    { 
        Timeout = TimeSpan.FromSeconds(requestTimeoutSecs) 
    };
});


// Anti-DDOS — valores vêm de RateLimit em appsettings / ambiente
var permitLimit = appSettings.RateLimit.Permits > 0 ? appSettings.RateLimit.Permits : 100;
var queueLimit = appSettings.RateLimit.Queue > 0 ? appSettings.RateLimit.Queue : 2;
var windowMinutes = appSettings.RateLimit.WindowMinutes > 0 ? appSettings.RateLimit.WindowMinutes : 1;


// Anti-DDOS Rate Limiter
builder.Services.AddRateLimiter(options =>
{
    options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(httpContext =>
        RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: httpContext.Connection.RemoteIpAddress?.ToString() ?? httpContext.Request.Headers.Host.ToString(),
            factory: partition => new FixedWindowRateLimiterOptions
            {
                AutoReplenishment = true,
                PermitLimit = permitLimit, 
                QueueLimit = queueLimit,
                Window = TimeSpan.FromMinutes(windowMinutes)
            }));
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
});


// Resiliencia HTTP Externa (Polly) - Retry, Timeout e Fallback
builder.Services.AddHttpClient("BankExternalApi")
       .AddStandardResilienceHandler();


// Tratamento de Erro Padronizado
builder.Services.AddProblemDetails();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc(OpenApiDocumentNames.V1En, OpenApiTranslationData.GetApiInfo(OpenApiDocumentNames.V1En));
    c.SwaggerDoc(OpenApiDocumentNames.V1Br, OpenApiTranslationData.GetApiInfo(OpenApiDocumentNames.V1Br));
    c.DocInclusionPredicate((documentName, _) =>
        documentName is OpenApiDocumentNames.V1En or OpenApiDocumentNames.V1Br);

    c.CustomOperationIds(apiDesc =>
    {
        var controller = apiDesc.ActionDescriptor.RouteValues.TryGetValue("controller", out var cv) ? cv : "Api";
        var action = apiDesc.ActionDescriptor.RouteValues.TryGetValue("action", out var av) ? av : "Action";
        return $"{controller}_{action}";
    });

    c.AddSecurityDefinition(OpenApiCookieAuthOperationFilter.SecuritySchemeId, new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
        In = Microsoft.OpenApi.Models.ParameterLocation.Cookie,
        Name = "AuthToken",
        Description = "HttpOnly cookie set by POST /api/Auth/login (JWT; JWE per server config)."
    });

    c.OperationFilter<OpenApiCookieAuthOperationFilter>();
    c.DocumentFilter<OpenApiBilingualDocumentFilter>();

    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
        c.IncludeXmlComments(xmlPath);
});

builder.Services.AddControllers(options =>
       {
           options.Filters.Add<InvoiceGenerator.Api.API.Filters.ApiResponseFilter>();
       })
       .AddJsonOptions(options =>
       {
           // Serialize all Enums back and forth through Strings bridging the Web Protocol Interfaces precisely
           options.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
       });

if (builder.Environment.IsEnvironment(HostingEnvironmentNames.IntegrationTests))
{
    builder.Services.AddDbContext<AppDbContext>(options =>
        options
            .UseInMemoryDatabase("IntegrationTests")
            .ConfigureWarnings(w => w.Ignore(InMemoryEventId.TransactionIgnoredWarning)));
}
else
{
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
        ?? throw new InvalidOperationException("Fatal Error: DefaultConnection is missing from configurations. Halt!");

    builder.Services.AddDbContext<AppDbContext>(options =>
        options.UseSqlServer(connectionString, sqlOptions =>
        {
            sqlOptions.EnableRetryOnFailure(
                maxRetryCount: 3,
                maxRetryDelay: TimeSpan.FromSeconds(5),
                errorNumbersToAdd: null);
        }));
}

// Register Unit of Work and Repositories
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<InvoiceGenerator.Api.Domain.Interfaces.IAuditService, InvoiceGenerator.Api.Application.Services.AuditService>();

builder.Services.AddSingleton<IFilePayloadProtector, AesGcmFilePayloadProtector>();
builder.Services.AddTransient<InvoiceGenerator.Api.Infrastructure.External.Storage.S3FileStorageService>();

builder.Services.AddTransient<InvoiceGenerator.Api.Infrastructure.External.Storage.IFileStorageService>(sp =>
{
    var inner = sp.GetRequiredService<InvoiceGenerator.Api.Infrastructure.External.Storage.S3FileStorageService>();
    var protector = sp.GetRequiredService<IFilePayloadProtector>();
    return new InvoiceGenerator.Api.Infrastructure.External.Storage.EncryptedPayloadFileStorageService(inner, protector);
});

builder.Services.AddScoped<IDistributedLock, RedisDistributedLock>();

builder.Services.AddSingleton<IFebrabanBarcodeComposer, FebrabanBarcodeComposer>();
builder.Services.AddSingleton<IItf25BarcodeHtmlRenderer, Itf25BarcodeHtmlRenderer>();
builder.Services.AddScoped<IBilletStorageObjectKeyBuilder, BilletStorageObjectKeyBuilder>();
builder.Services.AddScoped<IBilletStoragePdfRetriever, BilletStoragePdfRetriever>();
builder.Services.AddScoped<IBilletHtmlFallbackRenderer, BilletHtmlFallbackRenderer>();
builder.Services.AddSingleton<IApplicationRoleNames, ApplicationRoleNames>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ICurrentUserAccessor, HttpCurrentUserAccessor>();
builder.Services.AddSingleton<IJwtTokenIssuer, JwtTokenIssuer>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IAdminPanelService, AdminPanelService>();
builder.Services.AddScoped<IContractAccessService, ContractAccessService>();
builder.Services.AddScoped<IContractService, ContractService>();
builder.Services.AddSingleton<IDebtCalculationStrategy, InvoiceGeneratorCDebtCalculationStrategy>();
builder.Services.AddSingleton<IDebtCalculationStrategyResolver, DebtCalculationStrategyResolver>();

// 2. Distributed Cache with Redis (Redis:ConnectionString na configuração)
var redisConnection = appSettings.Redis.ConnectionString;
if (string.IsNullOrWhiteSpace(redisConnection))
    throw new InvalidOperationException("Redis:ConnectionString é obrigatório na configuração.");
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = redisConnection;
    options.InstanceName = "InvoiceGenerator_";
});

builder.Services.AddSingleton<StackExchange.Redis.IConnectionMultiplexer>(sp => 
    StackExchange.Redis.ConnectionMultiplexer.Connect(redisConnection));

// 3. MassTransit / RabbitMQ (RabbitMQ:* na configuração)
var rabbitHost = appSettings.RabbitMQ.Host;
var rabbitUser = appSettings.RabbitMQ.Username;
var rabbitPass = appSettings.RabbitMQ.Password;
if (string.IsNullOrWhiteSpace(rabbitHost) || string.IsNullOrWhiteSpace(rabbitUser) || string.IsNullOrWhiteSpace(rabbitPass))
    throw new InvalidOperationException("RabbitMQ:Host, Username e Password são obrigatórios na configuração.");

builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<AgreementFormalizedConsumer>();
    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host(rabbitHost, "/", h => {
            h.Username(rabbitUser);
            h.Password(rabbitPass);
        });
        cfg.ReceiveEndpoint("agreement-formalized", e =>
        {
            e.ConfigureConsumer<AgreementFormalizedConsumer>(context);
        });
    });
});

// Register MediatR for CQRS
builder.Services.AddMediatR(cfg => {
    cfg.RegisterServicesFromAssembly(typeof(CreateAgreementCommand).Assembly);
    cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(InvoiceGenerator.Api.Infrastructure.Validation.ValidationBehavior<,>));
});

// Register FluentValidation Validators
builder.Services.AddValidatorsFromAssembly(typeof(CreateAgreementCommand).Assembly);

// JWT Authentication Configuration
var jwtSecretKey = appSettings.JwtSettings.Secret;
if (string.IsNullOrEmpty(jwtSecretKey)) throw new InvalidOperationException("Fatal Error: JwtSettings:Secret is totally absent from environment.");
var key = Encoding.ASCII.GetBytes(jwtSecretKey);

builder.Services.AddAuthentication(x =>
{
    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(x =>
{
    x.RequireHttpsMetadata = false; 
    x.SaveToken = true;
    x.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        TokenDecryptionKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(appSettings.JwtSettings.JweSecret)),
        ValidateIssuer = false,
        ValidateAudience = false,
        RoleClaimType = System.Security.Claims.ClaimTypes.Role
    };
    x.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            // Extract from Cookie
            if (context.Request.Cookies.ContainsKey("AuthToken"))
            {
                context.Token = context.Request.Cookies["AuthToken"];
            }
            return Task.CompletedTask;
        },
        OnChallenge = context =>
        {
            context.HandleResponse();
            context.Response.StatusCode = 401;
            context.Response.ContentType = "application/json";
            var result = System.Text.Json.JsonSerializer.Serialize(new InvoiceGenerator.Api.Application.DTOs.ApiResponse<object> 
                { Success = false, StatusCode = 401, Message = "Não autorizado (Token ausente ou inválido)" });
            return context.Response.WriteAsync(result);
        },
        OnForbidden = context =>
        {
            context.Response.StatusCode = 403;
            context.Response.ContentType = "application/json";
            var result = System.Text.Json.JsonSerializer.Serialize(new InvoiceGenerator.Api.Application.DTOs.ApiResponse<object> 
                { Success = false, StatusCode = 403, Message = "Acesso negado (Permissão insuficiente)" });
            return context.Response.WriteAsync(result);
        }
    };
});

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("FormalizeAgreements", p =>
        p.RequireRole(ApplicationRoles.Admin, ApplicationRoles.User));
});

var corsOriginsStr = appSettings.Security.CorsOrigins;
if (string.IsNullOrWhiteSpace(corsOriginsStr))
    throw new InvalidOperationException("Security:CorsOrigins é obrigatório (lista separada por vírgulas, " +
                                        "ex.: http://localhost:4200,http://localhost:8081).");
var corsOrigins = corsOriginsStr.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

// Configure CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins(corsOrigins)
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials(); // Required for HttpOnly Cookies
    });
});

var webApp = builder.Build();

// Custom Middlewares

// Catch Unmatched Routes natively (e.g. 404s) intercepting status codes globally
webApp.UseStatusCodePages(async statusCodeContext =>
{
    var response = statusCodeContext.HttpContext.Response;
    if (response.StatusCode == 404 && !response.HasStarted)
    {
        response.ContentType = "application/json";
        var result = System.Text.Json.JsonSerializer.Serialize(new InvoiceGenerator.Api.Application.DTOs.ApiResponse<object> 
        { 
            Success = false, 
            StatusCode = 404, 
            Message = "URL/Recurso não encontrado" 
        });
        await response.WriteAsync(result);
    }
});

// Anti-DDOS implementation
webApp.UseRateLimiter();
webApp.UseRequestTimeouts();

// Captura Exception Global com tratamento padronizado 
webApp.UseMiddleware<ExceptionMiddleware>();
webApp.UseMiddleware<SecurityHeadersMiddleware>();
webApp.UseMiddleware<DatadogDummyMiddleware>();
webApp.UseMiddleware<AuditLogMiddleware>();


if (webApp.Environment.IsDevelopment() || webApp.Environment.IsProduction())
{
    webApp.MapOpenApi();

    webApp.UseSwagger(c => c.RouteTemplate = "swagger/{documentName}/swagger.json");

    webApp.UseSwaggerUI(c =>
    {
        c.RoutePrefix = "docs/en/swagger";
        c.DocumentTitle = "Invoice Generator API — Swagger (English)";
        c.SwaggerEndpoint($"/swagger/{OpenApiDocumentNames.V1En}/swagger.json", "OpenAPI v1 (English)");
    });
    webApp.UseSwaggerUI(c =>
    {
        c.RoutePrefix = "docs/br/swagger";
        c.DocumentTitle = "Invoice Generator API — Swagger (pt-BR)";
        c.SwaggerEndpoint($"/swagger/{OpenApiDocumentNames.V1Br}/swagger.json", "OpenAPI v1 (Português Brasil)");
    });

    webApp.UseReDoc(c =>
    {
        c.RoutePrefix = "docs/en/redoc";
        c.DocumentTitle = "Invoice Generator API — ReDoc (English)";
        c.SpecUrl($"/swagger/{OpenApiDocumentNames.V1En}/swagger.json");
    });
    webApp.UseReDoc(c =>
    {
        c.RoutePrefix = "docs/br/redoc";
        c.DocumentTitle = "Invoice Generator API — ReDoc (pt-BR)";
        c.SpecUrl($"/swagger/{OpenApiDocumentNames.V1Br}/swagger.json");
    });

    webApp.MapGet("/docs", () => Results.Content(OpenApiDocsLandingPage.Html, "text/html; charset=utf-8"));
}

webApp.UseCors("AllowFrontend");

webApp.UseAuthentication();
webApp.UseAuthorization();

webApp.MapControllers();


using (var scope = webApp.Services.CreateScope())
{
    // Run EF Core schema creation — ensures all tables exist before seeding
    // (no migration files in this project; EnsureCreatedAsync creates schema from model)
    var dbContext = scope.ServiceProvider.GetRequiredService<InvoiceGenerator.Api.Infrastructure.Data.Context.AppDbContext>();
    await dbContext.Database.EnsureCreatedAsync();


    var uow = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
    var appSettingsSeed = scope.ServiceProvider.GetRequiredService<AppSettings>();

    var requiredRoles = new[] { ApplicationRoles.Admin, ApplicationRoles.User };
    foreach (var roleName in requiredRoles)
    {
        var existing = await uow.Roles.FindAsync(r => r.Name == roleName);
        if (!existing.Any())
        {
            await uow.Roles.AddAsync(new InvoiceGenerator.Api.Domain.Entities.Role
            {
                Name = roleName,
                Description = roleName == ApplicationRoles.Admin ? "Administrador do sistema" : "Usuário padrão"
            });
        }
    }
    await uow.CommitAsync();

    var adminUserList = await uow.Users.FindAsync(u => u.Email == appSettingsSeed.AdminSettings.Email);

    if (!adminUserList.Any())
    {
        var adminRoleList = await uow.Roles.FindAsync(r => r.Name == ApplicationRoles.Admin);
        var adminRole = adminRoleList.FirstOrDefault();
        if (adminRole != null)
        {
            await uow.Users.AddAsync(new InvoiceGenerator.Api.Domain.Entities.User
            {
                Username = appSettingsSeed.AdminSettings.Username,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(appSettingsSeed.AdminSettings.Password),
                Email = appSettingsSeed.AdminSettings.Email,
                IsActive = true,
                RoleId = adminRole.Id
            });
            await uow.CommitAsync();
            Log.Information("Default Admin user successfully seeded from environment.");
        }
    }

    // Demo contracts (admin / dashboard) quando a BD não tem contratos
    // Não executar em IntegrationTests: várias WebApplicationFactory partilham InMemory com o mesmo nome.
    if (!webApp.Environment.IsEnvironment(HostingEnvironmentNames.IntegrationTests))
        await ContractDemoSeed.SeedIfEmptyAsync(uow);
}


webApp.Run();

public class AssemblyMarker { }
