namespace Tilray.Integrations.Core.Domain.Aggregates.Invoices
{
    public class APMatchError
    {
        public string TransactionType { get; private set; }
        public string LineAmount { get; private set; }
        public string LineDescription { get; private set; }
        public string SubLedgerAccount { get; private set; }
        public string PurchaseOrderReceipt { get; private set; }
        public string Error { get; private set; }

        private APMatchError() { }

        public static APMatchError Create(string transactionType, string lineAmount, string lineDescription, string subLedgerAccount, string purchaseOrderReceipt, string error)
        {
            return new APMatchError
            {
                TransactionType = transactionType,
                LineAmount = lineAmount,
                LineDescription = lineDescription,
                SubLedgerAccount = subLedgerAccount,
                PurchaseOrderReceipt = purchaseOrderReceipt,
                Error = error
            };
        }
    }
}
