namespace InvoiceGenerator.Api.Domain.Entities
{
    public class Agreement : BaseEntity
    {
        public Guid ContractId { get; set; }
        public decimal TotalNegotiatedValue { get; set; }
        public int InstallmentsCount { get; set; }
        public InvoiceGenerator.Api.Domain.Enums.AgreementStatus Status { get; set; } = InvoiceGenerator.Api.Domain.Enums.AgreementStatus.Pending;

        public Contract? Contract { get; set; }
        public ICollection<Billet> Billets { get; set; } = new List<Billet>();
    }
}
