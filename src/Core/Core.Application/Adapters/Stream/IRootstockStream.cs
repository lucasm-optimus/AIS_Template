namespace Tilray.Integrations.Core.Application.Adapters.Stream;

public interface IRootstockStream
{
    Task SendMessageAsync(string queue, string message);
    Task SendMessageAsync(string queue, IEnumerable<string> messages);
}
