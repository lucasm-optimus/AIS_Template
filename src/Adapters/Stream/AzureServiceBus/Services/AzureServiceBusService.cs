using Azure.Messaging.ServiceBus;
using Newtonsoft.Json;
using Tilray.Integrations.Core.Common.Stream;

namespace Tilray.Integrations.Stream.Bus.Services;

public class AzureServiceBusService(ServiceBusClient client) : IStream
{
    public async Task SendEventAsync<T>(T notification, string topicName)
    {
        var sender = client.CreateSender(topicName);
        var message = new ServiceBusMessage(JsonConvert.SerializeObject(notification));
        await sender.SendMessageAsync(message);
    }

    public async Task SendEventAsync(string notification, string topicName, Dictionary<string, object>? properties = null)
    {
        var sender = client.CreateSender(topicName);
        var message = new ServiceBusMessage(JsonConvert.SerializeObject(notification));

        if (properties != null)
        {
            foreach (var property in properties)
            {
                message.ApplicationProperties[property.Key] = property.Value;
            }
        }

        await sender.SendMessageAsync(message);
    }

    public async Task SendEventAsync<T>(T notification, string topicName, DateTime? scheduleMessage)
    {
        var sender = client.CreateSender(topicName);
        var message = new ServiceBusMessage(JsonConvert.SerializeObject(notification))
        {
            ScheduledEnqueueTime = scheduleMessage.HasValue ? scheduleMessage.Value.ToUniversalTime() : DateTime.UtcNow
        };
        await sender.SendMessageAsync(message);
    }
}
