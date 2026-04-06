namespace InvoiceGenerator.Api.Application.DTOs.Common
{
    public sealed record PagedResultDto<T>(int Total, int Page, int Size, IReadOnlyList<T> Items);
}
