using System.Net;
using FluentAssertions;
using Xunit;

namespace InvoiceGenerator.Api.Tests.API.IntegrationTests
{
    public sealed class AgreementsAuthzIntegrationTests : IClassFixture<InvoiceGeneratorApiFactory>
    {
        private readonly HttpClient _client;

        public AgreementsAuthzIntegrationTests(InvoiceGeneratorApiFactory factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task CreateAgreement_WithoutBearer_ShouldReturn401()
        {
            _client.DefaultRequestHeaders.Authorization = null;
            var body = new { contractId = Guid.NewGuid(), negotiatedValue = 100m, installmentsCount = 2 };
            var response = await _client.PostAsJsonAsync("/api/Agreements", body);
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }
    }
}
