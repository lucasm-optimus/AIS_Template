using Azure.Messaging.ServiceBus;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.WebJobs;
using Newtonsoft.Json;
using System.Collections.Generic;
using Tilray.Integrations.Core.Domain.Aggregates.Sales;
using Tilray.Integrations.Core.Domain.Aggregates.Sales.Commands;
using Tilray.Integrations.Core.Domain.Aggregates.SalesOrders.Events;
using static Tilray.Integrations.Functions.Constants;

namespace Tilray.Integrations.Functions.UseCases.Ecom;

public class EcomSalesOrdersReceived_Webhook(IMediator mediator, ServiceBusClient serviceBusClient)
{
    #region Function Implementation

    /// <summary>
    /// Function responsible for validating the sales order
    /// </summary>
    /// <param name="req">Http Request</param>
    /// <param name="log"></param>
    [Function(nameof(EcomSalesOrdersReceived_Webhook))]
    public async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req)
    {
        var requestBody = await new StreamReader(req.Body).ReadToEndAsync();
        var ecomSalesOrders = JsonConvert.DeserializeObject<List<Core.Models.Ecom.SalesOrder>>(requestBody);

        FluentResults.Result<SalesOrdersProcessed> response = await mediator.Send(new ProcessSalesOrdersCommand(ecomSalesOrders));

        if (response.IsSuccess)
        {
            var salesOrders = response.Value.salesOrder;
            var sender = serviceBusClient.CreateSender(ServiceBus.Topics.EcomSalesOrderReceived);

            foreach (var salesOrder in salesOrders)
            {
                var message = new ServiceBusMessage(JsonConvert.SerializeObject(salesOrder));
                await sender.SendMessageAsync(message);
            }

            var responseMessage = $"Sales orders processed:{salesOrders.Count}, failed:{ecomSalesOrders.Count - salesOrders.Count}. {((ecomSalesOrders.Count - salesOrders.Count) > 0 ? "Check logs for failed sales orders." : "")}";
            return new OkObjectResult(responseMessage);
        }

        return new OkObjectResult("No Sales order found.");
    }

    #endregion
}
