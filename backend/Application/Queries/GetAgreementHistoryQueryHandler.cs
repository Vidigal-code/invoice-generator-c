using InvoiceGenerator.Api.Application.Abstractions;
using InvoiceGenerator.Api.Application.DTOs.Agreements;
using InvoiceGenerator.Api.Domain.Enums;
using InvoiceGenerator.Api.Domain.Interfaces;
using MediatR;

namespace InvoiceGenerator.Api.Application.Queries
{
    public sealed class GetAgreementHistoryQueryHandler : IRequestHandler<GetAgreementHistoryQuery, IReadOnlyList<AgreementHistoryItemDto>>
    {
        private readonly IUnitOfWork _uow;
        private readonly ICurrentUserAccessor _currentUser;

        public GetAgreementHistoryQueryHandler(IUnitOfWork uow, ICurrentUserAccessor currentUser)
        {
            _uow = uow;
            _currentUser = currentUser;
        }

        public async Task<IReadOnlyList<AgreementHistoryItemDto>> Handle(GetAgreementHistoryQuery request, CancellationToken cancellationToken)
        {
            var agreements = (await _uow.Agreements.GetAllAsync()).AsEnumerable();
            var contracts = (await _uow.Contracts.GetAllAsync()).AsEnumerable();
            var billets = await _uow.Billets.GetAllAsync();

            if (!_currentUser.IsAdmin())
            {
                var uid = _currentUser.GetUserId();
                contracts = contracts.Where(c => c.OwnerUserId == uid);
                var allowed = contracts.Select(c => c.Id).ToHashSet();
                agreements = agreements.Where(a => allowed.Contains(a.ContractId));
            }

            var contractsList = contracts.ToList();
            var agreementsList = agreements.ToList();

            var list = agreementsList.Select(a =>
            {
                var contract = contractsList.FirstOrDefault(c => c.Id == a.ContractId);
                var firstBillet = billets.FirstOrDefault(b => b.AgreementId == a.Id);
                var contractStatus = contract?.Status.ToString();
                var effectiveStatus = a.Status.ToString();
                if (contract?.Status == ContractStatus.Cancelled && a.Status != AgreementStatus.Completed)
                    effectiveStatus = $"{a.Status} (contrato cancelado)";

                return new AgreementHistoryItemDto(
                    a.Id,
                    a.ContractId,
                    contract?.ContractNumber,
                    contractStatus,
                    a.TotalNegotiatedValue,
                    a.InstallmentsCount,
                    a.Status.ToString(),
                    effectiveStatus,
                    a.CreatedAt,
                    firstBillet?.Id);
            }).OrderByDescending(x => x.Date).ToList();

            return list;
        }
    }
}
