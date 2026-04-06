namespace InvoiceGenerator.Api.Domain.Entities
{
    public class ContractHistory : BaseEntity
    {
        public Guid ContractId { get; set; }
        public Guid? ChangedByUserId { get; set; }
        public string ChangeType { get; set; } = string.Empty;
        public string PayloadJson { get; set; } = string.Empty;

        public Contract? Contract { get; set; }
    }
}
