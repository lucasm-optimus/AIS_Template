namespace Tilray.Integrations.Core.Domain.Aggregates.Invoices
{
    public class DetailSummation
    {
        public string TransactionType { get; set; }
        public double? LineAmount { get; set; }
        public string? LineDescription { get; set; }
        public string? SubLedgerAccount { get; set; }
        public List<string> PurchaseOrderReceipts { get; set; }
    }
}
