namespace InvoiceGenerator.Api.Application.Services.Billets
{
    /// <summary>Markup e CSS do boleto — apenas composição, sem regra de negócio.</summary>
    public static class BilletHtmlTemplate
    {
        public static string BuildDocument(BilletHtmlDisplayModel m)
        {
            return $@"<!DOCTYPE html>
<html lang=""pt-BR"">
<head>
<meta charset=""UTF-8"">
<meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
<title>Boleto Bancário — Invoice Generator</title>
<style>
{EmbeddedStyles}
</style>
</head>
<body>

<!-- RECIBO DO PAGADOR -->
<div class=""boleto"">
  <div class=""recibo"">
    <div class=""header"">
      <div class=""bank-logo"">invoice-generator-c</div>
      <div class=""bank-code"">033-7</div>
      <div>
        <div class=""field-label"">Recibo do Pagador</div>
        <div class=""linha-digitavel"">{m.LinhaDigitavel}</div>
      </div>
    </div>

    <div class=""section"">
      <div class=""field-row"">
        <div class=""field"" style=""flex:3"">
          <div class=""field-label"">Beneficiário (Cedente)</div>
          <div class=""field-value"">Invoice Generator C — Sistema de Cobrança</div>
        </div>
        <div class=""field"">
          <div class=""field-label"">Agência / Código Beneficiário</div>
          <div class=""field-value"">0033 / 10000-1</div>
        </div>
      </div>
    </div>

    <div class=""section"">
      <div class=""field-row"">
        <div class=""field"">
          <div class=""field-label"">Vencimento</div>
          <div class=""field-value"">{m.Vencimento}</div>
        </div>
        <div class=""field"">
          <div class=""field-label"">Parcela</div>
          <div class=""field-value"">Nº {m.ParcelaLabel}</div>
        </div>
        <div class=""field"">
          <div class=""field-label"">Nosso Número</div>
          <div class=""field-value"">{m.NossoNumeroCurto}</div>
        </div>
        <div class=""field amount-box"">
          <div class=""field-label"">Valor do Documento (R$)</div>
          <div class=""field-value large"">R$ {m.ValorFormatado}</div>
        </div>
      </div>
    </div>

    <div class=""instructions"">
      <strong>Instruções:</strong> Não receber após o vencimento. Em caso de dúvidas, entre em contato com o beneficiário.<br>
      Pagável em qualquer banco, lotérica ou internet banking até a data de vencimento.
    </div>
  </div>

  <div class=""corte"">✂ — — — — — — — — — — — — — — — — RECORTE AQUI — — — — — — — — — — — — — — — — ✂</div>

<!-- BOLETO BANCÁRIO -->
  <div class=""header"">
    <div class=""bank-logo"">invoice-generator-c</div>
    <div class=""bank-code"">033-7</div>
    <div>
      <div class=""field-label"">Linha Digitável</div>
      <div class=""linha-digitavel"">{m.LinhaDigitavel}</div>
    </div>
  </div>

  <div class=""section"">
    <div class=""field-row"">
      <div class=""field"" style=""flex:3"">
        <div class=""field-label"">Pagador (Sacado)</div>
        <div class=""field-value"">Sistema Invoice Generator C</div>
      </div>
      <div class=""field"">
        <div class=""field-label"">Data do Documento</div>
        <div class=""field-value"">{m.DataDocumento}</div>
      </div>
      <div class=""field amount-box"">
        <div class=""field-label"">Vencimento</div>
        <div class=""field-value"">{m.Vencimento}</div>
      </div>
    </div>
  </div>

  <div class=""section"">
    <div class=""field-row"">
      <div class=""field"">
        <div class=""field-label"">Espécie Doc.</div>
        <div class=""field-value"">DM</div>
      </div>
      <div class=""field"">
        <div class=""field-label"">Aceite</div>
        <div class=""field-value"">N</div>
      </div>
      <div class=""field"">
        <div class=""field-label"">Nosso Número</div>
        <div class=""field-value"">{m.NossoNumeroCurto}</div>
      </div>
      <div class=""field"">
        <div class=""field-label"">Carteira</div>
        <div class=""field-value"">RG</div>
      </div>
      <div class=""field"">
        <div class=""field-label"">Moeda</div>
        <div class=""field-value"">BRL</div>
      </div>
      <div class=""field amount-box"">
        <div class=""field-label"">Valor Cobrado (R$)</div>
        <div class=""field-value large"">R$ {m.ValorFormatado}</div>
      </div>
    </div>
  </div>

  <div class=""section"">
    <div class=""field-row"">
      <div class=""field"" style=""flex:3"">
        <div class=""field-label"">Beneficiário</div>
        <div class=""field-value"">Invoice Generator C — CNPJ: 00.000.000/0001-00</div>
      </div>
      <div class=""field"">
        <div class=""field-label"">Agência Beneficiário</div>
        <div class=""field-value"">0033 / 10000-1</div>
      </div>
    </div>
  </div>

  <div class=""instructions"">
    <strong>Instruções ao Caixa:</strong><br>
    Cobrar multa de 2% após o vencimento. Juros de 0,033% ao dia.
    Não receber após 30 dias do vencimento.<br>
    <strong>Acordo ID:</strong> {m.AgreementId}
  </div>

  <div class=""barcode-section"">
    {m.BarcodeHtml}
    <div class=""barcode-number"">{m.BarcodeRaw}</div>
  </div>

  <div class=""footer"">
    Invoice Generator C — Sistema de Gestão de Cobrança | Boleto gerado em {m.GeradoEm}
  </div>
</div>
</body>
</html>";
        }

