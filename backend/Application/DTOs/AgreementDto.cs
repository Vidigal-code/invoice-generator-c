namespace InvoiceGenerator.Api.Application.DTOs
{
    public class AgreementDto
    {
        public Guid Id { get; set; }
        public Guid ContractId { get; set; }
        public decimal TotalNegotiatedValue { get; set; }
        public int InstallmentsCount { get; set; }
        public InvoiceGenerator.Api.Domain.Enums.AgreementStatus Status { get; set; } = InvoiceGenerator.Api.Domain.Enums.AgreementStatus.Pending;
    }
}
