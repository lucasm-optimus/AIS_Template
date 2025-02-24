using Tilray.Integrations.Core.Application.Constants;

namespace Tilray.Integrations.Functions.UseCases.Expenses.SAPConcur;

public class GetExpensesFromSAPConcur(IMediator mediator)
{
    /// <summary>
    /// This function is responsible for fetching expenses from SAP Concur.
    /// </summary>
    //[Function("GetExpensesFromSAPConcur")]
    [ServiceBusOutput(Topics.SAPConcurExpensesFetched, Connection = "ServiceBusConnectionString")]
    public async Task<IEnumerable<string>> Run([TimerTrigger("%GetExpensesFromSAPConcurCRON%")] TimerInfo myTimer)
    {
        var result = await mediator.Send(new GetAllExpenses());
        if (result.IsSuccess)
        {
            return result.Value;
        }

        return [];
    }
}
