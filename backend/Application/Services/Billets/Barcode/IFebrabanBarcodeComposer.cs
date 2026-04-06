using InvoiceGenerator.Api.Domain.Entities;

namespace InvoiceGenerator.Api.Application.Services.Billets.Barcode
{
    /// <summary>Monta código de barras numérico e linha digitável (layout FEBRABAN simplificado).</summary>
    public interface IFebrabanBarcodeComposer
    {
        /// <summary>Dígitos contínuos usados no ITF-25 e na linha digitável.</summary>
        string ResolveNumericBarcode(Billet billet);

        /// <summary>Formata exibição da linha digitável com pontos e espaços.</summary>
        string ToLinhaDigitavel(string numericBarcode);
    }
}
