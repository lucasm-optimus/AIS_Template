namespace Tilray.Integrations.Core.Domain.Aggregates.SalesOrders.Rootstock
{
    public class RstkCustomerInfoResponse
    {
        #region Properties

        [JsonProperty("rstk__socust_custno__c")]
        public string CustomerNo { get; private set; }

        public string Name { get; private set; }
        public string CustomerId { get; private set; }

        #endregion

        #region Constructors

        private RstkCustomerInfoResponse() { }
        public static RstkCustomerInfoResponse MapFromPayload(dynamic records)
        {
            if (records.Count > 0)
            {
                return new RstkCustomerInfoResponse
                {
                    CustomerNo = records[0]["rstk__socust_custno__c"],
                    Name = records[0]["Name"],
                    CustomerId = records[0]["Id"]
                };
            }
            else
            {
                return null;
            }
        }

        #endregion
    }
}
