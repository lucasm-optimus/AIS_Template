using Azure.Messaging.ServiceBus;
using System.Text;
using Tilray.Integrations.Core.Application.Rootstock.Stream;
using Tilray.Integrations.Stream.Rootstock.Startup;

namespace Tilray.Integrations.Stream.Rootstock.ServiceBus
{
    public class RootstockStream(RootstockServiceBusSettings serviceBusSettings) : IRootstockStream
    {
        private readonly ServiceBusClient serviceBusClient = new(serviceBusSettings.ConnestionString);

        public async Task SendMessageAsync(string queue, string message)
        {
            ServiceBusSender serviceBusSender = serviceBusClient.CreateSender(queue);
            var serviceBusMessage = new ServiceBusMessage(Encoding.UTF8.GetBytes(message));
            await serviceBusSender.SendMessageAsync(serviceBusMessage);
        }

        public async Task SendMessageAsync(string queue, IEnumerable<string> messages)
        {
            ServiceBusSender serviceBusSender = serviceBusClient.CreateSender(queue);
            var serviceBusMessages = messages.Select(message => new ServiceBusMessage(Encoding.UTF8.GetBytes(message)));
            await serviceBusSender.SendMessagesAsync(serviceBusMessages);
        }
    }
}
