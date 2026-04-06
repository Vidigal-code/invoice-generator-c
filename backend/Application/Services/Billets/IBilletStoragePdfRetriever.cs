using InvoiceGenerator.Api.Domain.Entities;

namespace InvoiceGenerator.Api.Application.Services.Billets
{
    /// <summary>Tenta obter PDF pré-gerado no armazenamento de objetos.</summary>
    public interface IBilletStoragePdfRetriever
    {
        Task<byte[]?> TryDownloadPdfAsync(Billet billet, CancellationToken cancellationToken);
    }
}
