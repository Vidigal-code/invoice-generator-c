using MediatR;
using InvoiceGenerator.Api.Application.DTOs;

namespace InvoiceGenerator.Api.Application.Commands
{
    public class CreateAgreementCommand : IRequest<AgreementDto>
    {
        public Guid ContractId { get; set; }
        public decimal NegotiatedValue { get; set; }
        public int InstallmentsCount { get; set; }

        public CreateAgreementCommand(Guid contractId, decimal negotiatedValue, int installmentsCount)
        {
            ContractId = contractId;
            NegotiatedValue = negotiatedValue;
            InstallmentsCount = installmentsCount;
        }
    }
}
