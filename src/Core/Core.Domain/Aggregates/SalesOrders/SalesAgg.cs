using Microsoft.Extensions.Logging;
using Tilray.Integrations.Core.Domain.Aggregates.Sales.Customer;
using Tilray.Integrations.Core.Domain.Aggregates.Sales.Events;
using Tilray.Integrations.Core.Domain.Aggregates.Sales.Rootstock;
using Tilray.Integrations.Core.Domain.Aggregates.SalesOrders;
using Tilray.Integrations.Core.Domain.Aggregates.SalesOrders.Events;
using Tilray.Integrations.Core.Models.Ecom;
using static Tilray.Integrations.Core.Domain.Aggregates.SalesOrders.Constants.Ecom;

namespace Tilray.Integrations.Core.Domain.Aggregates.Sales
{
    public class SalesAgg : AggRoot
    {
        #region Agg Entities

        public MedSalesOrder SalesOrder { get; private set; }
        public SalesOrderCustomer SalesOrderCustomer { get; private set; }
        public SalesOrderCustomerAddress SalesOrderCustomerAddress { get; private set; }
        public SalesOrderPrepayment SalesOrderPrepayment { get; private set; }

        public RstkCustomer RootstockCustomer { get; private set; }
        public RstkCustomerAddress RootstockCustomerAddress { get; private set; }
        public RstkSalesOrder RootstockSalesOrder { get; private set; }
        public List<RstkSalesOrderLineItem> RootstockOrderLines { get; set; }
        public RstkSalesOrderPrePayment PrePayment { get; private set; }

        #endregion

        #region Constructors

        private SalesAgg() { }

        public static Result<SalesAgg> Create(Models.Ecom.SalesOrder payload, OrderDefaultsSettings orderDefaults)
        {
            var validationResult = ValidateSalesOrder(payload);

            if (validationResult.IsFailed)
            {
                return Result.Fail<SalesAgg>(validationResult.Errors);
            }

            if (string.IsNullOrWhiteSpace(payload.StoreName))
            {
                return Result.Fail<SalesAgg>("Store name is required");
            }

            var salesAgg = new SalesAgg();
            try
            {
                if (payload.StoreName == Constants.Ecom.SalesOrder.StoreName_AphriaMed)
                {
                    salesAgg.SalesOrder = CreateForAphria(payload, orderDefaults);
                }
                else if (payload.StoreName == Constants.Ecom.SalesOrder.StoreName_SweetWater)
                {
                    salesAgg.SalesOrder = CreateForSweetWater(payload, orderDefaults);
                }

                salesAgg.SalesOrderCustomer = SalesOrderCustomer.Create(payload, orderDefaults);
                salesAgg.SalesOrderCustomerAddress = SalesOrderCustomerAddress.Create(payload);

                return Result.Ok(salesAgg);
            }
            catch (Exception e)
            {
                return Result.Fail<SalesAgg>(e.Message);
            }
        }

        public static Result<SalesAgg> Create(MedSalesOrder salesOrder)
        {
            var salesAgg = new SalesAgg
            {
                SalesOrder = salesOrder,
                RootstockOrderLines = new List<RstkSalesOrderLineItem>()
            };

            var rsoResult = RstkSalesOrder.Create(salesOrder);
            if (rsoResult.IsFailed)
            {
                return Result.Fail<SalesAgg>(rsoResult.Errors);
            }
            salesAgg.RootstockSalesOrder = rsoResult.Value;

            for (var i = 1; i < salesOrder.LineItems.Count; i++)
            {
                var lineResult = RstkSalesOrderLineItem.Create(salesOrder: salesOrder, lineItem: salesOrder.LineItems[i]);
                if (lineResult.IsFailed)
                {
                    return Result.Fail<SalesAgg>(lineResult.Errors);
                }
                salesAgg.RootstockOrderLines.Add(lineResult.Value);
            }

            //if (salesOrder.CCPrepayment != null)
            //{
            //    var rsoPrePaymentResult = RstkSalesOrderPrePayment.Create(salesOrder);
            //    if (rsoPrePaymentResult.IsFailed)
            //    {
            //        return Result.Fail<SalesAgg>(rsoPrePaymentResult.Errors);
            //    }
            //    salesAgg.PrePayment = rsoPrePaymentResult.Value;
            //}

            return Result.Ok(salesAgg);
        }

