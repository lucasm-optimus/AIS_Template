namespace Tilray.Integrations.Core.Domain.Aggregates.SalesOrders.Ecom
{
    public class SalesOrderTax
    {
        #region Properties

        [JsonProperty("taxType")]
        public string TaxType { get; internal set; }

        [JsonProperty("amount")]
        public double Amount { get; internal set; }

        [JsonProperty("coveredByInsurance")]
        public bool CoveredByInsurance { get; internal set; }

        #endregion

        #region Constructors

        public static SalesOrderTax Create(string taxType, double amount, bool coveredByInsurance)
        {
            return new SalesOrderTax
            {
                TaxType = taxType,
                Amount = amount,
                CoveredByInsurance = coveredByInsurance
            };
        }

        #endregion
    }
}
