namespace Tilray.Integrations.Core.Domain.AuditItems;

public class SOXReportAgg
{
    public IEnumerable<AuditItem> AuditItems { get; set; }
    public string AuditItemsQuery { get; set; }
}
