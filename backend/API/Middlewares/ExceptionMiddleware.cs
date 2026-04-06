using System.Text.Json;
using FluentValidation;
using InvoiceGenerator.Api.Application.Constants;
using InvoiceGenerator.Api.Application.DTOs;
using InvoiceGenerator.Api.Application.Exceptions;

namespace InvoiceGenerator.Api.API.Middlewares
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;

        public ExceptionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);
            }
            catch (BilletNotFoundException ex)
            {
                await WriteApiResponseAsync(httpContext, 404, false, ex.Message, null);
            }
            catch (ApiException ex)
            {
                await WriteApiResponseAsync(httpContext, ex.StatusCode, false, ex.Message, null);
            }
            catch (ValidationException ex)
            {
                var errors = ex.Errors
                    .GroupBy(e => string.IsNullOrEmpty(e.PropertyName) ? "_" : e.PropertyName)
                    .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray());
                await WriteApiResponseAsync(httpContext, 400, false, ApiResponseMessages.ValidationFailed, errors);
            }
            catch (Exception ex)
            {
                object errors = new { details = ex.Message, path = httpContext.Request.Path };
                if (httpContext.Items.TryGetValue("CorrelationId", out var correlationId))
                    errors = new { details = ex.Message, traceId = correlationId };

                await WriteApiResponseAsync(httpContext, 500, false, "Internal Server Error", errors);
            }
        }

        private static async Task WriteApiResponseAsync(HttpContext httpContext, int statusCode, bool success, string message, object? errors)
        {
            httpContext.Response.ContentType = "application/json";
            httpContext.Response.StatusCode = statusCode;

            var errorResponse = new ApiResponse<object>
            {
                Success = success,
                StatusCode = statusCode,
                Message = message,
                Data = null,
                Errors = errors
            };

            await httpContext.Response.WriteAsync(JsonSerializer.Serialize(errorResponse));
        }
    }
}
