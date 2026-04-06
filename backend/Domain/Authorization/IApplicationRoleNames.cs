namespace InvoiceGenerator.Api.Domain.Authorization
{
    /// <summary>
    /// Contrato com os nomes de perfis da aplicação (evita strings soltas em serviços e testes).
    /// </summary>
    public interface IApplicationRoleNames
    {
        string Admin { get; }
        string User { get; }
    }
}
