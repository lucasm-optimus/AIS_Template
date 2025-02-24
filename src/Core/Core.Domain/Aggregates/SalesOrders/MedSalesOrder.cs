using Tilray.Integrations.Core.Domain.Aggregates.SalesOrders.Ecom;
using Tilray.Integrations.Core.Domain.Aggregates.SalesOrders.Rootstock;

namespace Tilray.Integrations.Core.Domain.Aggregates.SalesOrders
{
    public class MedSalesOrder : Entity
    {

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
        //public CCPrepayment SyDataPrePayment { get; private set; }

        #endregion

        #region Constructors

        private MedSalesOrder() { LineItems = new(); }

        public static Result<MedSalesOrder> Create(Ecom.SalesOrder payload, OrderDefaultsSettings orderDefaults)
        {
            var validationResult = Validate(payload);

            if (validationResult.IsFailed)
            {
                return Result.Fail<MedSalesOrder>(validationResult.Errors);
            }

            var salesOrder = new MedSalesOrder();
            try
            {
                if (payload.StoreName == SalesOrderConstants.StoreName_AphriaMed)
                {
                    salesOrder = CreateForAphria(payload, orderDefaults);
                }
                else if (payload.StoreName == SalesOrderConstants.StoreName_SweetWater)
                {
                    salesOrder = CreateForSweetWater(payload, orderDefaults);
                }

                return Result.Ok(salesOrder);
            }
            catch (Exception e)
            {
                return Result.Fail<MedSalesOrder>(e.Message);
            }
        }

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
                return SalesOrderPrepayment.Create(StandardPrepayment.AmountPaid, CustomerId, Division, createdSalesOrderId, prePaymentAccount, CustomerAddressId);
            }
            if (CCPrepayment != null)
            {
                return SalesOrderPrepayment.Create(CCPrepayment.AmountPrepaidByCC, CustomerId, Division, createdSalesOrderId, prePaymentAccount, CustomerAddressId);
            }

