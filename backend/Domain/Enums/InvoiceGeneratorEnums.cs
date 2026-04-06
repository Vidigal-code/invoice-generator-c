namespace InvoiceGenerator.Api.Domain.Enums
{
    public enum InstallmentStatus
    {
        Open,
        Paid,
        Late,
        Renegotiated
    }

    public enum ContractStatus
    {
        Active,
        Negotiated,
        Defaulted,
        Closed,
        Cancelled
    }


    public enum AgreementStatus
    {
        Pending,
        Active,
        Broken,
        Completed
    }

    public enum BilletStatus
    {
        Pending,
        Paid,
        Expired,
        Canceled
    }
}
