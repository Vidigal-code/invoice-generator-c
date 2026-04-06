using System.Security.Cryptography;
using System.Text;
using InvoiceGenerator.Api.Domain.Entities;
using InvoiceGenerator.Api.Domain.Interfaces;
using InvoiceGenerator.Api.Infrastructure.Configuration;
using Microsoft.Extensions.Options;

namespace InvoiceGenerator.Api.Application.Services
{
    /// <summary>
    /// Audit Service — Application Layer implementation of IAuditService.
    /// Encrypts raw IP addresses with AES-256-CBC before storing in DB.
    /// All logs are written as structured audit trail entries.
    /// </summary>
    public class AuditService : IAuditService
    {
        private readonly IUnitOfWork _uow;
        private readonly byte[] _key;
        private readonly byte[] _iv;

        public AuditService(IUnitOfWork uow, IOptions<AppSettings> settings)
        {
            _uow = uow;

            var rawKey = settings.Value.Security.AuditEncryptionKey;
            if (string.IsNullOrEmpty(rawKey))
                throw new InvalidOperationException("AuditEncryptionKey is not configured.");

            // Derive exactly 32 bytes (AES-256) and 16 bytes IV from the key
            using var sha = SHA256.Create();
            _key = sha.ComputeHash(Encoding.UTF8.GetBytes(rawKey));
            _iv = _key[..16];
        }

        public async Task WriteAsync(
            Guid? userId,
            string action,
            string entityName,
            string? entityId,
            string? oldValues,
            string? newValues,
            string? rawIpAddress)
        {
            var encryptedIp = string.IsNullOrEmpty(rawIpAddress)
                ? null
                : EncryptIp(rawIpAddress);

            var log = new AuditLog
            {
                UserId = userId,
                Action = action,
                EntityName = entityName,
                EntityId = entityId,
                OldValues = oldValues,
                NewValues = newValues,
                IpAddress = encryptedIp
            };

            await _uow.AuditLogs.AddAsync(log);
        }

        public string DecryptIp(string? encryptedIp)
        {
            if (string.IsNullOrEmpty(encryptedIp)) return "N/A";
            try
            {
                var cipherBytes = Convert.FromBase64String(encryptedIp);
                using var aes = Aes.Create();
                aes.Key = _key;
                aes.IV = _iv;
                using var decryptor = aes.CreateDecryptor();
                var plain = decryptor.TransformFinalBlock(cipherBytes, 0, cipherBytes.Length);
                return Encoding.UTF8.GetString(plain);
            }
            catch
            {
                return "***";
            }
        }

        public string MaskIp(string? ip)
        {
            if (string.IsNullOrEmpty(ip)) return "N/A";
            var parts = ip.Split('.');
            if (parts.Length == 4) return $"{parts[0]}.{parts[1]}.{parts[2]}.*";
            var colonIndex = ip.LastIndexOf(':');
            return colonIndex > 0 ? ip[..colonIndex] + ":***" : "***";
        }

        private string EncryptIp(string rawIp)
        {
            using var aes = Aes.Create();
            aes.Key = _key;
            aes.IV = _iv;
            using var encryptor = aes.CreateEncryptor();
            var plain = Encoding.UTF8.GetBytes(rawIp);
            var cipher = encryptor.TransformFinalBlock(plain, 0, plain.Length);
            return Convert.ToBase64String(cipher);
        }
    }
}
