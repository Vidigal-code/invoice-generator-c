using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using InvoiceGenerator.Api.Application.DTOs;

namespace InvoiceGenerator.Api.API.Filters
{
    public class ApiResponseFilter : IResultFilter
    {
        public void OnResultExecuting(ResultExecutingContext context)
        {
            if (context.Result is ObjectResult objectResult)
            {
                var statusCode = objectResult.StatusCode ?? 200;
                var success = statusCode >= 200 && statusCode < 300;
                
                // Avoid double wrapping 
                var valueType = objectResult.Value?.GetType();
                if (valueType != null && valueType.IsGenericType && valueType.GetGenericTypeDefinition() == typeof(ApiResponse<>))
                    return;

                var response = new ApiResponse<object>
                {
                    Success = success,
                    StatusCode = statusCode,
                    Message = success ? "Operação processada com sucesso" : "Ocorreu um erro na requisição",
                    Data = success ? objectResult.Value : null,
                    Errors = !success ? objectResult.Value : null
                };

                context.Result = new ObjectResult(response) { StatusCode = statusCode };
            }
            else if (context.Result is EmptyResult)
            {
                context.Result = new ObjectResult(new ApiResponse<object>
                {
                    Success = true,
                    StatusCode = 200,
                    Message = "Operação executada com sucesso"
                }) { StatusCode = 200 };
            }
        }

        public void OnResultExecuted(ResultExecutedContext context) { }
    }
}
