using InvoiceGenerator.Api.Domain.Entities;
using InvoiceGenerator.Api.Domain.Interfaces;
using InvoiceGenerator.Api.Infrastructure.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace InvoiceGenerator.Api.Infrastructure.Repositories
{
    public class UnitOfWork : IUnitOfWork, IDisposable
    {
        private readonly AppDbContext _context;

        public UnitOfWork(AppDbContext context)
        {
            _context = context;
        }

        private IRepository<Contract>? _contracts;
        public IRepository<Contract> Contracts => _contracts ??= new GenericRepository<Contract>(_context);

        private IRepository<Installment>? _installments;
        public IRepository<Installment> Installments => _installments ??= new GenericRepository<Installment>(_context);

        private IRepository<Agreement>? _agreements;
        public IRepository<Agreement> Agreements => _agreements ??= new GenericRepository<Agreement>(_context);

        private IRepository<Billet>? _billets;
        public IRepository<Billet> Billets => _billets ??= new GenericRepository<Billet>(_context);

        private IRepository<AuditLog>? _auditLogs;
        public IRepository<AuditLog> AuditLogs => _auditLogs ??= new GenericRepository<AuditLog>(_context);

        private IRepository<User>? _users;
        public IRepository<User> Users => _users ??= new GenericRepository<User>(_context);

        private IRepository<Role>? _roles;
        public IRepository<Role> Roles => _roles ??= new GenericRepository<Role>(_context);

        private IRepository<ContractHistory>? _contractHistories;
        public IRepository<ContractHistory> ContractHistories =>
            _contractHistories ??= new GenericRepository<ContractHistory>(_context);

        public async Task<int> CommitAsync()
        {
            var strategy = _context.Database.CreateExecutionStrategy();
            return await strategy.ExecuteAsync(async () =>
            {
                using var transaction = await _context.Database.BeginTransactionAsync();
                try
                {
                    var result = await _context.SaveChangesAsync();
                    await transaction.CommitAsync();
                    return result;
                }
                catch (Exception)
                {
                    await transaction.RollbackAsync();
                    Rollback(); // Empties ChangeTracker memory
                    throw; // Retry/Fallback handled upper level
                }
            });
        }

        public int Commit()
        {
            var strategy = _context.Database.CreateExecutionStrategy();
            return strategy.Execute(() =>
            {
                using var transaction = _context.Database.BeginTransaction();
                try
                {
                    var result = _context.SaveChanges();
                    transaction.Commit();
                    return result;
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    Rollback();
                    throw;
                }
            });
        }

        public void Rollback()
        {
            // Failsafe Memory Fallback: Detach modified entities locally.
            foreach (var entry in _context.ChangeTracker.Entries())
            {
                entry.State = Microsoft.EntityFrameworkCore.EntityState.Unchanged;
            }
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
