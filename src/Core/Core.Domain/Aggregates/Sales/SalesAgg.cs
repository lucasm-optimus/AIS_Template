using FluentResults;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.Azure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tilray.Integrations.Core.Domain.Aggregates.Sales.Customer;
using Tilray.Integrations.Core.Domain.Aggregates.Sales.Events;
using Tilray.Integrations.Core.Domain.Aggregates.Sales.Rootstock;
using Tilray.Integrations.Core.Models.Ecom;

namespace Tilray.Integrations.Core.Domain.Aggregates.Sales
{
    public class SalesAgg : AggRoot
    {
        #region Agg Entities

        public string StoreName { get; }
        public SalesOrder SalesOrder { get; private set; }
        public SalesOrderCustomer SalesOrderCustomer { get; private set; }
        public SalesOrderCustomerAddress SalesOrderCustomerAddress { get; private set; }

        #endregion

        #region Constructors

        private SalesAgg(string storeName)
        {
            StoreName = storeName;
        }

        public static SalesAgg Create(string storeName)
        {
            return new SalesAgg(storeName);
        }

        public static SalesAgg Create(SalesOrder salesOrder)
        {
            var salesAgg = new SalesAgg(salesOrder.StoreName);
            salesAgg.SalesOrder = salesOrder;
            return salesAgg;
        }

        #endregion

        #region Agg Methods 

        public Result<SalesOrderProcessed> ProcessOrder(Models.Ecom.SalesOrder payload, OrderDefaultsSettings orderDefaults)
        {
            try
            {
                if (payload.StoreName == Constants.Ecom.SalesOrder.StoreName_AphriaMed)
                {
                    SalesOrder = CreateForAphria(payload, orderDefaults);
                }
                else if (payload.StoreName == Constants.Ecom.SalesOrder.StoreName_SweetWater)
                {
                    SalesOrder = CreateForSweetWater(payload, orderDefaults);
                }

                SalesOrder.StoreName = payload.StoreName;
                SalesOrderCustomer = SalesOrderCustomer.Create(payload, orderDefaults);
                SalesOrderCustomerAddress = SalesOrderCustomerAddress.Create(payload);

                return Result.Ok(new SalesOrderProcessed(SalesOrder, SalesOrderCustomer, SalesOrderCustomerAddress));
            }
            catch (Exception ex)
            {
                return Result.Fail<SalesOrderProcessed>($"Failed to process sales order. Exception Message:{ex.Message}");
            }
        }

        public Result<SalesOrderValidated> ValidateSalesOrder(Models.Ecom.SalesOrder salesOrder)
        {
            var result = Result.Ok();

            if (salesOrder.StoreName != Constants.Ecom.SalesOrder.StoreName_AphriaMed && salesOrder.StoreName != Constants.Ecom.SalesOrder.StoreName_SweetWater)
            {
                result.WithError("Store name not recognized");
            }
            if (!ConfirmShippingInfoPopulated(salesOrder))
            {
                result.WithError("Shipping Carrier or Shipping Method is blank");
            }
            if (!ConfirmOrderTotalMatchesPayments(salesOrder, out var totalPayment, out var orderTotal))
            {
                result.WithError("Order total does not match payments");
            }
            if (!ConfirmPatientType(salesOrder))
            {
                result.WithError("Patient type is not valid");
            }

            if (result.IsFailed)
            {
                return result;
            }

            return Result.Ok(new SalesOrderValidated(result.IsSuccess, result.Reasons.Select(r => r.Message)));
        }

        public void UpdateCustomerAddressReference(string customerAddressReference)
        {
            if (StoreName == Constants.Ecom.SalesOrder.StoreName_AphriaMed)
            {
                SalesOrder.CustomerAddresses = CustomerAddresses.Create(
                    Address.Create(AddressReference.Create(customerAddressReference)),
                    Address.Create(AddressReference.Create(customerAddressReference)),
                    Address.Create(AddressReference.Create(customerAddressReference)),
                    Address.Create(AddressReference.Create(customerAddressReference)));
            }
            if (StoreName == Constants.Ecom.SalesOrder.StoreName_SweetWater)
            {
                SalesOrder.ShipTo = customerAddressReference;
            }
        }

      

