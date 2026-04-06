using InvoiceGenerator.Api.Domain.Enums;
using MediatR;

namespace InvoiceGenerator.Api.Application.Commands
{
    public sealed record CreateContractCommand(
        string ContractNumber,
        string DebtorName,
        string DebtorDocument,
        decimal OriginalValue,
        decimal CurrentBalance,
        WalletPortfolio Portfolio,
        ContractStatus Status,
        Guid? OwnerUserId,
        int InstallmentsCount,
        DateTime FirstDueDateUtc) : IRequest<Guid>;
}
