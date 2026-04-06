namespace InvoiceGenerator.Api.Domain.Entities
{
    public class Installment : BaseEntity
    {
        public Guid ContractId { get; set; }
        public int InstallmentNumber { get; set; }
        public DateTime DueDate { get; set; }
        public decimal OriginalValue { get; set; }
        public decimal CurrentValue { get; set; }
        public InvoiceGenerator.Api.Domain.Enums.InstallmentStatus Status { get; set; } = InvoiceGenerator.Api.Domain.Enums.InstallmentStatus.Open;

        public Contract? Contract { get; set; }
    }
}
