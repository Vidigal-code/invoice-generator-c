using System.Text;
using FluentAssertions;
using InvoiceGenerator.Api.Infrastructure.Configuration;
using InvoiceGenerator.Api.Infrastructure.Security;
using Xunit;

namespace InvoiceGenerator.Api.Tests.Infrastructure
{
    public sealed class AesGcmFilePayloadProtectorTests
    {
        [Fact]
        public void ProtectThenUnprotect_ShouldRoundTrip()
        {
            var settings = new AppSettings
            {
                JwtSettings = new JwtSettings { JweSecret = new string('A', 64) }
            };
            var protector = new AesGcmFilePayloadProtector(settings);
            var plain = Encoding.UTF8.GetBytes("invoice-generator-c — boleto PDF");
            var blob = protector.Protect(plain);
            blob.Should().NotBeEquivalentTo(plain);
            var back = protector.Unprotect(blob);
            back.Should().Equal(plain);
        }

        [Fact]
        public void Unprotect_WhenNotEncryptedPayload_ShouldReturnCopyUnchanged()
        {
            var settings = new AppSettings
            {
                JwtSettings = new JwtSettings { JweSecret = new string('B', 64) }
            };
            var protector = new AesGcmFilePayloadProtector(settings);
            var raw = Encoding.UTF8.GetBytes("plain-bytes");
            var back = protector.Unprotect(raw);
            back.Should().Equal(raw);
        }
    }
}