        public RstkSalesOrderLineItem ProcessRootstockLineItem(int arrayIndex, dynamic CreatedSalesOrder)
        {
            var rstkSalesOrderLineItem = RstkSalesOrderLineItem.Create();
            rstkSalesOrderLineItem.SetRstk__soapi_mode__c("Add Line");
            rstkSalesOrderLineItem.SetRstk__soapi_sohdr__c(CreatedSalesOrder[0]["rstk__soapi_sohdr__c"]);
            rstkSalesOrderLineItem.SetRstk__soapi_soprod__r(ExternalReferenceId.Create("rstk__soprod__c", $"{SalesOrder.Division}_{SalesOrder.LineItems[arrayIndex].ItemNumber}"));
            rstkSalesOrderLineItem.SetRstk__soapi_qtyorder__c(SalesOrder.LineItems[arrayIndex].Quantity);
            //if(salesOrder.BackgroundProcessing!=null) rstkSalesOrderLineItem.SetRstk__soapi_async__c(SalesOrder.LineItems[arrayIndex].BackgroundProcessing);
            //if(salesOrder.UploadGroup) rstkSalesOrderLineItem.SetRstk__soapi_upgroup__c(SalesOrder.LineItems[arrayIndex].UploadGroup);
            if (SalesOrder.LineItems[arrayIndex].UnitPrice != null) rstkSalesOrderLineItem.SetRstk__soapi_price__c(SalesOrder.LineItems[arrayIndex].UnitPrice);
            if (SalesOrder.LineItems[arrayIndex].Firm != null) rstkSalesOrderLineItem.SetRstk__soapi_firm__c(SalesOrder.LineItems[arrayIndex].Firm);
            //if (SalesOrder.LineItems[arrayIndex].TaxExempt == true) rstkSalesOrderLineItem.SetRstk__soapi_taxexempt__c(SalesOrder.LineItems[arrayIndex].TaxExempt);
            if (SalesOrder.LineItems[arrayIndex].AmountCoveredByInsurance != null) rstkSalesOrderLineItem.SetAmount_Covered_By_Insurance__c(SalesOrder.LineItems[arrayIndex].AmountCoveredByInsurance);
            if (SalesOrder.LineItems[arrayIndex].GramsCoveredByInsurance != null) rstkSalesOrderLineItem.SetGrams_Covered_By_Insurance__c(SalesOrder.LineItems[arrayIndex].GramsCoveredByInsurance);
            if (SalesOrder.LineItems[arrayIndex].RequiredLotToPick != null) rstkSalesOrderLineItem.SetRequired_Lot_To_Pick__c(SalesOrder.LineItems[arrayIndex].RequiredLotToPick);
            //if (SalesOrder.LineItems[arrayIndex].DefaultShipFromDivision != null) rstkSalesOrderLineItem.SetRstk__soapi_shipsite__r(ExternalReferenceId.Create("rstk__sysite__c", $"{salesOrder.Division}_{SalesOrder.LineItems[arrayIndex].DefaultShipFromDivision}"));
            //if (paSalesOrder.LineItems[arrayIndex]load.DefaultShipFromLocationNo != null) rstkSalesOrderLineItem.SetRstk__soapi_shiplocid__r(ExternalReferenceId.Create("rstk__sylocid__c", $"{salesOrder.Division}_{SalesOrder.LineItems[arrayIndex].DefaultShipFromLocationNo}"));
            //if (SalesOrder.LineItems[arrayIndex].DefaultShipFromLocationNo != null) rstkSalesOrderLineItem.SetRstk__soapi_shiplocnum__c(SalesOrder.LineItems[arrayIndex].DefaultShipFromLocationNo);
            //if (SalesOrder.LineItems[arrayIndex].CurrencyIsoCode != null) rstkSalesOrderLineItem.SetCurrencyIsoCode(SalesOrder.LineItems[arrayIndex].CurrencyIsoCode);
            rstkSalesOrderLineItem.SetRstk__soapi_updatecustfields__c(true);

            return rstkSalesOrderLineItem;
        }

