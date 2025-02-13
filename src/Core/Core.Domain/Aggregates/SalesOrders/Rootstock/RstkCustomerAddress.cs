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
        public static RstkCustomerAddress Create(
            ExternalReferenceId rstkSocaddrCustnoC,
            string externalCustomerNumberC,
            string rstkSocaddrNameC,
            string rstkSocaddrAddress1C,
            string rstkSocaddrAddress2C,
            string rstkSocaddrCityC,
            string rstkSocaddrCountryC,
            string rstkSocaddrStateC,
            string rstkSocaddrZipC,
            string rstkSocaddrEmailC,
            bool rstkSocaddrUseasackC,
            bool rstkSocaddrUseasbilltoC,
            bool rstkSocaddrUseasinstallC,
            bool rstkSocaddrUseasshiptoC,
            bool rstkSocaddrDefaultackC,
            bool rstkSocaddrDefaultbilltoC,
            bool rstkSocaddrDefaultinstallC,
            bool rstkSocaddrDefaultshiptoC,
            bool rstkSocaddrDefaultackUiC,
            bool rstkSocaddrDefaultbilltoUiC,
            bool rstkSocaddrDefaultinstallUiC,
            bool rstkSocaddrDefaultshiptoUiC,
            double? rstkSocaddrSeqC,
            ExternalReferenceId rstkSocaddrTaxlocR)
        {
            return new RstkCustomerAddress
            {
                rstk__socaddr_custno__c = rstkSocaddrCustnoC.rstk__externalid__c,
                External_Customer_Number__c = externalCustomerNumberC,
                rstk__socaddr_name__c = rstkSocaddrNameC,
                rstk__socaddr_address1__c = rstkSocaddrAddress1C,
                rstk__socaddr_address2__c = rstkSocaddrAddress2C,
                rstk__socaddr_city__c = rstkSocaddrCityC,
                rstk__socaddr_country__c = rstkSocaddrCountryC,
                rstk__socaddr_state__c = rstkSocaddrStateC,
                rstk__socaddr_zip__c = rstkSocaddrZipC,
                rstk__socaddr_email__c = rstkSocaddrEmailC,
                rstk__socaddr_useasack__c = rstkSocaddrUseasackC,
                rstk__socaddr_useasbillto__c = rstkSocaddrUseasbilltoC,
                rstk__socaddr_useasinstall__c = rstkSocaddrUseasinstallC,
                rstk__socaddr_useasshipto__c = rstkSocaddrUseasshiptoC,
                rstk__socaddr_defaultack__c = rstkSocaddrDefaultackC,
                rstk__socaddr_defaultbillto__c = rstkSocaddrDefaultbilltoC,
                rstk__socaddr_defaultinstall__c = rstkSocaddrDefaultinstallC,
                rstk__socaddr_defaultshipto__c = rstkSocaddrDefaultshiptoC,
                rstk__socaddr_defaultack_ui__c = rstkSocaddrDefaultackUiC,
                rstk__socaddr_defaultbillto_ui__c = rstkSocaddrDefaultbilltoUiC,
                rstk__socaddr_defaultinstall_ui__c = rstkSocaddrDefaultinstallUiC,
                rstk__socaddr_defaultshipto_ui__c = rstkSocaddrDefaultshiptoUiC,
                rstk__socaddr_seq__c = rstkSocaddrSeqC,
                rstk__socaddr_taxloc__r = rstkSocaddrTaxlocR
            };
        }

        #endregion
    }
}
