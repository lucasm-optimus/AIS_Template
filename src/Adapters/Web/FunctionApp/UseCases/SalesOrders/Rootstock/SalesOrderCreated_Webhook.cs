using Tilray.Integrations.Functions.UseCases.SalesOrders.Rootstock.Models;

namespace Tilray.Integrations.Functions.UseCases.SalesOrders.Rootstock;

public class SalesOrderCreated_Webhook(IMediator mediator, ILogger<SalesOrderCreated_Webhook> logger)
{
    /// <summary>
    /// This function is responsible for validating the sales order.
    /// </summary>
    [Function("SalesOrderCreated_Webhook")]
    public async Task<SalesOrderOutput> Run([HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequest req)
    {
        var requestBody = await new StreamReader(req.Body).ReadToEndAsync();
        var salesOrders = requestBody.ToObject<IEnumerable<SalesOrder>>();
        var result = await mediator.Send(new ValidateSalesOrderQuery(salesOrders));

        return result.IsFailed
            ? new SalesOrderOutput { SalesOrder = null, Result = new BadRequestObjectResult(string.Join(", ", result.Errors)) }
            : new SalesOrderOutput { SalesOrder = result.Value, Result = new OkObjectResult("Sales Order(s) are being processed") };
    }
}
