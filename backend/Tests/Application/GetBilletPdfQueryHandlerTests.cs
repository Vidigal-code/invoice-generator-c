using System.Text;
using FluentAssertions;
using InvoiceGenerator.Api.Application.Abstractions;
using InvoiceGenerator.Api.Application.Exceptions;
using InvoiceGenerator.Api.Application.Queries;
using InvoiceGenerator.Api.Application.Services.Billets;
using InvoiceGenerator.Api.Application.Services.Contracts;
using InvoiceGenerator.Api.Application.Services.Billets.Barcode;
using InvoiceGenerator.Api.Domain.Entities;
using InvoiceGenerator.Api.Domain.Interfaces;
using Moq;
using Xunit;

namespace InvoiceGenerator.Api.Tests.Application
{
    public class GetBilletPdfQueryHandlerTests
    {
        private readonly Mock<IUnitOfWork> _mockUow;
        private readonly Mock<InvoiceGenerator.Api.Infrastructure.External.Storage.IFileStorageService> _mockStorage;
        private readonly GetBilletPdfQueryHandler _handler;

        public GetBilletPdfQueryHandlerTests()
        {
            _mockUow = new Mock<IUnitOfWork>();
            _mockStorage = new Mock<InvoiceGenerator.Api.Infrastructure.External.Storage.IFileStorageService>();

            var keyBuilder = new BilletStorageObjectKeyBuilder();
            var retriever = new BilletStoragePdfRetriever(_mockStorage.Object, keyBuilder);
            var barcode = new FebrabanBarcodeComposer();
            var itf25 = new Itf25BarcodeHtmlRenderer();
            var htmlRenderer = new BilletHtmlFallbackRenderer(barcode, itf25);

            var mockUser = new Mock<ICurrentUserAccessor>();
            mockUser.Setup(x => x.IsAdmin()).Returns(true);
            var access = new ContractAccessService(_mockUow.Object, mockUser.Object);

            _handler = new GetBilletPdfQueryHandler(_mockUow.Object, retriever, htmlRenderer, access);
        }

        [Fact]
        public async Task Handle_GivenValidBillet_WhenStorageFails_ShouldReturnHtmlFallback()
        {
            var billetId = Guid.NewGuid();
            var agreementId = Guid.NewGuid();
            var contractId = Guid.NewGuid();
            var billet = new Billet
            {
                Id = billetId,
                AgreementId = agreementId,
                InstallmentNumber = 1,
                Value = 1050.25m,
                Barcode = "12345678901234567890123456789012345678901234",
                DueDate = new DateTime(2026, 5, 1, 0, 0, 0, DateTimeKind.Utc)
            };

            _mockUow.Setup(u => u.Billets.GetByIdAsync(billetId)).ReturnsAsync(billet);
            _mockUow.Setup(u => u.Agreements.GetByIdAsync(agreementId)).ReturnsAsync(new Agreement
            {
                Id = agreementId,
                ContractId = contractId
            });
            _mockUow.Setup(u => u.Contracts.GetByIdAsync(contractId)).ReturnsAsync(new Contract { Id = contractId });
            _mockStorage
                .Setup(s => s.DownloadFileAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new FileNotFoundException());

            var result = await _handler.Handle(new GetBilletPdfQuery(billetId), CancellationToken.None);

            result.Should().NotBeNull();
            var html = Encoding.UTF8.GetString(result);
            html.Should().Contain("invoice-generator-c");
            html.Should().Match("*1.050,25*", "valor formatado em pt-BR");
            html.Should().Contain(billetId.ToString()[..8].ToUpperInvariant());
            html.Should().Contain(agreementId.ToString());
        }

        [Fact]
        public async Task Handle_GivenValidBillet_WhenStorageReturnsPdf_ShouldReturnPdfBytes()
        {
            var billetId = Guid.NewGuid();
            var agreementId = Guid.NewGuid();
            var contractId = Guid.NewGuid();
            var billet = new Billet
            {
                Id = billetId,
                AgreementId = agreementId,
                InstallmentNumber = 2,
                Value = 100m,
                DueDate = DateTime.UtcNow.AddDays(10)
            };

            var pdfFromStorage = new byte[] { 0x25, 0x50, 0x44, 0x46 };

            _mockUow.Setup(u => u.Billets.GetByIdAsync(billetId)).ReturnsAsync(billet);
            _mockUow.Setup(u => u.Agreements.GetByIdAsync(agreementId)).ReturnsAsync(new Agreement
            {
                Id = agreementId,
                ContractId = contractId
            });
            _mockUow.Setup(u => u.Contracts.GetByIdAsync(contractId)).ReturnsAsync(new Contract { Id = contractId });
            _mockStorage
                .Setup(s => s.DownloadFileAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new MemoryStream(pdfFromStorage));

            var result = await _handler.Handle(new GetBilletPdfQuery(billetId), CancellationToken.None);

            result.Should().Equal(pdfFromStorage);
        }

        [Fact]
        public async Task Handle_GivenMissingBillet_ShouldThrowBilletNotFoundException()
        {
            _mockUow.Setup(u => u.Billets.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((Billet)null!);

            var act = async () => await _handler.Handle(new GetBilletPdfQuery(Guid.NewGuid()), CancellationToken.None);

            await act.Should().ThrowAsync<BilletNotFoundException>()
                .WithMessage("Boleto não encontrado.");
        }
    }
}
