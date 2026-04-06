using System.Globalization;
using System.Text;
using InvoiceGenerator.Api.Application.Services.Billets.Barcode;
using InvoiceGenerator.Api.Domain.Entities;

namespace InvoiceGenerator.Api.Application.Services.Billets
{
    public sealed class BilletHtmlFallbackRenderer : IBilletHtmlFallbackRenderer
    {
        private readonly IFebrabanBarcodeComposer _barcodeComposer;
        private readonly IItf25BarcodeHtmlRenderer _itf25Renderer;

        public BilletHtmlFallbackRenderer(
            IFebrabanBarcodeComposer barcodeComposer,
            IItf25BarcodeHtmlRenderer itf25Renderer)
        {
            _barcodeComposer = barcodeComposer ?? throw new ArgumentNullException(nameof(barcodeComposer));
            _itf25Renderer = itf25Renderer ?? throw new ArgumentNullException(nameof(itf25Renderer));
        }

        public byte[] RenderUtf8(Billet billet)
        {
            if (billet == null) throw new ArgumentNullException(nameof(billet));

            var numericBarcode = _barcodeComposer.ResolveNumericBarcode(billet);
            var linhaDigitavel = _barcodeComposer.ToLinhaDigitavel(numericBarcode);
            var barcodeHtml = _itf25Renderer.Render(numericBarcode);

            var now = DateTime.UtcNow;
            var model = new BilletHtmlDisplayModel(
                LinhaDigitavel: linhaDigitavel,
                BarcodeHtml: barcodeHtml,
                BarcodeRaw: numericBarcode,
                NossoNumeroCurto: billet.Id.ToString()[..8].ToUpperInvariant(),
                Vencimento: billet.DueDate.ToString("dd/MM/yyyy", CultureInfo.GetCultureInfo("pt-BR")),
                ValorFormatado: billet.Value.ToString("N2", CultureInfo.GetCultureInfo("pt-BR")),
                ParcelaLabel: billet.InstallmentNumber.ToString(CultureInfo.InvariantCulture),
                AgreementId: billet.AgreementId.ToString(),
                DataDocumento: now.ToString("dd/MM/yyyy", CultureInfo.GetCultureInfo("pt-BR")),
                GeradoEm: now.ToString("dd/MM/yyyy HH:mm", CultureInfo.GetCultureInfo("pt-BR")));

            var html = BilletHtmlTemplate.BuildDocument(model);
            return Encoding.UTF8.GetBytes(html);
        }
    }
}
