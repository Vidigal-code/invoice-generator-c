using InvoiceGenerator.Api.Application.Abstractions;
using InvoiceGenerator.Api.Application.Constants;
using InvoiceGenerator.Api.Application.DTOs;
using InvoiceGenerator.Api.Application.Events;
using InvoiceGenerator.Api.Application.Exceptions;
using InvoiceGenerator.Api.Domain.Entities;
using InvoiceGenerator.Api.Domain.Enums;
using InvoiceGenerator.Api.Domain.Interfaces;
using InvoiceGenerator.Api.Application.Services.Contracts;
using InvoiceGenerator.Api.Infrastructure.External.Storage;
using MassTransit;
using MediatR;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace InvoiceGenerator.Api.Application.Commands
{
    public class CreateAgreementCommandHandler : IRequestHandler<CreateAgreementCommand, AgreementDto>
    {
        private readonly IUnitOfWork _uow;
        private readonly IPublishEndpoint _publishEndpoint;
        private readonly IFileStorageService _fileStorageService;
        private readonly IDistributedLock _distributedLock;
        private readonly IContractAccessService _contractAccess;

        public CreateAgreementCommandHandler(
            IUnitOfWork uow,
            IPublishEndpoint publishEndpoint,
            IFileStorageService fileStorageService,
            IDistributedLock distributedLock,
            IContractAccessService contractAccess)
        {
            _uow = uow;
            _publishEndpoint = publishEndpoint;
            _fileStorageService = fileStorageService;
            _distributedLock = distributedLock;
            _contractAccess = contractAccess;
        }

        public async Task<AgreementDto> Handle(CreateAgreementCommand request, CancellationToken cancellationToken)
        {
            if (request.InstallmentsCount <= 0 || request.InstallmentsCount > NegotiationLimits.MaxInstallments)
                throw new ApiException(400, ApiResponseMessages.InvalidInstallmentsCount);

            await using var lease = await _distributedLock.TryAcquireAsync(
                $"formalize:{request.ContractId}",
                TimeSpan.FromSeconds(45),
                cancellationToken).ConfigureAwait(false);
            if (lease == null)
                throw new ApiException(409, ApiResponseMessages.ContractFormalizationLocked);

            var contract = await _uow.Contracts.GetByIdAsync(request.ContractId).ConfigureAwait(false);
            _contractAccess.EnsureCanAccessContract(contract);

            var existingAgreement = await _uow.Agreements
                .FindAsync(a => a.ContractId == request.ContractId && a.Status == AgreementStatus.Active)
                .ConfigureAwait(false);
            if (existingAgreement.Any())
                throw new ApiException(400, ApiResponseMessages.AgreementAlreadyActive);

            var agreement = await CreateAgreementEntityAsync(request, contract!.Id).ConfigureAwait(false);
            var installmentValue = Math.Round(request.NegotiatedValue / request.InstallmentsCount, 2);
            QuestPDF.Settings.License = LicenseType.Community;

            for (var i = 1; i <= request.InstallmentsCount; i++)
                await CreateBilletForInstallmentAsync(agreement, contract, request.InstallmentsCount, i, installmentValue, cancellationToken)
                    .ConfigureAwait(false);

            await FinalizeContractAndPublishAsync(agreement, contract, cancellationToken).ConfigureAwait(false);

            return new AgreementDto
            {
                Id = agreement.Id,
                ContractId = agreement.ContractId,
                TotalNegotiatedValue = agreement.TotalNegotiatedValue,
                InstallmentsCount = agreement.InstallmentsCount,
                Status = agreement.Status
            };
        }

        private async Task<Agreement> CreateAgreementEntityAsync(CreateAgreementCommand request, Guid contractId)
        {
            var agreement = new Agreement
            {
                ContractId = contractId,
                TotalNegotiatedValue = request.NegotiatedValue,
                InstallmentsCount = request.InstallmentsCount,
                Status = AgreementStatus.Active,
                CreatedAt = DateTime.UtcNow
            };
            await _uow.Agreements.AddAsync(agreement).ConfigureAwait(false);
            return agreement;
        }

        private async Task CreateBilletForInstallmentAsync(
            Agreement agreement,
            Contract contract,
            int totalInstallments,
            int index,
            decimal installmentValue,
            CancellationToken cancellationToken)
        {
            var barcode = GenerateDummyBarcode();
            var dueDate = DateTime.UtcNow.Date.AddDays(30 * index);
            var pdfBytes = BuildBilletPdf(agreement, contract, totalInstallments, index, installmentValue, dueDate, barcode);

            await using var stream = new MemoryStream(pdfBytes);
            var fileName = $"billets/{agreement.Id}/installment_{index}.pdf";
            var s3Url = await TryUploadPdfAsync(fileName, stream, cancellationToken).ConfigureAwait(false);

            var billet = new Billet
            {
                AgreementId = agreement.Id,
                InstallmentNumber = index,
                DueDate = dueDate,
                Value = installmentValue,
                Barcode = barcode,
                PdfUrl = s3Url,
                Status = BilletStatus.Pending,
                CreatedAt = DateTime.UtcNow
            };
            await _uow.Billets.AddAsync(billet).ConfigureAwait(false);
        }

        private static byte[] BuildBilletPdf(
            Agreement agreement,
            Contract contract,
            int totalInstallments,
            int index,
            decimal installmentValue,
            DateTime dueDate,
            string barcode)
        {
            return Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(2, QuestPDF.Infrastructure.Unit.Centimetre);
                    page.Header().Column(col =>
                    {
                        col.Item().Text("invoice-generator-c — cobrança").SemiBold().FontSize(20).FontColor(Colors.Blue.Darken3);
                        col.Item().Text("Boleto bancário — parcela do acordo").FontSize(11).FontColor(Colors.Grey.Darken1);
                        col.Item().PaddingTop(5).LineHorizontal(1).LineColor(Colors.Blue.Darken2);
                    });
                    page.Content().PaddingVertical(1, QuestPDF.Infrastructure.Unit.Centimetre).Column(x =>
                    {
                        x.Item().PaddingBottom(10).Row(row =>
                        {
                            row.RelativeItem().Column(c =>
                            {
                                c.Item().Text("DADOS DO CONTRATO").Bold().FontSize(11).FontColor(Colors.Blue.Darken2);
                                c.Item().Text($"Número: {contract.ContractNumber}");
                                c.Item().Text($"Devedor: {contract.DebtorName}");
                                c.Item().Text($"Documento: {contract.DebtorDocument}");
                                c.Item().Text($"Carteira: {WalletPortfolioJson.ToJsonValue(contract.Portfolio)}");
                            });
                            row.RelativeItem().Column(c =>
                            {
                                c.Item().Text("DADOS DO BOLETO").Bold().FontSize(11).FontColor(Colors.Blue.Darken2);
                                c.Item().Text($"Parcela: {index} de {totalInstallments}");
                                c.Item().Text($"Vencimento: {dueDate:dd/MM/yyyy}").Bold();
                                c.Item().Text($"Valor: R$ {installmentValue:N2}").Bold().FontSize(14).FontColor(Colors.Green.Darken2);
                            });
                        });
                        x.Item().LineHorizontal(1).LineColor(Colors.Grey.Lighten1);
                        x.Item().PaddingTop(15).Text("LINHA DIGITÁVEL").Bold().FontSize(11).FontColor(Colors.Blue.Darken2);
                        x.Item().Text(barcode).FontSize(14).FontColor(Colors.Black);
                        x.Item().PaddingTop(10).Text($"Acordo: {agreement.Id}").FontSize(9).FontColor(Colors.Grey.Darken1);
                    });
                    page.Footer().AlignCenter().Text(t =>
                    {
                        t.Span("Documento gerado eletronicamente em ").FontSize(9).FontColor(Colors.Grey.Darken1);
                        t.Span($"{DateTime.UtcNow:dd/MM/yyyy HH:mm} UTC").FontSize(9).FontColor(Colors.Grey.Darken1);
                    });
                });
            }).GeneratePdf();
        }

        private async Task<string> TryUploadPdfAsync(string fileName, Stream stream, CancellationToken cancellationToken)
        {
            try
            {
                stream.Position = 0;
                return await _fileStorageService.UploadFileAsync(fileName, stream, "application/pdf", cancellationToken)
                    .ConfigureAwait(false);
            }
            catch
            {
                return $"https://s3.local-mock.com/invoice-generator-c-boletos/{fileName}";
            }
        }

        private async Task FinalizeContractAndPublishAsync(Agreement agreement, Contract contract, CancellationToken cancellationToken)
        {
            contract.Status = ContractStatus.Negotiated;
            await _uow.Contracts.UpdateAsync(contract).ConfigureAwait(false);

            await _publishEndpoint.Publish(new AgreementFormalizedEvent
            {
                AgreementId = agreement.Id,
                ContractId = contract.Id,
                TotalNegotiatedValue = agreement.TotalNegotiatedValue,
                InstallmentsCount = agreement.InstallmentsCount,
                FormalizedAt = DateTime.UtcNow
            }, cancellationToken).ConfigureAwait(false);
        }

        private static string GenerateDummyBarcode()
        {
            var random = new Random();
            return $"{random.Next(100, 999)}9.{random.Next(10000, 99999)} {random.Next(10000, 99999)}.{random.Next(100000, 999999)} 9 {random.Next(1000, 9999)}000000";
        }
    }
}
