using Microsoft.EntityFrameworkCore;
using InvoiceGenerator.Api.Domain.Entities;
using InvoiceGenerator.Api.Domain.Enums;

namespace InvoiceGenerator.Api.Infrastructure.Data.Context
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<User> Users => Set<User>();
        public DbSet<Role> Roles => Set<Role>();
        public DbSet<AuditLog> AuditLogs => Set<AuditLog>();
        public DbSet<Contract> Contracts => Set<Contract>();
        public DbSet<Installment> Installments => Set<Installment>();
        public DbSet<Agreement> Agreements => Set<Agreement>();
        public DbSet<Billet> Billets => Set<Billet>();
        public DbSet<ContractHistory> ContractHistories => Set<ContractHistory>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            
            // Decimal precision configs
            foreach (var property in modelBuilder.Model.GetEntityTypes()
                .SelectMany(t => t.GetProperties())
                .Where(p => p.ClrType == typeof(decimal) || p.ClrType == typeof(decimal?)))
            {
                property.SetPrecision(18);
                property.SetScale(2);
            }
            
            // Enum to String Conversion Interfaces (Enterprise Database strictness)
            modelBuilder.Entity<Installment>().Property(e => e.Status).HasConversion<string>();
            modelBuilder.Entity<Contract>().Property(e => e.Status).HasConversion<string>();
            modelBuilder.Entity<Contract>().Property(e => e.Portfolio).HasConversion(
                v => WalletPortfolioJson.ToJsonValue(v),
                v => WalletPortfolioJson.Parse(v));
            modelBuilder.Entity<Agreement>().Property(e => e.Status).HasConversion<string>();
            modelBuilder.Entity<Billet>().Property(e => e.Status).HasConversion<string>();

            // AuditLog table does not have UpdatedAt/CreatedAt columns — ignore BaseEntity timestamp cols
            modelBuilder.Entity<AuditLog>().Ignore(e => e.UpdatedAt).Ignore(e => e.CreatedAt);
            modelBuilder.Entity<ContractHistory>().Ignore(e => e.UpdatedAt);

            modelBuilder.Entity<Contract>()
                .HasOne(c => c.OwnerUser)
                .WithMany(u => u.OwnedContracts)
                .HasForeignKey(c => c.OwnerUserId)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}
