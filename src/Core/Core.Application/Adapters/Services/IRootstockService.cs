using Tilray.Integrations.Core.Domain.Aggregates.Expenses;
using Tilray.Integrations.Core.Domain.Aggregates.Expenses.Events;
using Tilray.Integrations.Core.Domain.Aggregates.Sales.Rootstock;
using Tilray.Integrations.Core.Domain.Aggregates.SalesOrders.Rootstock;

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
    Task<Result<ExpensesProcessed>> CreateJournalEntryAsync(IEnumerable<Expense> expenses);
    Task<Result> PostExpensesChatterMessageAsync(string companyNumber, int errorCount);
}
