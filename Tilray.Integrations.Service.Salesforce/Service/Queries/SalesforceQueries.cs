
namespace Tilray.Integrations.Services.Salesforce.Service.Queries
{
    public static class SalesforceQueries
    {
        public static string GetSOXReportQuery(string reportDate)
        {
            //return $@"
            //    SELECT Action, CreatedBy.Username, CreatedDate, DelegateUser, Display, Id, ResponsibleNamespacePrefix, Section
            //    FROM SetupAuditTrail
            //    WHERE Action NOT IN ('addeduserpackagelicense', 'granteduserpackagelicense', 'removeduserpackagelicense', 'revokeduserpackagelicense', 'activateduser', 'changedApproverRequestEmails', 'changedcommunitynickname', 'changedDelegateApprover', 'changedemail', 'changedfederationid', 'changedinteractionuseroffon', 'changedliveagentuseronoff', 'changedManager', 'changedpassword', 'changedprofileforuser', 'changedprofileforusercusttostd', 'changedroleforuser', 'changedroleforuserfromnone', 'changedroleforusertonone', 'changedsupportuseroffon', 'changedsupportuseronoff', 'changedUserEmailVerifiedStatusUnverified', 'changedUserEmailVerifiedStatusVerified', 'changedusername', 'createdrole', 'createduser', 'deactivateduser', 'frozeuser', 'PermSetAssign', 'PermSetDisableUserPerm', 'PermSetEnableUserPerm', 'PermSetUnassign', 'registeredUserPhoneNumber', 'resetpassword', 'suNetworkAdminLogin', 'suNetworkAdminLogout', 'suOrgAdminLogin', 'suOrgAdminLogout', 'unlockeduser', 'unregisterdUserPhoneNumber', 'useremailchangesent', 'groupMembership', 'queueMembership', 'createdcustomersuccessuser', 'value_PROV_SCRATCH_DAILY_LIMIT', 'value_PROV_SCRATCH_ACTIVE_LIMIT', 'value_MAX_STREAMING_TOPICS_PROV')
            //    AND DAY_ONLY(convertTimezone(CreatedDate)) = {reportDate}
            //    ORDER BY CreatedDate";
            return $@"
                SELECT Action, CreatedBy.Username, CreatedDate, DelegateUser, Display, Id, ResponsibleNamespacePrefix, Section
                FROM SetupAuditTrail
                ORDER BY CreatedDate";
        }

    }
}
