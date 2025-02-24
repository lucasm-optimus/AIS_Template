namespace Tilray.Integrations.Core.Domain.Aggregates.SalesOrders
{
    public class SalesAgg : AggRoot
    {
        #region Agg Entities

        public MedSalesOrder SalesOrder { get; private set; }
        public SalesOrderCustomer SalesOrderCustomer { get; private set; }
        public SalesOrderCustomerAddress SalesOrderCustomerAddress { get; private set; }

        #endregion

        #region Constructors

        private SalesAgg() { }

        public static Result<SalesAgg> Create(Ecom.SalesOrder payload, OrderDefaultsSettings orderDefaults)
        {
            try
            {
                var result = Result.Ok();

                var salesOrderCreatedResult = MedSalesOrder.Create(payload, orderDefaults);
                result.WithErrors(salesOrderCreatedResult.Errors);

                var salesOrderCustomerCreatedResult = SalesOrderCustomer.Create(payload, orderDefaults);
                result.WithErrors(salesOrderCustomerCreatedResult.Errors);

                var salesOrderCustomerAddressCreatedResult = SalesOrderCustomerAddress.Create(payload);
                result.WithErrors(salesOrderCustomerAddressCreatedResult.Errors);

                return result.IsFailed ? result : Result.Ok(new SalesAgg
                {
                    SalesOrder = salesOrderCreatedResult.Value,
                    SalesOrderCustomer = salesOrderCustomerCreatedResult.Value,
                    SalesOrderCustomerAddress = salesOrderCustomerAddressCreatedResult.Value
                });
            }
            catch (Exception e)
            {
                return Result.Fail<SalesAgg>(e.Message);
            }
        }

        #endregion
    }
}