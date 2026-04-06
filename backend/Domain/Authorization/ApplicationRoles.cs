namespace InvoiceGenerator.Api.Domain.Authorization
{
    /// <summary>
    /// Nomes de perfis persistidos em <c>Roles</c> e emitidos no JWT.
    /// Usar em <see cref="Microsoft.AspNetCore.Authorization.AuthorizeAttribute"/> (requer constante de compilação).
    /// </summary>
    public static class ApplicationRoles
    {
        public const string Admin = "Admin";
        public const string User = "User";
    }
}
