namespace InvoiceGenerator.Api.Domain.Validation
{
    /// <summary>Validação de CPF (11 dígitos, dígitos verificadores).</summary>
    public static class CpfValidator
    {
        public static string NormalizeDigits(string? raw)
        {
            if (string.IsNullOrWhiteSpace(raw))
                return string.Empty;
            return new string(raw.Where(char.IsDigit).ToArray());
        }

        /// <summary>Indica se a string contém exatamente 11 dígitos e CPF válido.</summary>
        public static bool IsValid(string? raw)
        {
            var d = NormalizeDigits(raw);
            if (d.Length != 11)
                return false;
            return IsValidDigitSequence(d);
        }

        private static bool IsValidDigitSequence(string d)
        {
            if (d.Distinct().Count() == 1)
                return false;

            var nums = d.Select(c => c - '0').ToArray();
            var s1 = 0;
            for (var i = 0; i < 9; i++)
                s1 += nums[i] * (10 - i);
            var r1 = s1 % 11;
            var dv1 = r1 < 2 ? 0 : 11 - r1;
            if (dv1 != nums[9])
                return false;

            var s2 = 0;
            for (var i = 0; i < 10; i++)
                s2 += nums[i] * (11 - i);
            var r2 = s2 % 11;
            var dv2 = r2 < 2 ? 0 : 11 - r2;
            return dv2 == nums[10];
        }
    }
}
