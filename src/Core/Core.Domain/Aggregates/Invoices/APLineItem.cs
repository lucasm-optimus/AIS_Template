namespace Tilray.Integrations.Core.Domain.Aggregates.Invoices;

public enum APMatchedLineType
{
    VATAmount1Line,
    VATAmount2Line,
    VATAmount3Line,
}

public class APLineItem
{
    #region Constructors

    private APLineItem() { }

    public static Result<APLineItem> Create(Invoice invoice)
    {
        try
        {
            var header = new APLineItem
            {
                LineType = "HEADER",
                TransactionType = "PO AP Match Using Detail",
                ProcessingIndicator = "Hold",
                Title = invoice.Name?.Replace(",", " | "),
                Description = invoice.Description?.Replace(",", " | "),
                VendorInvoiceNumber = invoice.InvoiceNumber,
                PurchaseOrderNumber = invoice.PurchaseOrderNumber?.Replace(",", " | "),
                PurchaseOrderHeader = invoice.Custom9,
                VendorInvoiceDate = invoice.InvoiceDate?.ToString("yyyy-MM-dd"),
                PaymentDueDate = invoice.PaymentDueDate?.ToString("yyyy-MM-dd"),
                InvoiceReceivedDate = invoice.InvoiceReceivedDate?.ToString("yyyy-MM-dd"),
                LastModifiedDate = invoice.LastModifiedDate?.ToString("yyyy-MM-dd HH:mm:ss"),
                ExtractedDate = invoice.ExtractDate?.ToString("yyyy-MM-dd HH:mm:ss"),
                VendorInvoiceAmount = (double?)invoice.InvoiceAmount,
                TotalAmount = (double?)invoice.CalculatedAmount,
                CurrencyCode = invoice.CurrencyCode,
                VendorCode = invoice.VendorRemitAddress?.VendorCode?.Replace(",", " | "),

                ShippingAmount = (double?)invoice.ShippingAmount ?? null,
                VATAmount1 = (double?)invoice.VatAmountOne,
                VATAmount2 = (double?)invoice.VatAmountTwo,
            };

            return Result.Ok(header);
        }
        catch (Exception e)
        {
            return Result.Fail<APLineItem>(e.Message);
        }
    }

    public static Result<APLineItem> Create(Invoice invoice, LineItem lineItem, int lineCount)
    {
        try
        {
            var detail = new APLineItem
            {
                LineType = "DETAIL",
                TransactionType = "PO Receipt",
                VendorInvoiceNumber = invoice.InvoiceNumber,
                PurchaseOrderNumber = invoice.PurchaseOrderNumber?.Replace(",", " | "),
                VendorInvoiceDate = invoice.InvoiceDate?.ToString("yyyy-MM-dd"),
                PaymentDueDate = invoice.PaymentDueDate?.ToString("yyyy-MM-dd"),
                InvoiceReceivedDate = invoice.InvoiceReceivedDate?.ToString("yyyy-MM-dd"),
                VendorInvoiceAmount = (double?)invoice.InvoiceAmount,
                TotalAmount = (double?)invoice.CalculatedAmount,
                CurrencyCode = invoice.CurrencyCode,
                VendorCode = invoice.VendorRemitAddress?.VendorCode?.Replace(",", " | "),
                LineItemSequenceOrder = lineCount,
                LineDescription = lineItem.Description?.Replace(",", " | "),
                LineAmount = (double?)lineItem.TotalPrice,
                APEntryQuantity = (double?)lineItem.Quantity,
                APEntryPrice = (double?)lineItem.UnitPrice,
                PurchaseOrderReceipt = string.Join(" | ", lineItem.MatchedPurchaseOrderReceipts?.MatchedPurchaseOrderReceipt?.Select(r => r.GoodsReceiptNumber).AsEnumerable()),
                DeliverySlipNumber = invoice.DeliverySlipNumber,
                DetailType = "LineItem"
            };

            return Result.Ok(detail);
        }
        catch (Exception e)
        {
            return Result.Fail<APLineItem>(e.Message);
        }
    }

    public static Result<APLineItem> Create(Invoice invoice, int lineCount, string companyNumber, GLAccountsSettings glAccounts)
    {
        try
        {
            var line = new APLineItem
            {
                LineType = "DETAIL",
                TransactionType = "GL Entry",
                VendorInvoiceNumber = invoice.InvoiceNumber,
                VendorInvoiceDate = invoice.InvoiceDate?.ToString("yyyy-MM-dd"),
                PurchaseOrderNumber = invoice.PurchaseOrderNumber?.Replace(",", " | "),
                PaymentDueDate = invoice.PaymentDueDate?.ToString("yyyy-MM-dd"),
                InvoiceReceivedDate = invoice.InvoiceReceivedDate?.ToString("yyyy-MM-dd"),
                VendorInvoiceAmount = (double?)invoice.InvoiceAmount,
                TotalAmount = (double?)invoice.CalculatedAmount,
                CurrencyCode = invoice.CurrencyCode,
                VendorCode = invoice.VendorRemitAddress?.VendorCode?.Replace(",", " | "),
                LineItemSequenceOrder = lineCount,
                ShippingAmount = (double?)invoice.ShippingAmount ?? null,
                LineDescription = "Freight Amount",
                LineAmount = (double?)invoice.ShippingAmount ?? null,
                APEntryQuantity = 1,
                APEntryPrice = (double?)invoice.ShippingAmount ?? null,
                DetailType = "Freight",
                SubLedgerAccount = companyNumber == "003"
                ? $"{companyNumber}_{glAccounts.SWB.Freight}"
                : $"{companyNumber}_{glAccounts.A1.Freight}"
            };

            return Result.Ok(line);
        }
        catch (Exception e)
        {
            return Result.Fail<APLineItem>(e.Message);
        }
    }

