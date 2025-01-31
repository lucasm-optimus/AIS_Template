namespace Tilray.Integrations.Services.Rootstock.Service.Queries
{
    internal static class RootstockQueries
    {
        internal const string GetAllCompanyReferenceQuery = @"SELECT ID, Company_Name__c, Concur_Company__c, Rootstock_Company__c,
            Expenses__c, Non_PO_Invoices__c, PO_AP_Match_Invoices__c, OBeer_Invoices__c
            FROM External_Company_Reference__c";
    }
}
