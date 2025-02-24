namespace Tilray.Integrations.Functions.UseCases.AuditItems.Rootstock;

public class GetAuditDataFromRootstock(IMediator mediator)
{
    /// <summary>
    /// This function is responsible for fetching list of updated audit items from rootstock using SOQL query.
    /// </summary>
    [Function("GetAuditItemsFromRootstock")]
    [ServiceBusOutput(Topics.RootstockAuditItemsFetched, Connection = "ServiceBusConnectionString")]
    public async Task<AuditItemAgg> Run([TimerTrigger("%GetAuditItemsFromRootstockCRON%")] TimerInfo myTimer)
    {
        var result = await mediator.Send(new GetAuditData());
        if (result.IsSuccess)
        {
            return result.Value;
        }

        return new();
    }
}
