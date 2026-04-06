using System.Text.Json;
using InvoiceGenerator.Api.Application.Abstractions;
using InvoiceGenerator.Api.Application.Constants;
using InvoiceGenerator.Api.Application.DTOs.Common;
using InvoiceGenerator.Api.Application.DTOs.Contracts;
using InvoiceGenerator.Api.Application.DTOs.Requests;
using InvoiceGenerator.Api.Application.Exceptions;
using InvoiceGenerator.Api.Domain.Entities;
using InvoiceGenerator.Api.Domain.Enums;
using InvoiceGenerator.Api.Domain.Interfaces;
using FluentValidation;
using InvoiceGenerator.Api.Domain.Validation;

namespace InvoiceGenerator.Api.Application.Services.Contracts
{
    public sealed class ContractService : IContractService
    {
        private readonly IUnitOfWork _uow;
        private readonly IAuditService _audit;
        private readonly ICurrentUserAccessor _currentUser;
        private readonly IContractAccessService _contractAccess;
        private readonly IValidator<ContractUpdateRequest> _updateValidator;

        public ContractService(
            IUnitOfWork uow,
            IAuditService audit,
            ICurrentUserAccessor currentUser,
            IContractAccessService contractAccess,
            IValidator<ContractUpdateRequest> updateValidator)
        {
            _uow = uow;
            _audit = audit;
            _currentUser = currentUser;
            _contractAccess = contractAccess;
            _updateValidator = updateValidator;
        }

        public async Task<PagedResultDto<ContractListItemDto>> ListAsync(ContractListQuery query, CancellationToken cancellationToken = default)
        {
            var all = (await _uow.Contracts.GetAllAsync()).AsEnumerable();

            if (!_currentUser.IsAdmin())
            {
                var uid = _currentUser.GetUserId();
                all = all.Where(c => c.OwnerUserId == uid);
            }

            if (query.ActiveOnly)
                all = all.Where(c => c.Status != ContractStatus.Cancelled);

            if (!string.IsNullOrWhiteSpace(query.Search))
            {
                var q = query.Search.ToLowerInvariant();
                all = all.Where(c =>
                    c.ContractNumber.ToLowerInvariant().Contains(q) ||
                    c.DebtorName.ToLowerInvariant().Contains(q) ||
                    c.DebtorDocument.ToLowerInvariant().Contains(q) ||
                    c.Id.ToString().ToLowerInvariant().Contains(q));
            }

            if (!string.IsNullOrWhiteSpace(query.Status) &&
                Enum.TryParse<ContractStatus>(query.Status, out var parsedStatus))
                all = all.Where(c => c.Status == parsedStatus);

            var list = all.ToList();
            var total = list.Count;
            var items = list
                .OrderByDescending(c => c.Id)
                .Skip((query.Page - 1) * query.Size)
                .Take(query.Size)
                .Select(c => new ContractListItemDto(
                    c.Id,
                    c.OwnerUserId,
                    c.ContractNumber,
                    c.DebtorName,
                    c.DebtorDocument,
                    c.OriginalValue,
                    c.CurrentBalance,
                    WalletPortfolioJson.ToJsonValue(c.Portfolio),
                    c.Status.ToString()))
                .ToList();

            return new PagedResultDto<ContractListItemDto>(total, query.Page, query.Size, items);
        }

        public async Task<ContractDetailDto> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var contract = await _uow.Contracts.GetByIdAsync(id);
            _contractAccess.EnsureCanAccessContract(contract);

            return new ContractDetailDto(
                contract!.Id,
                contract.OwnerUserId,
                contract.ContractNumber,
                contract.DebtorName,
                contract.DebtorDocument,
                contract.OriginalValue,
                contract.CurrentBalance,
                WalletPortfolioJson.ToJsonValue(contract.Portfolio),
                contract.Status.ToString());
        }

