using InvoiceGenerator.Api.Infrastructure.Configuration;

namespace InvoiceGenerator.Api.API.Middlewares
{
    public class SecurityHeadersMiddleware
    {
        private readonly RequestDelegate _next;

        public SecurityHeadersMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, AppSettings appSettings)
        {
            var allowedOrigins = string.IsNullOrEmpty(appSettings.Security.CorsOrigins) 
                ? "'self'" 
                : string.Join(" ", appSettings.Security.CorsOrigins.Split(","));

            context.Response.Headers.Append("X-Content-Type-Options", "nosniff");
            context.Response.Headers.Append("X-Frame-Options", "DENY");
            context.Response.Headers.Append("X-XSS-Protection", "1; mode=block");
            
            // Strict Content Security Policy (XSS protection)
            context.Response.Headers.Append("Content-Security-Policy", $"default-src 'self'; script-src 'self'; style-src 'self' 'unsafe-inline'; img-src 'self' data:; connect-src 'self' {allowedOrigins};");
            
            // Banking Specific: HSTS and Referrer-Policy
            context.Response.Headers.Append("Strict-Transport-Security", "max-age=31536000; includeSubDomains; preload");
            context.Response.Headers.Append("Referrer-Policy", "strict-origin-when-cross-origin");

            // Banking Specific: Permissions-Policy (Blocks browser hardware/APIs locally)
            context.Response.Headers.Append("Permissions-Policy", "accelerometer=(), camera=(), geolocation=(), gyroscope=(), magnetometer=(), microphone=(), payment=(), usb=()");

            // Cross domain blocking
            context.Response.Headers.Append("X-Permitted-Cross-Domain-Policies", "none");


            
            // Nunca expor header de API ou Server original
            context.Response.Headers.Remove("Server");
            context.Response.Headers.Remove("X-Powered-By");

            await _next(context);
        }
    }
}
