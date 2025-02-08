using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tilray.Integrations.Core.Domain.Aggregates.Sales;

namespace Tilray.Integrations.Core.Domain.Aggregates.Sales.Rootstock
{
    public class RstkCustomerAddress
    {
        private RstkCustomerAddress() { }

        public ExternalReferenceId rstk__socaddr_custno__r { get; private set; }
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
        public string rstk__socaddr_taxloc__r { get; private set; }

        public static RstkCustomerAddress Create()
        {
            return new RstkCustomerAddress();
        }

        public void SetRstkSocaddrCustnoR(ExternalReferenceId value) => rstk__socaddr_custno__r = value;
        public void SetExternalCustomerNumberC(string value) => External_Customer_Number__c = value;
        public void SetRstkSocaddrNameC(string value) => rstk__socaddr_name__c = value;
        public void SetRstkSocaddrAddress1C(string value) => rstk__socaddr_address1__c = value;
        public void SetRstkSocaddrAddress2C(string value) => rstk__socaddr_address2__c = value;
        public void SetRstkSocaddrCityC(string value) => rstk__socaddr_city__c = value;
        public void SetRstkSocaddrCountryC(string value) => rstk__socaddr_country__c = value;
        public void SetRstkSocaddrStateC(string value) => rstk__socaddr_state__c = value;
        public void SetRstkSocaddrZipC(string value) => rstk__socaddr_zip__c = value;
        public void SetRstkSocaddrEmailC(string value) => rstk__socaddr_email__c = value;
        public void SetRstkSocaddrUseasackC(bool value) => rstk__socaddr_useasack__c = value;
        public void SetRstkSocaddrUseasbilltoC(bool value) => rstk__socaddr_useasbillto__c = value;
        public void SetRstkSocaddrUseasinstallC(bool value) => rstk__socaddr_useasinstall__c = value;
        public void SetRstkSocaddrUseasshiptoC(bool value) => rstk__socaddr_useasshipto__c = value;
        public void SetRstkSocaddrDefaultackC(bool value) => rstk__socaddr_defaultack__c = value;
        public void SetRstkSocaddrDefaultbilltoC(bool value) => rstk__socaddr_defaultbillto__c = value;
        public void SetRstkSocaddrDefaultinstallC(bool value) => rstk__socaddr_defaultinstall__c = value;
        public void SetRstkSocaddrDefaultshiptoC(bool value) => rstk__socaddr_defaultshipto__c = value;
        public void SetRstkSocaddrDefaultackUiC(bool value) => rstk__socaddr_defaultack_ui__c = value;
        public void SetRstkSocaddrDefaultbilltoUiC(bool value) => rstk__socaddr_defaultbillto_ui__c = value;
        public void SetRstkSocaddrDefaultinstallUiC(bool value) => rstk__socaddr_defaultinstall_ui__c = value;
        public void SetRstkSocaddrDefaultshiptoUiC(bool value) => rstk__socaddr_defaultshipto_ui__c = value;
        public void SetRstkSocaddrSeqC(double? value) => rstk__socaddr_seq__c = value;
        public void SetRstkSocaddrTaxlocR(string value) => rstk__socaddr_taxloc__r = value;

        public static string GetCreatedRowId(dynamic payload)
        {
            return payload.items[0]["id"];
        }
    }
}
