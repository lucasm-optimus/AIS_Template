using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tilray.Integrations.Core.Domain.Aggregates.Sales.Rootstock
{
    public class RstkCustomerAddressInfoResponse
    {
        private RstkCustomerAddressInfoResponse() { }

        [JsonProperty("customerID")]
        public string CustomerID { get; private set; }

        [JsonProperty("customerAddressID")]
        public string CustomerAddressID { get; private set; } = string.Empty;

        [JsonProperty("name")]
        public string Name { get; private set; }

        [JsonProperty("locationReference")]
        public string LocationReference { get; private set; }

        public static Result<RstkCustomerAddressInfoResponse> MapFromPayload(dynamic payload)
        {
            if (payload != null && payload.Count > 0)
            {
                return Result.Ok(new RstkCustomerAddressInfoResponse
                {
                    CustomerID = payload[0]["id"],
                    CustomerAddressID = payload[0]["rstk__externalid__c"],
                    Name = payload[0]["name"],
                    LocationReference = payload[0]["External_Customer_Number__c"]
                });
            }
            else
            {
                return Result.Fail<RstkCustomerAddressInfoResponse>("Faild to convert payload");
            }
        }

        public static int? GetNextSequenceNumber(dynamic payload)
        {
            var MaxSequenceNum = payload[0]["MaxSequenceNum"];
            if (MaxSequenceNum != null)
            {
                return Convert.ToInt32(MaxSequenceNum) + 1;
            }
            else
            {
                return 1;
            }
        }
    }
}
