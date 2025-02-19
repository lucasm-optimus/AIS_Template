using Address = Tilray.Integrations.Services.Rootstock.Service.Models.Address;

namespace Tilray.Integrations.Services.Rootstock.Service.MappingProfiles;

public class RootstockSalesOrderMapper : Profile
{
    public RootstockSalesOrderMapper()
    {
        CreateMap<SalesOrder, RootstockSalesOrder>()
            .ForMember(dest => dest.SoapiMode, opt => opt.MapFrom(src => "Add Both"))
            .ForMember(dest => dest.SoapiAddOrUpdate, opt => opt.MapFrom(src => false))
            .ForMember(dest => dest.SalesDivision, opt => opt.MapFrom(src => src.Division.ToString()))
            .ForMember(dest => dest.UpdateCustomerFields, opt => opt.MapFrom(src => true))
            .ForMember(dest => dest.SoapiOrderType, opt => opt.MapFrom(src => new OrderType
            {
                ExternalId = $"{src.Division ?? ""}_{src.OrderType ?? ""}"
            }))
            .ForMember(dest => dest.OrderDate, opt => opt.MapFrom(src => src.OrderDate))
            .ForMember(dest => dest.ExpectedDeliveryDate, opt => opt.MapFrom(src => src.ExpectedDeliveryDate))
            .ForMember(dest => dest.CustomerPO, opt => opt.MapFrom(src => src.CustomerPO))
            .ForMember(dest => dest.SoapiCustomer, opt => opt.MapFrom(src => new Customer
            {
                ExternalId = src.Customer
            }))
            .ForMember(dest => dest.ShipToAddress, opt => opt.MapFrom(src => src.ShipToID != null ? new Address { ExternalId = src.ShipToID } : null))
            .ForMember(dest => dest.BackgroundProcessing, opt => opt.MapFrom(src => src.BackgroundProcessing))
            .ForMember(dest => dest.UploadGroup, opt => opt.MapFrom(src => src.UploadGroup ?? null))
            .ForMember(dest => dest.SoapiProduct, opt => opt.MapFrom(src => new Product
            {
                ExternalId = src.LineItems.Count != 0 ? $"{src.Division}_{src.LineItems[0].ItemNumber}" : $"{src.Division}"
            }))
            .ForMember(dest => dest.QuantityOrder, opt => opt.MapFrom(src => src.LineItems.Count != 0 ? src.LineItems[0].Quantity : 0))
            .ForMember(dest => dest.UnitPrice, opt => opt.MapFrom(src => src.LineItems.Count != 0 ? src.LineItems[0].UnitPrice : 0))
            .ForMember(dest => dest.ExternalOrderReference, opt => opt.MapFrom(src => src.ExternalRefNumber ?? null));

        CreateMap<(Core.Domain.Aggregates.SalesOrders.LineItem LineItem, string createdSalesOrderHeaderId, SalesOrder SalesOrder), RootstockSalesOrder>()
            .ForMember(dest => dest.SoapiMode, opt => opt.MapFrom(src => "Add Line"))
            .ForMember(dest => dest.SoapiSohdr, opt => opt.MapFrom(src => src.createdSalesOrderHeaderId))
            .ForMember(dest => dest.SoapiProduct, opt => opt.MapFrom(src => new Product()
            {
                ExternalId = $"{src.SalesOrder.Division}_{src.LineItem.ItemNumber}"
            }))
            .ForMember(dest => dest.QuantityOrder, opt => opt.MapFrom(src => src.LineItem.Quantity))
            .ForMember(dest => dest.BackgroundProcessing, opt => opt.MapFrom(src => src.SalesOrder.BackgroundProcessing))
            .ForMember(dest => dest.UploadGroup, opt => opt.MapFrom(src => src.SalesOrder.UploadGroup))
            .ForMember(dest => dest.UnitPrice, opt => opt.MapFrom(src => src.LineItem.UnitPrice))
            .ForMember(dest => dest.UpdateCustomerFields, opt => opt.MapFrom(src => true));
    }
}
