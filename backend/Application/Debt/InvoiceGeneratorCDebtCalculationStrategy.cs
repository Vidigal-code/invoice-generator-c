using InvoiceGenerator.Api.Domain.Entities;
using InvoiceGenerator.Api.Domain.Enums;
using InvoiceGenerator.Api.Infrastructure.Configuration;

namespace InvoiceGenerator.Api.Application.Debt
{
    public sealed class InvoiceGeneratorCDebtCalculationStrategy : IDebtCalculationStrategy
    {
        private readonly AppSettings _settings;

        public InvoiceGeneratorCDebtCalculationStrategy(AppSettings settings)
        {
            _settings = settings;
        }

        public WalletPortfolio Portfolio => WalletPortfolio.InvoiceGeneratorC;

        public InstallmentDebtProjection Project(Installment installment, DateTime referenceUtc)
        {
            var rules = _settings.DebtCalculation.InvoiceGeneratorC;
            var daysPerMonth = rules.ProRataDaysPerMonth > 0 ? rules.ProRataDaysPerMonth : 30;

            var daysOverdue = (referenceUtc.Date - installment.DueDate.Date).Days;
            if (daysOverdue < 0)
                daysOverdue = 0;

            var penalty = daysOverdue > 0
                ? installment.OriginalValue * (rules.LatePenaltyPercentOfOriginal / 100m)
                : 0m;

            var interest = daysOverdue > 0
                ? installment.OriginalValue * (rules.MonthlyInterestPercent / 100m / daysPerMonth) * daysOverdue
                : 0m;

            var current = installment.OriginalValue + penalty + interest;
            return new InstallmentDebtProjection(current, penalty, interest, daysOverdue);
        }
    }
}
