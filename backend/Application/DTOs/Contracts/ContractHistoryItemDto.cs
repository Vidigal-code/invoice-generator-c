namespace InvoiceGenerator.Api.Application.DTOs.Contracts
{
    public sealed record ContractHistoryItemDto(
        Guid Id,
        DateTime OccurredAt,
        Guid? ChangedByUserId,
        string ChangeType,
        string PayloadJson);
}
