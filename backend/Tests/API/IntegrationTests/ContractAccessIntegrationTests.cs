using System.Net;
using System.Net.Http.Headers;
using FluentAssertions;
using InvoiceGenerator.Api.Application.DTOs.Requests;
using InvoiceGenerator.Api.Domain.Authorization;
using InvoiceGenerator.Api.Domain.Entities;
using InvoiceGenerator.Api.Domain.Enums;
using InvoiceGenerator.Api.Infrastructure.Data.Context;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace InvoiceGenerator.Api.Tests.API.IntegrationTests
{
    public sealed class ContractAccessIntegrationTests : IClassFixture<InvoiceGeneratorApiFactory>
    {
        private readonly HttpClient _client;
        private readonly InvoiceGeneratorApiFactory _factory;

        public ContractAccessIntegrationTests(InvoiceGeneratorApiFactory factory)
        {
            _factory = factory;
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task CalculateDebt_WhenContractOwnedByAnotherUser_ShouldReturnNotFound()
        {
            var contractId = Guid.NewGuid();
            var userAId = Guid.NewGuid();
            var userBId = Guid.NewGuid();
            const string password = "UserAbc1!";

            using (var scope = _factory.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                var userRole = await db.Roles.FirstAsync(r => r.Name == ApplicationRoles.User);

                db.Users.Add(new User
                {
                    Id = userAId,
                    Username = "access-user-a",
                    Email = "access-a@test.local",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword(password),
                    IsActive = true,
                    RoleId = userRole.Id
                });
                db.Users.Add(new User
                {
                    Id = userBId,
                    Username = "access-user-b",
                    Email = "access-b@test.local",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword(password),
                    IsActive = true,
                    RoleId = userRole.Id
                });

                db.Contracts.Add(new Contract
                {
                    Id = contractId,
                    ContractNumber = "TEST-ACCESS-ISO",
                    OwnerUserId = userBId,
                    DebtorName = "Other Owner",
                    DebtorDocument = "52998224725",
                    OriginalValue = 50m,
                    CurrentBalance = 50m,
                    Portfolio = WalletPortfolio.InvoiceGeneratorC,
                    Status = ContractStatus.Active
                });
                db.Installments.Add(new Installment
                {
                    ContractId = contractId,
                    InstallmentNumber = 1,
                    OriginalValue = 50m,
                    DueDate = DateTime.UtcNow.AddDays(-3),
                    Status = InstallmentStatus.Open
                });
                await db.SaveChangesAsync();
            }

            var loginResp = await _client.PostAsJsonAsync("/api/Auth/login", new LoginRequest
            {
                Email = "access-a@test.local",
                Password = password
            });
            loginResp.IsSuccessStatusCode.Should().BeTrue();
            var loginJson = System.Text.Json.JsonDocument.Parse(await loginResp.Content.ReadAsStringAsync()).RootElement;
            var token = loginJson.GetProperty("data").GetProperty("token").GetString();
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await _client.GetAsync($"/api/Debt/{contractId}/calculate");

            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }
    }
}
