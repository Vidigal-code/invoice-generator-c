namespace InvoiceGenerator.Api.Application.Constants
{
    public static class DebtCacheKeys
    {
        public static string LatestCalculation(Guid contractId) => $"DebtCalc:v1:{contractId}";
    }
}
