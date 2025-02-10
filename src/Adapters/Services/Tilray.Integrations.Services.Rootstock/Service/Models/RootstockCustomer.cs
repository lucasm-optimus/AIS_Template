namespace Tilray.Integrations.Services.Rootstock.Service.Models;

public class RootstockCustomer
{
    public string Name { get; set; }
    public string rstk__socust_custno__c { get; set; }
}

public class RootstockCustomerCreateResponse
{
    public static string GetRecordId(dynamic records)
    {
        if (records.Count == 0)
        {
            return Convert.ToString(records[0]["id"]);
        }

        return null;
    }
}
