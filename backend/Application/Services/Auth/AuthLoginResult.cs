namespace InvoiceGenerator.Api.Application.Services.Auth
{
    public sealed record AuthLoginResult(string Token, string Role, string Username);
}
