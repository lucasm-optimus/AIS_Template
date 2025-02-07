namespace Tilray.Integrations.Functions.UseCases.Invoices.SAPConcur;

public class GetInvoicesFromSAPConcur(ILogger<GetInvoicesFromSAPConcur> logger, IMediator mediator)
{
    /// <summary>
    /// This function is responsible for fetching invoices from SAP Concur.
    /// </summary>
    [Function("GetInvoicesFromSAPConcur")]
    [ServiceBusOutput("%TopicSAPConcurInvoicesFetched%", Connection = "ServiceBusConnectionString")]
    public async Task<IEnumerable<string>> Run([TimerTrigger("%GetInvoicesFromSAPConcurCRON%", RunOnStartup = true)] TimerInfo myTimer)
    {
        var result = await mediator.Send(new GetAllInvoices());
        if (result.IsSuccess)
        {
            return result.Value;
        }

        return null;
    }
}
