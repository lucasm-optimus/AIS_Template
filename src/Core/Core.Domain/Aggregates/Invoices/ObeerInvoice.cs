using Newtonsoft.Json;

namespace Tilray.Integrations.Core.Domain.Aggregates.Invoices;

public class InvoiceHeader
{
    public string ConcurOrderID { get; set; }
    public string CardCode { get; set; }
    public string CustomerRefNo { get; set; }
    public string PostingDate { get; set; }
    public string DueDate { get; set; }
    public string DocumentDate { get; set; }
    public string DocumentType { get; set; }
    public string PODocNum { get; set; }
    public string GRPODocNum { get; set; }
}

public class Item
{
    public string PODocNum { get; set; }
    public string GRPODocNum { get; set; }
    public string POLineNum { get; set; }
    public string GRPOLineNum { get; set; }
    public string ItemCode { get; set; }
    public string ItemDescription { get; set; }
    public string GLAccount { get; set; }
    public string BrandFamily { get; set; }
    public string Facility { get; set; }
    public string DistributorID { get; set; }
    public decimal Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal TotalPrice { get; set; }

    [JsonIgnore]
    public GRPOType Type { get; set; }

    [JsonIgnore]
    public string Custom4 { get; set; }

    public static Item CreateFromGroup(IGrouping<string, Item> group)
    {
        return new Item
        {
            PODocNum = group.First().PODocNum,
            GRPODocNum = group.First().GRPODocNum,
            POLineNum = group.First().POLineNum,
            GRPOLineNum = group.First().GRPOLineNum,
            ItemCode = group.First().ItemCode,
            ItemDescription = group.First().ItemDescription,
            GLAccount = group.First().GLAccount,
            BrandFamily = group.First().BrandFamily,
            Facility = group.First().Facility,
            DistributorID = group.First().DistributorID,
            Quantity = group.Sum(x => x.Quantity),
            UnitPrice = group.First().UnitPrice,
            TotalPrice = group.Sum(x => x.TotalPrice)
        };
    }
}

public class Import
{
    public List<InvoiceHeader> InvoiceHeader { get; set; } = [];
    public List<Item> Items { get; set; } = [];
}

public class ObeerInvoice
{
    public Import Import { get; set; } = new();
}
