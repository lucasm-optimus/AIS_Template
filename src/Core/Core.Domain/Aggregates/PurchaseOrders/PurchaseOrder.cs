namespace Tilray.Integrations.Core.Domain.Aggregates.PurchaseOrders;

public class PurchaseOrderAddress
{
    public string ExternalId { get; set; }
    public string Name { get; set; }
    public string Address1 { get; set; }
    public string City { get; set; }
    public string StateProvince { get; set; }
    public string CountryCode { get; set; }
    public string PostalCode { get; set; }
}

public class PurchaseOrderLineItem
{
    public string ExternalId { get; set; }
    public string Name { get; set; }
    public string Status { get; set; }
    public string LineNumber { get; set; }
    public string Description { get; set; }
    public double QtyRequired { get; set; }
    public decimal UnitPrice { get; set; }
    public string UOMCode { get; set; }
    public DateTime CreatedDate { get; set; }
    public string ExpenseAccount { get; set; }
    public decimal Amount { get; set; }
    public string OrderNumber { get; set; }
    public string Custom1 { get; set; }
    public string Custom2 { get; set; }
    public string Custom3 { get; set; }
    public int POR1_DocEntry { get; set; }
    public decimal POR1_Quantity { get; set; }
}

public class PurchaseOrder
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string PurchaseOrderNumber { get; set; }
    public string Status { get; set; }
    public string Division { get; set; }
    public string VendorCode { get; set; }
    public string CurrencyCode { get; set; }
    public DateTime OrderDate { get; set; }
    public DateTime CreatedDate { get; set; }
    public PurchaseOrderAddress? BillToAddress { get; set; }
    public PurchaseOrderAddress? ShipToAddress { get; set; }
    public IEnumerable<PurchaseOrderLineItem>? LineItems { get; set; }
    public IEnumerable<PurchaseOrderReceipt>? PurchaseOrderReceipts { get; set; }
    public double? VendorAddressNumber { get; set; }
    public int OPOR_DocEntry { get; set; }

    public void SetVendorAddress(double? vendorAddressNumber)
    {
        VendorAddressNumber = vendorAddressNumber ?? 0;
    }

    public static IEnumerable<string> GetDistinctPurchaseOrders(IEnumerable<PurchaseOrderReceipt> poReceiptResponse)
    {
        return poReceiptResponse
            .Select(x => x.PurchaseOrderId)
            .Distinct();
    }

    public static IEnumerable<PurchaseOrder> FilterPurchaseOrders(IEnumerable<PurchaseOrder> poData, IEnumerable<CompanyReference> companyReferences)
    {
        var rootstockCompanies = companyReferences.Select(cr => cr.Rootstock_Company__c);
        return poData.Where(item => item.Status != "2-Firmed" && item.Status != "3-Approvals Processing" && rootstockCompanies.Contains(item.Division));
    }

    public void SetPurchaseOrdersReceipt(IEnumerable<PurchaseOrderReceipt> poReceiptResponse)
    {
        PurchaseOrderReceipts = poReceiptResponse
            .Where(r => r.PurchaseOrderNumber == PurchaseOrderNumber)
            .ToList();
    }

    public void SetLineItems(IEnumerable<PurchaseOrderLineItem> poLineResponse)
    {
        LineItems = poLineResponse
            .Where(x => x.OrderNumber == Id && x.Amount > 0)
            .ToList();
    }

    public void FilterAndDistinctLineItemsAndReceipts()
    {
        LineItems = LineItems?
            .Where(li => li.Amount > 0.0m && li.QtyRequired > 0.0 && li.ExternalId.Split('-')[0] == OPOR_DocEntry.ToString())
            .GroupBy(li => li.ExternalId)
            .Select(group => group.First())
            .ToList();

        PurchaseOrderReceipts = PurchaseOrderReceipts?
            .GroupBy(pr => pr.Id)
            .Select(group => group.First())
            .Where(pr => pr.PurchaseOrderNumber == PurchaseOrderNumber)
            .ToList();
    }

    public static List<PurchaseOrder> GroupByPurchaseOrderNumber(IEnumerable<PurchaseOrder> purchaseOrders)
    {
        return purchaseOrders
            .GroupBy(po => po.PurchaseOrderNumber)
            .Select(group => group.First())
            .ToList();
    }

    public static List<PurchaseOrder> FilterNonEmptyLineItems(IEnumerable<PurchaseOrder> purchaseOrders)
    {
        return purchaseOrders
            .Where(po => po.LineItems != null && po.LineItems.Any())
            .ToList();
    }
}

public class PurchaseOrderReceipt
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string PurchaseOrderId { get; set; }
    public string PurchaseOrderNumber { get; set; }
    public string LineItemExternalId { get; set; }
    public DateTime ReceiptDate { get; set; }
    public string Item { get; set; }
    public double Quantity { get; set; }
    public string DeliverySlipNumber { get; set; }
    public string GoodsReceiptNumber { get; set; }
    public int PDN1_DocEntry { get; set; }
    public int PDN1_LineNum { get; set; }
}