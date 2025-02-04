using System.Text.RegularExpressions;

namespace Tilray.Integrations.Services.OBeer.Service.MappingProfiles;

public class ObeerInvoiceMapper : Profile
{
    public ObeerInvoiceMapper()
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
    }
}
