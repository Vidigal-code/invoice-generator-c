namespace InvoiceGenerator.Api.Infrastructure.Configuration
{
    public class AppSettings
    {
        public SecuritySettings Security { get; set; } = new();
        public JwtSettings JwtSettings { get; set; } = new();
        public AdminSettings AdminSettings { get; set; } = new();
        public RateLimitSettings RateLimit { get; set; } = new();
        public RedisSettings Redis { get; set; } = new();
        public RabbitMQSettings RabbitMQ { get; set; } = new();
        public ElasticSearchSettings ElasticSearch { get; set; } = new();
        public AWSSettings AWS { get; set; } = new();
        public LoggingSettings Logging { get; set; } = new();
        public DebtCalculationSettings DebtCalculation { get; set; } = new();
    }

    public class DebtCalculationSettings
    {
        [ConfigurationKeyName("invoice-generator-c")]
        public WalletDebtRules InvoiceGeneratorC { get; set; } = new();
    }

    public class WalletDebtRules
    {
        /// <summary>Juros % ao mês (pro-rata por dia).</summary>
        public decimal MonthlyInterestPercent { get; set; } = 1m;

        /// <summary>Dias base para proporcionalizar juros mensal (ex.: 30).</summary>
        public int ProRataDaysPerMonth { get; set; } = 30;

        /// <summary>Multa % sobre valor original da parcela quando vencida.</summary>
        public decimal LatePenaltyPercentOfOriginal { get; set; } = 2m;
    }

    public class SecuritySettings
    {
        public long MaxPayloadMb { get; set; } = 10;
        public int RequestTimeoutSeconds { get; set; } = 10;
        public string CorsOrigins { get; set; } = string.Empty;
        public string AuditEncryptionKey { get; set; } = string.Empty;
    }


    public class JwtSettings
    {
        public string Secret { get; set; } = string.Empty;
        public string JweSecret { get; set; } = string.Empty;
    }

    public class AdminSettings
    {
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }

    public class RateLimitSettings
    {
        public int Permits { get; set; } = 100;
        public int Queue { get; set; } = 2;
        public int WindowMinutes { get; set; } = 1;
    }

    public class RedisSettings
    {
        public string ConnectionString { get; set; } = string.Empty;
    }

    public class RabbitMQSettings
    {
        public string Host { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }

    public class ElasticSearchSettings
    {
        public string Uri { get; set; } = string.Empty;
    }

    public class LoggingSettings
    {
        public string FilePath { get; set; } = "logs/invoice-api.log";
    }

    public class AWSSettings
    {
        public S3Settings S3 { get; set; } = new();
    }

    public class S3Settings
    {
        public string AccessKey { get; set; } = string.Empty;
        public string SecretKey { get; set; } = string.Empty;
        public string ServiceURL { get; set; } = string.Empty;
        public string BucketName { get; set; } = string.Empty;
    }
}
