namespace Optimus.Services.Vehicles.APIPlacas.Startup
{
    /// <summary>
    /// This class is responsible for reading the ServiceBus settings from the configuration file.
    /// </summary>
    public class APIPlacasSettings
    {
        public string ApiAddress { get; set; }
        public string Token { get; set; }
    }
}
