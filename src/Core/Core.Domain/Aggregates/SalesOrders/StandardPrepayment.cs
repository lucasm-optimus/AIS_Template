using Newtonsoft.Json;

namespace Tilray.Integrations.Core.Domain.Aggregates.SalesOrders
{
    public class StandardPrepayment : Entity
    {
        #region Properties

        [JsonProperty("amountPaid")]
        public double AmountPaid { get; private set; }

        [JsonProperty("prepaymentCustomer")]
        public string PrepaymentCustomer { get; private set; }

        #endregion

        #region Constructors

        private StandardPrepayment() { }

        private StandardPrepayment(double amountPaid, string prepaymentCustomer)
        {
            AmountPaid = amountPaid;
            PrepaymentCustomer = prepaymentCustomer;
        }

        public static StandardPrepayment Create(double amountPaid, string prepaymentCustomer)
        {
            return new StandardPrepayment(amountPaid, prepaymentCustomer);
        }

        #endregion
    }
}
