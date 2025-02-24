using Tilray.Integrations.Core.Domain.Aggregates.Invoices;

namespace Tilray.Integrations.Services.Rootstock.Service.Models
{
    public class APATOError
    {
        public string? rstkf__apato_glacct__c { get; private set; }
        public string? rstkf__apato_docno__c { get; private set; }
        public string? rstkf__apato_trandate__c { get; private set; }
        public string? rstkf__apato_line_desc__c { get; private set; }
        public double? rstkf__apato_quantity__c { get; private set; }
        public double? rstkf__apato_linemaintamt__c { get; private set; }
        public double? rstkf__apato_distmaintamt__c { get; private set; }
        public double? rstkf__apato_txnmaintamt__c { get; private set; }
        public string? rstkf__apato_trantype__c { get; private set; }
        public string? rstkf__apato_paytype__c { get; private set; }
        public string? rstkf__apato_status__c { get; private set; }
        public string? rstkf__apato_cmpno__c { get; private set; }
        public string? rstkf__apato_vendno__c { get; private set; }
        public string? rstkf__apato_txnset__c { get; private set; }
        public string? rstkf__apato_lineset__c { get; private set; }
        public string? error { get; private set; }

        private APATOError() { }

        public static APATOError Create(APATOLineItem item, string glAccountId, string companyId, string vendorId, string error)
        {
            return new APATOError
            {
                rstkf__apato_glacct__c = glAccountId,
                rstkf__apato_docno__c = item.DocumentNumber,
                rstkf__apato_trandate__c = item.TransactionDate,
                rstkf__apato_line_desc__c = item.Description.Length > 100 ? item.Description.Substring(0, 100) : item.Description,
                rstkf__apato_quantity__c = item.Quantity,
                rstkf__apato_linemaintamt__c = item.LineTotal,
                rstkf__apato_distmaintamt__c = item.LineTotal,
                rstkf__apato_txnmaintamt__c = item.TransactionTotal,
                rstkf__apato_trantype__c = item.TransactionType,
                rstkf__apato_paytype__c = item.PayType,
                rstkf__apato_status__c = item.Status,
                rstkf__apato_cmpno__c = companyId,
                rstkf__apato_vendno__c = vendorId,
                rstkf__apato_txnset__c = item.TransactionSet,
                rstkf__apato_lineset__c = item.LineSet,
                error = error
            };
        }
    }
}
