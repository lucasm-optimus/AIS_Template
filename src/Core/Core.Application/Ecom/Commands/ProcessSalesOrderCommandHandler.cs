using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Identity.Client;
using Tilray.Integrations.Core.Application.Rootstock.Services;
using Tilray.Integrations.Core.Domain;
using Tilray.Integrations.Core.Domain.Aggregates.Customer;
using Tilray.Integrations.Core.Domain.Aggregates.Customer.Commands;
using Tilray.Integrations.Core.Domain.Aggregates.Sales;
using Tilray.Integrations.Core.Domain.Aggregates.Sales.Commands;
using Tilray.Integrations.Core.Domain.Aggregates.Sales.Events;

namespace Tilray.Integrations.Core.Application.Ecom.Commands
{
    public class ProcessSalesOrderCommandHandler(
        IRootstockService rootstockService,
        IMediator mediator,
        ILogger<ProcessSalesOrderCommandHandler> logger,
        OrderDefaultsSettings orderDefaults) : ICommandHandler<ProcessSalesOrdersCommand, EcomSalesOrderProcessed>
    {
        public async Task<Result<EcomSalesOrderProcessed>> Handle(ProcessSalesOrdersCommand request, CancellationToken cancellationToken)
        {
            logger.LogInformation($"Begin processing {request.SalesOrder.Count()} sales orders.");

            var salesOrders = new List<SalesOrder>();
            var failedSalesOrders = new List<(EcomSalesOrder salesOrder, string message)>();

            var tasks = request.SalesOrder.Select(async salesOrder =>
            {
                //var checkIfOrderExists = await rootstockService.GetSalesOrder(salesOrder.ECommOrderID);

                var response = await ProcessIndividualSalesOrder(salesOrder);
                if (response.result && response.salesOrder != null)
                {
                    salesOrders.Add(response.salesOrder);
                }
                else
                {
                    failedSalesOrders.Add((salesOrder, response.message));
                }
            });

            await Task.WhenAll(tasks);

            if (failedSalesOrders.Any())
            {
                var failedSalesOrderMessages = failedSalesOrders.Select(f => $"Sales Order {f.salesOrder.ECommOrderID} failed to process: {f.message}");
                logger.LogError($"Failed to process sales orders: {failedSalesOrderMessages}");
            }

            return Result.Ok(new EcomSalesOrderProcessed(salesOrders));
        }

        private async Task<(bool result, string message, SalesOrder? salesOrder)> ProcessIndividualSalesOrder(EcomSalesOrder payload)
        {
            if (payload.StoreName != Constants.Ecom.SalesOrder.StoreName_AphriaMed && payload.StoreName != Constants.Ecom.SalesOrder.StoreName_SweetWater)
            {
                logger.LogInformation("Store name not recognized");
                return (false, "Store name not recognized", null);
            }

            if (!ConfirmShippingInfoPopulated(payload))
            {
                return (false, "Shipping Carrier or Shipping Method is blank", null);
            }

            if (payload.StoreName == Constants.Ecom.SalesOrder.StoreName_AphriaMed && !ConfirmOrderTotalMatchesPayments(payload, out double totalPayment, out double orderTotal))
            {
                return (false, $"Order total {orderTotal} does not match total payments {totalPayment}", null);
            }

            if (payload.StoreName == Constants.Ecom.SalesOrder.StoreName_AphriaMed && !ConfirmPatientType(payload))
            {
                return (false, "Patient Type is blank", null);
            }

            if (await rootstockService.GetCustomerInfo(payload.CustomerAccountID) == null)
            {
                await mediator.Send(new CreateCustomerCommand(payload));
            }

            var customerAddressInfo = await rootstockService.GetCustomerAddressInfo(payload.CustomerAccountNumber, payload.ShipToAddress1, payload.ShipToCity, payload.ShipToState, payload.ShipToZip);
            if (customerAddressInfo == null)
            {
                customerAddressInfo = (await mediator.Send(new CreateCustomerAddressCommand(payload))).Value.customerAddressInfo;
            }

            var salesOrder = SalesOrder.Create(payload, orderDefaults, customerAddressInfo.CustomerID);
            return (true, $"Sales order formatted for store {payload.StoreName}.", salesOrder);
        }

        private bool ConfirmShippingInfoPopulated(EcomSalesOrder sapSalesOrder)
        {
            return sapSalesOrder.ShippingCarrier != null && sapSalesOrder.ShippingMethod != null;
        }

        private bool ConfirmOrderTotalMatchesPayments(EcomSalesOrder sapSalesOrder, out double TotalPayment, out double OrderTotal)
        {
            var totalPayments = sapSalesOrder.AmountPaidByCustomer + sapSalesOrder.AmountPaidByBillTo;
            var orderTotal = sapSalesOrder.ShippingCost - sapSalesOrder.DiscountAmount + sapSalesOrder.Taxes.Sum(t => t.Amount) + sapSalesOrder.OrderLines.Sum(ol => ol.Quantity * ol.UnitPrice);
            TotalPayment = totalPayments;
            OrderTotal = orderTotal;
            return totalPayments == orderTotal;
        }

        private bool ConfirmPatientType(EcomSalesOrder sapSalesOrder)
        {
            var validPatientTypes = new List<string> { "Insured", "Non-Insured", "Veteran" };
            return validPatientTypes.Contains(sapSalesOrder.PatientType);
        }
    }
}
