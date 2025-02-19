namespace Tilray.Integrations.Services.Rootstock.Startup;

/// <summary>
/// This class is responsible for reading the Rootstock settings from the configuration file.
/// </summary>
public class RootstockSettings
{
    public string? BaseUrl { get; set; }
    public string? ClientId { get; set; }
    public string? ClientSecret { get; set; }
}
