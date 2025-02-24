namespace Tilray.Integrations.Functions.UseCases.Invoices.SAPConcur;

public class GetInvoicesFromSAPConcur(IMediator mediator)
{
    /// <summary>
    /// This function is responsible for fetching invoices from SAP Concur.
    /// </summary>
    //[Function("GetInvoicesFromSAPConcur")]
    public async Task Run([TimerTrigger("%GetInvoicesFromSAPConcurCRON%")] TimerInfo myTimer)
    {
        await mediator.Send(new GetAllInvoices());
    }
}
