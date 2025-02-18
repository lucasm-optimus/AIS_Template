using Newtonsoft.Json;
using Tilray.Integrations.Core.Domain.Aggregates.Sales.Commands;
using Tilray.Integrations.Core.Domain.Aggregates.SalesOrders.Events;

namespace Tilray.Integrations.Functions.UseCases.Ecom;

public class EcomSalesOrdersReceived_Webhook(IMediator mediator)
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
        var ecomSalesOrders = JsonConvert.DeserializeObject<List<Core.Domain.Aggregates.SalesOrders.Ecom.SalesOrder>>(requestBody);

        FluentResults.Result<SalesOrdersProcessed> response = await mediator.Send(new ProcessSalesOrdersCommand(ecomSalesOrders));

        if (response.IsSuccess)
        {
            await mediator.Publish(response.Value);

            var successfulSalesOrders = response.Value.SuccessfullOrders;
            var failedSalesOrders = response.Value.FailedOrders;
            return new OkObjectResult(new
            {
                Received = ecomSalesOrders.Count,
                Processed = successfulSalesOrders.Count,
                Failed = failedSalesOrders.Count,
                FailedSalesOrders = failedSalesOrders,
                Message = failedSalesOrders.Count > 0 ? "Check logs for failed sales orders." : null
            });
        }

        return new OkObjectResult("No Sales order found.");
    }

    #endregion
}
