namespace Tilray.Integrations.Functions.UseCases.Expenses.SAPConcur;

public class GetExpensesFromSAPConcur(ILogger<GetExpensesFromSAPConcur> logger, IMediator mediator)
{
    /// <summary>
    /// This function is responsible for fetching expenses from SAP Concur.
    /// </summary>
    [Function("GetExpensesFromSAPConcur")]
    [ServiceBusOutput(Topics.SAPConcurExpensesFetched, Connection = "ServiceBusConnectionString")]
    public async Task<IEnumerable<string>> Run([TimerTrigger("%GetExpensesFromSAPConcurCRON%", RunOnStartup = true)] TimerInfo myTimer)
    {
        var result = await mediator.Send(new GetAllExpenses());
        if (result.IsSuccess)
        {
            return result.Value;
        }

        return [];
    }
}
