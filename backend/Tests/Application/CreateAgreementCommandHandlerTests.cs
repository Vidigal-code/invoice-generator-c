using System.Linq.Expressions;
using InvoiceGenerator.Api.Application.Abstractions;
using InvoiceGenerator.Api.Application.Commands;
using InvoiceGenerator.Api.Application.Exceptions;
using InvoiceGenerator.Api.Domain.Entities;
using InvoiceGenerator.Api.Domain.Enums;
using InvoiceGenerator.Api.Domain.Interfaces;
using InvoiceGenerator.Api.Infrastructure.External.Storage;
using InvoiceGenerator.Api.Application.Services.Contracts;
using MassTransit;
using Moq;
using FluentAssertions;
using Xunit;

namespace InvoiceGenerator.Api.Tests.Application
{
    public class CreateAgreementCommandHandlerTests
    {
        private readonly Mock<IUnitOfWork> _mockUow;
        private readonly Mock<IPublishEndpoint> _mockPublish;
        private readonly Mock<IFileStorageService> _mockS3;
        private readonly Mock<IDistributedLock> _mockLock;
        private readonly Mock<IContractAccessService> _mockAccess;
        private readonly CreateAgreementCommandHandler _handler;

        private sealed class FakeLease : IAsyncDisposable
        {
            public ValueTask DisposeAsync() => ValueTask.CompletedTask;
        }

        public CreateAgreementCommandHandlerTests()
        {
            _mockUow = new Mock<IUnitOfWork>();
            _mockPublish = new Mock<IPublishEndpoint>();
            _mockS3 = new Mock<IFileStorageService>();
            _mockLock = new Mock<IDistributedLock>();

            _mockLock.Setup(l => l.TryAcquireAsync(It.IsAny<string>(), It.IsAny<TimeSpan>(), 
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(new FakeLease());

            _mockS3.Setup(s => s.UploadFileAsync(It.IsAny<string>(), It.IsAny<Stream>(), It.IsAny<string>(), 
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync("http://mock-s3-url.com/pdf");

            _mockAccess = new Mock<IContractAccessService>();
            _mockAccess.Setup(a => a.EnsureCanAccessContract(It.IsAny<Contract>()));

            _handler = new CreateAgreementCommandHandler(_mockUow.Object, _mockPublish.Object, _mockS3.Object, _mockLock.Object, _mockAccess.Object);
        }

        [Fact]
        public async Task Handle_GivenValidRequest_ShouldCreateAgreementAndBillets()
        {
            var contractId = Guid.NewGuid();
            var command = new CreateAgreementCommand(contractId, 3000m, 3);

            var contract = new Contract
            {
                Id = contractId,
                ContractNumber = "TEST-123",
                DebtorName = "X",
                DebtorDocument = "1",
                Status = ContractStatus.Active,
                Portfolio = WalletPortfolio.InvoiceGeneratorC
            };

            _mockUow.Setup(u => u.Contracts.GetByIdAsync(contractId)).ReturnsAsync(contract);
            _mockUow.Setup(u => u.Agreements.FindAsync(It.IsAny<Expression<Func<Agreement, bool>>>()))
                .ReturnsAsync(Enumerable.Empty<Agreement>());
            _mockUow.Setup(u => u.Agreements.AddAsync(It.IsAny<Agreement>())).Returns(Task.CompletedTask);
            _mockUow.Setup(u => u.Billets.AddAsync(It.IsAny<Billet>())).Returns(Task.CompletedTask);
            _mockUow.Setup(u => u.Contracts.UpdateAsync(It.IsAny<Contract>())).Returns(Task.CompletedTask);

            var result = await _handler.Handle(command, CancellationToken.None);

            result.Should().NotBeNull();
            result.ContractId.Should().Be(contractId);
            result.TotalNegotiatedValue.Should().Be(3000m);
            result.InstallmentsCount.Should().Be(3);
            result.Status.Should().Be(AgreementStatus.Active);

            _mockUow.Verify(u => u.Agreements.AddAsync(It.Is<Agreement>(a => a.InstallmentsCount == 3 && a.TotalNegotiatedValue == 3000m)), Times.Once);
            _mockUow.Verify(u => u.Billets.AddAsync(It.Is<Billet>(b => b.Value == 1000m)), Times.Exactly(3));
            _mockUow.Verify(u => u.Contracts.UpdateAsync(It.Is<Contract>(c => c.Status == ContractStatus.Negotiated)), Times.Once);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(25)]
        public async Task Handle_GivenInvalidInstallmentsCount_ShouldThrowApiException(int invalidCount)
        {
            var command = new CreateAgreementCommand(Guid.NewGuid(), 3000m, invalidCount);

            Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);
            await act.Should().ThrowAsync<ApiException>();
        }
    }
}
