using InvoiceGenerator.Api.Application.Abstractions;
using StackExchange.Redis;

namespace InvoiceGenerator.Api.Infrastructure.Redis
{
    public sealed class RedisDistributedLock : IDistributedLock
    {
        private readonly IConnectionMultiplexer _mux;

        public RedisDistributedLock(IConnectionMultiplexer mux)
        {
            _mux = mux;
        }

        public async Task<IAsyncDisposable?> TryAcquireAsync(string resourceKey, TimeSpan expiry, CancellationToken cancellationToken = default)
        {
            var db = _mux.GetDatabase();
            var key = $"lock:{resourceKey}";
            var token = Guid.NewGuid().ToString("N");
            if (!await db.LockTakeAsync(key, token, expiry).ConfigureAwait(false))
                return null;
            return new LockRelease(db, key, token);
        }

        private sealed class LockRelease : IAsyncDisposable
        {
            private readonly IDatabase _db;
            private readonly string _key;
            private readonly string _token;

            public LockRelease(IDatabase db, string key, string token)
            {
                _db = db;
                _key = key;
                _token = token;
            }

            public ValueTask DisposeAsync() =>
                new(_db.LockReleaseAsync(_key, _token));
        }
    }
}
