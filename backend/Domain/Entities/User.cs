namespace InvoiceGenerator.Api.Domain.Entities
{
    public class User : BaseEntity
    {
        public Guid RoleId { get; set; }
        public string Username { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;

        public Role? Role { get; set; }
        public ICollection<Contract> OwnedContracts { get; set; } = new List<Contract>();
    }
}
