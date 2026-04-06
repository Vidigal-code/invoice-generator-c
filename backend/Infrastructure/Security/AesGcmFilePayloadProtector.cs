using System.Security.Cryptography;
using System.Text;
using InvoiceGenerator.Api.Application.Abstractions;
using InvoiceGenerator.Api.Infrastructure.Configuration;

namespace InvoiceGenerator.Api.Infrastructure.Security
{
    public sealed class AesGcmFilePayloadProtector : IFilePayloadProtector
    {
        private static readonly byte[] Magic = Encoding.ASCII.GetBytes("INVENC01");
        private const int NonceSize = 12;
        private const int TagSize = 16;
        private readonly byte[] _key;

        public AesGcmFilePayloadProtector(AppSettings settings)
        {
            var secret = settings.JwtSettings.JweSecret;
            if (string.IsNullOrEmpty(secret))
                throw new InvalidOperationException("JwtSettings:JweSecret é obrigatório para encriptação de ficheiros.");
            _key = SHA256.HashData(Encoding.UTF8.GetBytes(secret));
        }

        public byte[] Protect(ReadOnlySpan<byte> plain)
        {
            var nonce = RandomNumberGenerator.GetBytes(NonceSize);
            var cipher = new byte[plain.Length];
            var tag = new byte[TagSize];
            using (var aes = new AesGcm(_key, TagSize))
            {
                aes.Encrypt(nonce, plain, cipher, tag);
            }

            var result = new byte[Magic.Length + NonceSize + TagSize + cipher.Length];
            Magic.CopyTo(result.AsSpan(0, Magic.Length));
            nonce.CopyTo(result.AsSpan(Magic.Length, NonceSize));
            tag.CopyTo(result.AsSpan(Magic.Length + NonceSize, TagSize));
            cipher.CopyTo(result.AsSpan(Magic.Length + NonceSize + TagSize));
            return result;
        }

        public byte[] Unprotect(ReadOnlySpan<byte> stored)
        {
            if (stored.Length < Magic.Length || !stored.Slice(0, Magic.Length).SequenceEqual(Magic))
                return stored.ToArray();

            var offset = Magic.Length;
            var nonce = stored.Slice(offset, NonceSize).ToArray();
            offset += NonceSize;
            var tag = stored.Slice(offset, TagSize).ToArray();
            offset += TagSize;
            var cipher = stored.Slice(offset).ToArray();
            var plain = new byte[cipher.Length];
            using (var aes = new AesGcm(_key, TagSize))
            {
                aes.Decrypt(nonce, cipher, tag, plain);
            }

            return plain;
        }
    }
}
