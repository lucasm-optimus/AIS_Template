namespace Tilray.Integrations.Core.Application.Expenses.CommandHandlers;

public class CreateJournalEntriesInRootstockCommandHandler(IRootstockService rootstockService, IBlobService blobService,
    ILogger<CreateJournalEntriesInRootstockCommandHandler> logger, IMediator mediator) : ICommandHandler<CreateJournalEntriesInRootstockCommand, ExpensesProcessed>
{
    public async Task<Result<ExpensesProcessed>> Handle(CreateJournalEntriesInRootstockCommand request, CancellationToken cancellationToken)
    {
        string expensesContent = await blobService.DownloadBlobContentAsync(request.ExpenseDetailsBlobName);
        var expenseDetails = expensesContent.ToObject<ExpenseDetails>();
        logger.LogInformation("Total expenses received: {TotalExpenses}", expenseDetails.Expenses.Count());

    var expensesProcessed = new ExpensesProcessed();
        var companyResult = await rootstockService.GetAllCompanyReferencesAsync();
        if (companyResult.IsFailed)
            return Result.Fail<ExpensesProcessed>(companyResult.Errors);

        foreach (var company in companyResult.Value)
        {
            expensesProcessed.CompanyReference = company;
            var companyExpenses = expenseDetails.Expenses
                .Where(e => e.CleanCompanyCode == company.Concur_Company__c)
                .ToList();

            if (!company.CanProcessExpenses)
            {
                logger.LogWarning(
                    "Company {CompanyReference} cannot process expenses. Conditions not met: Rootstock_Company__c = {RootstockCompany}, Expenses__c = {ExpensesFlag}. Skipping.",
                    company.Company_Name__c, company.Rootstock_Company__c, company.Expenses__c);
                continue;
            }

            if (companyExpenses.Count == 0)
            {
                logger.LogInformation("No expenses found for Company {CompanyReference} with ConcurCompanyCode {CompanyCode}.", company.Company_Name__c, company.Concur_Company__c);
                continue;
            }

            logger.LogInformation("Processing {ExpenseCount} expenses for Company {CompanyName}.", companyExpenses.Count, company.Company_Name__c);
            foreach (var expense in companyExpenses)
            {
                logger.LogInformation("Creating journal entry for expense with ReportId {ReportId} and Description {Description}", expense.ReportID, expense.ReportDescription);
                var result = await rootstockService.CreateJournalEntryAsync(expense, company);
                if (result.Value.Count != 0)
                {
                    expensesProcessed.ExpenseErrors.AddRange(result.Value);
                }
            }
        }

        if (expensesProcessed.HasErrors)
        {
            logger.LogWarning("Expenses processing completed with {ErrorCount} errors", expensesProcessed.ExpenseErrors.Count);
            await mediator.Publish(expensesProcessed, cancellationToken);
            return Result.Fail(expensesProcessed.ExpenseErrors.Select(e => e.Error));
        }

        logger.LogInformation("Successfully processed {ExpensesCount} expenses", expenseDetails.Expenses.Count());
        return Result.Ok(expensesProcessed);
    }
}
