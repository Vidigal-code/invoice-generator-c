using System.ComponentModel.DataAnnotations;

namespace InvoiceGenerator.Api.Application.DTOs.Requests
{
    public sealed class ResetPasswordRequest
    {
        [Required] public string NewPassword { get; set; } = string.Empty;
    }
}
