using Tilray.Integrations.Core.Domain.Aggregates.Expenses.Events;

namespace Tilray.Integrations.Core.Domain.Aggregates.Expenses.Commands;

public class CreateJournalEntriesInRootstockCommand(string expenseDetailsBlobName) : ICommand<ExpensesProcessed>
{
    public string ExpenseDetailsBlobName { get; set; } = expenseDetailsBlobName;
}
