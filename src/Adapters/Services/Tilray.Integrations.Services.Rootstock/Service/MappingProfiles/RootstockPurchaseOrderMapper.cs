namespace Tilray.Integrations.Services.Rootstock.Service.MappingProfiles;

public class RootstockPurchaseOrderMapper : Profile
{
    public RootstockPurchaseOrderMapper()
    {
        CreateMap<RootstockPurchaseOrder, PurchaseOrder>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id ?? string.Empty))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name ?? string.Empty))
            .ForMember(dest => dest.PurchaseOrderNumber, opt => opt.MapFrom(src => src.rstk__pohdr_ordno__c ?? string.Empty))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.rstk__pohdr_ordsts__c ?? string.Empty))
            .ForMember(dest => dest.Division, opt => opt.MapFrom(src => src.rstk__pohdr_div__r != null ? src.rstk__pohdr_div__r.rstk__externalid__c ?? string.Empty : string.Empty))
            .ForMember(dest => dest.VendorCode, opt => opt.MapFrom(src => src.rstk__pohdr_vendno__r != null ? src.rstk__pohdr_vendno__r.rstk__externalid__c ?? string.Empty : string.Empty))
            .ForMember(dest => dest.CurrencyCode, opt => opt.MapFrom(src => src.rstk__pohdr_maintcurr__r != null ? src.rstk__pohdr_maintcurr__r.rstk__externalid__c ?? string.Empty : string.Empty))
            .ForMember(dest => dest.OrderDate, opt => opt.MapFrom(src => src.rstk__pohdr_actplacedate__c))
            .ForMember(dest => dest.CreatedDate, opt => opt.MapFrom(src => src.CreatedDate))
            .ForMember(dest => dest.BillToAddress, opt => opt.MapFrom(src => new PurchaseOrderAddress
            {
                ExternalId = src.rstk__pohdr_pohdraddr_bt__c ?? string.Empty,
                Name = src.rstk__pohdr_pohdraddr_bt__r != null ? src.rstk__pohdr_pohdraddr_bt__r.Name ?? string.Empty : string.Empty,
                Address1 = src.rstk__pohdr_pohdraddr_bt__r != null ? src.rstk__pohdr_pohdraddr_bt__r.rstk__pohdraddr_street__c ?? string.Empty : string.Empty,
                City = src.rstk__pohdr_pohdraddr_bt__r != null ? src.rstk__pohdr_pohdraddr_bt__r.rstk__pohdraddr_city__c ?? string.Empty : string.Empty,
                StateProvince = src.rstk__pohdr_pohdraddr_bt__r != null ? src.rstk__pohdr_pohdraddr_bt__r.rstk__pohdraddr_stateprov__c ?? string.Empty : string.Empty,
                CountryCode = src.rstk__pohdr_pohdraddr_bt__r != null ? src.rstk__pohdr_pohdraddr_bt__r.rstk__pohdraddr_country__c ?? string.Empty : string.Empty,
                PostalCode = src.rstk__pohdr_pohdraddr_bt__r != null ? src.rstk__pohdr_pohdraddr_bt__r.rstk__pohdraddr_zippostalcode__c ?? string.Empty : string.Empty
            }))
            .ForMember(dest => dest.ShipToAddress, opt => opt.MapFrom(src => new PurchaseOrderAddress
            {
                ExternalId = src.rstk__pohdr_pohdraddr_st__c ?? string.Empty,
                Name = src.rstk__pohdr_pohdraddr_st__r != null ? src.rstk__pohdr_pohdraddr_st__r.Name ?? string.Empty : string.Empty,
                Address1 = src.rstk__pohdr_pohdraddr_st__r != null ? src.rstk__pohdr_pohdraddr_st__r.rstk__pohdraddr_street__c ?? string.Empty : string.Empty,
                City = src.rstk__pohdr_pohdraddr_st__r != null ? src.rstk__pohdr_pohdraddr_st__r.rstk__pohdraddr_city__c ?? string.Empty : string.Empty,
                StateProvince = src.rstk__pohdr_pohdraddr_st__r != null ? src.rstk__pohdr_pohdraddr_st__r.rstk__pohdraddr_stateprov__c ?? string.Empty : string.Empty,
                CountryCode = src.rstk__pohdr_pohdraddr_st__r != null ? src.rstk__pohdr_pohdraddr_st__r.rstk__pohdraddr_country__c ?? string.Empty : string.Empty,
                PostalCode = src.rstk__pohdr_pohdraddr_st__r != null ? src.rstk__pohdr_pohdraddr_st__r.rstk__pohdraddr_zippostalcode__c ?? string.Empty : string.Empty
            }))
            .ForMember(dest => dest.LineItems, opt => opt.MapFrom(src => src.LineItems))
            .ForMember(dest => dest.PurchaseOrderReceipts, opt => opt.MapFrom(src => src.PurchaseOrderReceipts));

        CreateMap<RootstockLineItem, PurchaseOrderLineItem>()
            .ForMember(dest => dest.ExternalId, opt => opt.MapFrom(src => src.Id ?? string.Empty))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name ?? string.Empty))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.rstk__poline_ordsts__c ?? string.Empty))
            .ForMember(dest => dest.LineNumber, opt => opt.MapFrom(src => src.rstk__poline_lne__c.ToString() ?? string.Empty))
            .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.rstk__poline_longdescr__c ?? string.Empty))
            .ForMember(dest => dest.QtyRequired, opt => opt.MapFrom(src => src.rstk__poline_qtyreq__c))
            .ForMember(dest => dest.UnitPrice, opt => opt.MapFrom(src => src.rstk__poline_unitpricemcurr__c))
            .ForMember(dest => dest.UOMCode, opt => opt.MapFrom(src => src.UOM_Code__c ?? string.Empty))
            .ForMember(dest => dest.CreatedDate, opt => opt.MapFrom(src => src.CreatedDate))
            .ForMember(dest => dest.ExpenseAccount, opt => opt.MapFrom(src => src.rstk__poline_expenseacct__r != null ? src.rstk__poline_expenseacct__r.rstk__syacc_mfgacct__c ?? string.Empty : string.Empty))
            .ForMember(dest => dest.Amount, opt => opt.MapFrom(src => src.rstk__poline_amtreqmcurr__c))
            .ForMember(dest => dest.OrderNumber, opt => opt.MapFrom(src => src.rstk__poline_ordno__c));

        CreateMap<RootstockPurchaseOrderReceipt, PurchaseOrderReceipt>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id ?? string.Empty))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name ?? string.Empty))
            .ForMember(dest => dest.PurchaseOrderId, opt => opt.MapFrom(src => src.rstk__porcptap_ordno__c ?? string.Empty))
            .ForMember(dest => dest.PurchaseOrderNumber, opt => opt.MapFrom(src => src.rstk__porcptap_ordno__r != null ? src.rstk__porcptap_ordno__r.rstk__pohdr_ordno__c ?? string.Empty : string.Empty))
            .ForMember(dest => dest.LineItemExternalId, opt => opt.MapFrom(src => src.rstk__porcptap_poline__c ?? string.Empty))
            .ForMember(dest => dest.ReceiptDate, opt => opt.MapFrom(src => src.rstk__porcptap_rcptdate__c))
            .ForMember(dest => dest.Item, opt => opt.MapFrom(src => src.rstk__porcptap_poitem__r != null ? src.rstk__porcptap_poitem__r.Name ?? string.Empty : string.Empty))
            .ForMember(dest => dest.Quantity, opt => opt.MapFrom(src => src.rstk__porcptap_qtycomp__c))
            .ForMember(dest => dest.DeliverySlipNumber, opt => opt.MapFrom(src => src.rstk__porcptap_packslipno__c ?? string.Empty))
            .ForMember(dest => dest.GoodsReceiptNumber, opt => opt.MapFrom(src => src.rstk__porcptap_rcptno__c.ToString()));
    }
}
