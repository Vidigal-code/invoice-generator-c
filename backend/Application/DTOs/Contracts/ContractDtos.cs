namespace InvoiceGenerator.Api.Application.DTOs.Contracts
{
    public sealed record ContractListItemDto(
        Guid Id,
        Guid? OwnerUserId,
        string ContractNumber,
        string DebtorName,
        string DebtorDocument,
        decimal OriginalValue,
        decimal CurrentBalance,
        string Portfolio,
        string Status);

    public sealed record ContractDetailDto(
        Guid Id,
        Guid? OwnerUserId,
        string ContractNumber,
        string DebtorName,
        string DebtorDocument,
        decimal OriginalValue,
        decimal CurrentBalance,
        string Portfolio,
        string Status);
}
