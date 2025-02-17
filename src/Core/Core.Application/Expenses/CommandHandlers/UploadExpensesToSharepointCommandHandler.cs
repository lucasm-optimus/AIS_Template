namespace Tilray.Integrations.Core.Application.Expenses.CommandHandlers;

public class UploadExpensesToSharepointCommandHandler(ISharepointService sharepointService, IRootstockService rootstockService,
    IBlobService blobService, ILogger<UploadExpensesToSharepointCommandHandler> logger) : ICommandHandler<UploadExpensesToSharepointCommand>
{
    public async Task<Result> Handle(UploadExpensesToSharepointCommand request, CancellationToken cancellationToken)
    {
        string expensesContent = await blobService.DownloadBlobContentAsync(request.ExpenseDetailsBlobName);
        var expenseDetails = expensesContent.ToObject<ExpenseDetails>();

        if (expenseDetails.Expenses?.Any() != true)
        {
            logger.LogInformation("No expenses to upload to Sharepoint.");
            return Result.Ok();
        }

        await sharepointService.UploadExpensesAsync(expenseDetails.Expenses, expenseDetails.StopTime);

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

                if (expenses.Count > 0)
                {
                    await sharepointService.UploadExpensesAsync(expenses, expenseDetails.StopTime, companyReference, expenseType);
                }
            }
        }

        return Result.Ok();
    }
}
