using System.Security.Claims;
using InvoiceGenerator.Api.Application.Abstractions;
using InvoiceGenerator.Api.Domain.Authorization;

namespace InvoiceGenerator.Api.API.Services
{
    public sealed class HttpCurrentUserAccessor : ICurrentUserAccessor
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public HttpCurrentUserAccessor(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public Guid? GetUserId()
        {
            var value = _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
            return Guid.TryParse(value, out var id) ? id : null;
        }

        public string GetClientIpAddress()
        {
            return _httpContextAccessor.HttpContext?.Connection.RemoteIpAddress?.ToString() ?? "unknown";
        }

        public bool IsAdmin() =>
            _httpContextAccessor.HttpContext?.User.IsInRole(ApplicationRoles.Admin) == true;
    }
}
