namespace Tilray.Integrations.Core.Domain.Aggregates.PurchaseOrders;

public class VendorResponse
{
    public List<SAPConcurCustomValues>? Vendor { get; set; }
}

public class SAPConcurCustomValues
{
    public string Custom1 { get; set; }
    public string Custom2 { get; set; }
    public string Custom3 { get; set; }
    public string Custom4 { get; set; }
}

public class ContentItem
{
    public string Id { get; set; }
    public string ShortCode { get; set; }
}

public class VendorCustomResponse
{
    public List<ContentItem>? Content { get; set; }
}
