using InvoiceGenerator.Api.Domain.Entities;
using InvoiceGenerator.Api.Infrastructure.External.Storage;

namespace InvoiceGenerator.Api.Application.Services.Billets
{
    public sealed class BilletStoragePdfRetriever : IBilletStoragePdfRetriever
    {
        private readonly IFileStorageService _fileStorage;
        private readonly IBilletStorageObjectKeyBuilder _keyBuilder;

        public BilletStoragePdfRetriever(
            IFileStorageService fileStorage,
            IBilletStorageObjectKeyBuilder keyBuilder)
        {
            _fileStorage = fileStorage;
            _keyBuilder = keyBuilder;
        }

        public async Task<byte[]?> TryDownloadPdfAsync(Billet billet, CancellationToken cancellationToken)
        {
            try
            {
                var objectKey = _keyBuilder.BuildObjectKey(billet);
                await using var stream = await _fileStorage.DownloadFileAsync(objectKey, cancellationToken);
                await using var memory = new MemoryStream();
                await stream.CopyToAsync(memory, cancellationToken);
                return memory.Length > 0 ? memory.ToArray() : null;
            }
            catch
            {
                return null;
            }
        }
    }
}
