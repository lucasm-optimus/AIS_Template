using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tilray.Integrations.Core.Application.Rootstock.Services;
using Tilray.Integrations.Core.Domain.Aggregates.Sales;
using Tilray.Integrations.Core.Domain.Aggregates.Sales.Commands;
using Tilray.Integrations.Core.Domain.Aggregates.Sales.Events;

namespace Tilray.Integrations.Core.Application.Rootstock.Commands
{
    public class CreateSalesOrderCommandHandler(IRootstockService rootstockService, ILogger<CreateSalesOrderCommandHandler> logger) : ICommandHandler<CreateSalesOrderCommand, SalesOrderCreated>
    {
        public async Task<Result<SalesOrderCreated>> Handle(CreateSalesOrderCommand request, CancellationToken cancellationToken)
        {
            var salesOrderExists = await rootstockService.SalesOrderExists(request.SalesOrder.ECommerceOrderID);

            if (salesOrderExists)
            {
                logger.LogInformation($"[{request.corelationId}] Sales order already exists for ECommerceOrderID:{request.SalesOrder.ECommerceOrderID}.");
                // check the return value
                return Result.Ok(new SalesOrderCreated());
            }

            logger.LogInformation($"[{request.corelationId}] Creating rootstock sales order started for ECommerceOrderID:{request.SalesOrder.ECommerceOrderID}.");

            //SalesOrders-ProcessHeader
            var rotstockSalesOrder = ProcessRootstockHeader(request.SalesOrder);
            logger.LogInformation($"[{request.corelationId}] Processed rootstock sales order header for ECommerceOrderID:{request.SalesOrder.ECommerceOrderID}.");

            var CreatedSalesOrder = await rootstockService.CreateSalesOrder(rotstockSalesOrder);
            logger.LogInformation($"[{request.corelationId}] Created rootstock sales order header for ECommerceOrderID:{request.SalesOrder.ECommerceOrderID}.");

            if (CreatedSalesOrder != null)
            {
                //SalesOrders-ProcessLineItems
                foreach (var lineItem in request.SalesOrder.LineItems.Skip(1))
                {
                    var rstkSalesOrderLineItem = ProcessRootstockLineItem(lineItem, request.SalesOrder, CreatedSalesOrder);
                    logger.LogInformation($"[{request.corelationId}] Processed rootstock sales order line item: {lineItem.ItemNumber} for ECommerceOrderID:{request.SalesOrder.ECommerceOrderID}.");

                    await rootstockService.CreateSalesOrderLineItem(rstkSalesOrderLineItem);
                    logger.LogInformation($"[{request.corelationId}] Created rootstock sales order line item: {lineItem.ItemNumber} for ECommerceOrderID:{request.SalesOrder.ECommerceOrderID}.");
                }

                //Prepayment - Process
                if (request.SalesOrder.CCPrepayment != null)
                {
                    var rstkPrePayment = ProcessCCPrePayment(request.SalesOrder);

                    await rootstockService.CreatePrePayment(rstkPrePayment);
                }

                //Standard Prepayment If insurance payment is not made
                if (request.SalesOrder.StandardPrepayment != null)
                {
                    var rstkPrePayment = ProcessCCPrePayment(request.SalesOrder);
                    await rootstockService.CreatePrePayment(rstkPrePayment);
                }
            }

            return Result.Ok(new SalesOrderCreated());
        }

