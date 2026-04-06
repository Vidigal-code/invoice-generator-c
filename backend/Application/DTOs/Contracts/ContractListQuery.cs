namespace InvoiceGenerator.Api.Application.DTOs.Contracts
{
    public sealed record ContractListQuery(
        string? Search,
        string? Status,
        bool ActiveOnly,
        int Page,
        int Size);
}
