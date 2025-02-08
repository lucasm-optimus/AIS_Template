using System;
using System.Collections.Generic;
using Tilray.Integrations.Core.Domain.Aggregates.Sales;

namespace Tilray.Integrations.Core.Domain.Aggregates.Sales.Rootstock
{
    public class RstkCustomer
    {
        private RstkCustomer() { }

        public string rstk__socust_custno__c { get; private set; }
        public string rstk__socust_sf_account__c { get; private set; }
        public ExternalReferenceId rstk__socust_cclass__r { get; private set; }
        public ExternalReferenceId rstk__socust_dimval__r { get; private set; }
        public ExternalReferenceId rstk__socust_dimval2__r { get; private set; }
        public string rstk__socust_dfltprodtype__c { get; private set; }
        public bool rstk__socust_prodind__c { get; private set; }
        public bool rstk__socust_serviceind__c { get; private set; }
        public bool rstk__socust_maintcurrind__c { get; private set; }
        public ExternalReferenceId rstk__socust_terms__r { get; private set; }

        public static RstkCustomer Create()
        {
            return new RstkCustomer();
        }

        public void SetRstkSocustCustnoC(string custNo) => rstk__socust_custno__c = custNo;
        public void SetRstkSocustSfAccountC(string sfAccount) => rstk__socust_sf_account__c = sfAccount;
        public void SetRstkSocustCclassR(ExternalReferenceId cclass) => rstk__socust_cclass__r = cclass;
        public void SetRstkSocustDimvalR(ExternalReferenceId dimval) => rstk__socust_dimval__r = dimval;
        public void SetRstkSocustDimval2R(ExternalReferenceId dimval2) => rstk__socust_dimval2__r = dimval2;
        public void SetRstkSocustDfltprodtypeC(string dfltProdType) => rstk__socust_dfltprodtype__c = dfltProdType;
        public void SetRstkSocustProdindC(bool prodInd) => rstk__socust_prodind__c = prodInd;
        public void SetRstkSocustServiceindC(bool serviceInd) => rstk__socust_serviceind__c = serviceInd;
        public void SetRstkSocustMaintcurrindC(bool maintCurrInd) => rstk__socust_maintcurrind__c = maintCurrInd;
        public void SetRstkSocustTermsR(ExternalReferenceId terms) => rstk__socust_terms__r = terms;

        //public static string GetCreatedRowId(ResponseResult payload)
        //{
        //    var x = payload;
        //    var x1 = payload[0];
        //    if (payload[0].errorCode != null)
        //    {

        //    }
        //    var x2 = payload["records"];
        //    return payload[0]["id"];
        //}
    }
}