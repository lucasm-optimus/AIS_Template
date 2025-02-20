using Tilray.Integrations.Core.Domain.Aggregates.Expenses;

using Tilray.Integrations.Core.Domain.Aggregates.Expenses;
using Tilray.Integrations.Core.Domain.Aggregates.PurchaseOrders;
using Tilray.Integrations.Core.Domain.Aggregates.PurchaseOrders.Events;

namespace Tilray.Integrations.Core.Application.Adapters.Services;

public interface ISAPConcurService
{
    Task<Result<IEnumerable<ExpenseDetails>>> GetExpenseFilesAsync();
    Task<Result<IEnumerable<Invoice>>> GetInvoicesAsync();
    Task<Result<VendorResponse>> GetVendorAsync(PurchaseOrder purchaseOrder);
    Task<Result<VendorCustomResponse>> GetVendorCustomAsync(string itemId, string custom3Value);
    Task<Result<VendorCustomResponse>> GetVendorCustomsAsync(string itemId, string custom3Value);
    Task<Result<bool>> PurchaseOrderExistsAsync(string purchaseOrderNumber);
    Task<Result> CreatePurchaseOrderAsync(PurchaseOrder purchaseOrder, SAPConcurCustomValues customFields);
    Task<Result> UpdatePurchaseOrderAsync(PurchaseOrder purchaseOrder, SAPConcurCustomValues customFields);
    Task<Result<bool>> PurchaseOrderReceiptExistsAsync(PurchaseOrderReceipt purchaseOrderReceipt);
    Task<Result> CreatePurchaseOrderReceiptAsync(PurchaseOrderReceipt purchaseOrderReceipt);
    Task<Result> UpdatePurchaseOrderReceiptAsync(PurchaseOrderReceipt purchaseOrderReceipt);

}
