using Tilray.Integrations.Core.Domain.Aggregates.SalesOrders.Events;

namespace Tilray.Integrations.Functions.UseCases.SalesOrders.Rootstock;

public class EcomSalesOrdersReceived_Webhook(IMediator mediator)
{
    #region Function Implementation
    /// <summary>
    /// Azure Function to process Sales Orders received from Medical store front
    /// </summary>
    /// <param name="req"></param>
    /// <returns></returns>
    [Function(nameof(EcomSalesOrdersReceived_Webhook))]
    public async Task<Result<SalesOrdersProcessed>> Run(
        [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req)
    {
        var requestBody = await new StreamReader(req.Body).ReadToEndAsync();
        var ecomSalesOrders = JsonConvert.DeserializeObject<List<Core.Domain.Aggregates.SalesOrders.Ecom.SalesOrder>>(requestBody);
        return await mediator.Send(new ProcessSalesOrdersCommand(ecomSalesOrders));
    }

    #endregion
}
