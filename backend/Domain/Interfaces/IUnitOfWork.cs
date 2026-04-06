using InvoiceGenerator.Api.Domain.Entities;

namespace InvoiceGenerator.Api.Domain.Interfaces
{
    public interface IUnitOfWork
    {
        IRepository<Contract> Contracts { get; }
        IRepository<Installment> Installments { get; }
        IRepository<Agreement> Agreements { get; }
        IRepository<Billet> Billets { get; }
        IRepository<AuditLog> AuditLogs { get; }
        IRepository<User> Users { get; }
        IRepository<Role> Roles { get; }
        IRepository<ContractHistory> ContractHistories { get; }
        Task<int> CommitAsync();
        int Commit();
        void Rollback();
    }
}
