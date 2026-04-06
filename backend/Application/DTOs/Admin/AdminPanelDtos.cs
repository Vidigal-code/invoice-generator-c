namespace InvoiceGenerator.Api.Application.DTOs.Admin
{
    public sealed record SystemLogsResponseDto(DateTime Timestamp, string Filename, IReadOnlyList<string> Logs);

    public sealed record AuditLogRowDto(
        Guid Id,
        string Action,
        string EntityName,
        string? EntityId,
        string? OldValues,
        string? NewValues,
        string IpAddress,
        Guid? UserId);

    public sealed record LoginEventRowDto(
        Guid Id,
        string Action,
        Guid? UserId,
        string Username,
        string IpAddress,
        string? EntityId);

    public sealed record AgreementActionRowDto(
        Guid Id,
        string Action,
        string EntityName,
        string? EntityId,
        string? NewValues,
        string IpAddress,
        Guid? UserId);

    public sealed record AdminUserListItemDto(
        Guid Id,
        string Username,
        string Email,
        bool IsActive,
        Guid RoleId,
        string RoleName);

    public sealed record RoleListItemDto(Guid Id, string Name);
}
