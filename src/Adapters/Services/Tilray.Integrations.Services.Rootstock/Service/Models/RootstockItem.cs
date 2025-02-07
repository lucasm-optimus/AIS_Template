namespace Tilray.Integrations.Services.Rootstock.Service.Models;

public class RootstockItem
{
    public string ID { get; set; }
    public string rstk__externalid__c { get; set; }
    public string rstk__peitem_div__r_rstk__externalid__c { get; set; }
    public string rstk__peitem_item__c { get; set; }
    public string rstk__peitem_descr__c { get; set; }
    public double? Case_Quantity__c { get; set; }
    public string? rstk__peitem_tracklot_pl__c { get; set; }
}
