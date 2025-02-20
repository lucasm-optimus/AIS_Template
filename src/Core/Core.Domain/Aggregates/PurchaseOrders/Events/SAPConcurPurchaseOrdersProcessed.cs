namespace Tilray.Integrations.Core.Domain.Aggregates.PurchaseOrders.Events
{
    public class SAPConcurPurchaseOrdersProcessed : IDomainEvent
    {
        public IEnumerable<ImportBatchItem>? ImportBatchItem { get; set; } = new List<ImportBatchItem>();

        public IEnumerable<ErrorBatchItem>? ErrorBatchItem { get; set; } = new List<ErrorBatchItem>();

        public void AddImportBatchItem(string type, string name)
        {
            ImportBatchItem = ImportBatchItem.Append(new ImportBatchItem
            {
                Type = type,
                Name = name
            });
        }

        public void AddErrorBatchItem(string type, string name, string error)
        {
            ErrorBatchItem = ErrorBatchItem.Append(new ErrorBatchItem
            {
                Type = type,
                Name = name,
                Error = error
            });
        }

        public bool HasImportBatchItems()
        {
            return ImportBatchItem != null && ImportBatchItem.Any();
        }

        public bool HasErrorBatchItems()
        {
            return ErrorBatchItem != null && ErrorBatchItem.Any();
        }
    }

    public class ImportBatchItem
    {
        public string Type { get; set; }
        public string Name { get; set; }
    }

    public class ErrorBatchItem
    {
        public string Type { get; set; }
        public string Name { get; set; }
        public string Error { get; set; }
    }
}