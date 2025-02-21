namespace Tilray.Integrations.Core.Application.Constants;

public static class BlobNames
{
    public static string GetInvoiceBlobName() => $"invoice-{Guid.NewGuid()}-{DateTime.UtcNow:yyyyMMddHHmmss}.json";
    public static string GetExpenseBlobName() => $"expense-{Guid.NewGuid()}-{DateTime.UtcNow:yyyyMMddHHmmss}.json";
}
