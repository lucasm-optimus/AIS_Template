using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tilray.Integrations.Core.Domain.Aggregates.Sales.Rootstock
{
    public class RstkSalesOrderPrePayment
    {
        #region Properties

        public ExternalReferenceId rstk__soppy_div__r { get; private set; }
        public string rstk__soppy_type__c { get; private set; }
        public ExternalReferenceId rstk__soppy_order__r { get; private set; }
        public ExternalReferenceId rstk__soppy_custno__r { get; private set; }
        public ExternalReferenceId rstk__soppy_addrseq__r { get; private set; }
        public double rstk__soppy_amount__c { get; private set; }
        public string rstk__soppy_appmethod__c { get; private set; }
        public ExternalReferenceId rstk__soppy_sohdrcust__r { get; private set; }
        public ExternalReferenceId rstk__soppy_ppyacct__r { get; private set; }
        public bool rstk__soppy_cctxn__c { get; private set; }

        #endregion

        #region Constructors

        private RstkSalesOrderPrePayment() { }
        public static Result<RstkSalesOrderPrePayment> Create(SalesOrderPrepayment soPrepayment)
        {
            try
            {
                var prePayment = new RstkSalesOrderPrePayment
                {
                    rstk__soppy_div__r = ExternalReferenceId.Create("rstk__sydiv__c", soPrepayment.Division),
                    rstk__soppy_type__c = soPrepayment.PrepaymentType,
                    rstk__soppy_order__r = ExternalReferenceId.Create("rstk__sohdr__c", soPrepayment.OrderID),
                    rstk__soppy_custno__r = ExternalReferenceId.Create("rstk__socust__c", soPrepayment.Customer),
                    rstk__soppy_addrseq__r = ExternalReferenceId.Create("rstk__socaddr__c", soPrepayment.CustomerBillToAddressID),
                    rstk__soppy_amount__c = soPrepayment.Amount,
                    rstk__soppy_appmethod__c = soPrepayment.ApplicationMethod,
                    rstk__soppy_sohdrcust__r = ExternalReferenceId.Create("rstk__socust__c", soPrepayment.SOCustomerNo),
                    rstk__soppy_ppyacct__r = ExternalReferenceId.Create("rstk__syacc__c", $"{soPrepayment.Division}_{soPrepayment.PrepaymentAccount}"),
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
