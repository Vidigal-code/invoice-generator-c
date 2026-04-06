using InvoiceGenerator.Api.Application.DTOs.Admin;
using InvoiceGenerator.Api.Application.DTOs.Common;
using InvoiceGenerator.Api.Application.DTOs.Requests;

namespace InvoiceGenerator.Api.Application.Services.Admin
{
    public interface IAdminPanelService
    {
        Task<SystemLogsResponseDto> GetSystemLogsAsync(int lines, CancellationToken cancellationToken = default);

        Task<PagedResultDto<AuditLogRowDto>> GetAuditLogsAsync(
            int page, int size, string? entity, string? action, CancellationToken cancellationToken = default);

        Task<PagedResultDto<LoginEventRowDto>> GetLoginEventsAsync(int page, int size, CancellationToken cancellationToken = default);

        Task<PagedResultDto<AgreementActionRowDto>> GetAgreementActionsAsync(int page, int size, CancellationToken cancellationToken = default);

        Task<IReadOnlyList<AdminUserListItemDto>> GetUsersAsync(string? search, CancellationToken cancellationToken = default);

        Task UpdateUserAsync(Guid id, UserUpdateRequest request, CancellationToken cancellationToken = default);

        Task ResetPasswordAsync(Guid id, ResetPasswordRequest request, CancellationToken cancellationToken = default);

        Task<IReadOnlyList<RoleListItemDto>> GetRolesAsync(CancellationToken cancellationToken = default);
    }
}
