namespace Tilray.Integrations.Services.SAPConcur.Service.Model;

public record InvoiceDigests
{
    public List<InvoiceDigest> PaymentRequestDigest { get; set; } = [];
    public string NextPage { get; set; }
    public int TotalCount { get; set; }
}

public record InvoiceDigest
{
    public string ID { get; set; }
    public string ApprovalStatusCode { get; set; }
    public string PaymentRequestId { get; set; }
    public string PaymentRequestUri { get; set; }
    public string PaymentStatusCode { get; set; }
    public string VendorName { get; set; }
    public DateTime? CreateDate { get; set; }
    public string CurrencyCode { get; set; }
    public string InvoiceNumber { get; set; }
    public bool? IsDeleted { get; set; }
    public string OwnerLoginID { get; set; }
    public string OwnerName { get; set; }
    public DateTime? PaidDate { get; set; }
    public decimal? Total { get; set; }
    public string URI { get; set; }
    public DateTime? UserDefinedDate { get; set; }
    public string PaymentMethod { get; set; }
    public DateTime? LastModifiedDate { get; set; }
    public DateTime? ExtractedDate { get; set; }
}
