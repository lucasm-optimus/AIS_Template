using System;
using System.Collections.Generic;
using Tilray.Integrations.Core.Domain.Aggregates.Sales;

namespace Tilray.Integrations.Core.Domain.Aggregates.Sales.Rootstock
{
    public class RstkCustomer
    {
        private RstkCustomer()
        {
        }

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

        public static RstkCustomer Create(
            string custNo,
            string sfAccount,
            ExternalReferenceId cclass,
            ExternalReferenceId dimval,
            ExternalReferenceId dimval2,
            string dfltProdType,
            bool prodInd,
            bool serviceInd,
            bool maintCurrInd,
            ExternalReferenceId terms)
        {
            return new RstkCustomer
            {
                rstk__socust_custno__c = custNo,
                rstk__socust_sf_account__c = sfAccount,
                rstk__socust_cclass__r = cclass,
                rstk__socust_dimval__r = dimval,
                rstk__socust_dimval2__r = dimval2,
                rstk__socust_dfltprodtype__c = dfltProdType,
                rstk__socust_prodind__c = prodInd,
                rstk__socust_serviceind__c = serviceInd,
                rstk__socust_maintcurrind__c = maintCurrInd,
                rstk__socust_terms__r = terms
            };
        }
    }
}