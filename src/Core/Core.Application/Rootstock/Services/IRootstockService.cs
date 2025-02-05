using Microsoft.AspNetCore.Mvc;
using Tilray.Integrations.Core.Domain.Aggregates.Customer;
using Tilray.Integrations.Core.Domain.Aggregates.Sales;

namespace Tilray.Integrations.Core.Application.Rootstock.Services
{
    public interface IRootstockService
    {
        Task<string?> CreateCustomer(RstkCustomer customer);
        Task<RstkCustomerInfoResponse?> GetCustomerInfo(string sfAccountId);
        Task<string?> CreateCustomerAddress(RstkCustomerAddress customerAddress);
        Task<int?> GetCustomerAddressNextSequence(string customerNo);
        Task<RstkCustomerAddressInfoResponse> GetCustomerAddressInfo(string customerNo, string addressType);
        Task<RstkCustomerAddressInfoResponse> GetCustomerAddressInfo(string customerNo, string address, string city, string state, string zip);
        Task<dynamic> CreateSalesOrder(RstkSalesOrder salesOrder);
        Task<dynamic> CreateSalesOrderLineItem(RstkSalesOrderLineItem salesOrderLineItem);
        Task CreatePrePayment(RstkPrePayment rstkPrePayment);
        Task<bool> SalesOrderExists(string customerReferenceNumber);
    }
}
