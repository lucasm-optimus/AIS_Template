using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tilray.Integrations.Core.Domain.Aggregates.Sales
{
    public class SalesOrderPrepayment : Entity
    {
        [JsonProperty("applicationMethod")]
        public string ApplicationMethod { get; private set; }

        [JsonProperty("prepaymentType")]
        public string PrepaymentType { get; private set; }

        [JsonProperty("orderId")]
        public string OrderID { get; private set; }

        [JsonProperty("customer")]
        public string Customer { get; private set; }

        [JsonProperty("useDefaultBillToAddress")]
        public bool UseDefaultBillToAddress { get; private set; }

        [JsonProperty("soCustomerNo")]
        public string SOCustomerNo { get; private set; }

        [JsonProperty("prepaymentAccount")]
        public string PrepaymentAccount { get; private set; }

        [JsonProperty("division")]
        public string Division { get; private set; }

        [JsonProperty("amount")]
        public double Amount { get; private set; }

        [JsonProperty("customerBillToAddressId")]
        public string CustomerBillToAddressID { get; internal set; }

        private SalesOrderPrepayment() { }

        public static Result<SalesOrderPrepayment> Create(StandardPrepayment standardPrepayment, string soCustomerNo, string division, string createdSalesOrderId, string prePaymentAccount)
        {
            try
            {
                var prePayment = new SalesOrderPrepayment
                {
                    ApplicationMethod = "Maximum Amount",
                    PrepaymentType = "Sales Order",
                    OrderID = createdSalesOrderId,
                    Customer = standardPrepayment.PrepaymentCustomer,
                    UseDefaultBillToAddress = true,
                    SOCustomerNo = soCustomerNo,
                    //PrepaymentAccount = prePaymentAccount,
                    PrepaymentAccount = "20011700",
                    Division = division,
                    Amount = standardPrepayment.AmountPaid,
                };

                return Result.Ok(prePayment);
            }
            catch (Exception e)
            {
                return Result.Fail<SalesOrderPrepayment>(e.Message);
            }

        }
    }
}
