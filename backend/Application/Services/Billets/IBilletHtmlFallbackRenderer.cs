using InvoiceGenerator.Api.Domain.Entities;

namespace InvoiceGenerator.Api.Application.Services.Billets
{
    /// <summary>Fallback: HTML do boleto quando não há PDF no storage.</summary>
    public interface IBilletHtmlFallbackRenderer
    {
        byte[] RenderUtf8(Billet billet);
    }
}
