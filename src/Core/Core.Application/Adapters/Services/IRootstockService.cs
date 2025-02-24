using Tilray.Integrations.Core.Application.Invoices.Models;
using Tilray.Integrations.Core.Domain.Aggregates.SalesOrders.Customer;
using Tilray.Integrations.Core.Domain.Aggregates.SalesOrders.Rootstock;

namespace Tilray.Integrations.Core.Application.Adapters.Services;

public interface IRootstockService
{
    Task<Result<IEnumerable<CompanyReference>>> GetAllCompanyReferencesAsync();
    Task<Result<IEnumerable<SalesOrder>>> ValidateSalesOrders(IEnumerable<SalesOrder> salesOrders);
    Task<Result> CreateSalesOrderAsync(SalesOrder salesOrder);
    Task<Result<string>> CreateCustomer(SalesOrderCustomer customer);
    Task<Result<RstkCustomerInfoResponse>> GetCustomerInfo(string sfAccountId);
    Task<Result<string>> CreateCustomerAddress(SalesOrderCustomerAddress salesOrderCustomerAddress, string customerId, int customerNextAddressSequence, string customerAccountNumber);
    Task<int?> GetCustomerAddressNextSequence(string customerNo);
    Task<Result<RstkCustomerAddressInfoResponse>> GetCustomerAddressInfo(string customerNo, string address, string city, string state, string zip);
    Task<Result<string>> CreateSalesOrder(MedSalesOrder salesOrder);
    Task<Result<string>> CreateSalesOrderLineItem(SalesOrderLineItem salesOrderLineItem, string soHdrId);
    Task<Result<string>> CreatePrePayment(SalesOrderPrepayment soPrepayment, string divisionId, string paymentAccountId);
    Task<Result<string>> CreatePrePayment(CCPrepayment prePayment);
    Task<bool> SalesOrderExists(string customerReferenceNumber);
    Task<Result<string>> GetIdFromExternalColumnReference(string objectName, string externalIdColumnName, string externalIdValue);
    Task<Result<string>> GetSoHdr(string soapiId);
    Task<Result<List<ExpenseError>>> CreateJournalEntryAsync(Expense expense, CompanyReference company);
    Task<Result> PostExpenseMessageToChatterAsync(string companyNumber, int errorCount);
    Task<Result<string>> CreateSyData(POAPLineItem data);
    Task<Result> UpdateSyDataProcess(string syDataId, string processName);
    Task<Result<RootstockPODetail>> GetPODetails(string poName);
    Task<Result> CreateSyDataDetail(List<object> rootstockSyDataDetails);
    Task<Result> CreateApato(APATOLineItem apatoLine, string glAccountId, string companyId, string vendorId);
    Task<Result> PostInvoicesErrorGeneratedToChatterAsync(string message, string companyNumber);
}
