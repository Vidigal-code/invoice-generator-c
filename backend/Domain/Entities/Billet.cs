namespace InvoiceGenerator.Api.Domain.Entities
{
    public class Billet : BaseEntity
    {
        public Guid AgreementId { get; set; }
        public int InstallmentNumber { get; set; }
        public DateTime DueDate { get; set; }
        public decimal Value { get; set; }
        public string Barcode { get; set; } = string.Empty;
        public string? PdfUrl { get; set; }
        public InvoiceGenerator.Api.Domain.Enums.BilletStatus Status { get; set; } = InvoiceGenerator.Api.Domain.Enums.BilletStatus.Pending;

        public Agreement? Agreement { get; set; }
    }
}
