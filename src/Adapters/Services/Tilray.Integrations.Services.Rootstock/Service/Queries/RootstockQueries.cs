﻿
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


    internal const string GetCustomerBySfAccountQuery = @"SELECT rstk__socust_custno__c, name, id FROM rstk__socust__c WHERE rstk__socust_sf_account__c = '{0}'";

    internal const string GetShipToCustomerAddressInfoQuery = @"SELECT rstk__socaddr_custno__r.rstk__socust_custno__c, rstk__socaddr_name__c, rstk__externalid__c, rstk__socaddr_locationref__c FROM rstk__socaddr__c WHERE rstk__socaddr_custno__r.rstk__socust_custno__c = ':{0}' AND rstk__socaddr_defaultshipto__c = true";

    internal const string GetDefaultBillToCustomerAddressInfoQuery = @"SELECT rstk__socaddr_custno__r.rstk__socust_custno__c, rstk__socaddr_name__c, rstk__externalid__c, rstk__socaddr_locationref__c FROM rstk__socaddr__c WHERE rstk__socaddr_custno__r.rstk__socust_custno__c = ':{0}' AND rstk__socaddr_defaultbillto__c = true";

    internal const string GetAcknowledgementCustomerAddressInfoQuery = @"SELECT rstk__socaddr_custno__r.rstk__socust_custno__c, rstk__socaddr_name__c, rstk__externalid__c, rstk__socaddr_locationref__c FROM rstk__socaddr__c WHERE rstk__socaddr_custno__r.rstk__socust_custno__c = ':{0}' AND rstk__socaddr_defaultack__c = true";

    internal const string GetInstallationCustomerAddressInfoQuery = @"SELECT rstk__socaddr_custno__r.rstk__socust_custno__c, rstk__socaddr_name__c, rstk__externalid__c, rstk__socaddr_locationref__c FROM rstk__socaddr__c WHERE rstk__socaddr_custno__r.rstk__socust_custno__c = ':{0}' AND rstk__socaddr_defaultinstall__c = true";

    internal const string GetExternalIdByExtCustNo = @"SELECT ID, rstk__externalid__c, Name FROM rstk__socaddr__c WHERE External_Customer_Number__c = '{0}'";

    internal const string GetAllCustomerAddressByCustomerNo = @"SELECT ID FROM rstk__socaddr__c WHERE rstk__socaddr_custno__r.rstk__socust_custno__c = '{0}'";

    internal const string GetMaxAddressSequenceByCustomerNo = @"SELECT MAX(rstk__socaddr_seq__c) MaxSequenceNum FROM rstk__socaddr__c WHERE rstk__socaddr_custno__c = '{0}'";

    internal const string GetNewlyCreatedCustomerAddress = @"SELECT ID, rstk__externalid__c, Name, External_Customer_Number__c FROM rstk__socaddr__c WHERE ID = '{0}'";

    internal const string GetNewlyCreatedSalesOrderByRecordId = @"SELECT rstk__soapi_sohdr__r.rstk__sohdr_order__c, rstk__soapi_sohdr__r.rstk__externalid__c, rstk__soapi_sohdr__r.rstk__sohdr_custref__c, rstk__soapi_sohdr__c FROM rstk__soapi__c WHERE id = '{0}'";

    internal const string GetListofCustomerReferencesByCustomerReferences = @"SELECT rstk__sohdr_custref__c FROM rstk__sohdr__c WHERE rstk__sohdr_custref__c IN ('{0}')";

    internal const string GetIdByExternalReferenceId = @"select id from {0} where {1} = '{2}'"; // {0} = TableName, {1} = ColumnName, {2} = ExternalId

    internal const string GetSoHdrFromSoApi = @"SELECT  rstk__soapi_sohdr__c  FROM rstk__soapi__c WHERE id='{0}'";//soapi record id

    internal const string GetChatterGroupIdQuery = "SELECT Id FROM CollaborationGroup WHERE Name = '{0}'";

    internal const string GetChatterBodyQuery = "SELECT Body FROM FeedItem WHERE ParentId = '{0}'";
}