        public async Task UpdateAsync(Guid id, ContractUpdateRequest request, CancellationToken cancellationToken = default)
        {
            var contract = await _uow.Contracts.GetByIdAsync(id);
            if (contract == null)
                throw new ApiException(404, ApiResponseMessages.ContractNotFound);

            var vr = await _updateValidator.ValidateAsync(request, cancellationToken);
            if (!vr.IsValid)
                throw new ValidationException(vr.Errors);

            if (request.OwnerUserId.HasValue)
            {
                var owner = await _uow.Users.GetByIdAsync(request.OwnerUserId.Value);
                if (owner == null)
                    throw new ApiException(400, ApiResponseMessages.UserNotFound);
            }

            if (!string.Equals(request.ContractNumber, contract.ContractNumber, StringComparison.Ordinal))
            {
                var dup = await _uow.Contracts.FindAsync(c =>
                    c.ContractNumber == request.ContractNumber && c.Id != id);
                if (dup.Any())
                    throw new ApiException(400, ApiResponseMessages.ContractNumberExists);
            }

            var previous = new
            {
                contract.OwnerUserId,
                contract.ContractNumber,
                contract.DebtorName,
                contract.DebtorDocument,
                contract.OriginalValue,
                contract.CurrentBalance,
                Portfolio = WalletPortfolioJson.ToJsonValue(contract.Portfolio),
                Status = contract.Status.ToString()
            };

            var oldSnapshot =
                $"Owner:{contract.OwnerUserId}|Number:{contract.ContractNumber}|Debtor:{contract.DebtorName}|Value:{contract.OriginalValue}|" +
                $"Portfolio:{WalletPortfolioJson.ToJsonValue(contract.Portfolio)}|Status:{contract.Status}";

            contract.OwnerUserId = request.OwnerUserId;
            contract.ContractNumber = request.ContractNumber;
            contract.DebtorName = request.DebtorName;
            contract.DebtorDocument = CpfValidator.NormalizeDigits(request.DebtorDocument);
            contract.OriginalValue = request.OriginalValue;
            contract.CurrentBalance = request.CurrentBalance;
            contract.Portfolio = request.Portfolio;
            contract.Status = request.Status;

            await _uow.Contracts.UpdateAsync(contract);

            if (contract.Status == ContractStatus.Cancelled)
                await MarkAgreementsBrokenForContractAsync(id);

            var current = new
            {
                contract.OwnerUserId,
                contract.ContractNumber,
                contract.DebtorName,
                contract.DebtorDocument,
                contract.OriginalValue,
                contract.CurrentBalance,
                Portfolio = WalletPortfolioJson.ToJsonValue(contract.Portfolio),
                Status = contract.Status.ToString()
            };

            await AppendContractHistoryAsync(id, ContractHistoryChangeTypes.Updated, new { previous, current });

            var newSnapshot =
                $"Owner:{contract.OwnerUserId}|Number:{contract.ContractNumber}|Debtor:{contract.DebtorName}|" +
                $"Value:{contract.OriginalValue}|Portfolio:{WalletPortfolioJson.ToJsonValue(contract.Portfolio)}|Status:{contract.Status}";
            await _audit.WriteAsync(_currentUser.GetUserId(), "ContractUpdated", nameof(Contract), id.ToString(), oldSnapshot, newSnapshot,
                _currentUser.GetClientIpAddress());
        }

        public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var contract = await _uow.Contracts.GetByIdAsync(id);
            if (contract == null)
                throw new ApiException(404, ApiResponseMessages.ContractNotFound);

            var oldSnapshot = $"Status:{contract.Status}";
            contract.Status = ContractStatus.Cancelled;
            await _uow.Contracts.UpdateAsync(contract);

            await MarkAgreementsBrokenForContractAsync(id);

            await AppendContractHistoryAsync(id, ContractHistoryChangeTypes.Cancelled, new { previousStatus = oldSnapshot });

            await _audit.WriteAsync(_currentUser.GetUserId(), "ContractDeleted", nameof(Contract), id.ToString(), oldSnapshot, "Status:Cancelled",
                _currentUser.GetClientIpAddress());
        }

        private async Task AppendContractHistoryAsync(Guid contractId, string changeType, object payload)
        {
            await _uow.ContractHistories.AddAsync(new ContractHistory
            {
                ContractId = contractId,
                ChangedByUserId = _currentUser.GetUserId(),
                ChangeType = changeType,
                PayloadJson = JsonSerializer.Serialize(payload)
            });
        }

        private async Task MarkAgreementsBrokenForContractAsync(Guid contractId)
        {
            var list = await _uow.Agreements.FindAsync(a => a.ContractId == contractId);
            foreach (var ag in list.Where(a => a.Status != AgreementStatus.Completed))
            {
                ag.Status = AgreementStatus.Broken;
                await _uow.Agreements.UpdateAsync(ag);
            }
        }
    }
}
