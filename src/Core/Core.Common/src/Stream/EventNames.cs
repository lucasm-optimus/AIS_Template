namespace Tilray.Integrations.Core.Common.Stream;

public static class Topics
{
    public const string SalesOrderCreated = "salesordercreated";
    public const string EcomSalesOrderReceived = "EcomSalesOrderReceived";
}

public static class Subscriptions
{
    public const string CreateSalesOrderInRootStock = "CreateSalesOrderInRootStock";
    public const string CreateMedSalesOrderInRootStock = "EcomSalesOrderReceived_CreateSalesOrderInRootStock";
}
