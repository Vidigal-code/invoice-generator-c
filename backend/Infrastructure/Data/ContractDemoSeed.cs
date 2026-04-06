using InvoiceGenerator.Api.Domain.Entities;
using InvoiceGenerator.Api.Domain.Enums;
using InvoiceGenerator.Api.Domain.Interfaces;

namespace InvoiceGenerator.Api.Infrastructure.Data;

/// <summary>Insere contratos de demonstração quando não existir nenhum (dev / primeira subida sem init.sql).</summary>
public static class ContractDemoSeed
{
    public static async Task SeedIfEmptyAsync(IUnitOfWork uow, CancellationToken cancellationToken = default)
    {
        var existing = await uow.Contracts.GetAllAsync();
        if (existing.Any())
            return;

        var utc = DateTime.UtcNow.Date;

        var legacy = new Contract
        {
            Id = Guid.Parse("11111111-1111-1111-1111-111111111111"),
            ContractNumber = "CT-2026-001",
            DebtorName = "João Silva",
            DebtorDocument = "52998224725",
            OriginalValue = 1500m,
            CurrentBalance = 1500m,
            Portfolio = WalletPortfolio.InvoiceGeneratorC,
            Status = ContractStatus.Active
        };
        await uow.Contracts.AddAsync(legacy);
        await AddInstallmentsAsync(uow, legacy.Id, new[]
        {
            (1, utc.AddMonths(1), 500m),
            (2, utc.AddMonths(2), 500m),
            (3, utc.AddMonths(3), 500m)
        });

        var c2 = new Contract
        {
            Id = Guid.Parse("22222222-2222-2222-2222-222222222222"),
            ContractNumber = "IGC-2026-100",
            DebtorName = "Maria Silva",
            DebtorDocument = "39053344705",
            OriginalValue = 2400m,
            CurrentBalance = 2400m,
            Portfolio = WalletPortfolio.InvoiceGeneratorC,
            Status = ContractStatus.Active
        };
        await uow.Contracts.AddAsync(c2);
        await AddInstallmentsAsync(uow, c2.Id, new[]
        {
            (1, utc.AddDays(-20), 800m),
            (2, utc.AddDays(-5), 800m),
            (3, utc.AddDays(30), 800m)
        });

        var c3 = new Contract
        {
            Id = Guid.Parse("33333333-3333-3333-3333-333333333333"),
            ContractNumber = "IGC-2026-101",
            DebtorName = "Carlos Negociação",
            DebtorDocument = "11144477735",
            OriginalValue = 900m,
            CurrentBalance = 900m,
            Portfolio = WalletPortfolio.InvoiceGeneratorC,
            Status = ContractStatus.Active
        };
        await uow.Contracts.AddAsync(c3);
        await AddInstallmentsAsync(uow, c3.Id, new[]
        {
            (1, utc.AddDays(-45), 450m),
            (2, utc.AddDays(10), 450m)
        });
    }

    private static async Task AddInstallmentsAsync(IUnitOfWork uow, Guid contractId, (int N, DateTime Due, decimal Value)[] rows)
    {
        foreach (var (n, due, value) in rows)
        {
            await uow.Installments.AddAsync(new Installment
            {
                ContractId = contractId,
                InstallmentNumber = n,
                DueDate = due,
                OriginalValue = value,
                CurrentValue = value,
                Status = InstallmentStatus.Open
            });
        }
    }
}
