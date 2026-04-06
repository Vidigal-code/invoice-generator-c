using InvoiceGenerator.Api.Application.Abstractions;
using InvoiceGenerator.Api.Application.Constants;
using InvoiceGenerator.Api.Application.DTOs.Requests;
using InvoiceGenerator.Api.Application.Exceptions;
using InvoiceGenerator.Api.Domain.Authorization;
using InvoiceGenerator.Api.Domain.Interfaces;

namespace InvoiceGenerator.Api.Application.Services.Auth
{
    public sealed class AuthService : IAuthService
    {
        private readonly IUnitOfWork _uow;
        private readonly IAuditService _audit;
        private readonly IJwtTokenIssuer _jwtTokenIssuer;
        private readonly IApplicationRoleNames _roleNames;

        public AuthService(
            IUnitOfWork uow,
            IAuditService audit,
            IJwtTokenIssuer jwtTokenIssuer,
            IApplicationRoleNames roleNames)
        {
            _uow = uow;
            _audit = audit;
            _jwtTokenIssuer = jwtTokenIssuer;
            _roleNames = roleNames;
        }

        public async Task<AuthLoginResult> LoginAsync(LoginRequest request, string? clientIp, CancellationToken cancellationToken = default)
        {
            var userList = await _uow.Users.FindAsync(u => u.Email == request.Email);
            var user = userList.FirstOrDefault();

            if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            {
                await _audit.WriteAsync(null, AuditActionNames.LoginFailed, "Auth", request.Email, null, null, clientIp);
                throw new ApiException(401, ApiResponseMessages.InvalidCredentials);
            }

            var roleList = await _uow.Roles.FindAsync(r => r.Id == user.RoleId);
            var role = roleList.FirstOrDefault();
            var roleName = role?.Name ?? _roleNames.User;

            var token = _jwtTokenIssuer.CreateEncryptedToken(user.Id.ToString(), roleName, user.Username);
            await _audit.WriteAsync(user.Id, AuditActionNames.Login, "Auth", user.Id.ToString(), null, null, clientIp);

            return new AuthLoginResult(token, roleName, user.Username);
        }

    }
}
