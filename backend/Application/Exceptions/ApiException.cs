namespace InvoiceGenerator.Api.Application.Exceptions
{
    /// <summary>Erro HTTP previsível (4xx/5xx) tratado pelo ExceptionMiddleware.</summary>
    public sealed class ApiException : Exception
    {
        public ApiException(int statusCode, string message) : base(message)
        {
            StatusCode = statusCode;
        }

        public int StatusCode { get; }
    }
}
