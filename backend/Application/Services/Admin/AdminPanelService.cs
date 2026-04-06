using InvoiceGenerator.Api.Application.Abstractions;
using InvoiceGenerator.Api.Application.Constants;
using InvoiceGenerator.Api.Application.DTOs.Admin;
using InvoiceGenerator.Api.Application.DTOs.Common;
using InvoiceGenerator.Api.Application.DTOs.Requests;
using InvoiceGenerator.Api.Application.Exceptions;
using InvoiceGenerator.Api.Domain.Entities;
using InvoiceGenerator.Api.Domain.Interfaces;
using InvoiceGenerator.Api.Infrastructure.Configuration;
using FluentValidation;

namespace InvoiceGenerator.Api.Application.Services.Admin
{
    public sealed class AdminPanelService : IAdminPanelService
    {
        private readonly IUnitOfWork _uow;
        private readonly IAuditService _audit;
        private readonly AppSettings _appSettings;
        private readonly ICurrentUserAccessor _currentUser;
        private readonly IValidator<ResetPasswordRequest> _resetPasswordValidator;

        public AdminPanelService(
            IUnitOfWork uow,
            IAuditService audit,
            AppSettings appSettings,
            ICurrentUserAccessor currentUser,
            IValidator<ResetPasswordRequest> resetPasswordValidator)
        {
            _uow = uow;
            _audit = audit;
            _appSettings = appSettings;
            _currentUser = currentUser;
            _resetPasswordValidator = resetPasswordValidator;
        }

        public async Task<SystemLogsResponseDto> GetSystemLogsAsync(int lines, CancellationToken cancellationToken = default)
        {
            var baseLogPath = _appSettings.Logging.FilePath;
            if (string.IsNullOrWhiteSpace(baseLogPath))
                throw new ApiException(500, ApiResponseMessages.LoggingPathNotConfigured);

            var directory = Path.GetDirectoryName(baseLogPath) ?? "logs";
            var baseName = Path.GetFileNameWithoutExtension(baseLogPath);
            var files = Directory.GetFiles(directory, $"{baseName}*");
            var latest = files.OrderByDescending(f => f).FirstOrDefault();

            if (latest == null || !File.Exists(latest))
                throw new ApiException(404, ApiResponseMessages.NoLogFileFound);

            await using var fs = new FileStream(latest, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            using var reader = new StreamReader(fs);
            var content = await reader.ReadToEndAsync(cancellationToken);
            var logLines = content
                .Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries)
                .Reverse()
                .Take(lines)
                .Reverse()
                .ToList();

            return new SystemLogsResponseDto(DateTime.UtcNow, Path.GetFileName(latest), logLines);
        }

        public async Task<PagedResultDto<AuditLogRowDto>> GetAuditLogsAsync(
            int page, int size, string? entity, string? action, CancellationToken cancellationToken = default)
        {
            var all = (await _uow.AuditLogs.GetAllAsync()).AsEnumerable();

            if (!string.IsNullOrWhiteSpace(entity))
                all = all.Where(l => l.EntityName.Contains(entity, StringComparison.OrdinalIgnoreCase));
            if (!string.IsNullOrWhiteSpace(action))
                all = all.Where(l => l.Action.Contains(action, StringComparison.OrdinalIgnoreCase));

            var materialized = all.ToList();
            var total = materialized.Count;
            var items = materialized
                .OrderByDescending(l => l.Id)
                .Skip((page - 1) * size)
                .Take(size)
                .Select(ToAuditLogRow)
                .ToList();

            return new PagedResultDto<AuditLogRowDto>(total, page, size, items);
        }

        public async Task<PagedResultDto<LoginEventRowDto>> GetLoginEventsAsync(int page, int size, CancellationToken cancellationToken = default)
        {
            var all = await _uow.AuditLogs.GetAllAsync();
            var loginLogs = all.Where(l =>
                l.Action == AuditActionNames.Login ||
                l.Action == AuditActionNames.LoginFailed ||
                l.Action == AuditActionNames.Logout).ToList();

            var total = loginLogs.Count;
            var users = (await _uow.Users.GetAllAsync()).ToList();

            var items = loginLogs
                .OrderByDescending(l => l.Id)
                .Skip((page - 1) * size)
                .Take(size)
                .Select(l => new LoginEventRowDto(
                    l.Id,
                    l.Action,
                    l.UserId,
                    users.FirstOrDefault(u => u.Id == l.UserId)?.Username ?? ApiResponseMessages.NotApplicable,
                    _audit.MaskIp(_audit.DecryptIp(l.IpAddress)),
                    l.EntityId))
                .ToList();

            return new PagedResultDto<LoginEventRowDto>(total, page, size, items);
        }

