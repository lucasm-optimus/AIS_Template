using System.Text.RegularExpressions;

namespace Tilray.Integrations.Core.Application.Profiles;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<(LineItem lineItem, MatchedPurchaseOrderReceipt grpo), Item>()
            .ForMember(dest => dest.PODocNum, opt => opt.MapFrom(src => src.grpo.PODocNum))
            .ForMember(dest => dest.GRPODocNum, opt => opt.MapFrom(src => src.grpo.GRPODocNum))
            .ForMember(dest => dest.POLineNum, opt => opt.MapFrom(src => src.grpo.POLineNum))
            .ForMember(dest => dest.GRPOLineNum, opt => opt.MapFrom(src => src.grpo.GRPOLineNum))
            .ForMember(dest => dest.ItemCode, opt => opt.MapFrom(src =>
                src.grpo.Type == GRPOType.Item
                    ? Regex.Match(src.grpo.GoodsReceiptNumber, @".*\(([^)]+)\).*").Groups[1].Value
                    : string.Empty))
            .ForMember(dest => dest.ItemDescription, opt => opt.MapFrom(src =>
                string.IsNullOrEmpty(src.lineItem.Description)
                    ? "Description"
                    : src.lineItem.Description.Replace("\"", "").Replace("'", "")))
            .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.grpo.Type))
            .ForMember(dest => dest.GLAccount, opt => opt.MapFrom(src => src.lineItem.OBeerGLAccount))
            .ForMember(dest => dest.BrandFamily, opt => opt.MapFrom(src => src.lineItem.Custom8))
            .ForMember(dest => dest.Facility, opt => opt.MapFrom(src => src.lineItem.Custom9))
            .ForMember(dest => dest.DistributorID, opt => opt.MapFrom(src => src.lineItem.Custom7))
            .ForMember(dest => dest.Quantity, opt => opt.MapFrom(src => src.grpo.AllocatedQuantity))
            .ForMember(dest => dest.UnitPrice, opt => opt.MapFrom(src => src.lineItem.UnitPrice))
            .ForMember(dest => dest.TotalPrice, opt => opt.MapFrom(src => Math.Round(src.lineItem.UnitPrice * src.grpo.AllocatedQuantity, 2)));

        CreateMap<(Invoice invoice, IEnumerable<LineItem> lineItems, IEnumerable<Item> items, string documentType), ObeerInvoice>()
            .ForPath(dest => dest.Import.InvoiceHeader, opt => opt.MapFrom(src =>
                new List<InvoiceHeader>
                {
                    new()
                    {
                        ConcurOrderID = src.invoice.ID,
                        CustomerRefNo = src.invoice.InvoiceNumber,
                        CardCode = src.invoice.VendorRemitAddress.FormattedVendorCode,
                        PostingDate = src.invoice.PostingDate,
                        DueDate = src.invoice.DueDate,
                        DocumentDate = src.invoice.DocumentDate,
                        DocumentType = src.documentType
                    }
                }))
            .ForPath(dest => dest.Import.Items, opt => opt.MapFrom(src =>
                src.items.Concat(
                    (src.invoice.ShippingAmount > 0)
                        ? new List<Item>
                        {
                            new()
                            {
                                PODocNum = src.items.FirstOrDefault().PODocNum,
                                GRPODocNum = src.items.FirstOrDefault().GRPODocNum,
                                POLineNum = string.Empty,
                                GRPOLineNum = string.Empty,
                                ItemCode = "FREIGHT",
                                ItemDescription = "FREIGHT",
                                GLAccount = "_SYS00000000706",
                                BrandFamily = src.lineItems.FirstOrDefault().Custom8,
                                Facility = src.lineItems.FirstOrDefault().Custom9,
                                DistributorID = src.lineItems.FirstOrDefault().Custom7,
                                Quantity = 1,
                                UnitPrice = src.invoice.ShippingAmount,
                                TotalPrice = src.invoice.ShippingAmount
                            }
                        }
                        : Enumerable.Empty<Item>()
                    ).Concat(
                    (src.invoice.VatAmountOne > 0)
                        ? new List<Item>
                        {
                                new()
                                {
                                    PODocNum = src.items.FirstOrDefault().PODocNum,
                                    GRPODocNum = src.items.FirstOrDefault().GRPODocNum,
                                    POLineNum = string.Empty,
                                    GRPOLineNum = string.Empty,
                                    ItemCode = "VAT1",
                                    ItemDescription = "VAT1",
                                    GLAccount = "_SYS00000000507",
                                    BrandFamily = src.invoice.LineItems.LineItem.FirstOrDefault().Custom8,
                                    Facility = src.invoice.LineItems.LineItem.FirstOrDefault().Custom9,
                                    DistributorID = src.invoice.LineItems.LineItem.FirstOrDefault().Custom7,
                                    Quantity = 1,
                                    UnitPrice = src.invoice.VatAmountOne,
                                    TotalPrice = src.invoice.VatAmountOne
                                }
                        }
                        : Enumerable.Empty<Item>()
                    )
            ));

        CreateMap<(Invoice invoice, IEnumerable<LineItem> lineItems, string documentType, bool hasGrpoLines), ObeerInvoice>()
            .ForPath(dest => dest.Import.InvoiceHeader, opt => opt.MapFrom(src =>
                new List<InvoiceHeader>
                {
                    new()
                    {
                        ConcurOrderID = src.invoice.ID,
                        CustomerRefNo = src.invoice.InvoiceNumber,
                        CardCode = src.invoice.VendorRemitAddress.FormattedVendorCode,
                        PostingDate = src.invoice.PostingDate,
                        DueDate = src.invoice.DueDate,
                        DocumentDate = src.invoice.DocumentDate,
                        DocumentType = src.documentType,
                        PODocNum = string.Empty,
                        GRPODocNum = string.Empty,
                    }
                }))
            .ForPath(dest => dest.Import.Items, opt => opt.MapFrom(src =>
                src.lineItems
                    .Select(lineItem => new Item
                    {
                        POLineNum = string.Empty,
                        GRPOLineNum = string.Empty,
                        ItemCode = string.Empty,
                        ItemDescription = string.IsNullOrEmpty(lineItem.Description)
                            ? "Description"
                            : lineItem.Description.Replace("\"", "").Replace("'", ""),
                        GLAccount = lineItem.OBeerGLAccount,
                        BrandFamily = lineItem.Custom8,
                        Facility = lineItem.Custom9,
                        DistributorID = lineItem.Custom7,
                        Quantity = lineItem.Quantity,
                        UnitPrice = lineItem.UnitPrice,
                        TotalPrice = lineItem.TotalPrice,
                        Custom4 = lineItem.Custom4
                    })
                    .Concat(
                        src.invoice.ShippingAmount > 0 && !src.hasGrpoLines
                            ? new List<Item>
                            {
                                    new()
                                    {
                                        POLineNum = string.Empty,
                                        GRPOLineNum = string.Empty,
                                        ItemCode = string.Empty,
                                        ItemDescription = "FREIGHT",
                                        GLAccount = "_SYS00000000706",
                                        BrandFamily = src.lineItems.FirstOrDefault().Custom8,
                                        Facility = src.lineItems.FirstOrDefault().Custom9,
                                        DistributorID = src.lineItems.FirstOrDefault().Custom7,
                                        Quantity = 1,
                                        UnitPrice = src.invoice.ShippingAmount,
                                        TotalPrice = src.invoice.ShippingAmount
                                    }
                            }
                            : Enumerable.Empty<Item>()
                    )
                    .Concat(
                        src.invoice.VatAmountOne > 0 && !src.hasGrpoLines
                            ? new List<Item>
                            {
                                    new()
                                    {
                                        POLineNum = string.Empty,
                                        GRPOLineNum = string.Empty,
                                        ItemCode = string.Empty,
                                        ItemDescription = "VAT1",
                                        GLAccount = "_SYS00000000507",
                                        BrandFamily = src.lineItems.FirstOrDefault().Custom8,
                                        Facility = src.lineItems.FirstOrDefault().Custom9,
                                        DistributorID = src.lineItems.FirstOrDefault().Custom7,
                                        Quantity = 1,
                                        UnitPrice = src.invoice.VatAmountOne,
                                        TotalPrice = src.invoice.VatAmountOne
                                    }
                            }
                            : Enumerable.Empty<Item>()
                    )
            ));

        CreateMap<(Invoice Invoice, LineItem LineItem, int LineItemNumber), SharepointInvoice>()
            .ForMember(dest => dest.Detail, opt => opt.MapFrom(src => "DETAIL"))
            .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Invoice.Name.Replace(",", "|")))
            .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Invoice.Description.Replace(",", "|")))
            .ForMember(dest => dest.VendorInvoiceNumber, opt => opt.MapFrom(src => src.Invoice.InvoiceNumber))
            .ForMember(dest => dest.InvoiceDate, opt => opt.MapFrom(src =>
                src.Invoice.InvoiceDate.HasValue ? src.Invoice.InvoiceDate.Value.ToString("yyyy-MM-dd") : string.Empty))
            .ForMember(dest => dest.PaymentDueDate, opt => opt.MapFrom(src =>
                src.Invoice.PaymentDueDate.HasValue ? src.Invoice.PaymentDueDate.Value.ToString("yyyy-MM-dd") : string.Empty))
            .ForMember(dest => dest.InvoiceAmount, opt => opt.MapFrom(src => src.Invoice.InvoiceAmount))
            .ForMember(dest => dest.TotalAmount, opt => opt.MapFrom(src => src.Invoice.CalculatedAmount))
            .ForMember(dest => dest.PONumber, opt => opt.MapFrom(src => src.Invoice.PurchaseOrderNumber.Replace(",", "|")))
            .ForMember(dest => dest.ShippingAmount, opt => opt.MapFrom(src => src.Invoice.ShippingAmount))
            .ForMember(dest => dest.TaxAmount, opt => opt.MapFrom(src => src.Invoice.TaxAmount))
            .ForMember(dest => dest.RequestID, opt => opt.MapFrom(src => src.Invoice.ID))
            .ForMember(dest => dest.InvoiceReceivedDate, opt => opt.MapFrom(src =>
                src.Invoice.InvoiceReceivedDate.HasValue ? src.Invoice.InvoiceReceivedDate.Value.ToString("yyyy-MM-dd") : string.Empty))
            .ForMember(dest => dest.LastModifiedDate, opt => opt.MapFrom(src =>
                src.Invoice.LastModifiedDate.HasValue ? src.Invoice.LastModifiedDate.Value.ToString("yyyy-MM-dd") : string.Empty))
            .ForMember(dest => dest.ExtractedDate, opt => opt.MapFrom(src =>
                src.Invoice.ExtractDate.HasValue ? src.Invoice.ExtractDate.Value.ToString("yyyy-MM-dd") : string.Empty))
            .ForMember(dest => dest.PaymentMethodType, opt => opt.MapFrom(src => src.Invoice.PaymentMethod))
            .ForMember(dest => dest.JournalAmount, opt => opt.MapFrom(src => src.LineItem.AmountWithoutVat))
            .ForMember(dest => dest.RequestedAlphaCurrencyCode, opt => opt.MapFrom(src => src.Invoice.CurrencyCode))
            .ForMember(dest => dest.AllocationCustom1, opt => opt.MapFrom(src =>
                string.IsNullOrEmpty(src.Invoice.Custom1) ? "" : "'" + src.Invoice.Custom1))
            .ForMember(dest => dest.AllocationCustom2, opt => opt.MapFrom(src =>
                string.IsNullOrEmpty(src.Invoice.Custom1) ? "" : "'" + src.Invoice.Custom1))
            .ForMember(dest => dest.AllocationCustom3, opt => opt.MapFrom(src => src.Invoice.Custom3.Replace(",", "|") ?? string.Empty))
            .ForMember(dest => dest.AllocationCustom4, opt => opt.MapFrom(src => src.Invoice.Custom4.Replace(",", "|") ?? string.Empty))
            .ForMember(dest => dest.AllocationCustom5, opt => opt.MapFrom(src => src.Invoice.Custom5.Replace(",", "|") ?? string.Empty))
            .ForMember(dest => dest.AllocationCustom6, opt => opt.MapFrom(src => src.Invoice.Custom6.Replace(",", "|") ?? string.Empty))
            .ForMember(dest => dest.AllocationCustom7, opt => opt.MapFrom(src => src.Invoice.Custom7.Replace(",", "|") ?? string.Empty))
            .ForMember(dest => dest.AllocationCustom8, opt => opt.MapFrom(src => src.Invoice.Custom8.Replace(",", "|") ?? string.Empty))
            .ForMember(dest => dest.AllocationCustom9, opt => opt.MapFrom(src => src.Invoice.Custom9.Replace(",", "|") ?? string.Empty))
            .ForMember(dest => dest.AllocationCustom10, opt => opt.MapFrom(src => src.Invoice.Custom10.Replace(",", "|") ?? string.Empty))
            .ForMember(dest => dest.PartSupplierID, opt => opt.MapFrom(src => src.LineItem.SupplierPartId.Replace(",", "|") ?? string.Empty))
            .ForMember(dest => dest.VendorName, opt => opt.MapFrom(src => src.Invoice.VendorRemitAddress.VendorCode.Replace(",", "|") ?? string.Empty))
            .ForMember(dest => dest.VendorCode, opt => opt.MapFrom(src => src.Invoice.VendorRemitAddress.VendorCode.Replace(",", "|") ?? string.Empty))
            .ForMember(dest => dest.RequestVATAmount1, opt => opt.MapFrom(src => src.Invoice.VatAmountOne))
            .ForMember(dest => dest.RequestVATAmount2, opt => opt.MapFrom(src => src.Invoice.VatAmountTwo))
            .ForMember(dest => dest.DeliverySlipNumber, opt => opt.MapFrom(src => src.Invoice.DeliverySlipNumber))
            .ForMember(dest => dest.NumberLineItems, opt => opt.MapFrom(src => src.Invoice.LineItems.LineItem.Count))
            .ForMember(dest => dest.LineItemSequenceOrder, opt => opt.MapFrom(src => src.LineItemNumber))
            .ForMember(dest => dest.LineItemDescription, opt => opt.MapFrom(src => src.LineItem.Description))
            .ForMember(dest => dest.LineItemQuantity, opt => opt.MapFrom(src => src.LineItem.Quantity))
            .ForMember(dest => dest.LineItemUnitPrice, opt => opt.MapFrom(src => src.LineItem.UnitPrice))
            .ForMember(dest => dest.LineItemTotal, opt => opt.MapFrom(src => src.LineItem.TotalPrice))
            .ForMember(dest => dest.LineItemCustom1, opt => opt.MapFrom(src =>
                string.IsNullOrEmpty(src.LineItem.Custom1) ? "" : "'" + src.Invoice.Custom1))
            .ForMember(dest => dest.LineItemCustom2, opt => opt.MapFrom(src =>
                string.IsNullOrEmpty(src.LineItem.Custom1) ? "" : "'" + src.Invoice.Custom1))
            .ForMember(dest => dest.LineItemCustom3, opt => opt.MapFrom(src => src.LineItem.Custom3.Replace(",", "|") ?? string.Empty))
            .ForMember(dest => dest.LineItemCustom4, opt => opt.MapFrom(src => src.LineItem.Custom4.Replace(",", "|") ?? string.Empty))
            .ForMember(dest => dest.LineItemCustom5, opt => opt.MapFrom(src => src.LineItem.Custom5.Replace(",", "|") ?? string.Empty))
            .ForMember(dest => dest.LineItemCustom6, opt => opt.MapFrom(src => src.LineItem.Custom6.Replace(",", "|") ?? string.Empty))
            .ForMember(dest => dest.LineItemCustom7, opt => opt.MapFrom(src => src.LineItem.Custom7.Replace(",", "|") ?? string.Empty))
            .ForMember(dest => dest.LineItemCustom8, opt => opt.MapFrom(src => src.LineItem.Custom8.Replace(",", "|") ?? string.Empty))
            .ForMember(dest => dest.LineItemCustom9, opt => opt.MapFrom(src => src.LineItem.Custom9.Replace(",", "|") ?? string.Empty))
            .ForMember(dest => dest.LineItemCustom10, opt => opt.MapFrom(src => src.LineItem.Custom10.Replace(",", "|") ?? string.Empty))
            .ForMember(dest => dest.PartSupplierID, opt => opt.MapFrom(src => src.LineItem.SupplierPartId.Replace(",", "|") ?? string.Empty))
            .ForMember(dest => dest.VendorName, opt => opt.MapFrom(src => src.Invoice.VendorRemitAddress.VendorCode.Replace(",", "|") ?? string.Empty))
            .ForMember(dest => dest.VendorCode, opt => opt.MapFrom(src => src.Invoice.VendorRemitAddress.VendorCode.Replace(",", "|") ?? string.Empty))
            .ForMember(dest => dest.RequestVATAmount1, opt => opt.MapFrom(src => src.Invoice.VatAmountOne));
    }
}
