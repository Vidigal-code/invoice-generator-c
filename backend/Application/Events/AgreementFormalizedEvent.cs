namespace InvoiceGenerator.Api.Application.Events
{
    public record AgreementFormalizedEvent
    {
        public Guid AgreementId { get; init; }
        public Guid ContractId { get; init; }
        public decimal TotalNegotiatedValue { get; init; }
        public int InstallmentsCount { get; init; }
        public DateTime FormalizedAt { get; init; }
    }
}
