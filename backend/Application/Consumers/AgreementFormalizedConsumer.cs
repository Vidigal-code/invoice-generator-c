using InvoiceGenerator.Api.Application.Constants;
using InvoiceGenerator.Api.Application.Events;
using MassTransit;
using Microsoft.Extensions.Caching.Distributed;
using StackExchange.Redis;

namespace InvoiceGenerator.Api.Application.Consumers
{
    public sealed class AgreementFormalizedConsumer : IConsumer<AgreementFormalizedEvent>
    {
        private readonly IDistributedCache _cache;
        private readonly IConnectionMultiplexer _redis;
        private readonly ILogger<AgreementFormalizedConsumer> _logger;

        public AgreementFormalizedConsumer(
            IDistributedCache cache,
            IConnectionMultiplexer redis,
            ILogger<AgreementFormalizedConsumer> logger)
        {
            _cache = cache;
            _redis = redis;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<AgreementFormalizedEvent> context)
        {
            var m = context.Message;
            var db = _redis.GetDatabase();
            var idemKey = $"idem:agreement:{m.AgreementId:N}";

            if (await db.KeyExistsAsync(idemKey).ConfigureAwait(false))
            {
                _logger.LogInformation("Duplicate AgreementFormalizedEvent ignored for {AgreementId}", m.AgreementId);
                return;
            }

            var cacheKey = DebtCacheKeys.LatestCalculation(m.ContractId);
            await _cache.RemoveAsync(cacheKey, context.CancellationToken).ConfigureAwait(false);
            await db.StringSetAsync(idemKey, "1", TimeSpan.FromDays(30)).ConfigureAwait(false);

            _logger.LogInformation(
                "Processed AgreementFormalizedEvent agreement={AgreementId} contract={ContractId}",
                m.AgreementId,
                m.ContractId);
        }
    }
}
