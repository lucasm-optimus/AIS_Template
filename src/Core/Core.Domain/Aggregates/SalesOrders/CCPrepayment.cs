using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tilray.Integrations.Core.Domain.Aggregates.Sales
{
    public class CCPrepayment : Entity
    {
        [JsonProperty("amountPrepaidByCC")]
        public double AmountPrepaidByCC { get; private set; }

        [JsonProperty("prepaidCCTransactionID")]
        public string PrepaidCCTransactionID { get; private set; }

        [JsonProperty("ccPaymentGateway")]
        public string CCPaymentGateway { get; private set; }

        public string PaymentGatewayId { get; set; }

        private CCPrepayment() { }

        private CCPrepayment(double amountPrepaidByCC, string prepaidCCTransactionID, string ccPaymentGateway)
        {
            AmountPrepaidByCC = amountPrepaidByCC;
            PrepaidCCTransactionID = prepaidCCTransactionID;
            CCPaymentGateway = ccPaymentGateway;
        }

        public static CCPrepayment Create(double amountPrepaidByCC, string prepaidCCTransactionID, string ccPaymentGateway)
        {
            return new CCPrepayment(amountPrepaidByCC, prepaidCCTransactionID, ccPaymentGateway);
        }
    }
}
