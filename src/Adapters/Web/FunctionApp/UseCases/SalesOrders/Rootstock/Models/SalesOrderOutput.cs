using Tilray.Integrations.Core.Common.Stream;

namespace Tilray.Integrations.Functions.UseCases.SalesOrders.Rootstock.Models;

public class SalesOrderOutput
{
    [ServiceBusOutput(Topics.SalesOrderCreated, Connection = "ServiceBusConnectionString")]
    public IEnumerable<SalesOrder> SalesOrder { get; set; }

    [HttpResult]
    public IActionResult Result { get; set; }
}
