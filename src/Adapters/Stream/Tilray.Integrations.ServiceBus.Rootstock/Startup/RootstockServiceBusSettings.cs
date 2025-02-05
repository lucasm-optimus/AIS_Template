namespace Tilray.Integrations.Stream.Rootstock.Startup
{
    /// <summary>
    /// This class is responsible for reading the Rootstock settings from the configuration file.
    /// </summary>
    public class RootstockServiceBusSettings
    {
        public string? ConnestionString { get; set; }
        public static string? TopicSAPSalesOrderCreated = "SAPSalesOrderCreated";
        public static string? SubscriptionCreateSalesOrderInRootstock = "CreateSalesOrderInRootstock";
    }
}
