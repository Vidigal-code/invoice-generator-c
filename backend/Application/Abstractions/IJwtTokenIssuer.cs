namespace InvoiceGenerator.Api.Application.Abstractions
{
    public interface IJwtTokenIssuer
    {
        string CreateEncryptedToken(string userId, string roleName, string username);
    }
}
