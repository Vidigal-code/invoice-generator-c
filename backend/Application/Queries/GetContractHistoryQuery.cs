using InvoiceGenerator.Api.Application.DTOs.Contracts;
using MediatR;

namespace InvoiceGenerator.Api.Application.Queries
{
    public sealed record GetContractHistoryQuery(Guid ContractId) : IRequest<IReadOnlyList<ContractHistoryItemDto>>;
}
