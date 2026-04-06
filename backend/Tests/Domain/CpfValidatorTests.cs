using FluentAssertions;
using InvoiceGenerator.Api.Domain.Validation;
using Xunit;

namespace InvoiceGenerator.Api.Tests.Domain
{
    public sealed class CpfValidatorTests
    {
        [Theory]
        [InlineData("529.982.247-25", true)]
        [InlineData("52998224725", true)]
        [InlineData("11144477735", true)]
        [InlineData("00000000000", false)]
        [InlineData("11111111111", false)]
        [InlineData("12345678901", false)]
        [InlineData("1234567890", false)]
        [InlineData("", false)]
        [InlineData(null, false)]
        public void IsValid_ShouldMatchExpected(string? raw, bool expected)
        {
            CpfValidator.IsValid(raw).Should().Be(expected);
        }

        [Fact]
        public void NormalizeDigits_ShouldStripNonDigits()
        {
            CpfValidator.NormalizeDigits(" 529.982.247-25 ").Should().Be("52998224725");
        }
    }
}
