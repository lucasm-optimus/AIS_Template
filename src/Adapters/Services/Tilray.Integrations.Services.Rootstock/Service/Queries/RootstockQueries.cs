using System.Collections.Generic;

namespace Tilray.Integrations.Services.Rootstock.Service.Queries
{
    internal static class RootstockQueries
    {
        internal const string GetAllCompanyReferenceQuery = @"SELECT ID, Company_Name__c, Concur_Company__c, Rootstock_Company__c, Expenses__c, Non_PO_Invoices__c, PO_AP_Match_Invoices__c, OBeer_Invoices__c FROM External_Company_Reference__c";

        internal const string GetCustomerBySfAccountQuery = @"SELECT rstk__socust_custno__c, name FROM rstk__socust__c WHERE rstk__socust_sf_account__c = '{0}'";

        internal const string GetShipToCustomerAddressInfoQuery = @"SELECT rstk__socaddr_custno__r.rstk__socust_custno__c, rstk__socaddr_name__c, rstk__externalid__c, rstk__socaddr_locationref__c FROM rstk__socaddr__c WHERE rstk__socaddr_custno__r.rstk__socust_custno__c = ':{0}' AND rstk__socaddr_defaultshipto__c = true";

        internal const string GetDefaultBillToCustomerAddressInfoQuery = @"SELECT rstk__socaddr_custno__r.rstk__socust_custno__c, rstk__socaddr_name__c, rstk__externalid__c, rstk__socaddr_locationref__c FROM rstk__socaddr__c WHERE rstk__socaddr_custno__r.rstk__socust_custno__c = ':{0}' AND rstk__socaddr_defaultbillto__c = true";

        internal const string GetAcknowledgementCustomerAddressInfoQuery = @"SELECT rstk__socaddr_custno__r.rstk__socust_custno__c, rstk__socaddr_name__c, rstk__externalid__c, rstk__socaddr_locationref__c FROM rstk__socaddr__c WHERE rstk__socaddr_custno__r.rstk__socust_custno__c = ':{0}' AND rstk__socaddr_defaultack__c = true";

        internal const string GetInstallationCustomerAddressInfoQuery = @"SELECT rstk__socaddr_custno__r.rstk__socust_custno__c, rstk__socaddr_name__c, rstk__externalid__c, rstk__socaddr_locationref__c FROM rstk__socaddr__c WHERE rstk__socaddr_custno__r.rstk__socust_custno__c = ':{0}' AND rstk__socaddr_defaultinstall__c = true";

        internal const string GetExternalIdByExtCustNo = @"SELECT ID, rstk__externalid__c, Name FROM rstk__socaddr__c WHERE External_Customer_Number__c = '{0}'";

        internal const string GetAllCustomerAddressByCustomerNo = @"SELECT ID FROM rstk__socaddr__c WHERE rstk__socaddr_custno__r.rstk__socust_custno__c = '{0}'";

        internal const string GetMaxAddressSequenceByCustomerNo = @"SELECT MAX(rstk__socaddr_seq__c) MaxSequenceNum FROM rstk__socaddr__c WHERE rstk__socaddr_custno__r.rstk__socust_custno__c = '{0}'";

        internal const string GetNewlyCreatedCustomerAddress = @"SELECT ID, rstk__externalid__c, Name, External_Customer_Number__c FROM rstk__socaddr__c WHERE ID = '{0}'";

        internal const string GetNewlyCreatedSalesOrderByRecordId = @"SELECT rstk__soapi_sohdr__r.rstk__sohdr_order__c, rstk__soapi_sohdr__r.rstk__externalid__c, rstk__soapi_sohdr__r.rstk__sohdr_custref__c, rstk__soapi_sohdr__c FROM rstk__soapi__c WHERE id = '{0}'";

        internal const string GetListofCustomerReferencesByCustomerReferences = @"SELECT rstk__sohdr_custref__c FROM rstk__sohdr__c WHERE rstk__sohdr_custref__c IN ('{0}')";
    }
}
