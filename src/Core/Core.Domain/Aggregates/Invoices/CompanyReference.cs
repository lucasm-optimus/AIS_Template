namespace Tilray.Integrations.Core.Domain.Aggregates.Invoices;

public class CompanyReference
{
    public string Id { get; set; }
    public string Company_Name__c { get; set; }
    public string Concur_Company__c { get; set; }
    public string Rootstock_Company__c { get; set; }
    public bool Expenses__c { get; set; }
    public bool Non_PO_Invoices__c { get; set; }
    public bool PO_AP_Match_Invoices__c { get; set; }
    public bool OBeer_Invoices__c { get; set; }
}
