using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tilray.Integrations.Core.Domain.Aggregates.Sales
{
    public class SalesOrderLineItem : Entity
    {
        public string ItemNumber { get; private set; }
        public double Quantity { get; private set; }
        public double? UnitPrice { get; private set; }
        public string RequiredLotToPick { get; private set; }
        public double? AmountCoveredByInsurance { get; private set; }
        public double? GramsCoveredByInsurance { get; private set; }
        public bool? Firm { get; private set; }
        public string Location { get; private set; }
        public string Id { get; private set; }

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
    }
}
