using System.Globalization;
using InvoiceGenerator.Api.Domain.Entities;

namespace InvoiceGenerator.Api.Application.Services.Billets.Barcode
{
    public sealed class FebrabanBarcodeComposer : IFebrabanBarcodeComposer
    {
        private static readonly DateTime FatorVencimentoBase = new(1997, 10, 7, 0, 0, 0, DateTimeKind.Utc);

        public string ResolveNumericBarcode(Billet billet)
        {
            if (billet == null) throw new ArgumentNullException(nameof(billet));

            if (!string.IsNullOrWhiteSpace(billet.Barcode))
                return SanitizeDigits(billet.Barcode);

            return BuildFallbackNumericCode(billet);
        }

        public string ToLinhaDigitavel(string numericBarcode)
        {
            if (string.IsNullOrEmpty(numericBarcode)) return string.Empty;

            var b = SanitizeDigits(numericBarcode);
            if (b.Length < 44) return numericBarcode;

            return TryFormatFebrabanLine(b, out var formatted) ? formatted : numericBarcode;
        }

        private static string SanitizeDigits(string input)
        {
            var span = input.AsSpan();
            Span<char> buffer = stackalloc char[span.Length];
            var n = 0;
            foreach (var c in span)
            {
                if (char.IsDigit(c)) buffer[n++] = c;
            }
            return n == 0 ? string.Empty : new string(buffer[..n]);
        }

        private static string BuildFallbackNumericCode(Billet billet)
        {
            var bank = "033";
            var currency = "9";
            var dueDays = Math.Max(0, (billet.DueDate.Date - FatorVencimentoBase.Date).Days);
            var dueFactor = dueDays.ToString("D4", CultureInfo.InvariantCulture);
            var value = ((long)(billet.Value * 100m)).ToString("D10", CultureInfo.InvariantCulture);
            var freeField = billet.Id.ToString("N")[..25];
            var rawCode = string.Concat(bank, currency, dueFactor, value, freeField);
            var checkDigit = CalculateMod10(rawCode).ToString(CultureInfo.InvariantCulture);
            return string.Concat(bank, currency, checkDigit, dueFactor, value, freeField);
        }

        private static bool TryFormatFebrabanLine(string b, out string formatted)
        {
            formatted = string.Empty;
            try
            {
                var c1 = b[..10];
                var c2 = b.Substring(10, 11);
                var c3 = b.Substring(21, 11);
                var c4 = b[4].ToString();
                var c5 = b.Substring(5, 14);
                formatted = string.Concat(
                    c1[..5], ".", c1[5..], " ",
                    c2[..5], ".", c2[5..], " ",
                    c3[..5], ".", c3[5..], " ",
                    c4, " ",
                    c5);
                return true;
            }
            catch
            {
                return false;
            }
        }

        private static int CalculateMod10(string code)
        {
            var sum = 0;
            var mult = 2;
            for (var i = code.Length - 1; i >= 0; i--)
            {
                var digit = code[i] - '0';
                var product = digit * mult;
                sum += product > 9 ? product - 9 : product;
                mult = mult == 2 ? 1 : 2;
            }

            var remainder = sum % 10;
            return remainder == 0 ? 0 : 10 - remainder;
        }
    }
}
