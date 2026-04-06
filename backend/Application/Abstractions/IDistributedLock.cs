namespace InvoiceGenerator.Api.Application.Abstractions
{
    public interface IDistributedLock
    {
        Task<IAsyncDisposable?> TryAcquireAsync(string resourceKey, TimeSpan expiry, CancellationToken cancellationToken = default);
    }
}
