namespace Tilray.Integrations.Functions.Rootstock.Models;

public class SalesOrderOutput
{
    [ServiceBusOutput("%TopicSalesOrderCreated%", Connection = "ServiceBusConnectionString")]
    public IEnumerable<SalesOrder> SalesOrder { get; set; }

    [HttpResult]
    public IActionResult Result { get; set; }
}
