namespace Tilray.Integrations.Core.Domain.Aggregates.SalesOrders.Events;

public record SalesOrdersProcessed(List<MedSalesOrder> SuccessfullOrders, List<string> FailedOrders) : IDomainEvent
{
    public object ResponseResult = new
    {
        Received = SuccessfullOrders.Count + FailedOrders.Count,
        Processing = SuccessfullOrders.Count,
        Failed = FailedOrders.Count,
        FailedSalesOrders = FailedOrders,
        Message = FailedOrders.Count > 0 ? "Check logs for failed sales orders." : null
    };
}
