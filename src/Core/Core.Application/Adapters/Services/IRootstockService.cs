using Microsoft.AspNetCore.Mvc;
using Tilray.Integrations.Core.Domain.Aggregates.Sales.Rootstock;
using Tilray.Integrations.Core.Domain.Aggregates.SalesOrders;

namespace Tilray.Integrations.Core.Application.Adapters.Services;

public interface IRootstockService
{
    Task<Result<IEnumerable<CompanyReference>>> GetAllCompanyReferencesAsync();
    Task<Result<IEnumerable<SalesOrder>>> ValidateSalesOrders(IEnumerable<SalesOrder> salesOrders);
    Task<Result> CreateSalesOrderAsync(SalesOrder salesOrder);
    Task<Result<dynamic?>> CreateCustomer(RstkCustomer customer);
    Task<Result<RstkCustomerInfoResponse?>> GetCustomerInfo(string sfAccountId);
    Task<Result<dynamic?>> CreateCustomerAddress(RstkCustomerAddress customerAddress);
    Task<int?> GetCustomerAddressNextSequence(string customerNo);
    Task<Result<RstkCustomerAddressInfoResponse>> GetCustomerAddressInfo(string customerNo, string addressType);
    Task<Result<RstkCustomerAddressInfoResponse>> GetCustomerAddressInfo(string customerNo, string address, string city, string state, string zip);
    Task<Result<dynamic?>> CreateSalesOrder(RstkSalesOrder salesOrder);
    Task<Result<dynamic?>> CreateSalesOrderLineItem(RstkSalesOrderLineItem salesOrderLineItem);
    Task<Result<dynamic?>> CreatePrePayment(RstkPrePayment rstkPrePayment);
    Task<bool> SalesOrderExists(string customerReferenceNumber);
}
