namespace InvoiceGenerator.Api.Application.Services.Billets
{
    /// <summary>Dados já formatados para o template HTML do boleto (separação view / domínio).</summary>
    public sealed record BilletHtmlDisplayModel(
        string LinhaDigitavel,
        string BarcodeHtml,
        string BarcodeRaw,
        string NossoNumeroCurto,
        string Vencimento,
        string ValorFormatado,
        string ParcelaLabel,
        string AgreementId,
        string DataDocumento,
        string GeradoEm);
}
