
namespace Tilray.Integrations.Core.Domain.Aggregates.Invoices
{
    public enum APATOLineType
    {
        Line,
        ShippingLine,
        VATAmount1Line,
        VATAmount2Line,
        VATAmount3Line,
    }

    public class APATOLineItem
    {
        private APATOLineItem() { }

        public string TransactionType { get; private set; }
        public string Status { get; private set; }
        public string? DocumentNumber { get; private set; }
        public string? Description { get; private set; }
        public string? TransactionDate { get; private set; }
        public double? Quantity { get; private set; }
        public double? LineTotal { get; private set; }
        public double? TransactionTotal { get; private set; }
        public string PayType { get; private set; }
        public string? Company { get; private set; }
        public string? Vendor { get; private set; }
        public string? GLAccount { get; private set; }
        public string? TransactionSet { get; private set; }
        public string LineSet { get; private set; }
        public string? LastModifiedDate { get; private set; }
        public string? ExtractedDate { get; private set; }

        public static Result<APATOLineItem> Create(LineItem lineItem, Invoice invoice, int lineCount, string companyNumber, APATOLineType aPATOTransactionType, GLAccountsSettings gLAccounts)
        {
            try
            {
                var line = new APATOLineItem
                {
                    TransactionType = "Invoice",
                    Status = "OPEN",
                    DocumentNumber = invoice.InvoiceNumber,
                    Description = lineItem.Description?.Replace(",", " | "),
                    TransactionDate = invoice.InvoiceDate?.ToString("yyyy-MM-dd"),
                    TransactionTotal = (double?)invoice.InvoiceAmount ?? 0,
                    PayType = "Check",
                    Company = invoice.Company,
                    Vendor = invoice.VendorRemitAddress.VendorCode,
                    TransactionSet = invoice.InvoiceNumber,
                    LineSet = lineCount.ToString()
                };

                switch (aPATOTransactionType)
                {
                    case APATOLineType.Line:
                        PopulateLineItem(line, invoice, lineItem, companyNumber, lineCount, gLAccounts);
                        break;
                    case APATOLineType.ShippingLine:
                        PopulateLineItem(line, invoice, companyNumber, gLAccounts);
                        break;
                    case APATOLineType.VATAmount1Line:
                        PopulateLineItem(line, invoice, companyNumber, gLAccounts, "VAT Amount 1", invoice.VatAmountOne, gLAccounts.SWB.Tax, gLAccounts.A1.GST);
                        break;
                    case APATOLineType.VATAmount2Line:
                        PopulateLineItem(line, invoice, companyNumber, gLAccounts, "VAT Amount 2", invoice.VatAmountTwo, gLAccounts.SWB.Tax, gLAccounts.A1.QST);
                        break;
                    case APATOLineType.VATAmount3Line:
                        PopulateLineItem(line, invoice, companyNumber, gLAccounts, "VAT Amount 3", invoice.VatAmountThree, gLAccounts.SWB.Tax, gLAccounts.SWB.Tax);
                        break;
                    default:
                        return Result.Fail<APATOLineItem>("Invalid transaction type");
                }

                return Result.Ok(line);
            }
            catch (Exception e)
            {
                return Result.Fail<APATOLineItem>(e.Message);
            }
        }

        private static void PopulateLineItem(APATOLineItem line, Invoice invoice, LineItem lineItem, string companyNumber, int lineCount, GLAccountsSettings gLAccounts)
        {
            line.Quantity = (double?)lineItem.Quantity ?? 0;
            line.LineTotal = (double?)lineItem.TotalPrice ?? 0;
            line.GLAccount = $"{companyNumber}_{lineItem.Custom4}";
            line.LastModifiedDate = invoice.LastModifiedDate?.ToString("yyyy-MM-dd HH:mm:ss");
            line.ExtractedDate = invoice.ExtractDate?.ToString("yyyy-MM-dd HH:mm:ss");
        }

        private static void PopulateLineItem(APATOLineItem line, Invoice invoice, string companyNumber, GLAccountsSettings gLAccounts)
        {
            line.Description = "Freight Amount";
            line.Quantity = 1;
            line.LineTotal = (double?)invoice.ShippingAmount;
            line.GLAccount = companyNumber == "003"
                ? $"{companyNumber}_{gLAccounts.SWB.Freight}"
                : $"{companyNumber}_{gLAccounts.A1.Freight}";
        }

        private static void PopulateLineItem(APATOLineItem line, Invoice invoice, string companyNumber, GLAccountsSettings gLAccounts, string description, decimal vatAmount, string swbTax, string a1Tax)
        {
            line.Description = description;
            line.Quantity = 1;
            line.LineTotal = (double?)vatAmount;
            line.GLAccount = companyNumber == "003"
                ? $"{companyNumber}_{swbTax}"
                : $"{companyNumber}_{a1Tax}";
        }
    }
}