        public async Task<PagedResultDto<AgreementActionRowDto>> GetAgreementActionsAsync(int page, int size, CancellationToken cancellationToken = default)
        {
            var all = await _uow.AuditLogs.GetAllAsync();
            var agreementName = nameof(Agreement);
            var agreementLogs = all.Where(l =>
                l.EntityName == agreementName ||
                l.Action.Contains(agreementName, StringComparison.OrdinalIgnoreCase)).ToList();

            var total = agreementLogs.Count;
            var items = agreementLogs
                .OrderByDescending(l => l.Id)
                .Skip((page - 1) * size)
                .Take(size)
                .Select(l => new AgreementActionRowDto(
                    l.Id,
                    l.Action,
                    l.EntityName,
                    l.EntityId,
                    l.NewValues,
                    _audit.MaskIp(_audit.DecryptIp(l.IpAddress)),
                    l.UserId))
                .ToList();

            return new PagedResultDto<AgreementActionRowDto>(total, page, size, items);
        }

        public async Task<IReadOnlyList<AdminUserListItemDto>> GetUsersAsync(string? search, CancellationToken cancellationToken = default)
        {
            var users = (await _uow.Users.GetAllAsync()).AsEnumerable();
            var roles = (await _uow.Roles.GetAllAsync()).ToList();

            if (!string.IsNullOrWhiteSpace(search))
            {
                var q = search.ToLowerInvariant();
                users = users.Where(u =>
                    u.Username.ToLowerInvariant().Contains(q) ||
                    u.Email.ToLowerInvariant().Contains(q));
            }

            return users
                .Select(u => new AdminUserListItemDto(
                    u.Id,
                    u.Username,
                    u.Email,
                    u.IsActive,
                    u.RoleId,
                    roles.FirstOrDefault(r => r.Id == u.RoleId)?.Name ?? ApiResponseMessages.NotApplicable))
                .ToList();
        }

        public async Task UpdateUserAsync(Guid id, UserUpdateRequest request, CancellationToken cancellationToken = default)
        {
            var user = await _uow.Users.GetByIdAsync(id);
            if (user == null)
                throw new ApiException(404, ApiResponseMessages.UserNotFound);

            var roleExists = (await _uow.Roles.FindAsync(r => r.Id == request.RoleId)).Any();
            if (!roleExists)
                throw new ApiException(400, ApiResponseMessages.InvalidRole);

            var oldSnapshot = $"Username:{user.Username}|Email:{user.Email}|Active:{user.IsActive}|Role:{user.RoleId}";

            user.Username = request.Username;
            user.Email = request.Email;
            user.IsActive = request.IsActive;
            user.RoleId = request.RoleId;

            await _uow.Users.UpdateAsync(user);

            var newSnapshot = $"Username:{user.Username}|Email:{user.Email}|Active:{user.IsActive}|Role:{user.RoleId}";
            await _audit.WriteAsync(_currentUser.GetUserId(), "UserUpdated", nameof(User), id.ToString(), oldSnapshot, newSnapshot,
                _currentUser.GetClientIpAddress());
        }

        public async Task ResetPasswordAsync(Guid id, ResetPasswordRequest request, CancellationToken cancellationToken = default)
        {
            var user = await _uow.Users.GetByIdAsync(id);
            if (user == null)
                throw new ApiException(404, ApiResponseMessages.UserNotFound);

            var pwdValidation = await _resetPasswordValidator.ValidateAsync(request, cancellationToken);
            if (!pwdValidation.IsValid)
                throw new ValidationException(pwdValidation.Errors);

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);
            await _uow.Users.UpdateAsync(user);

            await _audit.WriteAsync(_currentUser.GetUserId(), "PasswordReset", nameof(User), id.ToString(), null, "PasswordReset",
                _currentUser.GetClientIpAddress());
        }

        public async Task<IReadOnlyList<RoleListItemDto>> GetRolesAsync(CancellationToken cancellationToken = default)
        {
            var roles = await _uow.Roles.GetAllAsync();
            return roles.Select(r => new RoleListItemDto(r.Id, r.Name)).ToList();
        }

        private AuditLogRowDto ToAuditLogRow(AuditLog l) =>
            new(
                l.Id,
                l.Action,
                l.EntityName,
                l.EntityId,
                l.OldValues,
                l.NewValues,
                _audit.MaskIp(_audit.DecryptIp(l.IpAddress)),
                l.UserId);
    }
}