            return Result.Fail<SalesOrderPrepayment>("No prepayment found");
        }

        public Result<CCPrepayment> HasPrePaymentData()
        {
            if (CCPrepayment != null)
            {
                return Result.Ok(CCPrepayment);
            }

            return Result.Fail<CCPrepayment>("No SyDataPrePayment found");
        }

        public void UpdateOrderId(string orderId)
        {
            OrderType = orderId;
        }

        public void UpdateShipViaId(string shipViaId)
        {
            ShippingMethod = shipViaId;
        }

        public void UpdateCarrierId(string carrierId)
        {
            ShippingCarrier = carrierId;
        }

        public void UpdateCustomerAddressId(string customerAddressId)
        {
            CustomerAddressId = customerAddressId;
        }

        public void UpdateCustomerId(string customerId)
        {
            CustomerId = customerId;
        }

        #endregion

        #region Private Methods

        private static Result<SalesOrderValidated> Validate(Ecom.SalesOrder payload)
        {
            var result = Result.Ok();

            if (payload.StoreName != SalesOrderConstants.StoreName_AphriaMed && payload.StoreName != SalesOrderConstants.StoreName_SweetWater)
            {
                result.WithError("Store name not recognized");
            }
            if (!ConfirmShippingInfoPopulated(payload))
            {
                result.WithError("Shipping Carrier or Shipping Method is blank");
            }
            if (!ConfirmOrderTotalMatchesPayments(payload, out var totalPayment, out var orderTotal))
            {
                result.WithError("Order total does not match payments");
            }
            if (!ConfirmPatientType(payload))
            {
                result.WithError("Patient type is not valid");
            }

            if (result.IsFailed)
            {
                return Result.Fail<SalesOrderValidated>(result.Errors);
            }

            return new SalesOrderValidated(true, null);
        }

        private static bool ConfirmShippingInfoPopulated(Ecom.SalesOrder salesOrder)
        {
            return salesOrder.ShippingCarrier != null && salesOrder.ShippingMethod != null;
        }

        private static bool ConfirmOrderTotalMatchesPayments(Ecom.SalesOrder salesOrder, out double TotalPayment, out double OrderTotal)
        {
            var totalPayments = salesOrder.AmountPaidByCustomer + salesOrder.AmountPaidByBillTo;
            var orderTotal = salesOrder.ShippingCost - salesOrder.DiscountAmount
                             + (salesOrder.Taxes?.Sum(t => t.Amount) ?? 0)
                             + (salesOrder.OrderLines?.Sum(ol => ol.Quantity * ol.UnitPrice) ?? 0);
            TotalPayment = totalPayments;
            OrderTotal = orderTotal;
            return totalPayments == orderTotal;
        }

        private static bool ConfirmPatientType(Ecom.SalesOrder salesOrder)
        {
            var validPatientTypes = new List<string> { "Insured", "Non-Insured", "Veteran" };
            return validPatientTypes.Contains(salesOrder.PatientType);
        }

        private static MedSalesOrder CreateForSweetWater(Ecom.SalesOrder payload, OrderDefaultsSettings orderDefaults)
        {
            var salesOrder = new MedSalesOrder
            {
                StoreName = payload.StoreName,
                Division = orderDefaults.SweetWater.Division,
                CustomerReference = payload.ECommOrderNo + orderDefaults.SweetWater.OrderReferenceSuffix,
                ECommerceOrderID = payload.ECommOrderID,
                Customer = payload.CustomerAccountNumber,
                UpdateOrderIfExists = false,
                OrderDate = payload.OrderedOn,
                OrderType = orderDefaults.Medical.OrderType,
                ShippingCarrier = payload.ShippingCarrier,
                ShippingMethod = payload.ShippingMethod,
                Notes = payload.Notes,
                ShipDate = string.IsNullOrWhiteSpace(payload.ShippingWeek) ? DateTime.UtcNow.Date : Convert.ToDateTime(payload.ShippingWeek),
                TaxExempt = true,
                CustomerPO = payload.CustomerPO != null ? payload.CustomerPO : null,
                CurrencyIsoCode = "USD",
                CCOrder = payload.ECommOrderID,
                //ShipTo = customerAddressReference,
                LineItems = new List<SalesOrderLineItem>()
            };

            foreach (var item in payload.OrderLines)
            {
                var lineItem = SalesOrderLineItem.Create(
                    itemNumber: string.IsNullOrWhiteSpace(item.ObeerSku) ? item.ObeerSku : item.Product,
                    quantity: item.Quantity,
                    unitPrice: item.UnitPrice,
                    requiredLotToPick: item.Lot ?? string.Empty,
                    amountCoveredByInsurance: null,
                    gramsCoveredByInsurance: null,
                    firm: false,
                    location: item.FulFillLoc,
                    id: item.Id
                );
                salesOrder.LineItems.Add(lineItem);
            }

            if (payload.Taxes.Any())
            {
                foreach (var tax in payload.Taxes)
                {
                    var lineItem = SalesOrderLineItem.Create(
                        itemNumber: GetTaxCode(orderDefaults, payload.ShipToState, tax),
                        quantity: 1,
                        unitPrice: tax.Amount,
                        requiredLotToPick: null,
                        amountCoveredByInsurance: tax.CoveredByInsurance ? tax.Amount : null,
                        gramsCoveredByInsurance: null,
                        firm: true,
                        location: null,
                        id: null
                    );
                    salesOrder.LineItems.Add(lineItem);
                }
            }

            return salesOrder;
        }

        private static MedSalesOrder CreateForAphria(Ecom.SalesOrder payload, OrderDefaultsSettings orderDefaults)
        {
            var salesOrder = new MedSalesOrder
            {
                StoreName = payload.StoreName,
                Division = orderDefaults.Medical.Division,
                CustomerReference = payload.ECommOrderNo + orderDefaults.Medical.OrderReferenceSuffix,
                ECommerceOrderID = payload.ECommOrderID,
                Customer = payload.CustomerAccountNumber,
                UpdateOrderIfExists = false,
                OrderDate = payload.OrderedOn,
                OrderType = orderDefaults.Medical.OrderType,
                ShippingCarrier = payload.ShippingCarrier == "Canada Post" ? "CANPOST" : payload.ShippingCarrier,
                ShippingMethod = payload.ShippingMethod == "Expedited Parcel" ? "ExpeditedParcel" : payload.ShippingMethod,
                Notes = payload.Notes,
                CCOrder = payload.ECommOrderID,
                LineItems = new List<SalesOrderLineItem>()
            };

            if (payload.AmountPaidByCustomer != 0)
            {
                salesOrder.CCPrepayment = CCPrepayment.Create(payload.AmountPaidByCustomer, payload.PrepaymentTransactionID, orderDefaults.Medical.PaymentGateway);
            }
            if (payload.AmountPaidByBillTo != 0)
            {
                salesOrder.StandardPrepayment = StandardPrepayment.Create(payload.AmountPaidByBillTo, payload.CustomerAccountNumber);
            }

            foreach (var item in payload.OrderLines)
            {
                var lineItem = SalesOrderLineItem.Create(
                    itemNumber: item.Product,
                    quantity: item.Quantity,
                    unitPrice: item.UnitPrice,
                    requiredLotToPick: item.Lot ?? string.Empty,
                    amountCoveredByInsurance: payload.ShippingCoveredByInsurance ? payload.ShippingCost : null,
                    gramsCoveredByInsurance: item.GramsCoveredByInsurance > 0 ? item.GramsCoveredByInsurance : null,
                    firm: true,
                    location: item.FulFillLoc,
                    id: item.Id
                );
                salesOrder.LineItems.Add(lineItem);
            }


            if (payload.ShippingCost != 0)
            {
                var lineItem = SalesOrderLineItem.Create(
                    itemNumber: orderDefaults.Medical.Items.Shipping,
                    quantity: 1,
                    unitPrice: payload.ShippingCost,
                    requiredLotToPick: string.Empty,
                    amountCoveredByInsurance: payload.ShippingCoveredByInsurance ? payload.ShippingCost : null,
                    gramsCoveredByInsurance: null,
                    firm: true,
                    location: null,
                    id: null
                );
                salesOrder.LineItems.Add(lineItem);
            }

            if (payload.DiscountAmount != 0)
            {
                var lineItem = SalesOrderLineItem.Create(
                    itemNumber: payload.PatientType == "Veteran" ? orderDefaults.Medical.Items.DiscountVeteran : orderDefaults.Medical.Items.DiscountCivilian,
                    quantity: 1,
                    unitPrice: -payload.DiscountAmount,
                    requiredLotToPick: string.Empty,
                    amountCoveredByInsurance: payload.AmountPaidByBillTo != null && payload.AmountPaidByCustomer == 0 ? -payload.DiscountAmount : null,
                    gramsCoveredByInsurance: null,
                    firm: true,
                    location: null,
                    id: null
                );
                salesOrder.LineItems.Add(lineItem);
            }

            if (payload.Taxes.Any())
            {
                foreach (var tax in payload.Taxes)
                {
                    var lineItem = SalesOrderLineItem.Create(
                        itemNumber: GetTaxCode(orderDefaults, payload.ShipToState, tax),
                        quantity: 1,
                        unitPrice: tax.Amount,
                        requiredLotToPick: null,
                        amountCoveredByInsurance: tax.CoveredByInsurance ? tax.Amount : null,
                        gramsCoveredByInsurance: null,
                        firm: true,
                        location: null,
                        id: null
                    );
                    salesOrder.LineItems.Add(lineItem);
                }
            }

            return salesOrder;
        }

        private static string? GetTaxCode(OrderDefaultsSettings orderDefaults, string shipToState, SalesOrderTax tax)
        {
            List<string> provinces = ["BC", "MB", "SK"];
            if (tax.TaxType != "PST")
            {
                return typeof(MedicalTaxCodesDefaults).GetProperties().FirstOrDefault(p => p.Name.Contains(tax.TaxType)).GetValue(orderDefaults.Medical.TaxCodes).ToString();
            }
            else if (tax.TaxType == "PST" && provinces.Contains(shipToState))
            {
                return $"PST{shipToState}" switch
                {
                    "PSTBC" => orderDefaults.Medical.TaxCodes.PSTBC,
                    "PSTMB" => orderDefaults.Medical.TaxCodes.PSTMB,
                    "PSTSK" => orderDefaults.Medical.TaxCodes.PSTSK,
                    _ => null,
                };
            }
            else
            {
                return null;
            }
        }

        #endregion
    }
}