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
        #region Properties

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

        #endregion

        #region Constructors

        private SalesOrderCustomerAddress() { }
        public static Result<SalesOrderCustomerAddress> Create(Models.Ecom.SalesOrder payload)
        {
            try
            {
                var customerAddress = new SalesOrderCustomerAddress
                {
                    Address1 = payload.ShipToAddress1,
                    Address2 = payload.ShipToAddress2,
                    City = payload.ShipToCity,
                    State = payload.ShipToState,
                    Zip = payload.ShipToZip,
                    Country = payload.ShipToCountry,
                    Email = payload.ShipToEmail,
                    IsShipTo = true,
                    IsDefaultShipTo = true,
                    IsBillTo = true,
                    IsDefaultBillTo = true,
                    IsInstallation = true,
                    IsDefaultInstallation = true,
                    IsAcknowledgement = true,
                    IsDefaultAcknowledgement = true,
                    TaxLocation = GetTaxLocation(payload.ShipToState),
                    Storefront = payload.StoreName
                };

                return Result.Ok(customerAddress);
            }
            catch (Exception e)
            {
                return Result.Fail<SalesOrderCustomerAddress>(e.Message);
            }
        }

        #endregion

        #region Private Methods

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

        public void UpdateTaxLocation(string value)
        {
            TaxLocation = value;
        }

        #endregion
    }
}