using Newtonsoft.Json;
using Tilray.Integrations.Core.Domain.Aggregates.Sales.Rootstock;
using Tilray.Integrations.Core.Domain.Aggregates.SalesOrders;

namespace Tilray.Integrations.Core.Domain.Aggregates.Sales
{
    public class MedSalesOrder : Entity
    {
        #region Constructors

        public MedSalesOrder() { LineItems = new(); }

        #endregion

        #region Properties

        [JsonProperty("division")]
        public string Division { get; set; }

        [JsonProperty("customerReference")]
        public string CustomerReference { get; set; }

        [JsonProperty("eCommerceOrderID")]
        public string ECommerceOrderID { get; set; }

        [JsonProperty("customer")]
        public string Customer { get; set; }

        [JsonProperty("updateOrderIfExists")]
        public bool UpdateOrderIfExists { get; set; }

        [JsonProperty("orderDate")]
        public DateTime OrderDate { get; set; }

        [JsonProperty("orderReceivedDate")]
        public DateTime OrderReceivedDate { get; set; }

        [JsonProperty("orderType")]
        public string OrderType { get; set; }

        [JsonProperty("shippingCarrier")]
        public string ShippingCarrier { get; set; }

        [JsonProperty("shippingMethod")]
        public string ShippingMethod { get; set; }

        [JsonProperty("shipDate")]
        public DateTime ShipDate { get; set; }

        [JsonProperty("taxExempt")]
        public bool TaxExempt { get; set; }

        [JsonProperty("notes")]
        public string Notes { get; set; }

        [JsonProperty("customerPO")]
        public string CustomerPO { get; set; }

        [JsonProperty("currencyIsoCode")]
        public string CurrencyIsoCode { get; set; }

        [JsonProperty("ccOrder")]
        public string CCOrder { get; set; }

        [JsonProperty("shipTo")]
        public string ShipTo { get; set; }

        [JsonProperty("cardCode")]
        public string CardCode { get; set; }

        [JsonProperty("ccPrepayment")]
        public CCPrepayment CCPrepayment { get; set; }

        [JsonProperty("standardPrepayment")]
        public StandardPrepayment StandardPrepayment { get; set; }

        [JsonProperty("customerAddresses")]
        public CustomerAddresses CustomerAddresses { get; set; }

        [JsonProperty("lineItems")]
        public List<SalesOrderLineItem> LineItems { get; set; }

        [JsonProperty("storeName")]
        public string StoreName { get; set; }
        public bool? BackgroundProcessing { get; set; }
        public string CustomerAddressId { get; set; }
        [JsonProperty("customerAddressReference")]
        public string CustomerAddressReference { get; set; }
        [JsonProperty("customerId ")]
        public string CustomerId { get; set; }

        #endregion

        #region Public Methods

        public void UpdateCustomerAddressReference(string customerAddressReference)
        {
            CustomerAddressReference = customerAddressReference;
        }

        public Result<SalesOrderPrepayment> HasPrepayment(string createdSalesOrderId, string prePaymentAccount)
        {
            if (StandardPrepayment != null)
            {
                return SalesOrderPrepayment.Create(StandardPrepayment, this.Customer, this.Division, createdSalesOrderId, prePaymentAccount);
            }

            return Result.Fail<SalesOrderPrepayment>("No prepayment found");
        }

        public void UpdateOrderId(string orderId)
        {
            OrderType = orderId;
        }

        public void UpdateShipViaId(string value)
        {
            ShippingMethod = value;
        }

        public void UpdateCarrierId(string value)
        {
            ShippingCarrier = value;
        }

        public void UpdateCustomerAddressId(string value)
        {
            CustomerAddressId = value;
        }

        public void UpdateCustomerId(string customerId)
        {
            CustomerId = customerId;
        }

        #endregion
    }
}