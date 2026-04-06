using InvoiceGenerator.Api.Application.DTOs.Common;
using InvoiceGenerator.Api.Application.DTOs.Contracts;
using InvoiceGenerator.Api.Application.DTOs.Requests;

namespace InvoiceGenerator.Api.Application.Services.Contracts
{
    public interface IContractService
    {
        Task<PagedResultDto<ContractListItemDto>> ListAsync(ContractListQuery query, CancellationToken cancellationToken = default);

        Task<ContractDetailDto> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

        Task UpdateAsync(Guid id, ContractUpdateRequest request, CancellationToken cancellationToken = default);

        Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    }
}
