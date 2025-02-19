namespace Tilray.Integrations.Services.Rootstock.Service.Models;

public class RootstockSalesOrderPayment
{
    public string Id { get; set; }
    public RootstockSalesOrderHeader rstk__sohdrpay_sohdr__r { get; set; }
    public string rstk__externalid__c { get; set; }
    public string rstk__sohdrpay_ordpayid__c { get; set; }
    public RootstockSalesOrderPaymentGateway rstk__sohdrpay_sogateway__r { get; set; }
    public decimal rstk__sohdrpay_payamount__c { get; set; }
}

public class RootstockSalesOrderHeader
{
    public RootstockDivisionMaster rstk__sohdr_div__r { get; set; }
}

public class RootstockDivisionMaster
{
    public string rstk__externalid__c { get; set; }
}

public class RootstockSalesOrderPaymentGateway
{
    public string rstk__externalid__c { get; set; }
}
