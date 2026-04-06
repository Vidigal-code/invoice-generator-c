namespace InvoiceGenerator.Api.Application.DTOs
{
    public class DebtCalculationResultDto
    {
        public Guid ContractId { get; set; }
        public string ContractNumber { get; set; } = string.Empty;
        public decimal OriginalTotalDebt { get; set; }
        public decimal CurrentTotalDebt { get; set; }
        public List<InstallmentDto> OpenInstallments { get; set; } = new List<InstallmentDto>();
    }

    public class InstallmentDto
    {
        public Guid Id { get; set; }
        public int InstallmentNumber { get; set; }
        public DateTime DueDate { get; set; }
        public decimal OriginalValue { get; set; }
        public decimal CurrentValue { get; set; }
        public int DaysOverdue { get; set; }
    }
}
