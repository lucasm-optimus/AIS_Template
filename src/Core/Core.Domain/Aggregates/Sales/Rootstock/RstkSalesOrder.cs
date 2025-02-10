namespace Tilray.Integrations.Core.Domain.Aggregates.Sales.Rootstock
{
    public class RstkSalesOrder
    {
        private RstkSalesOrder() { }

        public string rstk__soapi_mode__c { get; private set; }
        public bool rstk__soapi_addorupdate__c { get; private set; }
        public string rstk__soapi_custref__c { get; private set; }
        public string rstk__soapi_salesdiv__c { get; private set; }
        public bool rstk__soapi_updatecustfields__c { get; private set; }
        public string currencyIsoCode { get; private set; }
        public ExternalReferenceId rstk__soapi_otype__r { get; private set; }
        public string rstk__soapi_orderdate__c { get; private set; }
        public string allocation_Sent_Date__c { get; private set; }
        public string ship_Date__c { get; private set; }
        public string expected_Delivery_Date__c { get; private set; }
        public string order_Received_Date__c { get; private set; }
        public string rstk__soapi_custpo__c { get; private set; }
        public ExternalReferenceId rstk__soapi_socust__r { get; private set; }
        public ExternalReferenceId rstk__soapi_ackaddr__r { get; private set; }
        public ExternalReferenceId rstk__soapi_shiptoaddr__r { get; private set; }
        public ExternalReferenceId rstk__soapi_instaddr__r { get; private set; }
        public ExternalReferenceId rstk__soapi_billtoaddr__r { get; private set; }
        public ExternalReferenceId rstk__soapi_carrier__r { get; private set; }
        public ExternalReferenceId rstk__soapi_shipvia__r { get; private set; }
        public bool? rstk__soapi_taxexempt__c { get; private set; }
        public string rstk__soapi_intcomment__c { get; private set; }
        public bool? rstk__soapi_async__c { get; private set; }
        public string rstk__soapi_upgroup__c { get; private set; }
        public string cc_Order__c { get; private set; }
        public ExternalReferenceId rstk__soapi_soprod__r { get; private set; }
        public double rstk__soapi_qtyorder__c { get; private set; }
        public double? rstk__soapi_price__c { get; private set; }
        public bool? rstk__soapi_firm__c { get; private set; }
        public double? amount_Covered_By_Insurance__c { get; private set; }
        public double? grams_Covered_By_Insurance__c { get; private set; }
        public string required_Lot_To_Pick__c { get; private set; }
        public ExternalReferenceId rstk__soapi_shipsite__r { get; private set; }
        public ExternalReferenceId rstk__soapi_shiplocid__r { get; private set; }
        public string rstk__soapi_shiplocnum__c { get; private set; }
        public string external_Order_Reference__c { get; private set; }

        public static RstkSalesOrder Create(SalesOrder salesOrder)
        {
            return new RstkSalesOrder
            {
                rstk__soapi_mode__c = "Add Both",
                rstk__soapi_addorupdate__c = false,
                rstk__soapi_custref__c = salesOrder.CustomerReference,
                rstk__soapi_salesdiv__c = salesOrder.Division,
                rstk__soapi_updatecustfields__c = true,
                currencyIsoCode = salesOrder.CurrencyIsoCode,
                rstk__soapi_otype__r = ExternalReferenceId.Create("rstk__sootype__c", salesOrder.Division),
                rstk__soapi_orderdate__c = salesOrder.OrderDate.ToString("yyyy-MM-dd"),
                ship_Date__c = salesOrder.ShipDate != DateTime.MinValue ? salesOrder.ShipDate.ToString("yyyy-MM-dd") : null,
                order_Received_Date__c = salesOrder.OrderReceivedDate != DateTime.MinValue ? salesOrder.OrderReceivedDate.ToString("yyyy-MM-dd") : null,
                rstk__soapi_custpo__c = salesOrder.CustomerPO,
                rstk__soapi_socust__r = ExternalReferenceId.Create("rstk__socust__c", salesOrder.Customer),
                rstk__soapi_ackaddr__r = salesOrder.CustomerAddresses.Acknowledgement?.AddressReference?.Reference != null ? ExternalReferenceId.Create("rstk__socaddr__c", salesOrder.CustomerAddresses.Acknowledgement.AddressReference.Reference) : null,
                rstk__soapi_shiptoaddr__r = salesOrder.CustomerAddresses.ShipTo?.AddressReference?.Reference != null ? ExternalReferenceId.Create("rstk__socaddr__c", salesOrder.CustomerAddresses.ShipTo.AddressReference.Reference) : null,
                rstk__soapi_instaddr__r = salesOrder.CustomerAddresses.Installation?.AddressReference?.Reference != null ? ExternalReferenceId.Create("rstk__socaddr__c", salesOrder.CustomerAddresses.Installation.AddressReference.Reference) : null,
                rstk__soapi_billtoaddr__r = salesOrder.CustomerAddresses.BillTo?.AddressReference?.Reference != null ? ExternalReferenceId.Create("rstk__socaddr__c", salesOrder.CustomerAddresses.BillTo.AddressReference.Reference) : null,
                rstk__soapi_carrier__r = salesOrder.ShippingCarrier != null ? ExternalReferenceId.Create("rstk__sycarrier__c", salesOrder.ShippingCarrier) : null,
                rstk__soapi_shipvia__r = salesOrder.ShippingMethod != null ? ExternalReferenceId.Create("rstk__syshipviatype__c", salesOrder.ShippingMethod) : null,
                rstk__soapi_taxexempt__c = salesOrder.TaxExempt,
                rstk__soapi_intcomment__c = salesOrder.Notes,
                rstk__soapi_async__c = false,
                cc_Order__c = salesOrder.CCOrder,
                rstk__soapi_soprod__r = ExternalReferenceId.Create("rstk__soprod__c", $"{salesOrder.Division}_{salesOrder.LineItems[0].ItemNumber}"),
                rstk__soapi_qtyorder__c = salesOrder.LineItems[0].Quantity,
                rstk__soapi_price__c = salesOrder.LineItems[0].UnitPrice,
                rstk__soapi_firm__c = salesOrder.LineItems[0].Firm,
                amount_Covered_By_Insurance__c = salesOrder.LineItems[0].AmountCoveredByInsurance,
                grams_Covered_By_Insurance__c = salesOrder.LineItems[0].GramsCoveredByInsurance,
                required_Lot_To_Pick__c = salesOrder.LineItems[0].RequiredLotToPick
            };
        }
    }

}
