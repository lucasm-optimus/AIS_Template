namespace Tilray.Integrations.Core.Domain.Aggregates.SalesOrders.Rootstock
{
    public class RstkSalesOrderPrePayment
    {
        #region Properties

        public string rstk__soppy_div__c { get; private set; }
        public string rstk__soppy_type__c { get; private set; }
        public string rstk__soppy_order__c { get; private set; }
        public string rstk__soppy_custno__c { get; private set; }
        public string rstk__soppy_addrseq__c { get; private set; }
        public double rstk__soppy_amount__c { get; private set; }
        public string rstk__soppy_appmethod__c { get; private set; }
        public string rstk__soppy_sohdrcust__c { get; private set; }
        public string rstk__soppy_ppyacct__c { get; private set; }
        public bool rstk__soppy_cctxn__c { get; private set; }

        #endregion

        #region Constructors

        private RstkSalesOrderPrePayment() { }
        public static Result<RstkSalesOrderPrePayment> Create(SalesOrderPrepayment soPrepayment, string divisionId, string paymentAccountId)
        {
            try
            {
                var prePayment = new RstkSalesOrderPrePayment
                {
                    rstk__soppy_div__c = divisionId,
                    rstk__soppy_type__c = soPrepayment.PrepaymentType,
                    rstk__soppy_order__c = soPrepayment.OrderID,
                    rstk__soppy_custno__c = soPrepayment.Customer,
                    rstk__soppy_addrseq__c = soPrepayment.CustomerBillToAddressID,
                    rstk__soppy_amount__c = soPrepayment.Amount,
                    rstk__soppy_appmethod__c = soPrepayment.ApplicationMethod,
                    rstk__soppy_sohdrcust__c = soPrepayment.SOCustomerNo,
                    rstk__soppy_ppyacct__c = paymentAccountId,
                    rstk__soppy_cctxn__c = true
                };

                return Result.Ok(prePayment);
            }
            catch (Exception e)
            {
                return Result.Fail<RstkSalesOrderPrePayment>(e.Message);
            }
        }

        #endregion
    }
}
