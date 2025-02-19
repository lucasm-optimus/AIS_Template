using Tilray.Integrations.Core.Domain.Aggregates.Invoices;

namespace Tilray.Integrations.Core.Domain.Aggregates.Expenses.Events;

public class ExpensesProcessed : IDomainEvent
{
    public List<ExpenseError> ExpenseErrors { get; } = [];
    public CompanyReference CompanyReference { get; set; }
    public bool HasErrors => ExpenseErrors.Count > 0;
    public string Message => HasErrors
        ? $"Processing failed with {ExpenseErrors.Count} expenses errors"
        : "Processing succeeded.";
}

public class ExpenseError
{
    public string CompanyNumber { get; set; }
    public string CompanyName { get; set; }
    public DateTime? JournalDate { get; set; }
    public string EntryHeaderDescription { get; set; }
    public string EntryLineDescription { get; set; }
    public string AccountNumber { get; set; }
    public decimal DebitAmount { get; set; }
    public decimal CreditAmount { get; set; }
    public string UploadGroup { get; set; }
    public string ProcessType { get; set; }
    public string Error { get; set; }

    public static ExpenseError Create(
        string companyNumber, string companyName, DateTime? journalDate, string entryHeaderDescription,
        string entryLineDescription, string accountNumber, decimal debitAmount, decimal creditAmount,
        string uploadGroup, string error)
    {
        return new ExpenseError
        {
            CompanyNumber = companyNumber,
            CompanyName = companyName,
            JournalDate = journalDate,
            EntryHeaderDescription = entryHeaderDescription,
            EntryLineDescription = entryLineDescription,
            AccountNumber = accountNumber,
            DebitAmount = debitAmount,
            CreditAmount = creditAmount,
            UploadGroup = uploadGroup,
            ProcessType = "Expenses",
            Error = error
        };
    }
}
