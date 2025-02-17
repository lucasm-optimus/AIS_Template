namespace Tilray.Integrations.Core.Application.Expenses.CommandHandlers;

public class CreateJournalEntriesInRootstockCommandHandler(IRootstockService rootstockService, IBlobService blobService,
    ILogger<CreateJournalEntriesInRootstockCommandHandler> logger, IMediator mediator) : ICommandHandler<CreateJournalEntriesInRootstockCommand, ExpensesProcessed>
{
    public async Task<Result<ExpensesProcessed>> Handle(CreateJournalEntriesInRootstockCommand request, CancellationToken cancellationToken)
    {
        string expensesContent = await blobService.DownloadBlobContentAsync(request.ExpenseDetailsBlobName);
        var expenseDetails = expensesContent.ToObject<ExpenseDetails>();

        var result = await rootstockService.CreateJournalEntryAsync(expenseDetails.Expenses);
        if (result.IsFailed)
            return Result.Fail(result.Errors);

        var expensesProcessed = result.Value;
        if (expensesProcessed.HasErrors)
        {
            logger.LogWarning("Expenses processing completed with {ErrorCount} errors", expensesProcessed.ExpenseErrors.Count);
            await mediator.Publish(expensesProcessed, cancellationToken);
            return Result.Fail(expensesProcessed.Message);
        }

        logger.LogInformation("Successfully processed {ExpensesCount} expenses", expenseDetails.Expenses.Count());
        return Result.Ok(expensesProcessed);
    }
}
