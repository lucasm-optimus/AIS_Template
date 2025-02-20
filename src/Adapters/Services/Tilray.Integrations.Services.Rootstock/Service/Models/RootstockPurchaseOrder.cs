namespace Tilray.Integrations.Services.Rootstock.Service.Models;

public class RootstockPurchaseOrderReceipts
{
    public List<RootstockPurchaseOrderReceipt> records { get; set; } = new List<RootstockPurchaseOrderReceipt>();
    public string nextRecordsUrl { get; set; }
    public int totalSize { get; set; }
    public bool done { get; set; }
}

public class RootstockPurchaseOrderReceipt
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string rstk__externalid__c { get; set; }
    public string rstk__porcptap_ordno__c { get; set; }
    public RootstockPurchaseOrderReceiptOrderNumber? rstk__porcptap_ordno__r { get; set; }
    public string rstk__porcptap_poline__c { get; set; }
    public DateTime rstk__porcptap_rcptdate__c { get; set; }
    public RootstockPurchaseOrderReceiptItem? rstk__porcptap_poitem__r { get; set; }
    public double rstk__porcptap_qtycomp__c { get; set; }
    public string rstk__porcptap_packslipno__c { get; set; }
    public double rstk__porcptap_rcptno__c { get; set; }
    public RootstockPurchaseOrderReceiptLine? rstk__porcptap_poline__r { get; set; }
}

public class RootstockPurchaseOrderReceiptOrderNumber
{
    public string rstk__pohdr_ordno__c { get; set; }
}

public class RootstockPurchaseOrderReceiptItem
{
    public string Name { get; set; }
}

public class RootstockPurchaseOrderReceiptLine
{
    public string Name { get; set; }
    public string rstk__poline_longdescr__c { get; set; }
}

public class RootstockPurchaseOrder
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string rstk__pohdr_ordno__c { get; set; }
    public string rstk__pohdr_pohdraddr_bt__c { get; set; }
    public string rstk__pohdr_pohdraddr_st__c { get; set; }
    public Currency? rstk__pohdr_maintcurr__r { get; set; }
    public DateTime CreatedDate { get; set; }
    public string rstk__pohdr_ordsts__c { get; set; }
    public Division? rstk__pohdr_div__r { get; set; }
    public Vendor? rstk__pohdr_vendno__r { get; set; }
    public DateTime rstk__pohdr_actplacedate__c { get; set; }
    public RootstockAddress? rstk__pohdr_pohdraddr_bt__r { get; set; }
    public RootstockAddress? rstk__pohdr_pohdraddr_st__r { get; set; }
    public double VendorAddressNumber { get; set; }
    public IEnumerable<RootstockPurchaseOrderReceipt>? PurchaseOrderReceipts { get; set; }
    public List<RootstockLineItem>? LineItems { get; set; }
}

public class Currency
{
    public string rstk__externalid__c { get; set; }
}

public class Division
{
    public string rstk__externalid__c { get; set; }
}

public class Vendor
{
    public string rstk__externalid__c { get; set; }
}

public class RootstockAddress
{
    public string Name { get; set; }
    public string rstk__externalid__c { get; set; }
    public string rstk__pohdraddr_street__c { get; set; }
    public string rstk__pohdraddr_city__c { get; set; }
    public string rstk__pohdraddr_stateprov__c { get; set; }
    public string rstk__pohdraddr_country__c { get; set; }
    public string rstk__pohdraddr_zippostalcode__c { get; set; }
}

public class RootstockVendorAddress
{
    public string Id { get; set; }
    public double rstk__povendpoaddr_seq__c { get; set; }

    public static double ValidVendorAddress(IEnumerable<RootstockVendorAddress> vendorAddressResponseResult)
    {
        if (vendorAddressResponseResult.Any())
        {
            return vendorAddressResponseResult.First().rstk__povendpoaddr_seq__c;
        }
        return 0; // Default value when there is no valid vendor address
    }
}

public class RootstockLineItem
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string rstk__externalid__c { get; set; }
    public string rstk__poline_ordno__c { get; set; }
    public string rstk__poline_ordsts__c { get; set; }
    public double rstk__poline_lne__c { get; set; }
    public string rstk__poline_longdescr__c { get; set; }
    public double rstk__poline_qtyreq__c { get; set; }
    public decimal rstk__poline_unitpricemcurr__c { get; set; }
    public string UOM_Code__c { get; set; }
    public DateTime CreatedDate { get; set; }
    public ExpenseAccount? rstk__poline_expenseacct__r { get; set; }
    public decimal rstk__poline_amtreqmcurr__c { get; set; }
}

public class ExpenseAccount
{
    public string rstk__syacc_mfgacct__c { get; set; }
}
