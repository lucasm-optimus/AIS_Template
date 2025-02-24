namespace Tilray.Integrations.Services.Rootstock.Service.Models
{
    public class RootstockSalesOrderApi
    {
        #region Properties

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string rstk__soapi_mode__c { get; private set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string rstk__soapi_sohdr__c { get; private set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string rstk__soapi_soprod__c { get; private set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public double rstk__soapi_qtyorder__c { get; private set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public double? rstk__soapi_price__c { get; private set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public bool? rstk__soapi_firm__c { get; private set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public bool rstk__soapi_taxexempt__c { get; private set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string? required_Lot_To_Pick__c { get; private set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public bool rstk__soapi_updatecustfields__c { get; private set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public bool? rstk__soapi_async__c { get; private set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string rstk__soapi_upgroup__c { get; private set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public double? amount_Covered_By_Insurance__c { get; private set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public double? grams_Covered_By_Insurance__c { get; private set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string rstk__soapi_shiplocnum__c { get; private set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string? currencyIsoCode { get; private set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public bool rstk__soapi_addorupdate__c { get; private set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string rstk__soapi_custref__c { get; private set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string rstk__soapi_salesdiv__c { get; private set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string rstk__soapi_otype__c { get; private set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string rstk__soapi_orderdate__c { get; private set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string allocation_Sent_Date__c { get; private set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string? ship_Date__c { get; private set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string expected_Delivery_Date__c { get; private set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string? order_Received_Date__c { get; private set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string? rstk__soapi_custpo__c { get; private set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string rstk__soapi_socust__c { get; private set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string? rstk__soapi_ackaddr__c { get; private set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string? rstk__soapi_shiptoaddr__c { get; private set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string? rstk__soapi_instaddr__c { get; private set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string? rstk__soapi_billtoaddr__c { get; private set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string? rstk__soapi_carrier__c { get; private set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string? rstk__soapi_shipvia__c { get; private set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string? rstk__soapi_intcomment__c { get; private set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string cc_Order__c { get; private set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string external_Order_Reference__c { get; private set; }

        #endregion

        #region Constructors

        private RootstockSalesOrderApi() { }

        public static Result<RootstockSalesOrderApi> Create(MedSalesOrder SalesOrder)
        {
            try
            {
                var rstkSalesOrder = new RootstockSalesOrderApi
                {
                    rstk__soapi_mode__c = "Add Both",
                    rstk__soapi_addorupdate__c = false,
                    rstk__soapi_custref__c = SalesOrder.CustomerReference,
                    rstk__soapi_salesdiv__c = SalesOrder.Division,
                    rstk__soapi_updatecustfields__c = true,
                    currencyIsoCode = SalesOrder.CurrencyIsoCode ?? null,
                    rstk__soapi_otype__c = SalesOrder.OrderType ?? string.Empty,
                    rstk__soapi_orderdate__c = SalesOrder.OrderDate.ToString("yyyy-MM-dd"),
                    ship_Date__c = SalesOrder.ShipDate != DateTime.MinValue ? SalesOrder.ShipDate.ToString("yyyy-MM-dd") : null,
                    order_Received_Date__c = SalesOrder.OrderReceivedDate != DateTime.MinValue ? SalesOrder.OrderReceivedDate.ToString("yyyy-MM-dd") : null,
                    rstk__soapi_custpo__c = SalesOrder.CustomerPO ?? null,
                    rstk__soapi_socust__c = SalesOrder.CustomerId,
                    rstk__soapi_ackaddr__c = SalesOrder.CustomerAddresses?.Acknowledgement?.AddressReference?.Reference != null ? SalesOrder.CustomerAddressId : null,
                    rstk__soapi_shiptoaddr__c = SalesOrder.CustomerAddresses?.ShipTo?.AddressReference?.Reference != null ? SalesOrder.CustomerAddressId : null,
                    rstk__soapi_instaddr__c = SalesOrder.CustomerAddresses?.Installation?.AddressReference?.Reference != null ? SalesOrder.CustomerAddressId : null,
                    rstk__soapi_billtoaddr__c = SalesOrder.CustomerAddresses?.BillTo?.AddressReference?.Reference != null ? SalesOrder.CustomerAddressId : null,
                    rstk__soapi_carrier__c = SalesOrder.ShippingCarrier != null ? SalesOrder.ShippingCarrier : null,
                    rstk__soapi_shipvia__c = SalesOrder.ShippingMethod != null ? SalesOrder.ShippingMethod : null,
                    rstk__soapi_taxexempt__c = SalesOrder.TaxExempt,
                    rstk__soapi_intcomment__c = SalesOrder.Notes ?? null,
                    rstk__soapi_async__c = false,
                    cc_Order__c = SalesOrder.CCOrder,
                    rstk__soapi_soprod__c = SalesOrder.LineItems[0].ProductId,
                    rstk__soapi_qtyorder__c = SalesOrder.LineItems[0].Quantity,
                    rstk__soapi_price__c = SalesOrder.LineItems[0].UnitPrice ?? null,
                    rstk__soapi_firm__c = SalesOrder.LineItems[0].Firm ?? null,
                    amount_Covered_By_Insurance__c = SalesOrder.LineItems[0].AmountCoveredByInsurance ?? null,
                    grams_Covered_By_Insurance__c = SalesOrder.LineItems[0].GramsCoveredByInsurance ?? null,
                    required_Lot_To_Pick__c = SalesOrder.LineItems[0].RequiredLotToPick ?? null
                };

                return Result.Ok(rstkSalesOrder);
            }
            catch (Exception e)
            {
                return Result.Fail<RootstockSalesOrderApi>(e.Message);
            }
        }

        public static Result<RootstockSalesOrderApi> Create(SalesOrderLineItem lineItem)
        {
            try
            {
                var rstkSalesOrderLineItem = new RootstockSalesOrderApi
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
                    rstk__soapi_async__c = false,
                    rstk__soapi_upgroup__c = lineItem.UploadGroup,
                    currencyIsoCode = lineItem.CurrencyIsoCode ?? null
                };

                return Result.Ok(rstkSalesOrderLineItem);
            }
            catch (Exception e)
            {
                return Result.Fail<RootstockSalesOrderApi>(e.Message);
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
