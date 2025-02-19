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

        public static Result<RstkCustomerAddressInfoResponse> MapFromPayload(dynamic records)
        {
            if (records != null && records.Count > 0)
            {
                return Result.Ok(new RstkCustomerAddressInfoResponse
                {
                    CustomerID = records[0]["ID"],
                    CustomerAddressID = records[0]["rstk__externalid__c"],
                    Name = records[0]["Name"],
                    LocationReference = records[0]["External_Customer_Number__c"]
                });
            }
            else
            {
                return Result.Fail<RstkCustomerAddressInfoResponse>("Records not found");
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

        #endregion
    }
}
