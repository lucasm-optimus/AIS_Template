namespace Tilray.Integrations.Core.Domain.Aggregates.Invoices.Events;

public class InvoicesProcessed : IDomainEvent
{
    public List<NonPOLineItemError> ErrorsNonPO { get; } = [];
    public List<GrpoLineItemError> ErrorsGrpo { get; } = [];
    public bool HasErrors => ErrorsNonPO.Count > 0 || ErrorsGrpo.Count > 0;
    public CompanyReference CompanyReference { get; set; }
    public string Message => HasErrors
        ? $"Processing failed with {ErrorsGrpo.Count} GRPO errors and {ErrorsNonPO.Count} NonPO errors."
        : "Processing succeeded.";
}

public class GrpoLineItemError
{
    public string CardCode { get; set; }
    public string CustomerRefNo { get; set; }
    public string PODocNum { get; set; }
    public string GRPODocNum { get; set; }
    public string PostingDate { get; set; }
    public string DueDate { get; set; }
    public string DocumentDate { get; set; }
    public string ConcurOrderID { get; set; }
    public string DocumentType { get; set; }
    public string Error { get; set; }
}

public class NonPOLineItemError
{
    public string BPCode { get; set; }
    public string BPInvoiceNumber { get; set; }
    public string DocDate { get; set; }
    public string DueDate { get; set; }
    public string DocComments { get; set; }
    public string GLAccount { get; set; }
    public string Description { get; set; }
    public decimal TotalPrice { get; set; }
    public string PostingDate { get; set; }
    public string BrandFamily { get; set; }
    public string Facility { get; set; }
    public string Error { get; set; }
}
