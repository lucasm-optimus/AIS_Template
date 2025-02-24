using Tilray.Integrations.Core.Domain.Aggregates.SalesOrders.Customer;

namespace Tilray.Integrations.Services.Rootstock.Service.Models;

public class RootstockCustomer
{
    #region Properties

    public string Name { get; set; }
    public string rstk__socust_custno__c { get; private set; }
    public string rstk__socust_sf_account__c { get; private set; }
    public string rstk__socust_cclass__c { get; private set; }
    public string rstk__socust_dimval__c { get; private set; }
    public string rstk__socust_dimval2__c { get; private set; }
    public string rstk__socust_dfltprodtype__c { get; private set; }
    public bool rstk__socust_prodind__c { get; private set; }
    public bool rstk__socust_serviceind__c { get; private set; }
    public bool rstk__socust_maintcurrind__c { get; private set; }
    public string rstk__socust_terms__c { get; private set; }

    #endregion

    #region Constructors

    private RootstockCustomer() { }
    public static Result<RootstockCustomer> Create(SalesOrderCustomer salesOrderCustomer)
    {
        try
        {
            var rootstockCustomer = new RootstockCustomer
            {
                rstk__socust_custno__c = salesOrderCustomer.CustomerNo,
                rstk__socust_sf_account__c = salesOrderCustomer.SFAccountID,
                rstk__socust_cclass__c = salesOrderCustomer.CustomerClass,
                rstk__socust_dimval__c = salesOrderCustomer.AccountingDimension1,
                rstk__socust_dimval2__c = salesOrderCustomer.AccountingDimension2,
                rstk__socust_dfltprodtype__c = salesOrderCustomer.DefaultProductType,
                rstk__socust_prodind__c = salesOrderCustomer.CustomerBuysProduct,
                rstk__socust_serviceind__c = salesOrderCustomer.CustomerBuysService,
                rstk__socust_maintcurrind__c = salesOrderCustomer.PlaceOrdersInTheCustomerCurrency,
                rstk__socust_terms__c = salesOrderCustomer.PaymentTerms
            };

            return Result.Ok(rootstockCustomer);
        }
        catch (Exception e)
        {
            return Result.Fail<RootstockCustomer>(e.Message);
        }
    }

    #endregion
}
