using InvoiceGenerator.Api.Domain.Entities;

namespace InvoiceGenerator.Api.Application.Services.Contracts
{
    /// <summary>Garante que o utilizador autenticado pode aceder ao contrato (admin: tudo; user: apenas <see cref="Contract.OwnerUserId"/>).</summary>
    public interface IContractAccessService
    {
        void EnsureCanAccessContract(Contract? contract);

        Task<Contract> RequireAccessibleContractAsync(Guid contractId, CancellationToken cancellationToken = default);
    }
}
