namespace Tilray.Integrations.Services.Rootstock.Service.Models;

public class OrderType
{
    [JsonProperty("rstk__externalid__c")]
    public string ExternalId { get; set; }
}

public class Customer
{
    [JsonProperty("rstk__externalid__c")]
    public string ExternalId { get; set; }
}

public class Address
{
    [JsonProperty("rstk__externalid__c")]
    public string ExternalId { get; set; }
}

public class Product
{
    [JsonProperty("rstk__externalid__c")]
    public string ExternalId { get; set; }
}

public class RootstockSalesOrder
{
    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public string ID { get; set; }

    [JsonProperty("rstk__soapi_mode__c", NullValueHandling = NullValueHandling.Ignore)]
    public string SoapiMode { get; set; }

    [JsonProperty("rstk__soapi_addorupdate__c", NullValueHandling = NullValueHandling.Ignore)]
    public bool SoapiAddOrUpdate { get; set; } = false;

    [JsonProperty("rstk__soapi_sohdr__c", NullValueHandling = NullValueHandling.Ignore)]
    public string SoapiSohdr { get; set; }

    [JsonProperty("rstk__soapi_custref__c", NullValueHandling = NullValueHandling.Ignore)]
    public string CustomerReference { get; set; }

    [JsonProperty("rstk__soapi_salesdiv__c", NullValueHandling = NullValueHandling.Ignore)]
    public string SalesDivision { get; set; }

    [JsonProperty("rstk__soapi_updatecustfields__c", NullValueHandling = NullValueHandling.Ignore)]
    public bool UpdateCustomerFields { get; set; } = true;

    [JsonProperty("rstk__soapi_otype__r", NullValueHandling = NullValueHandling.Ignore)]
    public OrderType SoapiOrderType { get; set; }

    [JsonProperty("rstk__soapi_orderdate__c", NullValueHandling = NullValueHandling.Ignore)]
    public DateTime? OrderDate { get; set; }

    [JsonProperty("Expected_Delivery_Date__c", NullValueHandling = NullValueHandling.Ignore)]
    public DateTime? ExpectedDeliveryDate { get; set; }

    [JsonProperty("rstk__soapi_custpo__c", NullValueHandling = NullValueHandling.Ignore)]
    public string CustomerPO { get; set; }

    [JsonProperty("rstk__soapi_socust__r", NullValueHandling = NullValueHandling.Ignore)]
    public Customer SoapiCustomer { get; set; }

    [JsonProperty("rstk__soapi_shiptoaddr__r", NullValueHandling = NullValueHandling.Ignore)]
    public Address ShipToAddress { get; set; }

    [JsonProperty("rstk__soapi_async__c", NullValueHandling = NullValueHandling.Ignore)]
    public bool? BackgroundProcessing { get; set; }

    [JsonProperty("rstk__soapi_upgroup__c", NullValueHandling = NullValueHandling.Ignore)]
    public string UploadGroup { get; set; }

    [JsonProperty("rstk__soapi_soprod__r", NullValueHandling = NullValueHandling.Ignore)]
    public Product SoapiProduct { get; set; }

    [JsonProperty("rstk__soapi_qtyorder__c", NullValueHandling = NullValueHandling.Ignore)]
    public int QuantityOrder { get; set; }

    [JsonProperty("rstk__soapi_price__c", NullValueHandling = NullValueHandling.Ignore)]
    public decimal? UnitPrice { get; set; }

    [JsonProperty("External_Order_Reference__c", NullValueHandling = NullValueHandling.Ignore)]
    public string ExternalOrderReference { get; set; }
}