        private RstkSalesOrder ProcessRootstockHeader(SalesOrder payload)
        {
            var rstkSalesOrder = RstkSalesOrder.Create();

            rstkSalesOrder.SetRstk__soapi_mode__c("Add Both");
            rstkSalesOrder.SetRstk__soapi_addorupdate__c(false);
            rstkSalesOrder.SetRstk__soapi_custref__c(payload.CustomerReference);
            rstkSalesOrder.SetRstk__soapi_salesdiv__c(payload.Division);
            rstkSalesOrder.SetRstk__soapi_updatecustfields__c(true);
            if (payload.CurrencyIsoCode != null) rstkSalesOrder.SetCurrencyIsoCode(payload.CurrencyIsoCode);
            rstkSalesOrder.SetRstk__soapi_otype__r(ExternalReferenceId.Create("rstk__sootype__c", payload.Division));
            rstkSalesOrder.SetRstk__soapi_orderdate__c(payload.OrderDate.ToString("yyyy-MM-dd"));
            //if (salesOrder.AllocationSentDate != DateTime.MinValue) rstkSalesOrder.SetAllocation_Sent_Date__c(salesOrder.AllocationSentDate.ToString("yyyy-MM-dd"));
            if (payload.ShipDate != DateTime.MinValue) rstkSalesOrder.SetShip_Date__c(payload.ShipDate.ToString("yyyy-MM-dd"));
            //if(salesOrder.ExpectedDeliveryDate != DateTime.MinValue) rstkSalesOrder.SetExpected_Delivery_Date__c(salesOrder.ExpectedDeliveryDate.ToString("yyyy-MM-dd"));
            if (payload.OrderReceivedDate != DateTime.MinValue)
                rstkSalesOrder.SetOrder_Received_Date__c(payload.OrderReceivedDate.ToString("yyyy-MM-dd"));
            if (payload.CustomerPO != null)
                rstkSalesOrder.SetRstk__soapi_custpo__c(payload.CustomerPO);
            rstkSalesOrder.SetRstk__soapi_socust__r(ExternalReferenceId.Create("rstk__socust__c", payload.Customer));
            if (payload.CustomerAddresses.Acknowledgement?.AddressReference?.Reference != null)
                rstkSalesOrder.SetRstk__soapi_ackaddr__r(ExternalReferenceId.Create("rstk__socaddr__c", payload.CustomerAddresses.Acknowledgement.AddressReference.Reference));
            if (payload.CustomerAddresses.ShipTo?.AddressReference?.Reference != null)
                rstkSalesOrder.SetRstk__soapi_shiptoaddr__r(ExternalReferenceId.Create("rstk__socaddr__c", payload.CustomerAddresses.ShipTo.AddressReference.Reference));
            if (payload.ShipTo != null)
                rstkSalesOrder.SetRstk__soapi_shiptoaddr__r(ExternalReferenceId.Create("rstk__socaddr__c", payload.ShipTo));
            if (payload.CustomerAddresses.Installation?.AddressReference?.Reference != null)
                rstkSalesOrder.SetRstk__soapi_instaddr__r(ExternalReferenceId.Create("rstk__socaddr__c", payload.CustomerAddresses.Installation.AddressReference.Reference));
            if (payload.CustomerAddresses.BillTo?.AddressReference?.Reference != null)
                rstkSalesOrder.SetRstk__soapi_billtoaddr__r(ExternalReferenceId.Create("rstk__socaddr__c", payload.CustomerAddresses.BillTo.AddressReference.Reference));
            if (payload.ShippingCarrier != null)
                rstkSalesOrder.SetRstk__soapi_carrier__r(ExternalReferenceId.Create("rstk__sycarrier__c", payload.ShippingCarrier));
            if (payload.ShippingMethod != null)
                rstkSalesOrder.SetRstk__soapi_shipvia__r(ExternalReferenceId.Create("rstk__syshipviatype__c", payload.ShippingMethod));
            if (payload.TaxExempt == true) rstkSalesOrder.SetRstk__soapi_taxexempt__c(payload.TaxExempt);
            if (payload.Notes != null) rstkSalesOrder.SetRstk__soapi_intcomment__c(payload.Notes);
            rstkSalesOrder.SetRstk__soapi_async__c(false);
            //if (salesOrder.UploadGroup != null) rstkSalesOrder.SetRstk__soapi_upgroup__c(salesOrder.UploadGroup);
            if (payload.CCOrder != null) rstkSalesOrder.SetCC_Order__c(payload.CCOrder);
            rstkSalesOrder.SetRstk__soapi_soprod__r(ExternalReferenceId.Create("rstk__soprod__c", $"{payload.Division}_{payload.LineItems[0].ItemNumber}"));
            rstkSalesOrder.SetRstk__soapi_qtyorder__c(payload.LineItems[0].Quantity);
            if (payload.LineItems[0].UnitPrice != null) rstkSalesOrder.SetRstk__soapi_price__c(payload.LineItems[0].UnitPrice);
            if (payload.LineItems[0].Firm != null) rstkSalesOrder.SetRstk__soapi_firm__c(payload.LineItems[0].Firm);
            if (payload.LineItems[0].AmountCoveredByInsurance != null) rstkSalesOrder.SetAmount_Covered_By_Insurance__c(payload.LineItems[0].AmountCoveredByInsurance);
            if (payload.LineItems[0].GramsCoveredByInsurance != null) rstkSalesOrder.SetGrams_Covered_By_Insurance__c(payload.LineItems[0].GramsCoveredByInsurance);
            if (payload.LineItems[0].RequiredLotToPick != null) rstkSalesOrder.SetRequired_Lot_To_Pick__c(payload.LineItems[0].RequiredLotToPick);
            //if (salesOrder.LineItems[0].DefaultShipFromDivision != null) rstkSalesOrder.SetRstk__soapi_shipsite__r(GenericExternalIdReference.Create("rstk__sysite__c",$"{salesOrder.Division}_{salesOrder.LineItems[0].DefaultShipFromDivision}"));
            //if (salesOrder.LineItems[0].DefaultShipFromLocationNo != null) rstkSalesOrder.SetRstk__soapi_shiplocid__r(GenericExternalIdReference.Create("rstk__sysite__c",$"{salesOrder.Division}_{salesOrder.LineItems[0].DefaultShipFromLocationNo}"));
            //if(salesOrder.ExternalRefNumber) rstkSalesOrder.SetExternal_Order_Reference__c(salesOrder.ExternalRefNumber);

            return rstkSalesOrder;
        }

