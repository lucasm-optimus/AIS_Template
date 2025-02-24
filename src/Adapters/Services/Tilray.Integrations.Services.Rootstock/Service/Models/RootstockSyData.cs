namespace Tilray.Integrations.Services.Rootstock.Service.Models
{
    public class RootstockSyData
    {
        public string rstk__sydata_txntype__c { get; private set; }
        public string rstk__sydata_process__c { get; private set; }
        public string rstk__sydata_pohdr__c { get; private set; }
        public decimal rstk__sydata_batchinvoiceamount__c { get; private set; }
        public string rstk__sydata_batchinvoicenumber__c { get; private set; }
        public DateTime rstk__sydata_batchinvoicedate__c { get; private set; }

        private RootstockSyData() { }

        public static Result<RootstockSyData> Create(POAPLineItem payload, string poHdrId)
        {
            try
            {
                var rootstockInvoiceSyData = new RootstockSyData
                {
                    rstk__sydata_txntype__c = "PO AP Match Using Detail",
                    rstk__sydata_process__c = "Hold",
                    rstk__sydata_pohdr__c = poHdrId,
                    rstk__sydata_batchinvoiceamount__c = (decimal)payload.VendorInvoiceAmount,
                    rstk__sydata_batchinvoicenumber__c = payload.VendorInvoiceNumber,
                    rstk__sydata_batchinvoicedate__c = DateTime.Parse(payload.VendorInvoiceDate)
                };

                return Result.Ok(rootstockInvoiceSyData);
            }
            catch (Exception e)
            {
                return Result.Fail<RootstockSyData>(e.Message);
            }
        }
    }

    public class Pohdr
    {
        public string type { get; set; }
        public string rstk__externalid__c { get; set; }
    }
}
