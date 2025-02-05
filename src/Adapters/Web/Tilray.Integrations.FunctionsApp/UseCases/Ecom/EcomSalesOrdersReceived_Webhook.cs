using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.WebJobs;
using Newtonsoft.Json;
using Tilray.Integrations.Core.Domain.Aggregates.Sales;
using Tilray.Integrations.Core.Domain.Aggregates.Sales.Commands;

namespace Tilray.Integrations.FunctionsApp.UseCases.Ecom;

public class EcomSalesOrdersReceived_Webhook(IMediator mediator)
{
    #region Function Implementation

    /// <summary>
    /// Function responsible for validating the sales order
    /// </summary>
    /// <param name="req">Http Request</param>
    /// <param name="log"></param>
    [FunctionName("EcomSalesOrdersReceived_Webhook")]
    //[ServiceBusOutput("%TopicSAPConcurInvoicesFetched%", Connection = "ServiceBusConnectionString")]
    public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req)
    {
        string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
        var sapSalesOrders = JsonConvert.DeserializeObject<List<EcomSalesOrder>>(requestBody);

        if (sapSalesOrders.Any())
        {
            List<Task> tasks = new List<Task>();
            foreach (var sapSalesOrder in sapSalesOrders)
            {
                tasks.Add(mediator.Send(new ProcessEcomSalesOrderCommand(sapSalesOrder)));
            }
            await Task.WhenAll(tasks);
            return new OkObjectResult("Sales order processed successfully.");
        }
        return new OkObjectResult("No Sales order found.");
    }

    #endregion
}
