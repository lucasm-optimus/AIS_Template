namespace Tilray.Integrations.Core.Domain.Aggregates.SalesOrders
{
    public class CCPrepayment : Entity
    {
        #region Properties

        [JsonProperty("amountPrepaidByCC")]
        public double AmountPrepaidByCC { get; private set; }

        [JsonProperty("prepaidCCTransactionID")]
        public string PrepaidCCTransactionID { get; private set; }

        [JsonProperty("ccPaymentGateway")]
        public string CCPaymentGateway { get; private set; }

        public string PaymentGatewayId { get; set; }

        #endregion

        #region Constructors
        private CCPrepayment() { }

        public static CCPrepayment Create(double amountPrepaidByCC, string prepaidCCTransactionID, string ccPaymentGateway)
        {
            var ccPrepayment = new CCPrepayment()
            {
                AmountPrepaidByCC = amountPrepaidByCC,
                PrepaidCCTransactionID = prepaidCCTransactionID,
                CCPaymentGateway = ccPaymentGateway,
            };

            return ccPrepayment;
        }

        #endregion
    }
}
