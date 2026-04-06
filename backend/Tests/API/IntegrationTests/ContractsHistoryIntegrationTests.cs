using System.Net;
using System.Net.Http.Headers;
using System.Text.Json;
using FluentAssertions;
using InvoiceGenerator.Api.Application.Constants;
using InvoiceGenerator.Api.Application.DTOs.Requests;
using InvoiceGenerator.Api.Domain.Entities;
using InvoiceGenerator.Api.Infrastructure.Data.Context;
using Xunit;

namespace InvoiceGenerator.Api.Tests.API.IntegrationTests
{
    public sealed class ContractsHistoryIntegrationTests : IClassFixture<InvoiceGeneratorApiFactory>
    {
        private readonly HttpClient _client;
        private readonly InvoiceGeneratorApiFactory _factory;

        public ContractsHistoryIntegrationTests(InvoiceGeneratorApiFactory factory)
        {
            _factory = factory;
            _client = factory.CreateClient();
        }

        private async Task<string> LoginAdminAsync()
        {
            var loginReq = new LoginRequest { Email = "admin@system.local", Password = "Admin@12345" };
            var loginResp = await _client.PostAsJsonAsync("/api/Auth/login", loginReq);
            loginResp.IsSuccessStatusCode.Should().BeTrue();
            var loginJson = JsonDocument.Parse(await loginResp.Content.ReadAsStringAsync()).RootElement;
            return loginJson.GetProperty("data").GetProperty("token").GetString()!;
        }

        [Fact]
        public async Task GetContractHistory_AsAdmin_ShouldReturnRows()
        {
            var contractId = Guid.NewGuid();
            using (var scope = _factory.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                db.Contracts.Add(new Contract { Id = contractId, ContractNumber = "HIST-INT-1" });
                db.ContractHistories.Add(new ContractHistory
                {
                    Id = Guid.NewGuid(),
                    ContractId = contractId,
                    ChangeType = ContractHistoryChangeTypes.Updated,
                    PayloadJson = "{\"test\":true}",
                    CreatedAt = DateTime.UtcNow
                });
                await db.SaveChangesAsync();
            }

            var token = await LoginAdminAsync();
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await _client.GetAsync($"/api/Contracts/{contractId}/history");
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var json = JsonDocument.Parse(await response.Content.ReadAsStringAsync()).RootElement;
            var data = json.GetProperty("data");
            data.GetArrayLength().Should().BeGreaterThan(0);
        }

        [Fact]
        public async Task GetContractHistory_WithoutAuth_ShouldReturn401()
        {
            _client.DefaultRequestHeaders.Authorization = null;
            var response = await _client.GetAsync($"/api/Contracts/{Guid.NewGuid()}/history");
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }
    }
}
