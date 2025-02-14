using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tilray.Integrations.Core.Domain.Aggregates.Sales;
using Tilray.Integrations.Core.Domain.Aggregates.Sales.Rootstock;

namespace Tilray.Integrations.Core.Domain.Aggregates.Sales.Customer
{
    public class SalesOrderCustomer
    {
        #region Properties

        public string CustomerNo { get; private set; }
        public string AccountingDimension2 { get; private set; }
        public string PaymentTerms { get; private set; }
        public bool CustomerBuysProduct { get; private set; }
        public string AccountingDimension1 { get; private set; }
        public string CustomerClass { get; private set; }
        public string SFAccountID { get; private set; }
        public bool CustomerBuysService { get; private set; }
        public bool PlaceOrdersInTheCustomerCurrency { get; private set; }
        public bool UseSFAddresses { get; private set; }
        public string DefaultProductType { get; private set; }

        #endregion

        #region Constructors

        private SalesOrderCustomer() { }
        public static Result<SalesOrderCustomer> Create(Models.Ecom.SalesOrder payload, OrderDefaultsSettings orderDefaults)
        {
            try
            {
                var salesOrderCustomer = new SalesOrderCustomer
                {
                    CustomerNo = payload.CustomerAccountNumber,
                    AccountingDimension2 = $"{orderDefaults.Medical.Division}_{orderDefaults.Medical.Customer.AccountingDimension2Suffix}{payload.ShipToState}",
                    PaymentTerms = $"{orderDefaults.Medical.Customer.PaymentTerms}",
                    CustomerBuysProduct = true,
                    AccountingDimension1 = $"{orderDefaults.Medical.Division}_{(payload.PatientType == "Veteran" ? orderDefaults.Medical.Customer.AccountingDimension1Veteran : orderDefaults.Medical.Customer.AccountingDimension1Civilian)}",
                    CustomerClass = orderDefaults.Medical.Customer.CustomerClass,
                    SFAccountID = payload.CustomerAccountID,
                    CustomerBuysService = true,
                    PlaceOrdersInTheCustomerCurrency = true,
                    UseSFAddresses = false,
                    DefaultProductType = "All"
                };

                return Result.Ok(salesOrderCustomer);
            }
            catch (Exception e)
            {
                return Result.Fail<SalesOrderCustomer>(e.Message);
            }
        }

        #endregion
    }
}
