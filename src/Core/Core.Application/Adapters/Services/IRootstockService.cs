using Microsoft.AspNetCore.Mvc;
using Tilray.Integrations.Core.Domain.Aggregates.Sales.Rootstock;
using Tilray.Integrations.Core.Domain.Aggregates.SalesOrders;

namespace Tilray.Integrations.Core.Application.Adapters.Services;

public interface IRootstockService
{
    Task<Result<IEnumerable<CompanyReference>>> GetAllCompanyReferencesAsync();
    Task<Result<IEnumerable<SalesOrder>>> ValidateSalesOrders(IEnumerable<SalesOrder> salesOrders);
    Task<Result> CreateSalesOrderAsync(SalesOrder salesOrder);
    Task<ResponseResult> CreateCustomer(RstkCustomer customer);
    Task<RstkCustomerInfoResponse?> GetCustomerInfo(string sfAccountId);
    Task<ResponseResult> CreateCustomerAddress(RstkCustomerAddress customerAddress);
    Task<int?> GetCustomerAddressNextSequence(string customerNo);
    Task<RstkCustomerAddressInfoResponse> GetCustomerAddressInfo(string customerNo, string addressType);
    Task<RstkCustomerAddressInfoResponse> GetCustomerAddressInfo(string customerNo, string address, string city, string state, string zip);
    Task<ResponseResult> CreateSalesOrder(RstkSalesOrder salesOrder);
    Task<ResponseResult> CreateSalesOrderLineItem(RstkSalesOrderLineItem salesOrderLineItem);
    Task<ResponseResult> CreatePrePayment(RstkPrePayment rstkPrePayment);
    Task<bool> SalesOrderExists(string customerReferenceNumber);
}
