using System.Text.Json.Serialization;

namespace InvoiceGenerator.Api.Domain.Enums
{
    public enum WalletPortfolio
    {
        [JsonStringEnumMemberName("invoice-generator-c")]
        InvoiceGeneratorC = 0
    }

    /// <summary>Valor literal da carteira em API, persistência e auditoria.</summary>
    public static class WalletPortfolioJson
    {
        public const string Value = "invoice-generator-c";

        public static string ToJsonValue(WalletPortfolio portfolio) => portfolio switch
        {
            WalletPortfolio.InvoiceGeneratorC => Value,
            _ => throw new ArgumentOutOfRangeException(nameof(portfolio))
        };

        public static WalletPortfolio Parse(string? stored) => stored switch
        {
            null or "" => throw new ArgumentException("Carteira ausente.", nameof(stored)),
            Value => WalletPortfolio.InvoiceGeneratorC,
            _ => throw new ArgumentException($"Carteira desconhecida: {stored}", nameof(stored))
        };
    }
}
