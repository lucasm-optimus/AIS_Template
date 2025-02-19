namespace Tilray.Integrations.Services.Sharepoint.Service.MappingProfiles;

public class SharepointInvoiceMapper : Profile
{
    public SharepointInvoiceMapper()
    {
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
            .ForMember(dest => dest.NumberLineItems, opt => opt.MapFrom(src => src.Invoice.LineItems.LineItem.Count()))
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
