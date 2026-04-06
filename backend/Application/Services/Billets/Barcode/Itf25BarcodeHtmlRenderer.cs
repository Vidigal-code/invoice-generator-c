using System.Text;

namespace InvoiceGenerator.Api.Application.Services.Billets.Barcode
{
    public sealed class Itf25BarcodeHtmlRenderer : IItf25BarcodeHtmlRenderer
    {
        private const int MaxEncodedPairs = 22;
        private const int BarUnitNarrow = 1;
        private const int BarUnitWide = 3;

        public string Render(string numericBarcode)
        {
            if (string.IsNullOrEmpty(numericBarcode)) return string.Empty;

            var digits = PadToEvenDigits(numericBarcode);
            var sb = new StringBuilder(512);
            sb.Append("<div class=\"barcode-bars\">");
            AppendStartPattern(sb);
            EncodeInterleavedPairs(sb, digits);
            AppendStopPattern(sb);
            sb.Append("</div>");
            return sb.ToString();
        }

        private static string PadToEvenDigits(string barcode)
        {
            var len = barcode.Length;
            if (len % 2 == 0) return barcode;
            return string.Concat(barcode, "0");
        }

        private static void AppendStartPattern(StringBuilder sb)
        {
            sb.Append("<div class=\"bar\" style=\"width:2px\"></div><div class=\"gap\" style=\"width:2px\"></div>");
            sb.Append("<div class=\"bar\" style=\"width:2px\"></div><div class=\"gap\" style=\"width:2px\"></div>");
        }

        private static void AppendStopPattern(StringBuilder sb)
        {
            sb.Append("<div class=\"bar\" style=\"width:3px\"></div><div class=\"gap\" style=\"width:2px\"></div>");
            sb.Append("<div class=\"bar\" style=\"width:2px\"></div>");
        }

        private static void EncodeInterleavedPairs(StringBuilder sb, string digits)
        {
            var limit = Math.Min(digits.Length, MaxEncodedPairs * 2);
            for (var i = 0; i < limit; i += 2)
            {
                var d1 = digits[i] - '0';
                var d2 = i + 1 < digits.Length ? digits[i + 1] - '0' : 0;
                var bars = GetItfPattern(d1);
                var spaces = GetItfPattern(d2);

                for (var j = 0; j < 5; j++)
                {
                    var barW = bars[j] ? BarUnitWide : BarUnitNarrow;
                    var gapW = spaces[j] ? BarUnitWide : BarUnitNarrow;
                    sb.Append("<div class=\"bar\" style=\"width:").Append(barW).Append("px\"></div>");
                    sb.Append("<div class=\"gap\" style=\"width:").Append(gapW).Append("px\"></div>");
                }
            }
        }

        private static bool[] GetItfPattern(int digit)
        {
            var d = digit % 10;
            return d switch
            {
                0 => new[] { false, false, true, true, false },
                1 => new[] { true, false, false, false, true },
                2 => new[] { false, true, false, false, true },
                3 => new[] { true, true, false, false, false },
                4 => new[] { false, false, true, false, true },
                5 => new[] { true, false, true, false, false },
                6 => new[] { false, true, true, false, false },
                7 => new[] { false, false, false, true, true },
                8 => new[] { true, false, false, true, false },
                _ => new[] { false, true, false, true, false }
            };
        }
    }
}
