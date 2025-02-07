namespace Tilray.Integrations.Services.Sharepoint.Service.Models;

public class SharepointInvoice
{
    public string Detail { get; set; }
    public string RequestKey { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public string VendorInvoiceNumber { get; set; }
    public string InvoiceDate { get; set; }
    public string PaymentDueDate { get; set; }
    public double InvoiceAmount { get; set; }
    public double TotalAmount { get; set; }
    public string PONumber { get; set; }
    public double ShippingAmount { get; set; }
    public double TaxAmount { get; set; }
    public string RequestID { get; set; }
    public string InvoiceReceivedDate { get; set; }
    public string LastModifiedDate { get; set; }
    public string ExtractedDate { get; set; }
    public string PaymentMethodType { get; set; }
    public decimal JournalAmount { get; set; }
    public string RequestedAlphaCurrencyCode { get; set; }
    public string AllocationCustom1 { get; set; }
    public string AllocationCustom2 { get; set; }
    public string AllocationCustom3 { get; set; }
    public string AllocationCustom4 { get; set; }
    public string AllocationCustom5 { get; set; }
    public string AllocationCustom6 { get; set; }
    public string AllocationCustom7 { get; set; }
    public string AllocationCustom8 { get; set; }
    public string AllocationCustom9 { get; set; }
    public string AllocationCustom10 { get; set; }
    public string DeliverySlipNumber { get; set; }
    public int NumberLineItems { get; set; }
    public int LineItemSequenceOrder { get; set; }
    public string LineItemDescription { get; set; }
    public double LineItemQuantity { get; set; }
    public double LineItemUnitPrice { get; set; }
    public double LineItemTotal { get; set; }
    public string LineItemCustom1 { get; set; }
    public string LineItemCustom2 { get; set; }
    public string LineItemCustom3 { get; set; }
    public string LineItemCustom4 { get; set; }
    public string LineItemCustom5 { get; set; }
    public string LineItemCustom6 { get; set; }
    public string LineItemCustom7 { get; set; }
    public string LineItemCustom8 { get; set; }
    public string LineItemCustom9 { get; set; }
    public string LineItemCustom10 { get; set; }
    public string PartSupplierID { get; set; }
    public string VendorName { get; set; }
    public string VendorCode { get; set; }
    public double RequestVATAmount1 { get; set; }
    public double RequestVATAmount2 { get; set; }
    public double LineItemAmountWithoutVAT { get; set; }
}

