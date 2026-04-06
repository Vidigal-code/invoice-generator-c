using InvoiceGenerator.Api.Domain.Entities;

namespace InvoiceGenerator.Api.Application.Services.Billets
{
    public interface IBilletStorageObjectKeyBuilder
    {
        string BuildObjectKey(Billet billet);
    }
}
