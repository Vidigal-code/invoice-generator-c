namespace InvoiceGenerator.Api.Application.DTOs.Agreements
{
    public sealed record AgreementHistoryItemDto(
        Guid Id,
        Guid ContractId,
        string? ContractNumber,
        string? ContractStatus,
        decimal TotalValue,
        int Installments,
        string Status,
        string EffectiveStatus,
        DateTime Date,
        Guid? BilletId);
}
