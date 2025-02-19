namespace Tilray.Integrations.Core.Domain.Aggregates.SalesOrders.Ecom
{
    public class SalesOrderLine
    {
        #region Properties

        [JsonProperty("product")]
        public string Product { get; internal set; }

        [JsonProperty("quantity")]
        public double Quantity { get; internal set; }

        [JsonProperty("unitPrice")]
        public double UnitPrice { get; internal set; }

        [JsonProperty("lot")]
        public string Lot { get; internal set; }

        [JsonProperty("coveredByInsurance")]
        public double CoveredByInsurance { get; internal set; }

        [JsonProperty("gramsCoveredByInsurance")]
        public double GramsCoveredByInsurance { get; internal set; }

        [JsonProperty("obeerSku")]
        public string ObeerSku { get; internal set; }

        [JsonProperty("fulFillLoc")]
        public string FulFillLoc { get; internal set; }

        [JsonProperty("id")]
        public string Id { get; internal set; }

        #endregion

        #region Constructors

        public static SalesOrderLine Create(string product, double quantity, double unitPrice, string lot, double coveredByInsurance, double gramsCoveredByInsurance, string obeersku, string fulfillloc, string id)
        {
            return new SalesOrderLine
            {
                Product = product,
                Quantity = quantity,
                UnitPrice = unitPrice,
                Lot = lot,
                CoveredByInsurance = coveredByInsurance,
                GramsCoveredByInsurance = gramsCoveredByInsurance,
                ObeerSku = obeersku,
                FulFillLoc = fulfillloc,
                Id = id
            };
        }

        #endregion
    }
}
