namespace Tilray.Integrations.Core.Domain.Aggregates.PurchaseOrders.Commands;

public class CreatePurchaseOrderInSAPConcurCommand : ICommand<SAPConcurPurchaseOrdersProcessed>
{
    public PurchaseOrder PurchaseOrder { get; }

    public string ERP { get; set; }

    public CreatePurchaseOrderInSAPConcurCommand(PurchaseOrder purchaseOrders, string eRP)
    {
        PurchaseOrder = purchaseOrders;
        ERP = eRP;
    }
}
