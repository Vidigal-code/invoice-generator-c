using System.Globalization;
using InvoiceGenerator.Api.Domain.Entities;

namespace InvoiceGenerator.Api.Application.Services.Billets
{
    public sealed class BilletStorageObjectKeyBuilder : IBilletStorageObjectKeyBuilder
    {
        public string BuildObjectKey(Billet billet)
        {
            if (billet == null) throw new ArgumentNullException(nameof(billet));

            return string.Format(
                CultureInfo.InvariantCulture,
                "billets/{0}/installment_{1}.pdf",
                billet.AgreementId,
                billet.InstallmentNumber);
        }
    }
}
