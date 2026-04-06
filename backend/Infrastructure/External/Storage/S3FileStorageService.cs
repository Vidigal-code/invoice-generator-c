using Amazon.S3;
using Amazon.S3.Transfer;
using InvoiceGenerator.Api.Infrastructure.Configuration;
using Microsoft.Extensions.Options;

namespace InvoiceGenerator.Api.Infrastructure.External.Storage
{
    public class S3FileStorageService : IFileStorageService
    {
        private readonly IAmazonS3 _s3Client;
        private readonly AppSettings _appSettings;

        public S3FileStorageService(IOptions<AppSettings> appSettings)
        {
            _appSettings = appSettings.Value;

            var config = new AmazonS3Config
            {
                ServiceURL = _appSettings.AWS.S3.ServiceURL,
                ForcePathStyle = true 
            };

            _s3Client = new AmazonS3Client(_appSettings.AWS.S3.AccessKey, _appSettings.AWS.S3.SecretKey, config);
        }

        public async Task<string> UploadFileAsync(string fileName, Stream fileStream, string contentType, CancellationToken cancellationToken = default)
        {
            var bucketName = _appSettings.AWS.S3.BucketName;

            var bucketExists = await Amazon.S3.Util.AmazonS3Util.DoesS3BucketExistV2Async(_s3Client, bucketName);
            if (!bucketExists)
            {
                await ExecuteWithRetryAsync(
                    ct => _s3Client.PutBucketAsync(
                        new Amazon.S3.Model.PutBucketRequest { BucketName = bucketName, UseClientRegion = true },
                        ct),
                    cancellationToken).ConfigureAwait(false);
            }

            var fileTransferUtility = new TransferUtility(_s3Client);
            var uploadRequest = new TransferUtilityUploadRequest
            {
                InputStream = fileStream,
                Key = fileName,
                BucketName = bucketName,
                ContentType = contentType
            };
            await fileTransferUtility.UploadAsync(uploadRequest, cancellationToken);

            return $"{_appSettings.AWS.S3.ServiceURL}/{bucketName}/{fileName}";
        }

        public async Task<Stream> DownloadFileAsync(string fileName, CancellationToken cancellationToken = default)
        {
            return await ExecuteWithRetryAsync(
                async ct =>
                {
                    var request = new Amazon.S3.Model.GetObjectRequest
                    {
                        BucketName = _appSettings.AWS.S3.BucketName,
                        Key = fileName
                    };
                    var response = await _s3Client.GetObjectAsync(request, ct);
                    return response.ResponseStream;
                },
                cancellationToken).ConfigureAwait(false);
        }

        private static async Task<T> ExecuteWithRetryAsync<T>(Func<CancellationToken, Task<T>> action, CancellationToken cancellationToken, int attempts = 3)
        {
            Exception? last = null;
            for (var i = 0; i < attempts; i++)
            {
                try
                {
                    return await action(cancellationToken).ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    last = ex;
                    if (i == attempts - 1)
                        break;
                    await Task.Delay(TimeSpan.FromMilliseconds(100 * (i + 1)), cancellationToken).ConfigureAwait(false);
                }
            }

            throw last ?? new InvalidOperationException("S3 operation failed.");
        }

        private static async Task ExecuteWithRetryAsync(Func<CancellationToken, Task> action, CancellationToken cancellationToken, int attempts = 3)
        {
            await ExecuteWithRetryAsync(
                async ct =>
                {
                    await action(ct).ConfigureAwait(false);
                    return true;
                },
                cancellationToken,
                attempts).ConfigureAwait(false);
        }
    }
}
