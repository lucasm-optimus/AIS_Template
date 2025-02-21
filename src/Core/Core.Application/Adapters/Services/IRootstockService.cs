using Tilray.Integrations.Core.Domain.Aggregates.SalesOrders.Rootstock;

using Tilray.Integrations.Core.Domain.Aggregates.SalesOrders.Rootstock;
using Microsoft.AspNetCore.Mvc;

namespace Tilray.Integrations.Core.Application.Adapters.Services;

public interface IRootstockService
{
    Task<Result<IEnumerable<CompanyReference>>> GetAllCompanyReferencesAsync();
    Task<Result<IEnumerable<SalesOrder>>> ValidateSalesOrders(IEnumerable<SalesOrder> salesOrders);
    Task<Result> CreateSalesOrderAsync(SalesOrder salesOrder);
    Task<Result<string?>> CreateCustomer(RstkCustomer customer);
    Task<Result<RstkCustomerInfoResponse?>> GetCustomerInfo(string sfAccountId);
    Task<Result<string?>> CreateCustomerAddress(RstkCustomerAddress customerAddress);
    Task<int?> GetCustomerAddressNextSequence(string customerNo);
    Task<Result<RstkCustomerAddressInfoResponse>> GetCustomerAddressInfo(string customerNo, string addressType);
    Task<Result<RstkCustomerAddressInfoResponse>> GetCustomerAddressInfo(string customerNo, string address, string city, string state, string zip);
    Task<Result<string?>> CreateSalesOrder(RstkSalesOrder salesOrder);
    Task<Result<string?>> CreateSalesOrderLineItem(RstkSalesOrderLineItem salesOrderLineItem);
    Task<Result<string?>> CreatePrePayment(RstkSalesOrderPrePayment rstkPrePayment);
    Task<Result<string?>> CreatePrePayment(RstkSyDataPrePayment prePayment);
    Task<bool> SalesOrderExists(string customerReferenceNumber);
    Task<Result<string>> GetIdFromExternalColumnReference(string objectName, string externalIdColumnName, string externalId);
    Task<Result<string>> GetSoHdr(string soapiId);
    Task<Result<List<ExpenseError>>> CreateJournalEntryAsync(Expense expense, CompanyReference company);
    Task<Result> PostExpenseMessageToChatterAsync(string companyNumber, int errorCount);
    Task<Result> PostPurchaseOrdersMessageToChatterAsync(string erp, int errorCount);
    Task<Result<IEnumerable<PurchaseOrderReceipt>>> GetPurchaseOrderReceiptsAsync();
    Task<Result<IEnumerable<PurchaseOrder>>> GetPurchaseOrdersAsync(IEnumerable<string> distinctPOs);
    Task<Result<IEnumerable<CompanyReference>>> GetCompanyReferencesAsync();
    Task<Result<PurchaseOrder>> SetVendorAddressAndMap(PurchaseOrder po);
    Task<Result<IEnumerable<PurchaseOrderLineItem>>> GetPurchaseOrdersLineItemAsync(IEnumerable<string> distinctPOs);
}
