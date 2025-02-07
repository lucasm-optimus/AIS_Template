namespace Tilray.Integrations.Services.OBeer.Startup;

/// <summary>
/// This class is responsible for reading the Obeer settings from the configuration file.
/// </summary>
public class ObeerSettings
{
    public string BaseUrl { get; set; }
    public string APICommand { get; set; }
    public string EncompassId { get; set; }
    public string APIToken { get; set; }
}
