namespace Tilray.Integrations.Functions.UseCases.SalesOrdersPayments.AuthorizeNet;

public class RootstockSalesOrderPaymentFetched_CapturePaymentInAuthorizeNet(IMediator mediator)
{
    [Function(nameof(RootstockSalesOrderPaymentFetched_CapturePaymentInAuthorizeNet))]
    public async Task Run(
        [ServiceBusTrigger("mytopic", "mysubscription", Connection = "ServiceBusConnectionString")]
        ServiceBusReceivedMessage message,
        ServiceBusMessageActions messageActions)
    {
    }
}
