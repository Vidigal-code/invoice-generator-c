using System.Net;
using System.Net.Http.Headers;
using System.Text.Json;
using Xunit;
using FluentAssertions;
using InvoiceGenerator.Api.Infrastructure.Data.Context;
using InvoiceGenerator.Api.Domain.Entities;
using InvoiceGenerator.Api.Domain.Enums;
using InvoiceGenerator.Api.Application.DTOs;
using InvoiceGenerator.Api.Application.DTOs.Requests;
using Microsoft.EntityFrameworkCore;

namespace InvoiceGenerator.Api.Tests.API.IntegrationTests
{
    public class DebtControllerIntegrationTests : IClassFixture<InvoiceGeneratorApiFactory>
    {
        private readonly HttpClient _client;
        private readonly InvoiceGeneratorApiFactory _factory;

        public DebtControllerIntegrationTests(InvoiceGeneratorApiFactory factory)
        {
            _factory = factory;
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task CalculateDebt_GivenValidContractIdentifierAndToken_ShouldReturnProperDebtDTO()
        {
            // Arrange - Seed the InMemory Db
            var contractId = Guid.NewGuid();
            using (var scope = _factory.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                var admin = await db.Users.FirstAsync(u => u.Email == "admin@system.local");
                var contract = new Contract
                {
                    Id = contractId,
                    ContractNumber = "TEST-999",
                    OwnerUserId = admin.Id,
                    DebtorName = "Integration",
                    DebtorDocument = "52998224725",
                    OriginalValue = 100m,
                    CurrentBalance = 100m,
                    Portfolio = WalletPortfolio.InvoiceGeneratorC,
                    Status = ContractStatus.Active
                };
                db.Contracts.Add(contract);
                db.Installments.Add(new Installment { ContractId = contractId, InstallmentNumber = 1, 
                    OriginalValue = 100m, DueDate = DateTime.UtcNow.AddDays(-5), Status = InstallmentStatus.Open });
                await db.SaveChangesAsync();
            }

            // Generate an auth token for the test using the login endpoint with admin credentials. 
            // OR manually mock the bearer token. Let's do a fast login request to reuse the actual flow.
            
            var loginReq = new LoginRequest { Email = "admin@system.local", Password = "Admin@12345" };
            var loginResp = await _client.PostAsJsonAsync("/api/Auth/login", loginReq);
            loginResp.IsSuccessStatusCode.Should().BeTrue();
            
            var loginContent = await loginResp.Content.ReadAsStringAsync();
            var loginJson = JsonDocument.Parse(loginContent).RootElement;
            var tokenRaw = loginJson.GetProperty("data").GetProperty("token").GetString();

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokenRaw);

            // Act
            var response = await _client.GetAsync($"/api/Debt/{contractId}/calculate");

            // Assert
            response.IsSuccessStatusCode.Should().BeTrue();
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var debtJson = await response.Content.ReadAsStringAsync();
            var debtDataElement = JsonDocument.Parse(debtJson).RootElement.GetProperty("data");
            var responseData = JsonSerializer.Deserialize<DebtCalculationResultDto>(debtDataElement.GetRawText(),
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            responseData.Should().NotBeNull();
            responseData.ContractId.Should().Be(contractId);
            responseData.OriginalTotalDebt.Should().Be(100m);
            // Must have applied 2% fine + some Juros
            responseData.CurrentTotalDebt.Should().BeGreaterThan(102m); 
        }

        [Fact]
        public async Task CalculateDebt_WithoutAuthorization_ShouldReturnUnauthorized()
        {
            // Arrange
            _client.DefaultRequestHeaders.Authorization = null;
            
            // Act
            var response = await _client.GetAsync($"/api/Debt/{Guid.NewGuid()}/calculate");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }
    }
}
