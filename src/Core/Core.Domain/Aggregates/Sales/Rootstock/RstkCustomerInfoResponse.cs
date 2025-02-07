using Newtonsoft.Json;

namespace Tilray.Integrations.Core.Domain.Aggregates.Sales.Rootstock
{
    public class RstkCustomerInfoResponse
    {
        private RstkCustomerInfoResponse() { }

        [JsonProperty("rstk__socust_custno__c")]
        public string CustomerNo { get; private set; }

        public string Name { get; private set; }

        public static RstkCustomerInfoResponse MapFromPayload(dynamic records)
        {
            return new RstkCustomerInfoResponse
            {
                CustomerNo = records[0]["rstk__socust_custno__c"],
                Name = records[0]["Name"]
            };
        }
    }
}
