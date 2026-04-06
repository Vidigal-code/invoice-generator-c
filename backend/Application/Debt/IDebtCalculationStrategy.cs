using InvoiceGenerator.Api.Domain.Entities;
using InvoiceGenerator.Api.Domain.Enums;

namespace InvoiceGenerator.Api.Application.Debt
{
    public interface IDebtCalculationStrategy
    {
        WalletPortfolio Portfolio { get; }

        InstallmentDebtProjection Project(Installment installment, DateTime referenceUtc);
    }
}
