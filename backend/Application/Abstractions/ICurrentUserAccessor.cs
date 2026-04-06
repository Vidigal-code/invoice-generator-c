namespace InvoiceGenerator.Api.Application.Abstractions
{
    /// <summary>Utilizador autenticado e contexto HTTP corrente (evita duplicar claims nos controllers).</summary>
    public interface ICurrentUserAccessor
    {
        Guid? GetUserId();
        string GetClientIpAddress();
        bool IsAdmin();
    }
}
