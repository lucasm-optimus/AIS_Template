//using System;
//using System.Threading.Tasks;
//using Azure.Messaging.ServiceBus;
//using Microsoft.Azure.Functions.Worker;
//using Microsoft.Extensions.Logging;

//namespace Optimus.FunctionApp.UseCases
//{
//    public class ServiceBus(ILogger<ServiceBus> logger)
//    {
//        [Function(nameof(ServiceBus))]
//        public async Task Run(
//            [ServiceBusTrigger("SendDataToSomewhere", "mysubscription", Connection = "IntegrationSBConnection")]
//            ServiceBusReceivedMessage message,
//            ServiceBusMessageActions messageActions)
//        {
//            logger.LogInformation("Message ID: {id}", message.MessageId);
//            logger.LogInformation("Message Body: {body}", message.Body);
//            logger.LogInformation("Message Content-Type: {contentType}", message.ContentType);

//             // Complete the message
//            await messageActions.CompleteMessageAsync(message);
//        }
//    }
//}
