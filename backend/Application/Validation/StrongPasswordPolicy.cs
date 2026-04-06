namespace InvoiceGenerator.Api.Application.Validation
{
    public static class StrongPasswordPolicy
    {
        public const int MinimumLength = 8;

        public static bool IsSatisfied(string? password)
        {
            if (string.IsNullOrEmpty(password) || password.Length < MinimumLength)
                return false;
            if (!password.Any(char.IsUpper))
                return false;
            if (!password.Any(char.IsLower))
                return false;
            if (!password.Any(char.IsDigit))
                return false;
            if (!password.Any(ch => !char.IsLetterOrDigit(ch)))
                return false;
            return true;
        }
    }
}
