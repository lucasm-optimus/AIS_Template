using Newtonsoft.Json.Linq;

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
        public static Result<RstkCustomerInfoResponse> MapFromPayload(JArray records)
        {
            try
            {
                if (records.Count > 0)
                {
                    return Result.Ok(new RstkCustomerInfoResponse
                    {
                        CustomerNo = records.FirstOrDefault()!["rstk__socust_custno__c"].ToString(),
                        Name = records.FirstOrDefault()!["Name"].ToString(),
                        CustomerId = records.FirstOrDefault()!["Id"].ToString()
                    });
                }
                else
                {
                    return Result.Fail("No records found");
                }

            }
            catch (Exception e)
            {
                return Result.Fail<RstkCustomerInfoResponse>(e.Message);
            }
        }

        #endregion
    }
}
