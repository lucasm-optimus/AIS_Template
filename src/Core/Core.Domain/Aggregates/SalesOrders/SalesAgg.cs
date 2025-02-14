using Tilray.Integrations.Core.Domain.Aggregates.Sales.Customer;
using Tilray.Integrations.Core.Domain.Aggregates.Sales.Events;
using Tilray.Integrations.Core.Domain.Aggregates.Sales.Rootstock;
using Tilray.Integrations.Core.Domain.Aggregates.SalesOrders;
using Tilray.Integrations.Core.Models.Ecom;

namespace Tilray.Integrations.Core.Domain.Aggregates.Sales
{
    public class SalesAgg : AggRoot
    {
        #region Agg Entities

        public MedSalesOrder SalesOrder { get; private set; }
        public SalesOrderCustomer SalesOrderCustomer { get; private set; }
        public SalesOrderCustomerAddress SalesOrderCustomerAddress { get; private set; }

        public RstkSalesOrder RootstockSalesOrder { get; private set; }
        public List<RstkSalesOrderLineItem> RootstockOrderLines { get; private set; }

        #endregion

        #region Constructors

        private SalesAgg() { }

        public static Result<SalesAgg> Create(Models.Ecom.SalesOrder payload, OrderDefaultsSettings orderDefaults)
        {
            var salesAgg = new SalesAgg();
            try
            {
                var salesOrderCreatedResult = MedSalesOrder.Create(payload, orderDefaults);
                if (salesOrderCreatedResult.IsSuccess)
                {
                    salesAgg.SalesOrder = salesOrderCreatedResult.Value;
                    salesAgg.SalesOrderCustomer = SalesOrderCustomer.Create(payload, orderDefaults);
                    salesAgg.SalesOrderCustomerAddress = SalesOrderCustomerAddress.Create(payload);

                    return Result.Ok(salesAgg);
                }

                return Result.Fail<SalesAgg>(salesOrderCreatedResult.Errors);
            }
            catch (Exception e)
            {
                return Result.Fail<SalesAgg>(e.Message);
            }
        }

        public static Result<SalesAgg> Create(MedSalesOrder salesOrder)
        {
            var salesAgg = new SalesAgg
            {
                SalesOrder = salesOrder,
                RootstockOrderLines = new List<RstkSalesOrderLineItem>()
            };

            var rsoResult = RstkSalesOrder.Create(salesOrder);
            if (rsoResult.IsFailed)
            {
                return Result.Fail<SalesAgg>(rsoResult.Errors);
            }
            salesAgg.RootstockSalesOrder = rsoResult.Value;

            var result = Result.Ok();
            for (var i = 1; i < salesOrder.LineItems.Count; i++)
            {
                var lineResult = RstkSalesOrderLineItem.Create(salesOrder, salesOrder.LineItems[i]);
                if (lineResult.IsSuccess)
                {
                    salesAgg.RootstockOrderLines.Add(lineResult.Value);
                }
                else
                {
                    result.WithErrors(lineResult.Errors);
                }
            }

            if (result.IsFailed)
            {
                return Result.Fail<SalesAgg>(result.Errors);
            }

            var ccPrepaymentAddedResult = salesAgg.SalesOrder.AddCCPrePayment();
            if (ccPrepaymentAddedResult.IsFailed)
            {
                return ccPrepaymentAddedResult;
            }

            return Result.Ok(salesAgg);
        }

        #endregion
    }
}