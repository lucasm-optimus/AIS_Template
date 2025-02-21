namespace Tilray.Integrations.Core.Common.Stream;

public interface IStream
{
    Task SendEventAsync<T>(T notification, string topicName, Dictionary<string, object>? properties = null);
    Task SendEventAsync<T>(T notification, string topicName, DateTime? scheduleMessage);
}
