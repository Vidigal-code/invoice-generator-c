using MediatR;
using InvoiceGenerator.Api.Application.Constants;
using InvoiceGenerator.Api.Application.Debt;
using InvoiceGenerator.Api.Application.DTOs;
using System.Text.Json;
using InvoiceGenerator.Api.Domain.Interfaces;
using InvoiceGenerator.Api.Domain.Enums;
using InvoiceGenerator.Api.Application.Services.Contracts;
using Microsoft.Extensions.Caching.Distributed;

namespace InvoiceGenerator.Api.Application.Queries
{
    public class GetDebtCalculationQueryHandler : IRequestHandler<GetDebtCalculationQuery, DebtCalculationResultDto>
    {
        private readonly IUnitOfWork _uow;
        private readonly IDistributedCache _cache;
        private readonly IDebtCalculationStrategyResolver _strategyResolver;
        private readonly IContractAccessService _contractAccess;

        public GetDebtCalculationQueryHandler(
            IUnitOfWork uow,
            IDistributedCache cache,
            IDebtCalculationStrategyResolver strategyResolver,
            IContractAccessService contractAccess)
        {
            _uow = uow;
            _cache = cache;
            _strategyResolver = strategyResolver;
            _contractAccess = contractAccess;
        }

        public async Task<DebtCalculationResultDto> Handle(GetDebtCalculationQuery request, CancellationToken cancellationToken)
        {
            var contract = await _contractAccess.RequireAccessibleContractAsync(request.ContractId, cancellationToken);

            var strategy = _strategyResolver.Resolve(contract.Portfolio);
            var installments = await _uow.Installments.FindAsync(i => i.ContractId == contract.Id && i.Status == InstallmentStatus.Open);

            var dto = new DebtCalculationResultDto
            {
                ContractId = contract.Id,
                ContractNumber = contract.ContractNumber,
                OriginalTotalDebt = installments.Sum(i => i.OriginalValue)
            };

            var referenceUtc = DateTime.UtcNow;
            var calculatedTotal = 0m;
            foreach (var inst in installments)
            {
                var projection = strategy.Project(inst, referenceUtc);
                calculatedTotal += projection.CurrentValue;

                dto.OpenInstallments.Add(new InstallmentDto
                {
                    Id = inst.Id,
                    InstallmentNumber = inst.InstallmentNumber,
                    DueDate = inst.DueDate,
                    OriginalValue = inst.OriginalValue,
                    CurrentValue = projection.CurrentValue,
                    DaysOverdue = projection.DaysOverdue
                });
            }

            dto.CurrentTotalDebt = calculatedTotal;

            var cacheKey = DebtCacheKeys.LatestCalculation(contract.Id);
            var cacheOptions = new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(24) };
            await _cache.SetStringAsync(cacheKey, JsonSerializer.Serialize(dto), cacheOptions, cancellationToken);

            return dto;
        }
    }
}
