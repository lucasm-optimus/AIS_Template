using Tilray.Integrations.Core.Domain.Aggregates.Sales.Customer;
using Tilray.Integrations.Core.Domain.Aggregates.Sales.Events;
using Tilray.Integrations.Core.Domain.Aggregates.Sales.Rootstock;
using Tilray.Integrations.Core.Domain.Aggregates.SalesOrders;
using Tilray.Integrations.Core.Domain.Aggregates.SalesOrders.Events;
using Tilray.Integrations.Core.Models.Ecom;

namespace Tilray.Integrations.Core.Domain.Aggregates.Sales
{
    public class SalesAgg : AggRoot
    {
        #region Agg Entities

        public string StoreName { get; }
        public MedSalesOrder SalesOrder { get; private set; }
        public SalesOrderCustomer SalesOrderCustomer { get; private set; }
        public SalesOrderCustomerAddress SalesOrderCustomerAddress { get; private set; }

        #endregion

        #region Constructors

        private SalesAgg(string storeName)
        {
            StoreName = storeName;
        }

        public static Result<SalesAgg> Create(string storeName)
        {
            if (string.IsNullOrWhiteSpace(storeName))
            {
                return Result.Fail<SalesAgg>("Store name is required");
            }

            return new SalesAgg(storeName);
        }

        public static Result<SalesAgg> Create(MedSalesOrder salesOrder)
        {
            var salesAgg = new SalesAgg(salesOrder.StoreName);
            salesAgg.SalesOrder = salesOrder;
            return salesAgg;
        }

        #endregion

        #region Agg Methods 

        public Result<SalesOrderProcessed> Process(Models.Ecom.SalesOrder payload, OrderDefaultsSettings orderDefaults)
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

                SalesOrderCustomer = SalesOrderCustomer.Create(payload, orderDefaults);
                SalesOrderCustomerAddress = SalesOrderCustomerAddress.Create(payload);