        private const string EmbeddedStyles = @"
  * { box-sizing: border-box; margin: 0; padding: 0; }
  body { font-family: Arial, Helvetica, sans-serif; font-size: 10px; color: #000; background: #fff; padding: 12px; }
  .boleto { max-width: 780px; margin: 0 auto; border: 1px solid #000; }
  .header { display: flex; align-items: center; border-bottom: 2px solid #000; padding: 8px; justify-content: space-between; }
  .bank-logo { font-size: 22px; font-weight: bold; letter-spacing: 2px; font-style: italic; color: #003087; }
  .bank-code { font-size: 20px; font-weight: bold; border-left: 2px solid #000; border-right: 2px solid #000; padding: 4px 12px; color: #003087; }
  .linha-digitavel { font-size: 13px; font-weight: bold; letter-spacing: 1px; text-align: right; }
  .section { border-bottom: 1px solid #000; }
  .field-row { display: flex; }
  .field { border-right: 1px solid #000; padding: 3px 6px; flex: 1; }
  .field:last-child { border-right: none; }
  .field-label { font-size: 8px; color: #333; text-transform: uppercase; }
  .field-value { font-size: 11px; font-weight: bold; margin-top: 1px; }
  .field-value.large { font-size: 14px; }
  .amount-box { text-align: right; min-width: 160px; }
  .instructions { padding: 6px; font-size: 9px; line-height: 1.5; border-bottom: 1px solid #000; }
  .barcode-section { padding: 16px 8px 8px; text-align: center; }
  .barcode-bars { display: flex; align-items: flex-end; justify-content: center; height: 60px; gap: 0; margin: 0 auto; max-width: 600px; }
  .bar { display: inline-block; height: 100%; background: #000; }
  .gap { display: inline-block; background: #fff; }
  .barcode-number { font-size: 11px; letter-spacing: 3px; margin-top: 8px; font-family: monospace; }
  .recibo { border-bottom: 2px dashed #000; padding: 8px; margin-bottom: 6px; }
  .corte { text-align: center; font-size: 9px; color: #666; margin: 4px 0; border-bottom: 1px solid #000; padding-bottom: 4px; }
  .footer { padding: 4px 6px; font-size: 9px; text-align: center; color: #555; }
  @media print { body { padding: 0; } .boleto { border: none; } }
";
    }
}
