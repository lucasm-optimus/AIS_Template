using Tilray.Integrations.Core.Common;

namespace Tilray.Integrations.Services.SAPConcur.Service.Model
{
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

    public class PaymentRequest
    {
        public string AmountWithoutVat { get; set; }
        public string BuyerCostCenter { get; set; }
        public string CheckNumber { get; set; }
        public string CompanyBillToAddressCode { get; set; }
        public string CompanyShipToAddressCode { get; set; }
        public string CountryCode { get; set; }
        public string CurrencyCode { get; set; }
        public string[] CustomFields { get; set; } = new string[24];
        public string DataSource { get; set; }
        public string DeliverySlipNumber { get; set; }
        public string Description { get; set; }
        public string DiscountPercentage { get; set; }
        public string DiscountTerms { get; set; }
        public string EmployeeEmailAddress { get; set; }
        public string EmployeeId { get; set; }
        public string EmployeeLoginId { get; set; }
        public string ExternalPolicyId { get; set; }
        public string InvoiceAmount { get; set; }
        public DateTime? InvoiceDate { get; set; }
        public string InvoiceNumber { get; set; }
        public DateTime? InvoiceReceivedDate { get; set; }
        public string IsEmergencyCheckRun { get; set; }
        public bool? IsInvoiceConfirmed { get; set; }
        public string LedgerCode { get; set; }
        public List<LineItem> LineItems { get; set; } = [];
        public string Name { get; set; }
        public string NotesToVendor { get; set; }
        public string OB10BuyerId { get; set; }
        public string OB10TransactionId { get; set; }
        public string[] OrgUnits { get; set; } = new string[6];
        public string PaymentAdjustmentNotes { get; set; }
        public string PaymentAmount { get; set; }
        public DateTime? PaymentDueDate { get; set; }
        public string PaymentTermsDays { get; set; }
        public string ProvincialTaxId { get; set; }
        public string PurchaseOrderNumber { get; set; }
        public string ReceiptConfirmationType { get; set; }
        public string ShippingAmount { get; set; }
        public string TaxAmount { get; set; }
        public string VatAmountOne { get; set; }
        public string VatAmountTwo { get; set; }
        public string VatRateOne { get; set; }
        public string VatRateTwo { get; set; }
        public VendorRemitToIdentifier VendorRemitToIdentifier { get; set; }
        public string VendorShipFromAddressCode { get; set; }
        public string VendorTaxId { get; set; }
    }

    public class LineItem
    {
        public List<Allocation> Allocations { get; set; } = [];
        public string AmountWithoutVat { get; set; }
        public string[] CustomFields { get; set; } = new string[20];
        public string Description { get; set; }
        public string ExpenseTypeCode { get; set; }
        public string ItemCode { get; set; }
        public List<MatchedPurchaseOrderReceipt> MatchedPurchaseOrderReceipts { get; set; } = [];
        public string PurchaseOrderNumber { get; set; }
        public string Quantity { get; set; }
        public string ShipFromPostalCode { get; set; }
        public string ShipToPostalCode { get; set; }
        public string SupplierPartId { get; set; }
        public string Tax { get; set; }
        public string TotalPrice { get; set; }
        public string UnitOfMeasure { get; set; }
        public string UnitPrice { get; set; }
        public string VatAmount { get; set; }
        public string VatRate { get; set; }
    }

    public class Allocation
    {
        public string[] CustomFields { get; set; } = new string[20];
        public string Custom8 { get; set; }
        public string Custom9 { get; set; }
        public string Custom10 { get; set; }
        public string Percentage { get; set; }
    }

    public class MatchedPurchaseOrderReceipt
    {
        public string GoodsReceiptNumber { get; set; }
    }

    public class VendorRemitToIdentifier
    {
        public string Address1 { get; set; }
        public string AddressCode { get; set; }
        public string Name { get; set; }
        public string PostalCode { get; set; }
        public string VendorCode { get; set; }
    }
}
