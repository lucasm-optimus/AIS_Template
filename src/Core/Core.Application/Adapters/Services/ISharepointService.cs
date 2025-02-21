using Tilray.Integrations.Core.Domain.Aggregates.Expenses;
using Tilray.Integrations.Core.Domain.Aggregates.SOXReport;

namespace Tilray.Integrations.Core.Application.Adapters.Services;

public interface ISharepointService
{
    Task<Result> UploadExpensesAsync(IEnumerable<Expense> expenses, string stopTime, CompanyReference companyReference = null,
        ExpenseType? expenseType = null);
    Task<Result> UploadInvoicesAsync(IEnumerable<Invoice> invoices, CompanyReference companyReference);
    Task<Result> UploadFileAsync<T>(IEnumerable<T> content, CompanyReference companyReference = null, string uploadPath = null,
        string[] ignoredProperties = null);

    Task<Result> UploadSOXReportAsync(IEnumerable<AuditItem> auditItems, CompanyReference companyReference = null);

    Task<Result> UploadSFQueryAsync(string query, CompanyReference companyReference = null);
}
