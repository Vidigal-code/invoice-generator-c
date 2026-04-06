using InvoiceGenerator.Api.Application.Exceptions;
using InvoiceGenerator.Api.Application.Services.Billets;
using InvoiceGenerator.Api.Application.Services.Contracts;
using InvoiceGenerator.Api.Domain.Interfaces;
using MediatR;

namespace InvoiceGenerator.Api.Application.Queries
{
    public sealed class GetBilletPdfQueryHandler : IRequestHandler<GetBilletPdfQuery, byte[]>
    {
        private readonly IUnitOfWork _uow;
        private readonly IBilletStoragePdfRetriever _storagePdfRetriever;
        private readonly IBilletHtmlFallbackRenderer _htmlFallbackRenderer;
        private readonly IContractAccessService _contractAccess;

        public GetBilletPdfQueryHandler(
            IUnitOfWork uow,
            IBilletStoragePdfRetriever storagePdfRetriever,
            IBilletHtmlFallbackRenderer htmlFallbackRenderer,
            IContractAccessService contractAccess)
        {
            _uow = uow;
            _storagePdfRetriever = storagePdfRetriever;
            _htmlFallbackRenderer = htmlFallbackRenderer;
            _contractAccess = contractAccess;
        }

        public async Task<byte[]> Handle(GetBilletPdfQuery request, CancellationToken cancellationToken)
        {
            var billet = await _uow.Billets.GetByIdAsync(request.BilletId);
            if (billet == null)
                throw new BilletNotFoundException(request.BilletId);

            var agreement = await _uow.Agreements.GetByIdAsync(billet.AgreementId);
            if (agreement == null)
                throw new BilletNotFoundException(request.BilletId);

            var contract = await _uow.Contracts.GetByIdAsync(agreement.ContractId);
            _contractAccess.EnsureCanAccessContract(contract);

            var fromStorage = await _storagePdfRetriever.TryDownloadPdfAsync(billet, cancellationToken);
            if (fromStorage is { Length: > 0 })
                return fromStorage;

            return _htmlFallbackRenderer.RenderUtf8(billet);
        }
    }
}
