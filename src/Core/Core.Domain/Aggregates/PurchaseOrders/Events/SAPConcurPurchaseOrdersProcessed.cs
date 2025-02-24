namespace Tilray.Integrations.Core.Domain.Aggregates.PurchaseOrders.Events
{
    public class SAPConcurPurchaseOrdersProcessed : IDomainEvent
    {
        public IEnumerable<ProcessedPurchaseOrder>? ProcessedPurchaseOrders { get; set; } = new List<ProcessedPurchaseOrder>();

        public IEnumerable<FailedPurchaseOrder>? FailedPurchaseOrders { get; set; } = new List<FailedPurchaseOrder>();

        public void AddProcessedPurchaseOrder(string type, string name)
        {
            ProcessedPurchaseOrders = ProcessedPurchaseOrders.Append(new ProcessedPurchaseOrder
            {
                Type = type,
                Name = name
            });
        }

        public void AddFailedPurchaseOrder(string type, string name, string error)
        {
            FailedPurchaseOrders = FailedPurchaseOrders.Append(new FailedPurchaseOrder
            {
                Type = type,
                Name = name,
                Error = error
            });
        }

        public bool HasProcessedPurchaseOrders()
        {
            return ProcessedPurchaseOrders != null && ProcessedPurchaseOrders.Any();
        }

        public bool HasFailedPurchaseOrders()
        {
            return FailedPurchaseOrders != null && FailedPurchaseOrders.Any();
        }
    }

    public class ProcessedPurchaseOrder
    {
        public string Type { get; set; }
        public string Name { get; set; }
    }

    public class FailedPurchaseOrder
    {
        public string Type { get; set; }
        public string Name { get; set; }
        public string Error { get; set; }
    }
}