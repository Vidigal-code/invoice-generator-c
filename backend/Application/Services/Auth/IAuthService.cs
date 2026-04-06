using InvoiceGenerator.Api.Application.DTOs.Requests;

namespace InvoiceGenerator.Api.Application.Services.Auth
{
    public interface IAuthService
    {
        Task<AuthLoginResult> LoginAsync(LoginRequest request, string? clientIp, CancellationToken cancellationToken = default);
    }
}
