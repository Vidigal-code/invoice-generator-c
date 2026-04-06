namespace InvoiceGenerator.Api.Domain.Interfaces
{
    /// <summary>
    /// Contract for the Audit Service — writes encrypted audit trail entries
    /// following Clean Architecture separation of concerns.
    /// </summary>
    public interface IAuditService
    {
        /// <summary>
        /// Persists a structured audit log entry. IP is encrypted via AES-256 before storage.
        /// </summary>
        Task WriteAsync(
            Guid? userId,
            string action,
            string entityName,
            string? entityId,
            string? oldValues,
            string? newValues,
            string? rawIpAddress);

        /// <summary>
        /// Decrypts an encrypted IP address for Admin display only.
        /// </summary>
        string DecryptIp(string? encryptedIp);

        /// <summary>
        /// Masks the last octet of an IP address for display (e.g. 192.168.1.***)
        /// </summary>
        string MaskIp(string? ip);
    }
}
