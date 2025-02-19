namespace Tilray.Integrations.Core.Domain.Aggregates.SalesOrders.Ecom
{
    public class SalesOrder
    {
        #region Properties

        [JsonProperty("storeName")]
        public string StoreName { get; set; }

        [JsonProperty("eCommOrderNo")]
        public string ECommOrderNo { get; set; }

        [JsonProperty("eCommOrderID")]
        public string ECommOrderID { get; set; }

        [JsonProperty("notes")]
        public string Notes { get; set; }

        [JsonProperty("customerAccountID")]
        public string CustomerAccountID { get; set; }

        [JsonProperty("customerAccountNumber")]
        public string CustomerAccountNumber { get; set; }

        [JsonProperty("patientType")]
        public string PatientType { get; set; }

        [JsonProperty("shipToID")]
        public string ShipToID { get; set; }

        [JsonProperty("shipToAddress1")]
        public string ShipToAddress1 { get; set; }

        [JsonProperty("shipToAddress2")]
        public string ShipToAddress2 { get; set; }

        [JsonProperty("shipToCity")]
        public string ShipToCity { get; set; }

        [JsonProperty("shipToState")]
        public string ShipToState { get; set; }

        [JsonProperty("shipToZip")]
        public string ShipToZip { get; set; }

        [JsonProperty("shipToCountry")]
        public string ShipToCountry { get; set; }

        [JsonProperty("shipToEmail")]
        public string ShipToEmail { get; set; }

        [JsonProperty("billToAccountID")]
        public string BillToAccountID { get; set; }

        [JsonProperty("orderedOn")]
        public DateTime OrderedOn { get; set; }

        [JsonProperty("shippingCarrier")]
        public string ShippingCarrier { get; set; }

        [JsonProperty("shippingMethod")]
        public string ShippingMethod { get; set; }

        [JsonProperty("amountPaidByCustomer")]
        public double AmountPaidByCustomer { get; set; }

        [JsonProperty("amountPaidByBillTo")]
        public double AmountPaidByBillTo { get; set; }

        [JsonProperty("prepaymentTransactionID")]
        public string PrepaymentTransactionID { get; set; }

        [JsonProperty("shippingCost")]
        public double ShippingCost { get; set; }

        [JsonProperty("discountAmount")]
        public double DiscountAmount { get; set; }

        [JsonProperty("taxTransactionID")]
        public string TaxTransactionID { get; set; }

        [JsonProperty("taxes")]
        public List<SalesOrderTax> Taxes { get; set; }

        [JsonProperty("orderLines")]
        public List<SalesOrderLine> OrderLines { get; set; }

        [JsonProperty("shippingCoveredByInsurance")]
        public bool ShippingCoveredByInsurance { get; set; }

        [JsonProperty("shippingWeek")]
        public string ShippingWeek { get; set; }

        [JsonProperty("customerPO")]
        public string CustomerPO { get; set; }

        #endregion
    }
}