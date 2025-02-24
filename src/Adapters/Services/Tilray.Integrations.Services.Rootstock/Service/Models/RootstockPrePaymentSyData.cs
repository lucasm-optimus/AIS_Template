namespace Tilray.Integrations.Core.Domain.Aggregates.SalesOrders.Rootstock
{
    public class RootstockPrePaymentSyData
    {
        #region Properties

        public string rstk__sydata_txntype__c { get; private set; }
        public double rstk__sydata_ordpayamt__c { get; private set; }
        public string rstk__sydata_ordpayid__c { get; private set; }
        public string rstk__sydata_sogateway__c { get; private set; }
        public string rstk__sydata_sohdr__c { get; private set; }

        #endregion

        #region Constructors

        private RootstockPrePaymentSyData() { }

        public static Result<RootstockPrePaymentSyData> Create(CCPrepayment ccPrepayment)
        {
            try
            {
                var syDatPrePayment = new RootstockPrePaymentSyData
                {
                    rstk__sydata_txntype__c = "Sales Order Payment Authorization",
                    rstk__sydata_ordpayamt__c = ccPrepayment.AmountPrepaidByCC,
                    rstk__sydata_ordpayid__c = ccPrepayment.PrepaidCCTransactionID,
                    rstk__sydata_sogateway__c = ccPrepayment.PaymentGatewayId
                };

                return Result.Ok(syDatPrePayment);
            }
            catch (Exception e)
            {
                return Result.Fail<RootstockPrePaymentSyData>(e.Message);
            }
        }

        #endregion

        #region Public Methods

        public void UpdateSoHdrId(string salesOrderHeaderExternalId)
        {
            rstk__sydata_sohdr__c = salesOrderHeaderExternalId;
        }

        #endregion
    }
}
