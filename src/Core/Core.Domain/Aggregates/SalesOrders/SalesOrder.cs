namespace Tilray.Integrations.Core.Domain.Aggregates.SalesOrders;

public class SalesOrder
{
    public string Division { get; set; }
    public string Customer { get; set; }
    public DateTime OrderDate { get; set; }
    public DateTime ExpectedDeliveryDate { get; set; }
    public string OrderType { get; set; }
    public bool UpdateOrderIfExists { get; set; }
    public bool BackgroundProcessing { get; set; }
    public string UploadGroup { get; set; }
    public string CustomerPO { get; set; }
    public string ShipToID { get; set; }
    public string ExternalRefNumber { get; set; }
    public List<LineItem> LineItems { get; set; } = [];
}

public class LineItem
{
    public string ItemNumber { get; set; }
    public string UOM { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal ExtendedPrice { get; set; }
}
