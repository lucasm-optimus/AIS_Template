using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tilray.Integrations.Core.Domain.Aggregates.Customer
{
    public class RstkCustomerInfoResponse
    {
        private RstkCustomerInfoResponse() { }

        [JsonProperty("rstk__socust_custno__c")]
        public string CustomerNo { get; private set; }

        public string Name { get; private set; }

        public static RstkCustomerInfoResponse MapFromPayload(dynamic payload)
        {
            return new RstkCustomerInfoResponse
            {
                CustomerNo = payload[0]["rstk__socust_custno__c"],
                Name = payload[0]["Name"]
            };
        }
    }
}
