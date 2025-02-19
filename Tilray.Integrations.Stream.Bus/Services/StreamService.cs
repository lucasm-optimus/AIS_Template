using Azure.Messaging.ServiceBus;
using Newtonsoft.Json;
using Tilray.Integrations.Core.Common.Stream;

namespace Tilray.Integrations.Stream.Bus.Services
{
    public class AzureServiceBusService(ServiceBusClient client) : IStream
    {
        public async Task SendEventAsync<T>(T notification, string queueName)
        {
            var sender = client.CreateSender(queueName);
            var message = new ServiceBusMessage(JsonConvert.SerializeObject(notification));
            await sender.SendMessageAsync(message);
        }

        public async Task SendEventAsync<T>(T notification, string queueName, DateTime? scheduleMessage)
        {
            var sender = client.CreateSender(queueName);
            var message = new ServiceBusMessage(JsonConvert.SerializeObject(notification));
            message.ScheduledEnqueueTime = scheduleMessage.Value.ToUniversalTime();
            await sender.SendMessageAsync(message);
        }
    }
}
