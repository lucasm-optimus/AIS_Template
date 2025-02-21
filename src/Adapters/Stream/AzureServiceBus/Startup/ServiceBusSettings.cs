namespace Tilray.Integrations.Stream.Bus.Startup;

/// <summary>
/// This class is responsible for reading the Service Bus settings from the configuration file.
/// </summary>
public class ServiceBusSettings
{
    public string ConnectionString { get; set; }
}
