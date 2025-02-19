namespace Tilray.Integrations.Core.Application.Constants;

public static class Topics
{
    public const string SalesOrderCreated = "salesordercreated";
    public const string EcomSalesOrderReceived = "EcomSalesOrderReceived";
    public const string SAPConcurExpensesFetched = "sapconcurexpensesfetched";
    public const string SAPConcurInvoicesFetched = "sapconcurinvoicesfetched";
}

public static class Subscriptions
{
    public const string CreateSalesOrderInRootStock = "CreateSalesOrderInRootStock";
    public const string CreateMedSalesOrderInRootStock = "EcomSalesOrderReceived_CreateSalesOrderInRootStock";
    public const string CreateJournalEntriesInRootstock = "CreateJournalEntriesInRootstock";
    public const string UploadExpensesToSharepoint = "UploadExpensesToSharepoint";
    public const string UploadInvoicesToSharepoint = "UploadInvoicesToSharepoint";
    public const string CreateInvoicesInObeer = "CreateInvoicesInObeer";
}
