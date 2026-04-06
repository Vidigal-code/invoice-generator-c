namespace InvoiceGenerator.Api.Application.Exceptions
{
    /// <summary>Acordo formalizado sem boleto correspondente na base.</summary>
    public sealed class BilletNotFoundException : Exception
    {
        public BilletNotFoundException(Guid billetId)
            : base("Boleto não encontrado.")
        {
            BilletId = billetId;
        }

        public Guid BilletId { get; }
    }
}
