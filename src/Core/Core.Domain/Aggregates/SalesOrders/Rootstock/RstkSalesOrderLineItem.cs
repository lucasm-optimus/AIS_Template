using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tilray.Integrations.Core.Domain.Aggregates.Sales.Rootstock
{
    public class RstkSalesOrderLineItem
    {
        #region Properties

        public string rstk__soapi_mode__c { get; private set; }
        public string rstk__soapi_sohdr__c { get; private set; }
        public string rstk__soapi_soprod__c { get; private set; }
        public double rstk__soapi_qtyorder__c { get; private set; }
        public double? rstk__soapi_price__c { get; private set; }
        public bool? rstk__soapi_firm__c { get; private set; }
        public bool rstk__soapi_taxexempt__c { get; private set; }
        public string required_Lot_To_Pick__c { get; private set; }
        public bool rstk__soapi_updatecustfields__c { get; private set; }
        public bool? rstk__soapi_async__c { get; private set; }
        public string rstk__soapi_upgroup__c { get; private set; }
        public double? amount_Covered_By_Insurance__c { get; private set; }
        public double? grams_Covered_By_Insurance__c { get; private set; }
        public string rstk__soapi_shiplocnum__c { get; private set; }
        public string currencyIsoCode { get; private set; }

        #endregion

        #region Constructors

        private RstkSalesOrderLineItem() { }
        public static Result<RstkSalesOrderLineItem> Create(MedSalesOrder salesOrder, SalesOrderLineItem lineItem)
        {
            try
            {
                var rstkSalesOrderLineItem = new RstkSalesOrderLineItem
                {
                    rstk__soapi_mode__c = "Add Line",
                    rstk__soapi_soprod__c = lineItem.ProductId,
                    rstk__soapi_qtyorder__c = lineItem.Quantity,
                    rstk__soapi_price__c = lineItem.UnitPrice ?? null,
                    rstk__soapi_firm__c = lineItem.Firm ?? null,
                    amount_Covered_By_Insurance__c = lineItem.AmountCoveredByInsurance ?? null,
                    grams_Covered_By_Insurance__c = lineItem.GramsCoveredByInsurance ?? null,
                    required_Lot_To_Pick__c = lineItem.RequiredLotToPick ?? null,
                    rstk__soapi_updatecustfields__c = true,
                    rstk__soapi_async__c = salesOrder.BackgroundProcessing ?? false,
                    rstk__soapi_upgroup__c = lineItem.UploadGroup,
                    currencyIsoCode = lineItem.CurrencyIsoCode ?? null
                };

                return Result.Ok(rstkSalesOrderLineItem);
            }
            catch (Exception e)
            {
                return Result.Fail<RstkSalesOrderLineItem>(e.Message);
            }
        }

        #endregion

        #region Public Methods

        public void UpdateSoHdr(string value)
        {
            rstk__soapi_sohdr__c = value;
        }
        
        #endregion
    }
}
