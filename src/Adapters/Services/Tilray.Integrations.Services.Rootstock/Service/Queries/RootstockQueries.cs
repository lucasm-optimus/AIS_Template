namespace Tilray.Integrations.Services.Rootstock.Service.Queries;

internal static class RootstockQueries
{
    internal const string GetAllCompanyReferenceQuery = @"
            SELECT ID, Company_Name__c, Concur_Company__c, Rootstock_Company__c,
            Expenses__c, Non_PO_Invoices__c, PO_AP_Match_Invoices__c, OBeer_Invoices__c
            FROM External_Company_Reference__c";

    internal const string GetCustomerByIdQuery = @"
            SELECT rstk__socust_custno__c, name
            FROM rstk__socust__c
            WHERE rstk__socust_custno__c = '{0}'";

    internal const string GetUploadGroupByIdQuery = @"
            SELECT ID
            FROM rstk__soapi__c
            WHERE rstk__soapi_upgroup__c = '{0}'
              AND rstk__soapi_processed__c = false
            LIMIT 1";

    internal const string GetItemByIdQuery = @"
            SELECT ID, rstk__externalid__c, rstk__peitem_div__r.rstk__externalid__c, rstk__peitem_item__c, 
            rstk__peitem_descr__c, Case_Quantity__c, rstk__peitem_tracklot_pl__c
            FROM rstk__peitem__c
            WHERE rstk__externalid__c = '{0}'";

    internal const string GetSalesOrderByIdQuery = @"
            SELECT rstk__soapi_sohdr__c
            FROM rstk__soapi__c
            WHERE id = '{0}'";
}
