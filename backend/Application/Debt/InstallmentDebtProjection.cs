namespace InvoiceGenerator.Api.Application.Debt
{
    public readonly record struct InstallmentDebtProjection(
        decimal CurrentValue,
        decimal PenaltyAmount,
        decimal InterestAmount,
        int DaysOverdue);
}
