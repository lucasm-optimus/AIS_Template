using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tilray.Integrations.Core.Domain.Aggregates.Sales;

namespace Tilray.Integrations.Core.Domain.Aggregates.Customer
{
    public class SalesOrderCustomer
    {
        private SalesOrderCustomer() { }

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

        public static SalesOrderCustomer Create(EcomSalesOrder payload, OrderDefaultsSettings orderDefaults)
        {
            var salesOrderCustomer = new SalesOrderCustomer { CustomerNo = payload.CustomerAccountNumber };
            salesOrderCustomer.SetAccountingDimension2($"{orderDefaults.Medical.Division}_{orderDefaults.Medical.Customer.AccountingDimension2Suffix}{payload.ShipToState}");
            salesOrderCustomer.SetPaymentTerms($"{orderDefaults.Medical.Customer.PaymentTerms}");
            salesOrderCustomer.SetCustomerBuysProduct(true);
            salesOrderCustomer.SetAccountingDimension1($"{orderDefaults.Medical.Division}_{(payload.PatientType == "Veteran" ? orderDefaults.Medical.Customer.AccountingDimension1Veteran : orderDefaults.Medical.Customer.AccountingDimension1Civilian)}");
            salesOrderCustomer.SetCustomerClass(orderDefaults.Medical.Customer.CustomerClass);
            salesOrderCustomer.SetSFAccountID(payload.CustomerAccountID);
            salesOrderCustomer.SetCustomerBuysService(true);
            salesOrderCustomer.SetPlaceOrdersInTheCustomerCurrency(true);
            salesOrderCustomer.SetUseSFAddresses(false);
            salesOrderCustomer.SetDefaultProductType("All");

            return salesOrderCustomer;
        }

        public RstkCustomer GetRootstockCustomer()
        {
            var customer = RstkCustomer.Create();
            customer.SetRstkSocustCustnoC(CustomerNo);
            customer.SetRstkSocustSfAccountC(SFAccountID);
            customer.SetRstkSocustCclassR(ExternalReferenceId.Create("rstk__socclass__c", CustomerClass));
            customer.SetRstkSocustDimvalR(ExternalReferenceId.Create("rstk__sydim__c", AccountingDimension1));
            customer.SetRstkSocustDimval2R(ExternalReferenceId.Create("rstk__sydim__c", AccountingDimension2));
            customer.SetRstkSocustDfltprodtypeC(DefaultProductType);
            customer.SetRstkSocustProdindC(CustomerBuysProduct);
            customer.SetRstkSocustServiceindC(CustomerBuysService);
            customer.SetRstkSocustMaintcurrindC(PlaceOrdersInTheCustomerCurrency);
            customer.SetRstkSocustTermsR(ExternalReferenceId.Create("rstk__syterms__c", PaymentTerms));
            return customer;
        }

        public void SetAccountingDimension2(string accountingDimension2) => AccountingDimension2 = accountingDimension2;
        public void SetPaymentTerms(string paymentTerms) => PaymentTerms = paymentTerms;
        public void SetCustomerBuysProduct(bool customerBuysProduct) => CustomerBuysProduct = customerBuysProduct;
        public void SetAccountingDimension1(string accountingDimension1) => AccountingDimension1 = accountingDimension1;
        public void SetCustomerClass(string customerClass) => CustomerClass = customerClass;
        public void SetSFAccountID(string sFAccountID) => SFAccountID = sFAccountID;
        public void SetCustomerBuysService(bool customerBuysService) => CustomerBuysService = customerBuysService;
        public void SetPlaceOrdersInTheCustomerCurrency(bool placeOrdersInTheCustomerCurrency) => PlaceOrdersInTheCustomerCurrency = placeOrdersInTheCustomerCurrency;
        public void SetUseSFAddresses(bool useSFAddresses) => UseSFAddresses = useSFAddresses;
        public void SetDefaultProductType(string defaultProductType) => DefaultProductType = defaultProductType;
    }
}
