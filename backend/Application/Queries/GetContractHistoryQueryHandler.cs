using InvoiceGenerator.Api.Application.DTOs.Contracts;
using InvoiceGenerator.Api.Application.Exceptions;
using InvoiceGenerator.Api.Application.Constants;
using InvoiceGenerator.Api.Domain.Interfaces;
using MediatR;

namespace InvoiceGenerator.Api.Application.Queries
{
    public sealed class GetContractHistoryQueryHandler : IRequestHandler<GetContractHistoryQuery, IReadOnlyList<ContractHistoryItemDto>>
    {
        private readonly IUnitOfWork _uow;

        public GetContractHistoryQueryHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<IReadOnlyList<ContractHistoryItemDto>> Handle(GetContractHistoryQuery request, CancellationToken cancellationToken)
        {
            var contract = await _uow.Contracts.GetByIdAsync(request.ContractId);
            if (contract == null)
                throw new ApiException(404, ApiResponseMessages.ContractNotFound);

            var rows = await _uow.ContractHistories.FindAsync(h => h.ContractId == request.ContractId);
            return rows
                .OrderByDescending(h => h.CreatedAt)
                .Select(h => new ContractHistoryItemDto(h.Id, h.CreatedAt, h.ChangedByUserId, h.ChangeType, h.PayloadJson))
                .ToList();
        }
    }
}
