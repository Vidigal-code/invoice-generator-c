using System.ComponentModel.DataAnnotations;

namespace InvoiceGenerator.Api.Application.DTOs.Requests
{
    public sealed class UserUpdateRequest
    {
        [Required] public string Username { get; set; } = string.Empty;
        [Required][EmailAddress] public string Email { get; set; } = string.Empty;
        [Required] public bool IsActive { get; set; }
        [Required] public Guid RoleId { get; set; }
    }
}
