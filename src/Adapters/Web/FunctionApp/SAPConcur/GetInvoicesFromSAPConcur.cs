namespace Tilray.Integrations.Functions.SAPConcur;

public class GetInvoicesFromSAPConcur(ILogger<GetInvoicesFromSAPConcur> logger, IMediator mediator)
{
    /// <summary>
    /// This function is responsible for fetching invoices from SAP Concur.
    /// </summary>
    [Function("GetInvoicesFromSAPConcur")]
    [ServiceBusOutput("%TopicSAPConcurInvoicesFetched%", Connection = "ServiceBusConnectionString")]
    public async Task<InvoiceGroup> Run([TimerTrigger("%GetInvoicesFromSAPConcurCRON%")] TimerInfo myTimer)
    {
        var result = await mediator.Send(new GetAllInvoices());
        if (result.IsSuccess)
        {
            foreach (var invoice in result.Value)
            {
                return invoice;
            }
        }

        return null;
    }
}
