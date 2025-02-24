using FluentResults;

namespace Tilray.Integrations.Core.Domain.Aggregates.Invoices
{
    public class APATOMQLineItem
    {
        public string CompanyNumber { get; private set; }
        public string CompanyName { get; private set; }
        public string TransactionType { get; private set; }
        public string Status { get; private set; }
        public string DocumentNumber { get; private set; }
        public string Description { get; private set; }
        public string TransactionDate { get; private set; }
        public double? Quantity { get; private set; }
        public double? LineTotal { get; private set; }
        public double? TransactionTotal { get; private set; }
        public string PayType { get; private set; }
        public string Vendor { get; private set; }
        public string GLAccount { get; private set; }
        public string TransactionSet { get; private set; }
        public string LineSet { get; private set; }

        private APATOMQLineItem() { }

        public static APATOMQLineItem Create(APATOLineItem item, string companyNumber, string companyName)
        {
            return new APATOMQLineItem
            {
                CompanyNumber = companyNumber,
                CompanyName = companyName,
                TransactionType = "Invoice",
                Status = "OPEN",
                DocumentNumber = item.DocumentNumber,
                Description = item.Description,
                TransactionDate = item.TransactionDate,
                Quantity = item.Quantity,
                LineTotal = item.LineTotal,
                TransactionTotal = item.TransactionTotal,
                PayType = item.PayType,
                Vendor = item.Vendor,
                GLAccount = item.GLAccount,
                TransactionSet = item.TransactionSet,
                LineSet = item.LineSet
            };
        }
    }
}