                return Result.Ok(new SalesOrderProcessed(SalesOrder, SalesOrderCustomer, SalesOrderCustomerAddress));

            }
            catch (Exception e)
            {
                return Result.Fail<SalesOrderProcessed>(e.Message);
            }
        }

        public static Result<SalesOrderValidated> ValidateSalesOrder(Models.Ecom.SalesOrder payload)
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

        public Result<RstkSalesOrder> ProcessRootstockHeader()
        {
            try
            {
                var rstkSalesOrder = RstkSalesOrder.Create();

                rstkSalesOrder.SetRstk__soapi_mode__c("Add Both");
                rstkSalesOrder.SetRstk__soapi_addorupdate__c(false);
                rstkSalesOrder.SetRstk__soapi_custref__c(SalesOrder.CustomerReference);
                rstkSalesOrder.SetRstk__soapi_salesdiv__c(SalesOrder.Division);
                rstkSalesOrder.SetRstk__soapi_updatecustfields__c(true);
                if (SalesOrder.CurrencyIsoCode != null) rstkSalesOrder.SetCurrencyIsoCode(SalesOrder.CurrencyIsoCode);
                rstkSalesOrder.SetRstk__soapi_otype__r(ExternalReferenceId.Create("rstk__sootype__c", SalesOrder.Division));
                rstkSalesOrder.SetRstk__soapi_orderdate__c(SalesOrder.OrderDate.ToString("yyyy-MM-dd"));
                //if (SalesOrder.AllocationSentDate != DateTime.MinValue) rstkSalesOrder.SetAllocation_Sent_Date__c(SalesOrder.AllocationSentDate.ToString("yyyy-MM-dd"));
                if (SalesOrder.ShipDate != DateTime.MinValue) rstkSalesOrder.SetShip_Date__c(SalesOrder.ShipDate.ToString("yyyy-MM-dd"));
                //if(SalesOrder.ExpectedDeliveryDate != DateTime.MinValue) rstkSalesOrder.SetExpected_Delivery_Date__c(SalesOrder.ExpectedDeliveryDate.ToString("yyyy-MM-dd"));
                if (SalesOrder.OrderReceivedDate != DateTime.MinValue)
                    rstkSalesOrder.SetOrder_Received_Date__c(SalesOrder.OrderReceivedDate.ToString("yyyy-MM-dd"));
                if (SalesOrder.CustomerPO != null)
                    rstkSalesOrder.SetRstk__soapi_custpo__c(SalesOrder.CustomerPO);
                rstkSalesOrder.SetRstk__soapi_socust__r(ExternalReferenceId.Create("rstk__socust__c", SalesOrder.Customer));
                if (SalesOrder.CustomerAddresses.Acknowledgement?.AddressReference?.Reference != null)
                    rstkSalesOrder.SetRstk__soapi_ackaddr__r(ExternalReferenceId.Create("rstk__socaddr__c", SalesOrder.CustomerAddresses.Acknowledgement.AddressReference.Reference));
                if (SalesOrder.CustomerAddresses.ShipTo?.AddressReference?.Reference != null)
                    rstkSalesOrder.SetRstk__soapi_shiptoaddr__r(ExternalReferenceId.Create("rstk__socaddr__c", SalesOrder.CustomerAddresses.ShipTo.AddressReference.Reference));
                if (SalesOrder.ShipTo != null)
                    rstkSalesOrder.SetRstk__soapi_shiptoaddr__r(ExternalReferenceId.Create("rstk__socaddr__c", SalesOrder.ShipTo));
                if (SalesOrder.CustomerAddresses.Installation?.AddressReference?.Reference != null)
                    rstkSalesOrder.SetRstk__soapi_instaddr__r(ExternalReferenceId.Create("rstk__socaddr__c", SalesOrder.CustomerAddresses.Installation.AddressReference.Reference));
                if (SalesOrder.CustomerAddresses.BillTo?.AddressReference?.Reference != null)
                    rstkSalesOrder.SetRstk__soapi_billtoaddr__r(ExternalReferenceId.Create("rstk__socaddr__c", SalesOrder.CustomerAddresses.BillTo.AddressReference.Reference));
                if (SalesOrder.ShippingCarrier != null)
                    rstkSalesOrder.SetRstk__soapi_carrier__r(ExternalReferenceId.Create("rstk__sycarrier__c", SalesOrder.ShippingCarrier));
                if (SalesOrder.ShippingMethod != null)
                    rstkSalesOrder.SetRstk__soapi_shipvia__r(ExternalReferenceId.Create("rstk__syshipviatype__c", SalesOrder.ShippingMethod));
                if (SalesOrder.TaxExempt == true) rstkSalesOrder.SetRstk__soapi_taxexempt__c(SalesOrder.TaxExempt);
                if (SalesOrder.Notes != null) rstkSalesOrder.SetRstk__soapi_intcomment__c(SalesOrder.Notes);
                rstkSalesOrder.SetRstk__soapi_async__c(false);
                //if (SalesOrder.UploadGroup != null) rstkSalesOrder.SetRstk__soapi_upgroup__c(SalesOrder.UploadGroup);
                if (SalesOrder.CCOrder != null) rstkSalesOrder.SetCC_Order__c(SalesOrder.CCOrder);
                rstkSalesOrder.SetRstk__soapi_soprod__r(ExternalReferenceId.Create("rstk__soprod__c", $"{SalesOrder.Division}_{SalesOrder.LineItems[0].ItemNumber}"));
                rstkSalesOrder.SetRstk__soapi_qtyorder__c(SalesOrder.LineItems[0].Quantity);
                if (SalesOrder.LineItems[0].UnitPrice != null) rstkSalesOrder.SetRstk__soapi_price__c(SalesOrder.LineItems[0].UnitPrice);
                if (SalesOrder.LineItems[0].Firm != null) rstkSalesOrder.SetRstk__soapi_firm__c(SalesOrder.LineItems[0].Firm);
                if (SalesOrder.LineItems[0].AmountCoveredByInsurance != null) rstkSalesOrder.SetAmount_Covered_By_Insurance__c(SalesOrder.LineItems[0].AmountCoveredByInsurance);
                if (SalesOrder.LineItems[0].GramsCoveredByInsurance != null) rstkSalesOrder.SetGrams_Covered_By_Insurance__c(SalesOrder.LineItems[0].GramsCoveredByInsurance);
                if (SalesOrder.LineItems[0].RequiredLotToPick != null) rstkSalesOrder.SetRequired_Lot_To_Pick__c(SalesOrder.LineItems[0].RequiredLotToPick);
                //if (SalesOrder.LineItems[0].DefaultShipFromDivision != null) rstkSalesOrder.SetRstk__soapi_shipsite__r(GenericExternalIdReference.Create("rstk__sysite__c",$"{SalesOrder.Division}_{SalesOrder.LineItems[0].DefaultShipFromDivision}"));
                //if (SalesOrder.LineItems[0].DefaultShipFromLocationNo != null) rstkSalesOrder.SetRstk__soapi_shiplocid__r(GenericExternalIdReference.Create("rstk__sysite__c",$"{SalesOrder.Division}_{SalesOrder.LineItems[0].DefaultShipFromLocationNo}"));
                //if(SalesOrder.ExternalRefNumber) rstkSalesOrder.SetExternal_Order_Reference__c(SalesOrder.ExternalRefNumber);

                return Result.Ok(rstkSalesOrder);
            }
            catch (Exception e)
            {
                return Result.Fail<RstkSalesOrder>(e.Message);
            }
        }

        public Result<RstkSalesOrderLineItem> ProcessRootstockLineItem(int arrayIndex, dynamic CreatedSalesOrder)
        {
            try
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

                return Result.Ok(rstkSalesOrderLineItem);

            }
            catch (Exception e)
            {
                return Result.Fail<RstkSalesOrderLineItem>(e.Message);
            }
        }

        public Result<RstkPrePayment> ProcessCCPrePayment()
        {
            try
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

                return Result.Ok(rstkPrePayment);

            }
            catch (Exception e)
            {
                return Result.Fail<RstkPrePayment>(e.Message);
            }
        }

        #endregion

        #region Private Methods

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
