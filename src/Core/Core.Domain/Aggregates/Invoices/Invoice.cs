namespace Tilray.Integrations.Core.Domain.Aggregates.Invoices;

public enum GRPOType
{
    Item,
    Service
}
public class GrpoDetails
{
    public decimal OpenQty { get; set; }
    public decimal Price { get; set; }
    public decimal LineTotal { get; set; }
}

public class VendorRemitAddress
{
    public string VendorCode { get; set; }
    public string FormattedVendorCode
    {
        get
        {
            return VendorCode != null
                ? (VendorCode.Contains('-')
                    ? VendorCode.Substring(0, VendorCode.IndexOf("-"))
                    : string.Empty)
                : string.Empty;
        }
    }

}

public class MatchedPurchaseOrderReceipt
{
    public string GoodsReceiptNumber { get; set; }
    public decimal AllocatedQuantity { get; set; }

    private string[] _grnParts;
    public string[] GrnParts =>
        _grnParts ??= GoodsReceiptNumber?.Split('-') ?? [];

    public bool IsValidGrn =>
        !string.IsNullOrEmpty(GoodsReceiptNumber)
        && GrnParts.Length >= 6
        && (GoodsReceiptNumber.Contains('I') || GoodsReceiptNumber.Contains('S'));

    public string PODocNum =>
        IsValidGrn && GrnParts.Length > 0 ? GrnParts[0] : string.Empty;

    public string POLineNum =>
        IsValidGrn && GrnParts.Length > 2 ? GrnParts[2] : string.Empty;

    public string GRPODocNum =>
        IsValidGrn && GrnParts.Length > 3 && int.TryParse(GrnParts[3], out int num)
        ? (num + 9999).ToString("00000")
        : string.Empty;

    public string GRPOLineNum =>
        IsValidGrn && GrnParts.Length > 4 ? GrnParts[4] : string.Empty;

    public GRPOType Type =>
        IsValidGrn && GoodsReceiptNumber.Contains('I')
        ? GRPOType.Item
        : GRPOType.Service;

    public void CalculateAllocatedQuantity(decimal remainingQty, decimal openQty, int grpoCount)
    {
        AllocatedQuantity = (grpoCount > 1 && remainingQty > openQty)
            ? openQty
            : remainingQty;
    }

    public void UpdateGrpo(decimal openQty, ref decimal remainingQty, ref int grpoCount)
    {
        CalculateAllocatedQuantity(remainingQty, openQty, grpoCount);

        remainingQty -= AllocatedQuantity;
        grpoCount--;
    }
}

public class MatchedPurchaseOrderReceipts
{
    public IEnumerable<MatchedPurchaseOrderReceipt> MatchedPurchaseOrderReceipt { get; set; } = [];
}

public class LineItem
{
    public string LineItemId { get; set; }
    public decimal Quantity { get; set; }
    public string Description { get; set; }
    public string SupplierPartId { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal TotalPrice { get; set; }
    public decimal AmountWithoutVat { get; set; }
    public string Custom1 { get; set; }
    public string Custom2 { get; set; }
    public string Custom3 { get; set; }
    public string Custom4 { get; set; }
    public string Custom5 { get; set; }
    public string Custom6 { get; set; }
    public string Custom7 { get; set; }
    public string Custom8 { get; set; }
    public string Custom9 { get; set; }
    public string Custom10 { get; set; }
    public string OBeerGLAccount { get; set; }
    public MatchedPurchaseOrderReceipts MatchedPurchaseOrderReceipts { get; set; } = new();
    public bool IsValid() => Quantity != 0.0m && TotalPrice != 0.0m;
    public bool HasGrpoMatches() => MatchedPurchaseOrderReceipts?.MatchedPurchaseOrderReceipt?.Count() > 0;
    public void SetGlAccount(string glAccount) => OBeerGLAccount = glAccount;
}

public class LineItems
{
    public IEnumerable<LineItem> LineItem { get; set; } = [];
    public IEnumerable<LineItem> ValidLineItems()
    {
        return LineItem.Where(x => x.IsValid());
    }
    public IEnumerable<LineItem> GrpoLineItems()
    {
        return ValidLineItems().Where(li => li.HasGrpoMatches());
    }
    public IEnumerable<LineItem> NonPOLineItems()
    {
        return ValidLineItems().Except(GrpoLineItems());
    }
}

public class Invoice
{
    public string ID { get; set; }
    public string CurrencyCode { get; set; }
    public string InvoiceNumber { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public DateTime? InvoiceDate { get; set; }
    public DateTime? PaymentDueDate { get; set; }
    public DateTime? ExtractDate { get; set; }
    public DateTime? LastModifiedDate { get; set; }
    public DateTime? InvoiceReceivedDate { get; set; }
    public decimal InvoiceAmount { get; set; }
    public decimal CalculatedAmount { get; set; }
    public decimal ShippingAmount { get; set; }
    public decimal TaxAmount { get; set; }
    public string PaymentMethod { get; set; }
    public string DeliverySlipNumber { get; set; }
    public string PurchaseOrderNumber { get; set; }
    public string Custom1 { get; set; }
    public string Custom2 { get; set; }
    public string Custom3 { get; set; }
    public string Custom4 { get; set; }
    public string Custom5 { get; set; }
    public string Custom6 { get; set; }
    public string Custom7 { get; set; }
    public string Custom8 { get; set; }
    public string Custom9 { get; set; }
    public string Custom10 { get; set; }
    public decimal VatAmountOne { get; set; }
    public decimal VatAmountTwo { get; set; }
    public decimal VatAmountThree { get; set; }
    public decimal VatAmountFour { get; set; }
    public VendorRemitAddress VendorRemitAddress { get; set; }
    public LineItems LineItems { get; set; } = new();
    public string Company => !string.IsNullOrEmpty(Custom1)
                ? Custom1
                : LineItems?.LineItem?.FirstOrDefault()?.Custom1 ?? string.Empty;
    public bool IsValid() =>
        LineItems?.ValidLineItems()?.Any() == true;
    public DateTime FiscalYear => new(DateTime.Now.Year - (DateTime.Now.Month < 6 ? 1 : 0), 6, 1);

    public string PostingDate =>
        ExtractDate.HasValue
        ? ExtractDate.Value.ToString("yyyy-MM-dd")
        : DateTime.UtcNow.ToString("yyyy-MM-dd");

    public string DueDate =>
        PaymentDueDate.HasValue &&
        PaymentDueDate >= ExtractDate
            ? PaymentDueDate.Value.ToString("yyyy-MM-dd")
            : (ExtractDate.HasValue
                ? ExtractDate.Value.ToString("yyyy-MM-dd")
                : DateTime.UtcNow.ToString("yyyy-MM-dd"));

    public string DocumentDate =>
        InvoiceDate.HasValue
            ? (InvoiceDate.Value >= FiscalYear
                ? InvoiceDate.Value.ToString("yyyy-MM-dd")
                : FiscalYear.ToString("yyyy-MM-dd"))
            : FiscalYear.ToString("yyyy-MM-dd");
}

public class InvoiceGroup
{
    public CompanyReference Company { get; set; }
    public IEnumerable<Invoice> Invoices { get; set; }

    public static InvoiceGroup Create(
        CompanyReference company,
        IEnumerable<Invoice> invoices)
    {
        return new InvoiceGroup
        {
            Company = company,
            Invoices = invoices
        };
    }
}
