using Newtonsoft.Json.Linq;

namespace Tilray.Integrations.Core.Domain.Aggregates.SalesOrders.Rootstock
{
    public class RstkCustomerAddressInfoResponse
    {
        #region Properties

        [JsonProperty("customerID")]
        public string CustomerID { get; private set; }

        [JsonProperty("customerAddressID")]
        public string CustomerAddressID { get; private set; } = string.Empty;

        [JsonProperty("name")]
        public string Name { get; private set; }

        [JsonProperty("locationReference")]
        public string LocationReference { get; private set; }

        #endregion

        #region Constructors

        private RstkCustomerAddressInfoResponse() { }

        #endregion

        #region Public Methods

        public static Result<RstkCustomerAddressInfoResponse> Create(JArray records)
        {
            if (records != null && records.Count > 0)
            {
                return Result.Ok(new RstkCustomerAddressInfoResponse
                {
                    CustomerID = records.First()["Id"].ToString(),
                    CustomerAddressID = records.First()["rstk__externalid__c"].ToString(),
                    Name = records.First()["Name"].ToString(),
                    LocationReference = records.First()["External_Customer_Number__c"].ToString()
                });
            }
            else
            {
                return Result.Fail<RstkCustomerAddressInfoResponse>("Records not found");
            }
        }

        public static int? GetNextSequenceNumber(JArray payload)
        {
            var MaxSequenceNum = payload.First()["MaxSequenceNum"];
            return MaxSequenceNum != null
                ? Convert.ToInt32(MaxSequenceNum) + 1
                : 1;
        }

        #endregion
    }
}