        #endregion

        #region Private Methods

        private static Result<SalesOrderValidated> ValidateSalesOrder(Models.Ecom.SalesOrder payload)
        {
            var result = Result.Ok();

            if (payload.StoreName != Constants.Ecom.SalesOrder.StoreName_AphriaMed && payload.StoreName != Constants.Ecom.SalesOrder.StoreName_SweetWater)
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

        private static bool ConfirmShippingInfoPopulated(Models.Ecom.SalesOrder salesOrder)
        {
            return salesOrder.ShippingCarrier != null && salesOrder.ShippingMethod != null;
        }

        private static bool ConfirmOrderTotalMatchesPayments(Models.Ecom.SalesOrder salesOrder, out double TotalPayment, out double OrderTotal)
        {
            var totalPayments = salesOrder.AmountPaidByCustomer + salesOrder.AmountPaidByBillTo;
            var orderTotal = (salesOrder.ShippingCost - salesOrder.DiscountAmount)
                             + (salesOrder.Taxes?.Sum(t => t.Amount) ?? 0)
                             + (salesOrder.OrderLines?.Sum(ol => ol.Quantity * ol.UnitPrice) ?? 0);
            TotalPayment = totalPayments;
            OrderTotal = orderTotal;
            return totalPayments == orderTotal;
        }

        private static bool ConfirmPatientType(Models.Ecom.SalesOrder salesOrder)
        {
            var validPatientTypes = new List<string> { "Insured", "Non-Insured", "Veteran" };
            return validPatientTypes.Contains(salesOrder.PatientType);
        }

        private static MedSalesOrder CreateForSweetWater(Models.Ecom.SalesOrder payload, OrderDefaultsSettings orderDefaults)
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

        private static MedSalesOrder CreateForAphria(Models.Ecom.SalesOrder payload, OrderDefaultsSettings orderDefaults)
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

            if (payload.AmountPaidByCustomer != null && payload.AmountPaidByCustomer != 0)
            {
                salesOrder.CCPrepayment = CCPrepayment.Create(payload.AmountPaidByCustomer, payload.PrepaymentTransactionID, orderDefaults.Medical.PaymentGateway);
            }
            if (payload.AmountPaidByBillTo != null & payload.AmountPaidByBillTo != 0)
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

                if (payload.ShippingCost != 0)
                {
                    lineItem = SalesOrderLineItem.Create(
                        itemNumber: item.Product,
                        quantity: 1,
                        unitPrice: item.UnitPrice,
                        requiredLotToPick: item.Lot ?? string.Empty,
                        amountCoveredByInsurance: payload.ShippingCoveredByInsurance ? payload.ShippingCost : null,
                        gramsCoveredByInsurance: null,
                        firm: true,
                        location: item.FulFillLoc,
                        id: item.Id
                    );
                    salesOrder.LineItems.Add(lineItem);
                }
                if (payload.DiscountAmount != 0)
                {
                    lineItem = SalesOrderLineItem.Create(
                        itemNumber: payload.PatientType == "Veteran" ? orderDefaults.Medical.Items.DiscountVeteran : orderDefaults.Medical.Items.DiscountCivilian,
                        quantity: 1,
                        unitPrice: -payload.DiscountAmount,
                        requiredLotToPick: item.Lot ?? string.Empty,
                        amountCoveredByInsurance: payload.AmountPaidByBillTo != null && payload.AmountPaidByCustomer == 0 ? -payload.DiscountAmount : null,
                        gramsCoveredByInsurance: null,
                        firm: true,
                        location: null,
                        id: null
                    );
                    salesOrder.LineItems.Add(lineItem);
                }
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
