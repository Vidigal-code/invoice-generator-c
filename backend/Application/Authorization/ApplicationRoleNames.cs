using InvoiceGenerator.Api.Domain.Authorization;

namespace InvoiceGenerator.Api.Application.Authorization
{
    /// <summary>Implementação padrão alinhada a <see cref="ApplicationRoles"/>.</summary>
    public sealed class ApplicationRoleNames : IApplicationRoleNames
    {
        public string Admin => ApplicationRoles.Admin;
        public string User => ApplicationRoles.User;
    }
}
