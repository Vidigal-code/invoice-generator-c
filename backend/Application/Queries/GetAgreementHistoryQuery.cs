using MediatR;
using InvoiceGenerator.Api.Application.DTOs.Agreements;

namespace InvoiceGenerator.Api.Application.Queries
{
    public sealed record GetAgreementHistoryQuery : IRequest<IReadOnlyList<AgreementHistoryItemDto>>;
}
