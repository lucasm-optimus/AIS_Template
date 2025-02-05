using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tilray.Integrations.Core.Domain.Aggregates.Sales;

namespace Tilray.Integrations.Core.Domain.Aggregates.Customer
{
    public class SalesOrderCustomerAddress
    {
        private SalesOrderCustomerAddress() { }

        public string Name { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Zip { get; set; }
        public string Country { get; set; }
        public string Email { get; set; }
        public bool IsShipTo { get; set; }
        public bool IsDefaultShipTo { get; set; }
        public bool IsBillTo { get; set; }
        public bool IsDefaultBillTo { get; set; }
        public bool IsInstallation { get; set; }
        public bool IsDefaultInstallation { get; set; }
        public bool IsAcknowledgement { get; set; }
        public bool IsDefaultAcknowledgement { get; set; }
        public string TaxLocation { get; set; }
        public string Storefront { get; set; }
        public double CustomerNextAddressSequence { get; set; }


        private static string GetTaxLocation(string state)
        {
            return state switch
            {
                "AB" => "Alberta",
                "BC" => "British Columbia",
                "MB" => "Manitoba",
                "NB" => "New Brunswick",
                "NL" => "Newfoundland",
                "NS" => "Nova Scotia",
                "ON" => "Ontario",
                "PE" => "Prince Edward Island",
                "QC" => "Quebec",
                "SK" => "Saskatchewan",
                "NT" => "Northwest Territories",
                "NU" => "Nunavut",
                "YT" => "Yukon",
                _ => state,
            };
        }

        public static SalesOrderCustomerAddress Create(EcomSalesOrder order, double sequence, string name)
        {
            return new SalesOrderCustomerAddress
            {
                Address1 = order.ShipToAddress1,
                Address2 = order.ShipToAddress2,
                City = order.ShipToCity,
                State = order.ShipToState,
                Zip = order.ShipToZip,
                Country = order.ShipToCountry,
                Email = order.ShipToEmail,
                IsShipTo = true,
                IsDefaultShipTo = true,
                IsBillTo = true,
                IsDefaultBillTo = true,
                IsInstallation = true,
                IsDefaultInstallation = true,
                IsAcknowledgement = true,
                IsDefaultAcknowledgement = true,
                TaxLocation = GetTaxLocation(order.ShipToState),
                Storefront = order.StoreName,
                CustomerNextAddressSequence = sequence,
                Name = name
            };
        }

        public RstkCustomerAddress GetRootstockCustomerAddress(EcomSalesOrder payload)
        {
            var address = RstkCustomerAddress.Create();
            address.SetRstkSocaddrCustnoR(ExternalReferenceId.Create("rstk__socust__c", payload.CustomerAccountNumber));
            address.SetExternalCustomerNumberC($"{payload.CustomerAccountNumber}_{CustomerNextAddressSequence}");
            address.SetRstkSocaddrNameC(Name);
            address.SetRstkSocaddrAddress1C(Address1);
            address.SetRstkSocaddrAddress2C(Address2);
            address.SetRstkSocaddrCityC(City);
            address.SetRstkSocaddrCountryC(Country);
            address.SetRstkSocaddrStateC(State);
            address.SetRstkSocaddrZipC(Zip);
            address.SetRstkSocaddrEmailC(Email);
            address.SetRstkSocaddrUseasackC(IsAcknowledgement);
            address.SetRstkSocaddrUseasbilltoC(IsBillTo);
            address.SetRstkSocaddrUseasinstallC(IsInstallation);
            address.SetRstkSocaddrUseasshiptoC(IsShipTo);
            address.SetRstkSocaddrDefaultackC(IsDefaultAcknowledgement);
            address.SetRstkSocaddrDefaultbilltoC(IsDefaultBillTo);
            address.SetRstkSocaddrDefaultinstallC(IsDefaultInstallation);
            address.SetRstkSocaddrDefaultshiptoC(IsDefaultShipTo);
            address.SetRstkSocaddrDefaultackUiC(IsDefaultAcknowledgement);
            address.SetRstkSocaddrDefaultbilltoUiC(IsDefaultBillTo);
            address.SetRstkSocaddrDefaultinstallUiC(IsDefaultInstallation);
            address.SetRstkSocaddrDefaultshiptoUiC(IsDefaultShipTo);
            address.SetRstkSocaddrSeqC(CustomerNextAddressSequence);
            address.SetRstkSocaddrTaxlocR(TaxLocation);

            return address;
        }
    }
}