using System.Linq.Expressions;
using InvoiceGenerator.Api.Application.Abstractions;
using InvoiceGenerator.Api.Application.Debt;
using InvoiceGenerator.Api.Application.Exceptions;
using InvoiceGenerator.Api.Application.Queries;
using InvoiceGenerator.Api.Application.Services.Contracts;
using InvoiceGenerator.Api.Domain.Entities;
using InvoiceGenerator.Api.Domain.Enums;
using InvoiceGenerator.Api.Domain.Interfaces;
using InvoiceGenerator.Api.Infrastructure.Configuration;
using Microsoft.Extensions.Caching.Distributed;
using Moq;
using FluentAssertions;
using Xunit;

namespace InvoiceGenerator.Api.Tests.Application
{
    public class GetDebtCalculationQueryHandlerTests
    {
        private readonly Mock<IUnitOfWork> _mockUow;
        private readonly Mock<IDistributedCache> _mockCache;
        private readonly IDebtCalculationStrategyResolver _resolver;
        private readonly GetDebtCalculationQueryHandler _handler;

        public GetDebtCalculationQueryHandlerTests()
        {
            _mockUow = new Mock<IUnitOfWork>();
            _mockCache = new Mock<IDistributedCache>();
            var settings = new AppSettings();
            var strategy = new InvoiceGeneratorCDebtCalculationStrategy(settings);
            _resolver = new DebtCalculationStrategyResolver(new IDebtCalculationStrategy[] { strategy });
            var mockUser = new Mock<ICurrentUserAccessor>();
            mockUser.Setup(x => x.IsAdmin()).Returns(true);
            var access = new ContractAccessService(_mockUow.Object, mockUser.Object);
            _handler = new GetDebtCalculationQueryHandler(_mockUow.Object, _mockCache.Object, _resolver, access);
        }

        [Fact]
        public async Task Handle_GivenValidContractWithOverdueInstallment_ShouldCalculateCorrectTotalDebt()
        {
            var contractId = Guid.NewGuid();
            var contract = new Contract
            {
                Id = contractId,
                ContractNumber = "TEST-123",
                Portfolio = WalletPortfolio.InvoiceGeneratorC
            };
            var openInstallments = new List<Installment>
            {
                new Installment
                {
                    Id = Guid.NewGuid(),
                    ContractId = contractId,
                    InstallmentNumber = 1,
                    OriginalValue = 1000m,
                    DueDate = DateTime.UtcNow.AddDays(-10),
                    Status = InstallmentStatus.Open
                }
            };

            _mockUow.Setup(u => u.Contracts.GetByIdAsync(contractId)).ReturnsAsync(contract);
            _mockUow.Setup(u => u.Installments.FindAsync(It.IsAny<Expression<Func<Installment, bool>>>()))
                .ReturnsAsync(openInstallments.AsEnumerable());

            var result = await _handler.Handle(new GetDebtCalculationQuery(contractId), CancellationToken.None);

            result.Should().NotBeNull();
            result.ContractId.Should().Be(contractId);
            result.OriginalTotalDebt.Should().Be(1000m);

            var expectedJuros = (1000m * 0.01m / 30m) * 10m;
            var expectedMulta = 1000m * 0.02m;
            var expectedTotal = 1000m + expectedMulta + expectedJuros;

            result.CurrentTotalDebt.Should().BeApproximately(expectedTotal, 0.0001m);
            result.OpenInstallments.Should().HaveCount(1);
            result.OpenInstallments.First().DaysOverdue.Should().Be(10);
        }

        [Fact]
        public async Task Handle_GivenNonExistentContract_ShouldThrowApiException()
        {
            var contractId = Guid.NewGuid();
            _mockUow.Setup(u => u.Contracts.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((Contract?)null);

            Func<Task> act = async () => await _handler.Handle(new GetDebtCalculationQuery(contractId), CancellationToken.None);
            await act.Should().ThrowAsync<ApiException>();
        }
    }
}
