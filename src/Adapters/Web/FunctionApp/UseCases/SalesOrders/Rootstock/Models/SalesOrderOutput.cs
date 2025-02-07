namespace Tilray.Integrations.Functions.UseCases.SalesOrders.Rootstock.Models;

public class SalesOrderOutput
{
    [ServiceBusOutput("%TopicSalesOrderCreated%", Connection = "ServiceBusConnectionString")]
    public IEnumerable<SalesOrder> SalesOrder { get; set; }

    [HttpResult]
    public IActionResult Result { get; set; }
}
