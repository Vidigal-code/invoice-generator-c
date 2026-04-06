using MediatR;

namespace InvoiceGenerator.Api.Application.Queries
{
    public class GetBilletPdfQuery : IRequest<byte[]>
    {
        public Guid BilletId { get; set; }

        public GetBilletPdfQuery(Guid billetId)
        {
            BilletId = billetId;
        }
    }
}
