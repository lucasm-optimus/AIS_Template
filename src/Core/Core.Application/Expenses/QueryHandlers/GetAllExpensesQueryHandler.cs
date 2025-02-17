namespace Tilray.Integrations.Core.Application.Expenses.QueryHandlers;

public class GetAllExpensesQueryHandler(ISAPConcurService sapConcurService, IBlobService blobService) : IQueryManyHandler<GetAllExpenses, string>
{
    public async Task<Result<IEnumerable<string>>> Handle(GetAllExpenses request, CancellationToken cancellationToken)
    {
        var expensesResult = await sapConcurService.GetExpenseFilesAsync();
        if (expensesResult.IsFailed)
            return Result.Fail<IEnumerable<string>>(expensesResult.Errors);

        var blobList = await Task.WhenAll(expensesResult.Value.Select(expense =>
            blobService.UploadBlobContentAsync(expense, "expense")));

        return Result.Ok(blobList.AsEnumerable());
    }
}
