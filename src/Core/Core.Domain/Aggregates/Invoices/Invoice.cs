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
    public GRPOType Type { get; set; }
    public string PODocNum { get; set; }
    public string GRPODocNum { get; set; }
    public string POLineNum { get; set; }
    public string GRPOLineNum { get; set; }
    public decimal AllocatedQuantity { get; set; }

    public bool ValidateGoodsReceiptNumber =>
        !string.IsNullOrEmpty(GoodsReceiptNumber)
        && GoodsReceiptNumber.Split('-').Length == 5
        && (GoodsReceiptNumber.Contains("(I)") || GoodsReceiptNumber.Contains("(S)"));

    public void CalculateAllocatedQuantity(decimal remainingQty, decimal openQty, int grpoCount)
    {
        AllocatedQuantity = (grpoCount > 1 && remainingQty > openQty)
            ? openQty
            : remainingQty;
    }

    public void UpdateGrpo(decimal openQty, ref decimal remainingQty, ref int grpoCount)
    {
        var parts = GoodsReceiptNumber.Split('-');

        PODocNum = parts[0];
        POLineNum = parts[2];
        GRPODocNum = (int.Parse(parts[3]) + 9999).ToString("00000");
        GRPOLineNum = parts[4];
        Type = GoodsReceiptNumber.Contains("(I)") ? GRPOType.Item : GRPOType.Service;

        CalculateAllocatedQuantity(remainingQty, openQty, grpoCount);

        remainingQty -= AllocatedQuantity;
        grpoCount--;
    }
}

public class MatchedPurchaseOrderReceipts
{
    public List<MatchedPurchaseOrderReceipt> MatchedPurchaseOrderReceipt { get; set; }
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
    public MatchedPurchaseOrderReceipts MatchedPurchaseOrderReceipts { get; set; }
    public bool IsValid() => Quantity != 0.0m && TotalPrice != 0.0m;
    public bool HasGrpoMatches() => MatchedPurchaseOrderReceipts?.MatchedPurchaseOrderReceipt?.Count > 0;
    public void SetGlAccount(string glAccount) => OBeerGLAccount = glAccount;
}

public class LineItems
{
    public List<LineItem> LineItem { get; set; }
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
    public DateTime? ExtractedDate { get; set; }
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
    public LineItems LineItems { get; set; }
    public string Company
    {
        get
        {
            return !string.IsNullOrEmpty(Custom1)
                ? Custom1
                : LineItems?.LineItem?.FirstOrDefault()?.Custom1;
        }
    }
    public DateTime FiscalYear
    {
        get
        {
            return new DateTime(DateTime.Now.Year - (DateTime.Now.Month < 6 ? 1 : 0), 6, 1);
        }
    }

    public string PostingDate =>
        ExtractedDate.HasValue
        ? ExtractedDate.Value.ToString("yyyy-MM-dd")
        : DateTime.UtcNow.ToString("yyyy-MM-dd");

    public string DueDate =>
        PaymentDueDate.HasValue &&
        PaymentDueDate >= ExtractedDate
            ? PaymentDueDate.Value.ToString("yyyy-MM-dd")
            : (ExtractedDate.HasValue
                ? ExtractedDate.Value.ToString("yyyy-MM-dd")
                : DateTime.UtcNow.ToString("yyyy-MM-dd"));

    public string DocumentDate =>
        InvoiceDate.HasValue
            ? (InvoiceDate.Value >= FiscalYear
                ? InvoiceDate.Value.ToString("yyyy-MM-dd")
                : FiscalYear.ToString("yyyy-MM-dd"))
            : FiscalYear.ToString("yyyy-MM-dd");
}

public class InvoiceProcessingResult : IError
{
    public List<NonPOLineItemError> ErrorsNoPo { get; } = [];
    public List<GrpoLineItemError> ErrorsGrpo { get; } = [];
    public bool HasErrors => ErrorsNoPo.Count > 0 || ErrorsGrpo.Count > 0;

    public List<IError> Reasons => throw new NotImplementedException();

    public string Message => throw new NotImplementedException();

    public Dictionary<string, object> Metadata => throw new NotImplementedException();
}

public class GrpoLineItemError
{
    public string CardCode { get; set; }
    public string CustomerRefNo { get; set; }
    public string PODocNum { get; set; }
    public string GRPODocNum { get; set; }
    public string PostingDate { get; set; }
    public string DueDate { get; set; }
    public string DocumentDate { get; set; }
    public string ConcurOrderID { get; set; }
    public string DocumentType { get; set; }
    public string ErrorMessage { get; set; }

    public static GrpoLineItemError Create(Item item, InvoiceHeader header, string error)
    {
        return new GrpoLineItemError
        {
            CardCode = header?.CardCode,
            CustomerRefNo = header?.CustomerRefNo,
            PODocNum = item.PODocNum,
            GRPODocNum = item.GRPODocNum,
            PostingDate = header?.PostingDate,
            DueDate = header?.DueDate,
            DocumentDate = header?.DocumentDate,
            ConcurOrderID = header?.ConcurOrderID,
            DocumentType = header?.DocumentType,
            ErrorMessage = error
        };
    }

    public static GrpoLineItemError Create(Invoice invoice, string goodsReceipt)
    {
        return new GrpoLineItemError
        {
            CardCode = invoice.VendorRemitAddress.FormattedVendorCode,
            CustomerRefNo = invoice?.InvoiceNumber,
            PODocNum = string.Empty,
            GRPODocNum = string.Empty,
            PostingDate = invoice?.PostingDate,
            DueDate = invoice?.DueDate,
            DocumentDate = invoice?.DocumentDate,
            ConcurOrderID = invoice?.ID,
            DocumentType = "dDocument_Items",
            ErrorMessage = $"Could not find GRPO record - {goodsReceipt}"
        };
    }
}

public class NonPOLineItemError
{
    public string BPCode { get; set; }
    public string BPInvoiceNumber { get; set; }
    public string DocDate { get; set; }
    public string DueDate { get; set; }
    public string DocComments { get; set; }
    public string GLAccount { get; set; }
    public string Description { get; set; }
    public decimal TotalPrice { get; set; }
    public string PostingDate { get; set; }
    public string BrandFamily { get; set; }
    public string Facility { get; set; }
    public string Error { get; set; }

    public static NonPOLineItemError Create(Item item, InvoiceHeader header, string error)
    {
        return new NonPOLineItemError
        {
            BPCode = header?.CardCode,
            BPInvoiceNumber = header?.CustomerRefNo,
            DocDate = header?.DocumentDate,
            DueDate = header?.DueDate,
            GLAccount = item.ItemDescription switch
            {
                "FREIGHT" => "55030-00",
                "VAT1" => "66010-00",
                _ => item.Custom4
            },
            Description = item.ItemDescription?.Replace(",", " "),
            TotalPrice = item.TotalPrice,
            PostingDate = header?.PostingDate,
            BrandFamily = item.BrandFamily,
            Facility = item.Facility,
            Error = error
        };
    }
}

public sealed class InvoiceGroup
{
    public CompanyReference Company { get; private init; }
    public List<Invoice> Invoices { get; private init; }

    private InvoiceGroup() { }

    public static InvoiceGroup Create(
        CompanyReference company,
        List<Invoice> invoices)
    {
        return new InvoiceGroup
        {
            Company = company,
            Invoices = invoices
        };
    }
}
