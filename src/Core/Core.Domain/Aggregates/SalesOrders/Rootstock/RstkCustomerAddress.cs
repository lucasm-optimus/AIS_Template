using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tilray.Integrations.Core.Domain.Aggregates.Sales;
using Tilray.Integrations.Core.Domain.Aggregates.Sales.Customer;

namespace Tilray.Integrations.Core.Domain.Aggregates.Sales.Rootstock
{
    public class RstkCustomerAddress
    {
        #region Properties

        public string rstk__socaddr_custno__c { get; private set; }
        public string External_Customer_Number__c { get; private set; }
        public string rstk__socaddr_name__c { get; private set; }
        public string rstk__socaddr_address1__c { get; private set; }
        public string rstk__socaddr_address2__c { get; private set; }
        public string rstk__socaddr_city__c { get; private set; }
        public string rstk__socaddr_country__c { get; private set; }
        public string rstk__socaddr_state__c { get; private set; }
        public string rstk__socaddr_zip__c { get; private set; }
        public string rstk__socaddr_email__c { get; private set; }
        public bool rstk__socaddr_useasack__c { get; private set; }
        public bool rstk__socaddr_useasbillto__c { get; private set; }
        public bool rstk__socaddr_useasinstall__c { get; private set; }
        public bool rstk__socaddr_useasshipto__c { get; private set; }
        public bool rstk__socaddr_defaultack__c { get; private set; }
        public bool rstk__socaddr_defaultbillto__c { get; private set; }
        public bool rstk__socaddr_defaultinstall__c { get; private set; }
        public bool rstk__socaddr_defaultshipto__c { get; private set; }
        public bool rstk__socaddr_defaultack_ui__c { get; private set; }
        public bool rstk__socaddr_defaultbillto_ui__c { get; private set; }
        public bool rstk__socaddr_defaultinstall_ui__c { get; private set; }
        public bool rstk__socaddr_defaultshipto_ui__c { get; private set; }
        public double? rstk__socaddr_seq__c { get; private set; }
        public ExternalReferenceId rstk__socaddr_taxloc__r { get; private set; }

        #endregion

        #region Constructors
        private RstkCustomerAddress() { }
        public static Result<RstkCustomerAddress> Create(SalesOrderCustomerAddress salesOrderCustomerAddress, string customerId, int customerNextAddressSequence)
        {
            try
            {
                var rootstockCustomerAddress = new RstkCustomerAddress
                {
                    rstk__socaddr_custno__c = customerId,
                    External_Customer_Number__c = $"{customerId}_{customerNextAddressSequence}",
                    rstk__socaddr_name__c = salesOrderCustomerAddress.Name,
                    rstk__socaddr_address1__c = salesOrderCustomerAddress.Address1,
                    rstk__socaddr_address2__c = salesOrderCustomerAddress.Address2,
                    rstk__socaddr_city__c = salesOrderCustomerAddress.City,
                    rstk__socaddr_country__c = salesOrderCustomerAddress.Country,
                    rstk__socaddr_state__c = salesOrderCustomerAddress.State,
                    rstk__socaddr_zip__c = salesOrderCustomerAddress.Zip,
                    rstk__socaddr_email__c = salesOrderCustomerAddress.Email,
                    rstk__socaddr_useasack__c = salesOrderCustomerAddress.IsAcknowledgement,
                    rstk__socaddr_useasbillto__c = salesOrderCustomerAddress.IsBillTo,
                    rstk__socaddr_useasinstall__c = salesOrderCustomerAddress.IsInstallation,
                    rstk__socaddr_useasshipto__c = salesOrderCustomerAddress.IsShipTo,
                    rstk__socaddr_defaultack__c = salesOrderCustomerAddress.IsDefaultAcknowledgement,
                    rstk__socaddr_defaultbillto__c = salesOrderCustomerAddress.IsDefaultBillTo,
                    rstk__socaddr_defaultinstall__c = salesOrderCustomerAddress.IsDefaultInstallation,
                    rstk__socaddr_defaultshipto__c = salesOrderCustomerAddress.IsDefaultShipTo,
                    rstk__socaddr_seq__c = customerNextAddressSequence,
                    rstk__socaddr_taxloc__r = salesOrderCustomerAddress.TaxLocation
                };

                return Result.Ok(rootstockCustomerAddress);
            }
            catch (Exception e)
            {
                return Result.Fail<RstkCustomerAddress>(e.Message);
            }
        }

        #endregion
    }
}
