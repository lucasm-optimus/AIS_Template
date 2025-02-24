using Tilray.Integrations.Services.SAPConcur.Service.Models;

public class SAPConcurPurchaseOrderMapping : Profile
{
    public SAPConcurPurchaseOrderMapping()
    {
        CreateMap<PurchaseOrder, SAPConcurPurchaseOrder>()
            .ForMember(dest => dest.ID, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
            .ForMember(dest => dest.PurchaseOrderNumber, opt => opt.MapFrom(src => src.PurchaseOrderNumber))
            .ForMember(dest => dest.PolicyExternalID, opt => opt.MapFrom(src => "283D78BF6F8242F98E02")) // Assuming this is a constant or can be fetched from a config
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => "Transmitted"))
            .ForMember(dest => dest.IsTest, opt => opt.MapFrom(src => "N"))
            .ForMember(dest => dest.IsChangeOrder, opt => opt.MapFrom(src => "N"))
            .ForMember(dest => dest.LedgerCode, opt => opt.MapFrom(src => "Default"))
            .ForMember(dest => dest.OrderDate, opt => opt.MapFrom(src => src.OrderDate))
            .ForMember(dest => dest.CurrencyCode, opt => opt.MapFrom(src => src.CurrencyCode))
            .ForMember(dest => dest.VendorCode, opt => opt.MapFrom(src => src.VendorCode + "-" + src.Division))
            .ForMember(dest => dest.VendorAddressCode, opt => opt.MapFrom(src => src.VendorCode + "-" + src.Division + "-" + src.VendorAddressNumber))
            .ForMember(dest => dest.Custom9, opt => opt.MapFrom(src => src.PurchaseOrderNumber))
            .ForMember(dest => dest.ShipToAddress, opt => opt.MapFrom(src => src.ShipToAddress))
            .ForMember(dest => dest.BillToAddress, opt => opt.MapFrom(src => src.BillToAddress))
            .ForMember(dest => dest.LineItem, (Action<IMemberConfigurationExpression<PurchaseOrder, SAPConcurPurchaseOrder, List<SAPConcurLineItem>>>)(opt => opt.MapFrom<List<SAPConcurLineItem>>((Func<PurchaseOrder, SAPConcurPurchaseOrder, List<SAPConcurLineItem>, ResolutionContext, List<SAPConcurLineItem>>)((src, dest, destMember, context) =>
            {
                var customFields = context.Items["CustomFields"] as SAPConcurCustomValues;
                return src.LineItems.Select<PurchaseOrderLineItem, SAPConcurLineItem>((Func<PurchaseOrderLineItem, SAPConcurLineItem>)(lineItem => new SAPConcurLineItem
                {
                    ExternalID = lineItem.ExternalId,
                    CreatedDate = lineItem.CreatedDate,
                    IsReceiptRequired = "true",
                    PurchaseOrderReceiptType = "WQTY",
                    LineNumber = int.Parse(lineItem.LineNumber),
                    Description = lineItem.Description,
                    Quantity = (decimal)lineItem.QtyRequired,
                    UnitPrice = lineItem.UnitPrice,
                    UnitOfMeasureCode = lineItem.UOMCode,
                    AccountCode = "Default",
                    Custom1 = customFields?.Custom1,
                    Custom2 = customFields?.Custom2,
                    Custom3 = customFields?.Custom3,
                    Custom4 = lineItem.ExpenseAccount ?? customFields?.Custom4,
                    Allocation = new List<Allocation>
                    {
                        new Allocation
                        {
                            Amount = lineItem.Amount,
                            Percentage = "100.0",
                            GrossAmount = lineItem.Amount
                        }
                    }
                })).ToList<SAPConcurLineItem>();
            }))));

        CreateMap<PurchaseOrderAddress, SAPConcurAddress>()
            .ForMember(dest => dest.ExternalID, opt => opt.MapFrom(src => src.ExternalId))
            .ForMember(dest => dest.Address1, opt => opt.MapFrom(src => src.Address1))
            .ForMember(dest => dest.City, opt => opt.MapFrom(src => src.City))
            .ForMember(dest => dest.StateProvince, opt => opt.MapFrom(src => src.StateProvince))
            .ForMember(dest => dest.CountryCode, opt => opt.MapFrom(src =>
                src.CountryCode.ToLower() == "canada" ? "CA" :
                src.CountryCode.ToLower() == "united states" || src.CountryCode.ToLower() == "united states of america" || src.CountryCode.ToLower() == "usa" ? "US" :
                src.CountryCode.ToLower() == "united kingdom" ? "UK" :
                src.CountryCode))
            .ForMember(dest => dest.PostalCode, opt => opt.MapFrom(src => src.PostalCode));

        CreateMap<PurchaseOrderLineItem, SAPConcurLineItem>()
            .ForMember(dest => dest.ExternalID, opt => opt.MapFrom(src => src.ExternalId))
            .ForMember(dest => dest.CreatedDate, opt => opt.MapFrom(src => src.CreatedDate))
            .ForMember(dest => dest.IsReceiptRequired, opt => opt.MapFrom(src => "true"))
            .ForMember(dest => dest.PurchaseOrderReceiptType, opt => opt.MapFrom(src => "WQTY"))
            .ForMember(dest => dest.LineNumber, opt => opt.MapFrom(src => int.Parse(src.LineNumber)))
            .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
            .ForMember(dest => dest.Quantity, opt => opt.MapFrom(src => (decimal)src.QtyRequired))
            .ForMember(dest => dest.UnitPrice, opt => opt.MapFrom(src => src.UnitPrice))
            .ForMember(dest => dest.UnitOfMeasureCode, opt => opt.MapFrom(src => src.UOMCode))
            .ForMember(dest => dest.AccountCode, opt => opt.MapFrom(src => "Default"))
            .ForMember(dest => dest.Allocation, opt => opt.MapFrom(src => new List<Allocation>
            {
                new Allocation
                {
                    Amount = src.Amount,
                    Percentage = "100.0",
                    GrossAmount = src.Amount
                }
            }));

        CreateMap<PurchaseOrderReceipt, SAPConcurPurchaseOrderReceipt>()
            .ForMember(dest => dest.ID, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.PurchaseOrderNumber, opt => opt.MapFrom(src => src.PurchaseOrderNumber))
            .ForMember(dest => dest.LineItemExternalID, opt => opt.MapFrom(src => src.LineItemExternalId))
            .ForMember(dest => dest.ReceivedQuantity, opt => opt.MapFrom(src => (decimal)src.Quantity))
            .ForMember(dest => dest.ReceivedDate, opt => opt.MapFrom(src => src.ReceiptDate))
            .ForMember(dest => dest.GoodsReceiptNumber, opt => opt.MapFrom(src => src.Name))
            .ForMember(dest => dest.DeliverySlipNumber, opt => opt.MapFrom(src => src.DeliverySlipNumber))
            .ForMember(dest => dest.Deleted, opt => opt.MapFrom(src => "false"))
            .ForMember(dest => dest.URI, opt => opt.MapFrom(src => "string"));
    }
}