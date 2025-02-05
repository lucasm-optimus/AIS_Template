using MediatR;
using Newtonsoft.Json;
using Tilray.Integrations.Core.Domain.Aggregates.Sales;
using Tilray.Integrations.Core.Domain.Aggregates.Sales.Events;

public class EcomSalesOrder
{
    [JsonProperty("storeName")]
    public string StoreName { get; internal set; }

    [JsonProperty("eCommOrderNo")]
    public string ECommOrderNo { get; internal set; }

    [JsonProperty("eCommOrderID")]
    public string ECommOrderID { get; internal set; }

    [JsonProperty("notes")]
    public string Notes { get; internal set; }

    [JsonProperty("customerAccountID")]
    public string CustomerAccountID { get; internal set; }

    [JsonProperty("customerAccountNumber")]
    public string CustomerAccountNumber { get; internal set; }

    [JsonProperty("patientType")]
    public string PatientType { get; internal set; }

    [JsonProperty("shipToID")]
    public string ShipToID { get; internal set; }

    [JsonProperty("shipToAddress1")]
    public string ShipToAddress1 { get; internal set; }

    [JsonProperty("shipToAddress2")]
    public string ShipToAddress2 { get; internal set; }

    [JsonProperty("shipToCity")]
    public string ShipToCity { get; internal set; }

    [JsonProperty("shipToState")]
    public string ShipToState { get; internal set; }

    [JsonProperty("shipToZip")]
    public string ShipToZip { get; internal set; }

    [JsonProperty("shipToCountry")]
    public string ShipToCountry { get; internal set; }

    [JsonProperty("shipToEmail")]
    public string ShipToEmail { get; internal set; }

    [JsonProperty("billToAccountID")]
    public string BillToAccountID { get; internal set; }

    [JsonProperty("orderedOn")]
    public DateTime OrderedOn { get; internal set; }

    [JsonProperty("shippingCarrier")]
    public string ShippingCarrier { get; internal set; }

    [JsonProperty("shippingMethod")]
    public string ShippingMethod { get; internal set; }

    [JsonProperty("amountPaidByCustomer")]
    public double AmountPaidByCustomer { get; internal set; }

    [JsonProperty("amountPaidByBillTo")]
    public double AmountPaidByBillTo { get; internal set; }

    [JsonProperty("prepaymentTransactionID")]
    public string PrepaymentTransactionID { get; internal set; }

    [JsonProperty("shippingCost")]
    public double ShippingCost { get; internal set; }

    [JsonProperty("discountAmount")]
    public double DiscountAmount { get; internal set; }

    [JsonProperty("taxTransactionID")]
    public string TaxTransactionID { get; internal set; }

    [JsonProperty("taxes")]
    public List<Tax> Taxes { get; internal set; }

    [JsonProperty("orderLines")]
    public List<OrderLine> OrderLines { get; internal set; }

    [JsonProperty("shippingCoveredByInsurance")]
    public bool ShippingCoveredByInsurance { get; set; }

    [JsonProperty("shippingWeek")]
    public string ShippingWeek { get; set; }

    [JsonProperty("customerPO")]
    public string CustomerPO { get; set; }
}

public class Tax
{
    [JsonProperty("taxType")]
    public string TaxType { get; internal set; }

    [JsonProperty("amount")]
    public double Amount { get; internal set; }

    [JsonProperty("coveredByInsurance")]
    public bool CoveredByInsurance { get; internal set; }
}

public class OrderLine
{
    [JsonProperty("product")]
    public string Product { get; internal set; }

    [JsonProperty("quantity")]
    public double Quantity { get; internal set; }

    [JsonProperty("unitPrice")]
    public double UnitPrice { get; internal set; }

    [JsonProperty("lot")]
    public string Lot { get; internal set; }

    [JsonProperty("coveredByInsurance")]
    public double CoveredByInsurance { get; internal set; }

    [JsonProperty("gramsCoveredByInsurance")]
    public double GramsCoveredByInsurance { get; internal set; }

    [JsonProperty("obeerSku")]
    public string ObeerSku { get; set; }

    [JsonProperty("fulFillLoc")]
    public string FulFillLoc { get; set; }

    [JsonProperty("id")]
    public string Id { get; set; }
}
