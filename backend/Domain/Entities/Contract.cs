namespace InvoiceGenerator.Api.Domain.Entities
{
    public class Contract : BaseEntity
    {
        /// <summary>Utilizador (devedor) que vê este contrato no dashboard; null = apenas administradores.</summary>
        public Guid? OwnerUserId { get; set; }
        public User? OwnerUser { get; set; }

        public string ContractNumber { get; set; } = string.Empty;
        public string DebtorName { get; set; } = string.Empty;
        public string DebtorDocument { get; set; } = string.Empty;
        public decimal OriginalValue { get; set; }
        public decimal CurrentBalance { get; set; }
        public InvoiceGenerator.Api.Domain.Enums.WalletPortfolio Portfolio { get; set; } = InvoiceGenerator.Api.Domain.Enums.WalletPortfolio.InvoiceGeneratorC;
        public InvoiceGenerator.Api.Domain.Enums.ContractStatus Status { get; set; } = InvoiceGenerator.Api.Domain.Enums.ContractStatus.Active;

        public ICollection<Installment> Installments { get; set; } = new List<Installment>();
        public ICollection<Agreement> Agreements { get; set; } = new List<Agreement>();
    }
}
