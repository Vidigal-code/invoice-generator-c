using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace InvoiceGenerator.Api.API.OpenApi;

/// <summary>Documents HttpOnly cookie JWT and 401/403 for secured endpoints.</summary>
public sealed class OpenApiCookieAuthOperationFilter : IOperationFilter
{
    public const string SecuritySchemeId = "AuthTokenCookie";

    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        if (!RequiresAuthentication(context))
            return;

        operation.Responses.TryAdd("401", new OpenApiResponse { Description = "Unauthorized" });
        operation.Responses.TryAdd("403", new OpenApiResponse { Description = "Forbidden" });

        operation.Security ??= new List<OpenApiSecurityRequirement>();
        operation.Security.Add(new OpenApiSecurityRequirement
        {
            [new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = SecuritySchemeId }
            }] = Array.Empty<string>()
        });
    }

    private static bool RequiresAuthentication(OperationFilterContext context)
    {
        var methodInfo = context.MethodInfo;
        var declaring = methodInfo.DeclaringType;
        if (methodInfo.GetCustomAttributes(true).OfType<AllowAnonymousAttribute>().Any())
            return false;
        if (declaring != null && declaring.GetCustomAttributes(true).OfType<AllowAnonymousAttribute>().Any())
            return false;

        return methodInfo.GetCustomAttributes(true).OfType<AuthorizeAttribute>().Any()
               || (declaring != null && declaring.GetCustomAttributes(true).OfType<AuthorizeAttribute>().Any());
    }
}
