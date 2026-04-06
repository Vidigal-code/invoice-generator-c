using InvoiceGenerator.Api.Application.Abstractions;

namespace InvoiceGenerator.Api.Infrastructure.External.Storage
{
    public sealed class EncryptedPayloadFileStorageService : IFileStorageService
    {
        private readonly IFileStorageService _inner;
        private readonly IFilePayloadProtector _protector;

        public EncryptedPayloadFileStorageService(IFileStorageService inner, IFilePayloadProtector protector)
        {
            _inner = inner;
            _protector = protector;
        }

        public async Task<string> UploadFileAsync(string fileName, Stream fileStream, string contentType, CancellationToken cancellationToken = default)
        {
            await using var ms = new MemoryStream();
            await fileStream.CopyToAsync(ms, cancellationToken).ConfigureAwait(false);
            var encrypted = _protector.Protect(ms.ToArray());
            await using var encStream = new MemoryStream(encrypted);
            return await _inner.UploadFileAsync(fileName, encStream, "application/octet-stream", cancellationToken).ConfigureAwait(false);
        }

        public async Task<Stream> DownloadFileAsync(string fileName, CancellationToken cancellationToken = default)
        {
            await using var encStream = await _inner.DownloadFileAsync(fileName, cancellationToken).ConfigureAwait(false);
            await using var ms = new MemoryStream();
            await encStream.CopyToAsync(ms, cancellationToken).ConfigureAwait(false);
            var plain = _protector.Unprotect(ms.ToArray());
            return new MemoryStream(plain);
        }
    }
}
