using Microsoft.AspNetCore.Mvc;
using Tilray.Integrations.Core.Domain.Aggregates.Sales.Rootstock;

namespace Tilray.Integrations.Core.Application.Rootstock.Services
{
    public interface IRootstockService
    {
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
}
