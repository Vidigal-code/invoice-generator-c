using InvoiceGenerator.Api.Application.Abstractions;
using InvoiceGenerator.Api.Application.Constants;
using InvoiceGenerator.Api.Application.Exceptions;
using InvoiceGenerator.Api.Domain.Entities;
using InvoiceGenerator.Api.Domain.Interfaces;

namespace InvoiceGenerator.Api.Application.Services.Contracts
{
    public sealed class ContractAccessService : IContractAccessService
    {
        private readonly IUnitOfWork _uow;
        private readonly ICurrentUserAccessor _currentUser;

        public ContractAccessService(IUnitOfWork uow, ICurrentUserAccessor currentUser)
        {
            _uow = uow;
            _currentUser = currentUser;
        }

        public void EnsureCanAccessContract(Contract? contract)
        {
            if (contract == null)
                throw new ApiException(404, ApiResponseMessages.ContractNotFound);

            if (_currentUser.IsAdmin())
                return;

            var userId = _currentUser.GetUserId();
            if (userId == null || contract.OwnerUserId != userId)
                throw new ApiException(404, ApiResponseMessages.ContractNotFound);
        }

        public async Task<Contract> RequireAccessibleContractAsync(Guid contractId, CancellationToken cancellationToken = default)
        {
            var contract = await _uow.Contracts.GetByIdAsync(contractId);
            EnsureCanAccessContract(contract);
            return contract!;
        }
    }
}
