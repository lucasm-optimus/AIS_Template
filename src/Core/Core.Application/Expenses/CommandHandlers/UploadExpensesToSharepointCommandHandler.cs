namespace Tilray.Integrations.Core.Application.Expenses.CommandHandlers;

public class UploadExpensesToSharepointCommandHandler(ISharepointService sharepointService, IRootstockService rootstockService,
    IBlobService blobService, ILogger<UploadExpensesToSharepointCommandHandler> logger) : ICommandHandler<UploadExpensesToSharepointCommand>
{
    public async Task<Result> Handle(UploadExpensesToSharepointCommand request, CancellationToken cancellationToken)
    {
        string expensesContent = await blobService.DownloadBlobContentAsync(request.ExpenseDetailsBlobName);
        var expenseDetails = expensesContent.ToObject<ExpenseDetails>();
        logger.LogInformation("Total {TotalExpenses} Expenses received , StopTime: {StopTime}", expenseDetails.Expenses.Count(), expenseDetails.StopTime);

        if (expenseDetails.Expenses?.Any() != true)
        {
            logger.LogInformation("No expenses to upload to Sharepoint.");
            return Result.Ok();
        }

        var result = Result.Ok();
        logger.LogInformation("Uploading all expenses to SharePoint.");
        var uploadAllResult = await sharepointService.UploadExpensesAsync(expenseDetails.Expenses, expenseDetails.StopTime);
        if (uploadAllResult.IsFailed)
            result.WithErrors(uploadAllResult.Errors);

        var companyReferencesResult = await rootstockService.GetAllCompanyReferencesAsync();
        if (companyReferencesResult.IsFailed)
            return Result.Fail(companyReferencesResult.Errors);

        foreach (var companyReference in companyReferencesResult.Value)
        {
            foreach (var expenseType in Enum.GetValues<ExpenseType>())
            {
                var expenses = expenseDetails.Expenses
                    .Where(e => e.CleanCompanyCode == companyReference.Concur_Company__c &&
                               (expenseType == ExpenseType.Cash ? e.IsCashExpense : e.IsCompanyExpense))
                    .ToList();

                if (expenses.Count == 0)
                {
                    logger.LogInformation("No {ExpenseType} expenses for Company {CompanyName} with ConcurCompanyCode ({CompanyCode}).",
                        expenseType, companyReference.Company_Name__c, companyReference.Concur_Company__c);
                    continue;
                }

                logger.LogInformation("Uploading {ExpensesCount} {ExpenseType} expenses for Company {CompanyName} with ConcurCompanyCode ({CompanyCode}).",
                    expenses.Count, expenseType, companyReference.Company_Name__c, companyReference.Concur_Company__c);
                var uploadResult = await sharepointService.UploadExpensesAsync(expenses, expenseDetails.StopTime, companyReference, expenseType);
                if (uploadResult.IsFailed)
                    result.WithErrors(uploadResult.Errors);
            }
        }

        return result;
    }
}
