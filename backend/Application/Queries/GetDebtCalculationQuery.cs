using MediatR;
using InvoiceGenerator.Api.Application.DTOs;

namespace InvoiceGenerator.Api.Application.Queries
{
    public class GetDebtCalculationQuery : IRequest<DebtCalculationResultDto>
    {
        public Guid ContractId { get; set; }

        public GetDebtCalculationQuery(Guid contractId)
        {
            ContractId = contractId;
        }
    }
}
