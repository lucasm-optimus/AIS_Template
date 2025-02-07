using Tilray.Integrations.Core.Domain.Aggregates.Invoices.Events;

namespace Tilray.Integrations.Services.OBeer.Service.Models;

public static class ErrorFactory
{
    public static GrpoLineItemError CreateGrpoLineItemError(Item item, InvoiceHeader header, string error)
    {
        return new GrpoLineItemError
        {
            CardCode = header?.CardCode,
            CustomerRefNo = header?.CustomerRefNo,
            PODocNum = item.PODocNum,
            GRPODocNum = item.GRPODocNum,
            PostingDate = header?.PostingDate,
            DueDate = header?.DueDate,
            DocumentDate = header?.DocumentDate,
            ConcurOrderID = header?.ConcurOrderID,
            DocumentType = header?.DocumentType,
            ErrorMessage = error
        };
    }

    public static GrpoLineItemError CreateGrpoLineItemError(Invoice invoice, string goodsReceipt)
    {
        return new GrpoLineItemError
        {
            CardCode = invoice.VendorRemitAddress.FormattedVendorCode,
            CustomerRefNo = invoice?.InvoiceNumber,
            PODocNum = string.Empty,
            GRPODocNum = string.Empty,
            PostingDate = invoice?.PostingDate,
            DueDate = invoice?.DueDate,
            DocumentDate = invoice?.DocumentDate,
            ConcurOrderID = invoice?.ID,
            DocumentType = "dDocument_Items",
            ErrorMessage = $"Could not find GRPO record - {goodsReceipt}"
        };
    }

    public static NonPOLineItemError CreateNonPOLineItemError(Item item, InvoiceHeader header, string error)
    {
        return new NonPOLineItemError
        {
            BPCode = header?.CardCode,
            BPInvoiceNumber = header?.CustomerRefNo,
            DocDate = header?.DocumentDate,
            DueDate = header?.DueDate,
            GLAccount = item.ItemDescription switch
            {
                "FREIGHT" => "55030-00",
                "VAT1" => "66010-00",
                _ => item.Custom4
            },
            Description = item.ItemDescription?.Replace(",", " "),
            TotalPrice = item.TotalPrice,
            PostingDate = header?.PostingDate,
            BrandFamily = item.BrandFamily,
            Facility = item.Facility,
            Error = error
        };
    }
}