        private RstkSalesOrderLineItem ProcessRootstockLineItem(LineItem payload, SalesOrder salesOrder, dynamic CreatedSalesOrder)
        {
            var rstkSalesOrderLineItem = RstkSalesOrderLineItem.Create();
            rstkSalesOrderLineItem.SetRstk__soapi_mode__c("Add Line");
            rstkSalesOrderLineItem.SetRstk__soapi_sohdr__c(CreatedSalesOrder[0]["rstk__soapi_sohdr__c"]);
            rstkSalesOrderLineItem.SetRstk__soapi_soprod__r(ExternalReferenceId.Create("rstk__soprod__c", $"{salesOrder.Division}_{payload.ItemNumber}"));
            rstkSalesOrderLineItem.SetRstk__soapi_qtyorder__c(payload.Quantity);
            //if(salesOrder.BackgroundProcessing!=null) rstkSalesOrderLineItem.SetRstk__soapi_async__c(payload.BackgroundProcessing);
            //if(salesOrder.UploadGroup) rstkSalesOrderLineItem.SetRstk__soapi_upgroup__c(payload.UploadGroup);
            if (payload.UnitPrice != null) rstkSalesOrderLineItem.SetRstk__soapi_price__c(payload.UnitPrice);
            if (payload.Firm != null) rstkSalesOrderLineItem.SetRstk__soapi_firm__c(payload.Firm);
            //if (payload.TaxExempt == true) rstkSalesOrderLineItem.SetRstk__soapi_taxexempt__c(payload.TaxExempt);
            if (payload.AmountCoveredByInsurance != null) rstkSalesOrderLineItem.SetAmount_Covered_By_Insurance__c(payload.AmountCoveredByInsurance);
            if (payload.GramsCoveredByInsurance != null) rstkSalesOrderLineItem.SetGrams_Covered_By_Insurance__c(payload.GramsCoveredByInsurance);
            if (payload.RequiredLotToPick != null) rstkSalesOrderLineItem.SetRequired_Lot_To_Pick__c(payload.RequiredLotToPick);
            //if (payload.DefaultShipFromDivision != null) rstkSalesOrderLineItem.SetRstk__soapi_shipsite__r(ExternalReferenceId.Create("rstk__sysite__c", $"{salesOrder.Division}_{payload.DefaultShipFromDivision}"));
            //if (payload.DefaultShipFromLocationNo != null) rstkSalesOrderLineItem.SetRstk__soapi_shiplocid__r(ExternalReferenceId.Create("rstk__sylocid__c", $"{salesOrder.Division}_{payload.DefaultShipFromLocationNo}"));
            //if (payload.DefaultShipFromLocationNo != null) rstkSalesOrderLineItem.SetRstk__soapi_shiplocnum__c(payload.DefaultShipFromLocationNo);
            //if (payload.CurrencyIsoCode != null) rstkSalesOrderLineItem.SetCurrencyIsoCode(payload.CurrencyIsoCode);
            rstkSalesOrderLineItem.SetRstk__soapi_updatecustfields__c(true);

            return rstkSalesOrderLineItem;
        }

        private RstkPrePayment ProcessCCPrePayment(SalesOrder payload)
        {
            var rstkPrePayment = RstkPrePayment.Create();

            rstkPrePayment.SetRstk__soppy_div__r(ExternalReferenceId.Create("rstk__sydiv__c", payload.Division));
            rstkPrePayment.SetRstk__soppy_type__c("Sales Order Payment Authorization");
            rstkPrePayment.SetRstk__soppy_order__r(ExternalReferenceId.Create("rstk__sohdr__c", payload.ECommerceOrderID));
            rstkPrePayment.SetRstk__soppy_custno__r(ExternalReferenceId.Create("rstk__socust__c", payload.Customer));
            rstkPrePayment.SetRstk__soppy_addrseq__r(ExternalReferenceId.Create("rstk__socaddr__c", payload.CustomerAddresses.BillTo.AddressReference.Reference));
            rstkPrePayment.SetRstk__soppy_amount__c(payload.CCPrepayment.AmountPrepaidByCC);
            rstkPrePayment.SetRstk__soppy_appmethod__c(payload.CCPrepayment.CCPaymentGateway);
            rstkPrePayment.SetRstk__soppy_sohdrcust__r(ExternalReferenceId.Create("rstk__socust__c", payload.Customer));
            rstkPrePayment.SetRstk__soppy_ppyacct__r(ExternalReferenceId.Create("rstk__syacc__c", $"{payload.Division}_{payload.CCPrepayment.PrepaidCCTransactionID}"));
            rstkPrePayment.SetRstk__soppy_cctxn__c(true);

            return rstkPrePayment;
        }
    }
}
