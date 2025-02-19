using Tilray.Integrations.Core.Domain.Aggregates.Expenses;

namespace Tilray.Integrations.Core.Application.Adapters.Services;

public interface ISAPConcurService
{
    Task<Result<IEnumerable<ExpenseDetails>>> GetExpenseFilesAsync();
    Task<Result<IEnumerable<Invoice>>> GetInvoicesAsync();
}
