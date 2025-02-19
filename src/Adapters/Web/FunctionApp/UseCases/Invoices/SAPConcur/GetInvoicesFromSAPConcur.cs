using Tilray.Integrations.Core.Application.Constants;

namespace Tilray.Integrations.Functions.UseCases.Invoices.SAPConcur;

public class GetInvoicesFromSAPConcur(IMediator mediator)
{
    /// <summary>
    /// This function is responsible for fetching invoices from SAP Concur.
    /// </summary>
    [Function("GetInvoicesFromSAPConcur")]
    [ServiceBusOutput(Topics.SAPConcurInvoicesFetched, Connection = "ServiceBusConnectionString")]
    public async Task<IEnumerable<string>> Run([TimerTrigger("%GetInvoicesFromSAPConcurCRON%")] TimerInfo myTimer)
    {
        var result = await mediator.Send(new GetAllInvoices());
        if (result.IsSuccess)
        {
            return result.Value;
        }

        return [];
    }
}
