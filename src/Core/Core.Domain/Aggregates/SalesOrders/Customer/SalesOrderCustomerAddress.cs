using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tilray.Integrations.Core.Domain.Aggregates.Sales;
using Tilray.Integrations.Core.Domain.Aggregates.Sales.Rootstock;

namespace Tilray.Integrations.Core.Domain.Aggregates.Sales.Customer
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
        public ExternalReferenceId TaxLocation { get; set; }
        public string Storefront { get; set; }
        public double CustomerNextAddressSequence { get; set; }


        private static string GetTaxLocation(string state)
        {
            return state switch
            {
                "AB" => "ALBERTA",
                "BC" => "BRITISH COLUMBIA",
                "MB" => "MANITOBA",
                "NB" => "NEW BRUNSWICK",
                "NL" => "NEWFOUNDLAND",
                "NS" => "NOVA SCOTIA",
                "ON" => "ONTARIO",
                "PE" => "PRINCE EDWARD ISLAND",
                "QC" => "QUEBEC",
                "SK" => "SASKATCHEWAN",
                "NT" => "NORTHWEST TERRITORIES",
                "NU" => "NUNAVUT",
                "YT" => "YUKON",
                _ => state,
            };
        }

        public static SalesOrderCustomerAddress Create(Models.Ecom.SalesOrder order)
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
                TaxLocation = ExternalReferenceId.Create("rstk__sotax__c", GetTaxLocation(order.ShipToState)),
                Storefront = order.StoreName
            };
        }

        public void SetCustomerNextAddressSequence(double sequence) => CustomerNextAddressSequence = sequence;
        public void SetName(string name) => Name = name;

        public RstkCustomerAddress GetRootstockCustomerAddress(string CustomerAccountNumber)
        {
            var address = RstkCustomerAddress.Create();
            address.SetRstkSocaddrCustnoR(ExternalReferenceId.Create("rstk__socust__c", CustomerAccountNumber));
            address.SetExternalCustomerNumberC($"{CustomerAccountNumber}_{CustomerNextAddressSequence}");
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