namespace Tilray.Integrations.Core.Domain.Aggregates.Expenses.Commands;

public class UploadExpensesToSharepointCommand(string expenseDetailsBlobName) : ICommand
{
    public string ExpenseDetailsBlobName { get; set; } = expenseDetailsBlobName;
}
