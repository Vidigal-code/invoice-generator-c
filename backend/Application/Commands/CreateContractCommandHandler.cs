using InvoiceGenerator.Api.Application.Abstractions;
using InvoiceGenerator.Api.Application.Constants;
using InvoiceGenerator.Api.Application.Exceptions;
using InvoiceGenerator.Api.Domain.Entities;
using InvoiceGenerator.Api.Domain.Enums;
using InvoiceGenerator.Api.Domain.Interfaces;
using InvoiceGenerator.Api.Domain.Validation;
using MediatR;

namespace InvoiceGenerator.Api.Application.Commands
{
    public sealed class CreateContractCommandHandler : IRequestHandler<CreateContractCommand, Guid>
    {
        private readonly IUnitOfWork _uow;
        private readonly ICurrentUserAccessor _currentUser;

        public CreateContractCommandHandler(IUnitOfWork uow, ICurrentUserAccessor currentUser)
        {
            _uow = uow;
            _currentUser = currentUser;
        }

        public async Task<Guid> Handle(CreateContractCommand request, CancellationToken cancellationToken)
        {
            if (!_currentUser.IsAdmin())
                throw new ApiException(403, ApiResponseMessages.ForbiddenContractOperation);

            var dup = await _uow.Contracts.FindAsync(c => c.ContractNumber == request.ContractNumber);
            if (dup.Any())
                throw new ApiException(400, ApiResponseMessages.ContractNumberExists);

            if (request.OwnerUserId.HasValue)
            {
                var owner = await _uow.Users.GetByIdAsync(request.OwnerUserId.Value);
                if (owner == null)
                    throw new ApiException(400, ApiResponseMessages.UserNotFound);
            }

            var contract = new Contract
            {
                ContractNumber = request.ContractNumber,
                DebtorName = request.DebtorName,
                DebtorDocument = CpfValidator.NormalizeDigits(request.DebtorDocument),
                OriginalValue = request.OriginalValue,
                CurrentBalance = request.CurrentBalance,
                Portfolio = request.Portfolio,
                Status = request.Status,
                OwnerUserId = request.OwnerUserId
            };

            await _uow.Contracts.AddAsync(contract);

            var n = request.InstallmentsCount;
            var per = Math.Round(request.OriginalValue / n, 2, MidpointRounding.AwayFromZero);
            var first = request.FirstDueDateUtc.Date;

            for (var i = 1; i <= n; i++)
            {
                var value = i == n ? request.OriginalValue - per * (n - 1) : per;
                await _uow.Installments.AddAsync(new Installment
                {
                    ContractId = contract.Id,
                    InstallmentNumber = i,
                    DueDate = first.AddMonths(i - 1),
                    OriginalValue = value,
                    CurrentValue = value,
                    Status = InstallmentStatus.Open
                });
            }

            return contract.Id;
        }
    }
}
