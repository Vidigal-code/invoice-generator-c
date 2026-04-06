using FluentAssertions;
using InvoiceGenerator.Api.Application.Validation;
using Xunit;

namespace InvoiceGenerator.Api.Tests.Application
{
    public sealed class StrongPasswordPolicyTests
    {
        [Theory]
        [InlineData("Abcd123!", true)]
        [InlineData("Admin@12345", true)]
        [InlineData("short1!", false)]
        [InlineData("noupper1!", false)]
        [InlineData("NOLOWER1!", false)]
        [InlineData("NoDigit!!", false)]
        [InlineData("NoSpecial1", false)]
        [InlineData("", false)]
        [InlineData(null, false)]
        public void IsSatisfied_ShouldMatchExpected(string? password, bool expected)
        {
            StrongPasswordPolicy.IsSatisfied(password).Should().Be(expected);
        }
    }
}
