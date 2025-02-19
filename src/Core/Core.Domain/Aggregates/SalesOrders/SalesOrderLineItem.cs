namespace Tilray.Integrations.Core.Domain.Aggregates.SalesOrders
{
    public class SalesOrderLineItem : Entity
    {
        #region Properties

        [JsonProperty("itemNumber")]
        public string ItemNumber { get; private set; }

        [JsonProperty("quantity")]
        public double Quantity { get; private set; }

        [JsonProperty("unitPrice")]
        public double? UnitPrice { get; private set; }

        [JsonProperty("requiredLotToPick")]
        public string RequiredLotToPick { get; private set; }

        [JsonProperty("amountCoveredByInsurance")]
        public double? AmountCoveredByInsurance { get; private set; }

        [JsonProperty("gramsCoveredByInsurance")]
        public double? GramsCoveredByInsurance { get; private set; }

        [JsonProperty("firm")]
        public bool? Firm { get; private set; }

        [JsonProperty("location")]
        public string Location { get; private set; }

        [JsonProperty("id")]
        public string Id { get; private set; }

        [JsonProperty("uploadGroup")]
        public string UploadGroup { get; private set; }

        [JsonProperty("currencyIsoCode")]
        public string CurrencyIsoCode { get; private set; }
        public string ProductId { get; set; }

        #endregion

        #region Constructors

        private SalesOrderLineItem() { }

        public static SalesOrderLineItem Create(string itemNumber, double quantity, double unitPrice, string requiredLotToPick, double? amountCoveredByInsurance, double? gramsCoveredByInsurance, bool? firm, string location, string id)
        {
            return new SalesOrderLineItem
            {
                ItemNumber = itemNumber,
                Quantity = quantity,
                UnitPrice = unitPrice,
                RequiredLotToPick = requiredLotToPick,
                AmountCoveredByInsurance = amountCoveredByInsurance,
                GramsCoveredByInsurance = gramsCoveredByInsurance,
                Firm = firm,
                Location = location,
                Id = id
            };
        }

        public void UpdateProductId(string value)
        {
            ProductId = value;
        }

        #endregion
    }
}
