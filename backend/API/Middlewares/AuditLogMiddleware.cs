using System.Diagnostics;
using System.Text.RegularExpressions;
using InvoiceGenerator.Api.Domain.Interfaces;
using InvoiceGenerator.Api.Domain.Entities;

namespace InvoiceGenerator.Api.API.Middlewares
{
    public class AuditLogMiddleware
    {
        private readonly RequestDelegate _next;

        public AuditLogMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var sw = Stopwatch.StartNew();
            
            // 1. Correlation ID para rastreabilidade Cross-Service (Trace Matrix)
            var correlationId = context.Request.Headers["X-Correlation-ID"].ToString();
            if (string.IsNullOrEmpty(correlationId))
            {
                correlationId = Guid.NewGuid().ToString();
                context.Request.Headers["X-Correlation-ID"] = correlationId;
            }
            context.Response.Headers.Append("X-Correlation-ID", correlationId);

            // 2. Data Loss Prevention & PII Masking: Interceptando Body Base
            context.Request.EnableBuffering();
            var requestBody = string.Empty;
            using (var reader = new StreamReader(context.Request.Body, System.Text.Encoding.UTF8, true, 1024, leaveOpen: true))
            {
                requestBody = await reader.ReadToEndAsync();
                context.Request.Body.Position = 0;
            }

            var maskedRequest = MaskPiiData(requestBody);

            await _next(context);
            
            sw.Stop();

            if (context.Request.Method != "GET")
            {
                using var scope = context.RequestServices.CreateScope();
                var uow = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

                var log = new AuditLog
                {
                    Action = $"{context.Request.Method} {context.Request.Path}",
                    EntityName = "Trace_" + correlationId,
                    IpAddress = context.Connection.RemoteIpAddress?.ToString() ?? "Unknown",
                    NewValues = $"Status: {context.Response.StatusCode} | Elapsed: {sw.ElapsedMilliseconds}ms | Payload: {maskedRequest}",
                    CreatedAt = DateTime.UtcNow
                };

                try 
                {
                    await uow.AuditLogs.AddAsync(log);
                    await uow.CommitAsync(); // Rollback Tracker via Entity Framework Commit
                } 
                catch 
                {
                    // Fail silently: Trilha de auditoria corrompida não pode reverter fluxo critico final de acordos
                }
            }
        }

        private string MaskPiiData(string payload)
        {
            if (string.IsNullOrEmpty(payload)) return payload;
            
            // Padrão Fintech: Mascarar senhas, tokens ou cartões em logs da infra
            var pwdRegex = new Regex(@"([""'](?:password|senha|token)[""']\s*:\s*)([""'])(?:(?=(\\?))\3.)*?\2", RegexOptions.IgnoreCase);
            return pwdRegex.Replace(payload, "$1\"***MASKED_PII_LOG***\"");
        }
    }
}
