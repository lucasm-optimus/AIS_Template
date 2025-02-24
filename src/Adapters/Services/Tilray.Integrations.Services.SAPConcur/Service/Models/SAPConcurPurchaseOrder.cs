namespace Tilray.Integrations.Services.SAPConcur.Service.Models;

public class SAPConcurPurchaseOrder
{
    public string ID { get; set; }
    public string Name { get; set; }
    public string PurchaseOrderNumber { get; set; }
    public string PolicyExternalID { get; set; }
    public string Status { get; set; } = "Transmitted";
    public string IsTest { get; set; } = "N";
    public string IsChangeOrder { get; set; } = "N";
    public string LedgerCode { get; set; } = "Default";
    public DateTime OrderDate { get; set; }
    public string CurrencyCode { get; set; }
    public string VendorCode { get; set; }
    public string VendorAddressCode { get; set; }
    public string Custom9 { get; set; }
    public SAPConcurAddress ShipToAddress { get; set; }
    public SAPConcurAddress BillToAddress { get; set; }
    public List<SAPConcurLineItem> LineItem { get; set; }
}

public class SAPConcurAddress
{
    public string ExternalID { get; set; }
    public string Address1 { get; set; }
    public string City { get; set; }
    public string StateProvince { get; set; }
    public string CountryCode { get; set; }
    public string PostalCode { get; set; }
}

public class SAPConcurLineItem
{
    public string ExternalID { get; set; }
    public DateTime CreatedDate { get; set; }
    public string IsReceiptRequired { get; set; } = "true";
    public string PurchaseOrderReceiptType { get; set; } = "WQTY";
    public int LineNumber { get; set; }
    public string Description { get; set; }
    public decimal Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public string UnitOfMeasureCode { get; set; }
    public string AccountCode { get; set; } = "Default";
    public string Custom1 { get; set; }
    public string Custom2 { get; set; }
    public string Custom3 { get; set; }
    public string Custom4 { get; set; }
    public List<Allocation> Allocation { get; set; }
}

public class Allocation
{
    public decimal Amount { get; set; }
    public string Percentage { get; set; } = "100.0";
    public decimal GrossAmount { get; set; }
}

public class SAPConcurPurchaseOrderReceipt
{
    public string ID { get; set; }
    public string PurchaseOrderNumber { get; set; }
    public string LineItemExternalID { get; set; }
    public decimal ReceivedQuantity { get; set; }
    public DateTime ReceivedDate { get; set; }
    public string GoodsReceiptNumber { get; set; }
    public string DeliverySlipNumber { get; set; }
    public string Deleted { get; set; } = "false";
    public string URI { get; set; }
}
