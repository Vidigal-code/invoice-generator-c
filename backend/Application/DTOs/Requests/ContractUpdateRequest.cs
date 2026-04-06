using System.ComponentModel.DataAnnotations;
using InvoiceGenerator.Api.Domain.Enums;

namespace InvoiceGenerator.Api.Application.DTOs.Requests
{
    public sealed class ContractUpdateRequest
    {
        public Guid? OwnerUserId { get; set; }

        [Required] public string ContractNumber { get; set; } = string.Empty;
        [Required] public string DebtorName { get; set; } = string.Empty;
        [Required] public string DebtorDocument { get; set; } = string.Empty;
        [Required] public decimal OriginalValue { get; set; }
        [Required] public decimal CurrentBalance { get; set; }
        [Required] public WalletPortfolio Portfolio { get; set; } = WalletPortfolio.InvoiceGeneratorC;
        [Required] public ContractStatus Status { get; set; }
    }
}
