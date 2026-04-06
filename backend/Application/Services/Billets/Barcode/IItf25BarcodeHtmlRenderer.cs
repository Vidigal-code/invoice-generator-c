namespace InvoiceGenerator.Api.Application.Services.Billets.Barcode
{
    /// <summary>Representação HTML do código de barras ITF-25 (barras simuladas com divs).</summary>
    public interface IItf25BarcodeHtmlRenderer
    {
        string Render(string numericBarcode);
    }
}
