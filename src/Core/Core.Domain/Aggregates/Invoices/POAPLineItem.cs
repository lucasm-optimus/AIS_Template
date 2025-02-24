namespace Tilray.Integrations.Core.Domain.Aggregates.Invoices
{
    public class POAPLineItem
    {
        #region Properties

        public string LineType { get; private set; }
        public string CompanyNumber { get; private set; }
        public string CompanyName { get; private set; }
        public string? PurchaseOrderNumber { get; private set; }
        public string? PurchaseOrderHeader { get; private set; }
        public string? VendorInvoiceNumber { get; private set; }
        public double? VendorInvoiceAmount { get; private set; }
        public string? VendorInvoiceDate { get; private set; }
        public double? APEntryPrice { get; private set; }
        public double? APEntryQuantity { get; private set; }
        public double? LineAmount { get; private set; }
        public string? LineDescription { get; private set; }
        public int? LineItemSequenceOrder { get; private set; }
        public string? PurchaseOrderReceipt { get; private set; }
        public string? SubLedgerAccount { get; private set; }
        public string? TransactionType { get; private set; }
        public string? DetailType { get; private set; }

        #endregion

        #region Constructors

        private POAPLineItem() { }

        public static POAPLineItem Create(APLineItem item, string companyName, string companyNumber)
        {
            return new POAPLineItem
            {
                LineType = item.LineType,
                CompanyNumber = companyNumber,
                CompanyName = companyName,
                PurchaseOrderNumber = item.PurchaseOrderNumber,
                PurchaseOrderHeader = item.PurchaseOrderHeader,
                VendorInvoiceNumber = item.VendorInvoiceNumber,
                VendorInvoiceAmount = item.VendorInvoiceAmount,
                VendorInvoiceDate = item.VendorInvoiceDate,
                APEntryPrice = item.APEntryPrice,
                APEntryQuantity = item.APEntryQuantity,
                LineAmount = item.LineAmount,
                LineDescription = item.LineDescription,
                LineItemSequenceOrder = item.LineItemSequenceOrder,
                PurchaseOrderReceipt = item.PurchaseOrderReceipt,
                SubLedgerAccount = item.SubLedgerAccount,
                TransactionType = item.TransactionType,
                DetailType = item.DetailType
            };
        }

        #endregion
    }
}