    public static Result<APLineItem> Create(Invoice invoice, int lineCount, APMatchedLineType transactionType, string companyNumber, GLAccountsSettings gLAccounts)
    {
        try
        {
            var line = new APLineItem
            {
                LineType = "DETAIL",
                TransactionType = "GL Entry",
                VendorInvoiceNumber = invoice.InvoiceNumber,
                VendorInvoiceDate = invoice.InvoiceDate?.ToString("yyyy-MM-dd"),
                PurchaseOrderNumber = invoice.PurchaseOrderNumber?.Replace(",", " | "),
                PaymentDueDate = invoice.PaymentDueDate?.ToString("yyyy-MM-dd"),
                InvoiceReceivedDate = invoice.InvoiceReceivedDate?.ToString("yyyy-MM-dd"),
                VendorInvoiceAmount = (double?)invoice.InvoiceAmount,
                TotalAmount = (double?)invoice.CalculatedAmount,
                CurrencyCode = invoice.CurrencyCode,
                VendorCode = invoice.VendorRemitAddress?.VendorCode?.Replace(",", " | "),
                LineItemSequenceOrder = lineCount,
                APEntryQuantity = 1
            };

            switch (transactionType)
            {
                case APMatchedLineType.VATAmount1Line:
                    {
                        line.VATAmount1 = (double?)invoice.VatAmountOne;
                        line.LineAmount = (double?)invoice.VatAmountOne;
                        line.LineDescription = "VAT Amount 1";
                        line.APEntryPrice = (double?)invoice.VatAmountOne;
                        line.DetailType = "VAT1";
                        line.SubLedgerAccount = companyNumber == "003"
                            ? $"{companyNumber}_{gLAccounts.SWB.Tax}"
                            : $"{companyNumber}_{gLAccounts.A1.GST}";
                        break;
                    }
                case APMatchedLineType.VATAmount2Line:
                    {
                        line.VATAmount1 = (double?)invoice.VatAmountTwo;
                        line.LineAmount = (double?)invoice.VatAmountTwo;
                        line.LineDescription = "VAT Amount 2";
                        line.APEntryPrice = (double?)invoice.VatAmountTwo;
                        line.DetailType = "VAT2";
                        line.SubLedgerAccount = companyNumber == "003"
                            ? $"{companyNumber}_{gLAccounts.SWB.Tax}"
                            : $"{companyNumber}_{gLAccounts.A1.QST}";
                        break;
                    }
                case APMatchedLineType.VATAmount3Line:
                    {
                        line.VATAmount1 = (double?)invoice.VatAmountThree;
                        line.LineAmount = (double?)invoice.VatAmountThree;
                        line.LineDescription = "VAT Amount 3";
                        line.APEntryPrice = (double?)invoice.VatAmountThree;
                        line.DetailType = "VAT3";
                        line.SubLedgerAccount = $"{companyNumber}_{gLAccounts.SWB.Tax}";
                        break;
                    }
                default:
                    return Result.Fail<APLineItem>("Invalid Transaction Type");
            }


            return Result.Ok(line);
        }
        catch (Exception e)
        {
            return Result.Fail<APLineItem>(e.Message);
        }
    }

    #endregion

    #region Properties

    public string LineType { get; set; }
    public string TransactionType { get; set; }
    public string? ProcessingIndicator { get; set; }
    public string? Title { get; set; }
    public string? Description { get; set; }
    public string? VendorInvoiceNumber { get; set; }
    public string? PurchaseOrderNumber { get; set; }
    public string? PurchaseOrderHeader { get; set; }
    public string? VendorInvoiceDate { get; set; }
    public string? PaymentDueDate { get; set; }
    public string? InvoiceReceivedDate { get; set; }
    public string? LastModifiedDate { get; set; }
    public string? ExtractedDate { get; set; }
    public double? VendorInvoiceAmount { get; set; }
    public double? TotalAmount { get; set; }
    public double? ShippingAmount { get; set; }
    public double? VATAmount1 { get; set; }
    public double? VATAmount2 { get; set; }
    public double? VATAmount3 { get; set; }
    public double? VATAmount4 { get; set; }
    public string? CurrencyCode { get; set; }
    public string? VendorCode { get; set; }
    public string? SubLedgerAccount { get; set; }
    public int? LineItemSequenceOrder { get; set; }
    public string? LineDescription { get; set; }
    public double? LineAmount { get; set; }
    public double? APEntryQuantity { get; set; }
    public double? APEntryPrice { get; set; }
    public string? PurchaseOrderReceipt { get; set; }
    public string? DeliverySlipNumber { get; set; }
    public string? SystemTransactionObject { get; set; }
    public string? DetailType { get; set; }

    #endregion
}
