using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tilray.Integrations.Core.Domain.Aggregates.Sales.Rootstock
{
    public class RstkSoapiHeaderResponse
    {
        private RstkSoapiHeaderResponse() { }

        public string rstk__sohdr_order__c { get; private set; }
        public string rstk__externalid__c { get; private set; }
        public string rstk__sohdr_custref__c { get; private set; }
        public string rstk__soapi_sohdr__c { get; private set; }

        public static RstkSoapiHeaderResponse FromPayload(dynamic payload)
        {
            return new RstkSoapiHeaderResponse
            {
                rstk__sohdr_order__c = payload[0]["rstk__sohdr_order__c"],
                rstk__externalid__c = payload[0]["rstk__externalid__c"],
                rstk__sohdr_custref__c = payload[0]["rstk__sohdr_custref__c"],
                rstk__soapi_sohdr__c = payload[0]["rstk__soapi_sohdr__c"]
            };
        }
    }
}