        public RstkPrePayment ProcessCCPrePayment()
        {
            var rstkPrePayment = RstkPrePayment.Create();

            rstkPrePayment.SetRstk__soppy_div__r(ExternalReferenceId.Create("rstk__sydiv__c", SalesOrder.Division));
            rstkPrePayment.SetRstk__soppy_type__c("Sales Order Payment Authorization");
            rstkPrePayment.SetRstk__soppy_order__r(ExternalReferenceId.Create("rstk__sohdr__c", SalesOrder.ECommerceOrderID));
            rstkPrePayment.SetRstk__soppy_custno__r(ExternalReferenceId.Create("rstk__socust__c", SalesOrder.Customer));
            rstkPrePayment.SetRstk__soppy_addrseq__r(ExternalReferenceId.Create("rstk__socaddr__c", SalesOrder.CustomerAddresses.BillTo.AddressReference.Reference));
            rstkPrePayment.SetRstk__soppy_amount__c(SalesOrder.CCPrepayment.AmountPrepaidByCC);
            rstkPrePayment.SetRstk__soppy_appmethod__c(SalesOrder.CCPrepayment.CCPaymentGateway);
            rstkPrePayment.SetRstk__soppy_sohdrcust__r(ExternalReferenceId.Create("rstk__socust__c", SalesOrder.Customer));
            rstkPrePayment.SetRstk__soppy_ppyacct__r(ExternalReferenceId.Create("rstk__syacc__c", $"{SalesOrder.Division}_{SalesOrder.CCPrepayment.PrepaidCCTransactionID}"));
            rstkPrePayment.SetRstk__soppy_cctxn__c(true);

            return rstkPrePayment;
        }

        #endregion

        #region Private Methods

        private bool ConfirmShippingInfoPopulated(Models.Ecom.SalesOrder salesOrder)
        {
            return salesOrder.ShippingCarrier != null && salesOrder.ShippingMethod != null;
        }

        private bool ConfirmOrderTotalMatchesPayments(Models.Ecom.SalesOrder salesOrder, out double TotalPayment, out double OrderTotal)
        {
            var totalPayments = salesOrder.AmountPaidByCustomer + salesOrder.AmountPaidByBillTo;
            var orderTotal = salesOrder.ShippingCost - salesOrder.DiscountAmount + salesOrder.Taxes.Sum(t => t.Amount) + salesOrder.OrderLines.Sum(ol => ol.Quantity * ol.UnitPrice);
            TotalPayment = totalPayments;
            OrderTotal = orderTotal;
            return totalPayments == orderTotal;
        }

        private bool ConfirmPatientType(Models.Ecom.SalesOrder salesOrder)
        {
            var validPatientTypes = new List<string> { "Insured", "Non-Insured", "Veteran" };
            return validPatientTypes.Contains(salesOrder.PatientType);
        }

        private static SalesOrder CreateForSweetWater(Models.Ecom.SalesOrder payload, OrderDefaultsSettings orderDefaults)
        {
            var salesOrder = new SalesOrder
            {
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

        private static SalesOrder CreateForAphria(Models.Ecom.SalesOrder payload, OrderDefaultsSettings orderDefaults)
        {
            var salesOrder = new SalesOrder
            {
                Division = orderDefaults.Medical.Division,
                CustomerReference = payload.ECommOrderNo + orderDefaults.Medical.OrderReferenceSuffix,
                ECommerceOrderID = payload.ECommOrderID,
                Customer = payload.CustomerAccountID,
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
