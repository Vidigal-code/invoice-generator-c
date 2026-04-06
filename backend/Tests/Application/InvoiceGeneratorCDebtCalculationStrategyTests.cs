using FluentAssertions;
using InvoiceGenerator.Api.Application.Debt;
using InvoiceGenerator.Api.Domain.Entities;
using InvoiceGenerator.Api.Domain.Enums;
using InvoiceGenerator.Api.Infrastructure.Configuration;
using Xunit;

namespace InvoiceGenerator.Api.Tests.Application
{
    public sealed class InvoiceGeneratorCDebtCalculationStrategyTests
    {
        private static InvoiceGeneratorCDebtCalculationStrategy CreateStrategy(decimal monthlyPct = 1m, int daysPerMonth = 30, decimal penaltyPct = 2m)
        {
            var settings = new AppSettings
            {
                DebtCalculation = new DebtCalculationSettings
                {
                    InvoiceGeneratorC = new WalletDebtRules
                    {
                        MonthlyInterestPercent = monthlyPct,
                        ProRataDaysPerMonth = daysPerMonth,
                        LatePenaltyPercentOfOriginal = penaltyPct
                    }
                }
            };
            return new InvoiceGeneratorCDebtCalculationStrategy(settings);
        }

        [Fact]
        public void Project_WhenNotOverdue_ShouldHaveZeroPenaltyAndInterest()
        {
            var s = CreateStrategy();
            var due = new DateTime(2026, 2, 15, 0, 0, 0, DateTimeKind.Utc);
            var inst = new Installment { OriginalValue = 500m, DueDate = due };
            var r = s.Project(inst, due.AddDays(-1));
            r.DaysOverdue.Should().Be(0);
            r.PenaltyAmount.Should().Be(0m);
            r.InterestAmount.Should().Be(0m);
            r.CurrentValue.Should().Be(500m);
        }

        [Fact]
        public void Project_WhenOverdue_ShouldApplyPenaltyAndProRataInterest()
        {
            var s = CreateStrategy();
            var due = new DateTime(2026, 1, 10, 0, 0, 0, DateTimeKind.Utc);
            var reference = new DateTime(2026, 2, 9, 0, 0, 0, DateTimeKind.Utc);
            var inst = new Installment { OriginalValue = 1000m, DueDate = due };
            var r = s.Project(inst, reference);
            r.DaysOverdue.Should().Be(30);
            r.PenaltyAmount.Should().Be(20m);
            r.InterestAmount.Should().BeApproximately(10m, 0.0001m);
            r.CurrentValue.Should().BeApproximately(1030m, 0.0001m);
        }

        [Fact]
        public void Portfolio_ShouldBeInvoiceGeneratorC()
        {
            CreateStrategy().Portfolio.Should().Be(WalletPortfolio.InvoiceGeneratorC);
        }
    }
}
