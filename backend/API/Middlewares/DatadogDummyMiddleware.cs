using Datadog.Trace;

namespace InvoiceGenerator.Api.API.Middlewares
{
    public class DatadogDummyMiddleware
    {
        private readonly RequestDelegate _next;

        public DatadogDummyMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Dummy support as requested: Suporte apenas
            var scope = Tracer.Instance.ActiveScope;
            if (scope != null)
            {
                scope.Span.SetTag("custom.tier", "Enterprise");
            }
            
            await _next(context);
        }
    }
}
