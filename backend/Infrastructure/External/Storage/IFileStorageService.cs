namespace InvoiceGenerator.Api.Infrastructure.External.Storage
{
    public interface IFileStorageService
    {
        Task<string> UploadFileAsync(string fileName, Stream fileStream, string contentType, CancellationToken cancellationToken = default);
        Task<Stream> DownloadFileAsync(string fileName, CancellationToken cancellationToken = default);
    }
}
