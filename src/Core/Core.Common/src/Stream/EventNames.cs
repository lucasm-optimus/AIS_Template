namespace Tilray.Integrations.Core.Common.Stream;

public static class TOPICS
{
    public const string SalesOrderCreated = "salesordercreated";
    public const string EcomSalesOrderReceived = "EcomSalesOrderReceived";
}

public static class SUBSCRIPTIONS
{
    public const string CreateSalesOrderInRootStock = "CreateSalesOrderInRootStock";
    public const string CreateMedSalesOrderInRootStock = "EcomSalesOrderReceived_CreateSalesOrderInRootStock";
}
