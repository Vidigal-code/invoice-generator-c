namespace InvoiceGenerator.Api.Application.Abstractions
{
    public interface IFilePayloadProtector
    {
        byte[] Protect(ReadOnlySpan<byte> plain);

        byte[] Unprotect(ReadOnlySpan<byte> stored);
    }
}
