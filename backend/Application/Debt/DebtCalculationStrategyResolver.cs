using InvoiceGenerator.Api.Domain.Enums;

namespace InvoiceGenerator.Api.Application.Debt
{
    public sealed class DebtCalculationStrategyResolver : IDebtCalculationStrategyResolver
    {
        private readonly IReadOnlyDictionary<WalletPortfolio, IDebtCalculationStrategy> _byPortfolio;

        public DebtCalculationStrategyResolver(IEnumerable<IDebtCalculationStrategy> strategies)
        {
            _byPortfolio = strategies.ToDictionary(s => s.Portfolio);
        }

        public IDebtCalculationStrategy Resolve(WalletPortfolio portfolio) =>
            _byPortfolio.TryGetValue(portfolio, out var s)
                ? s
                : _byPortfolio[WalletPortfolio.InvoiceGeneratorC];
    }
}
