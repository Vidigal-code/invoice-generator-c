using System.Linq.Expressions;
using FluentAssertions;
using InvoiceGenerator.Api.Application.Constants;
using InvoiceGenerator.Api.Application.Exceptions;
using InvoiceGenerator.Api.Application.Queries;
using InvoiceGenerator.Api.Domain.Entities;
using InvoiceGenerator.Api.Domain.Interfaces;
using Moq;
using Xunit;

namespace InvoiceGenerator.Api.Tests.Application
{
    public sealed class GetContractHistoryQueryHandlerTests
    {
        [Fact]
        public async Task Handle_WhenContractMissing_ShouldThrowApiException404()
        {
            var contractId = Guid.NewGuid();
            var mockContracts = new Mock<IRepository<Contract>>();
            mockContracts.Setup(x => x.GetByIdAsync(contractId)).ReturnsAsync((Contract?)null);
            var uow = new Mock<IUnitOfWork>();
            uow.SetupGet(x => x.Contracts).Returns(mockContracts.Object);

            var handler = new GetContractHistoryQueryHandler(uow.Object);
            var act = async () => await handler.Handle(new GetContractHistoryQuery(contractId), CancellationToken.None);
            var ex = await act.Should().ThrowAsync<ApiException>();
            ex.Which.StatusCode.Should().Be(404);
            ex.Which.Message.Should().Contain(ApiResponseMessages.ContractNotFound);
        }

        [Fact]
        public async Task Handle_WhenContractExists_ShouldOrderHistoryByCreatedAtDescending()
        {
            var contractId = Guid.NewGuid();
            var mockContracts = new Mock<IRepository<Contract>>();
            mockContracts.Setup(x => x.GetByIdAsync(contractId)).ReturnsAsync(new Contract { Id = contractId });

            var older = new ContractHistory
            {
                Id = Guid.NewGuid(),
                ContractId = contractId,
                ChangeType = "Update",
                PayloadJson = "{}",
                CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            };
            var newer = new ContractHistory
            {
                Id = Guid.NewGuid(),
                ContractId = contractId,
                ChangeType = "Update",
                PayloadJson = "{\"x\":1}",
                CreatedAt = new DateTime(2026, 2, 1, 0, 0, 0, DateTimeKind.Utc)
            };

            var mockHist = new Mock<IRepository<ContractHistory>>();
            mockHist
                .Setup(x => x.FindAsync(It.IsAny<Expression<Func<ContractHistory, bool>>>()))
                .ReturnsAsync(new List<ContractHistory> { older, newer }.AsEnumerable());

            var uow = new Mock<IUnitOfWork>();
            uow.SetupGet(x => x.Contracts).Returns(mockContracts.Object);
            uow.SetupGet(x => x.ContractHistories).Returns(mockHist.Object);

            var handler = new GetContractHistoryQueryHandler(uow.Object);
            var result = await handler.Handle(new GetContractHistoryQuery(contractId), CancellationToken.None);

            result.Should().HaveCount(2);
            result[0].ChangeType.Should().Be("Update");
            result[0].PayloadJson.Should().Be("{\"x\":1}");
            result[1].PayloadJson.Should().Be("{}");
        }
    }
}
