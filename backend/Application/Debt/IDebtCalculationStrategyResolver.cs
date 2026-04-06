using InvoiceGenerator.Api.Domain.Enums;

namespace InvoiceGenerator.Api.Application.Debt
{
    public interface IDebtCalculationStrategyResolver
    {
        IDebtCalculationStrategy Resolve(WalletPortfolio portfolio);
    }
}
