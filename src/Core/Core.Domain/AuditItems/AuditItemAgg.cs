namespace Tilray.Integrations.Core.Domain.AuditItems;

public class AuditItemAgg
{
    public IEnumerable<AuditItem> AuditItems { get; set; }
    public string AuditItemsQuery { get; set; }
    public static AuditItemAgg Create(IEnumerable<AuditItem> auditItems, string auditItemsQuery)
    {
        return new AuditItemAgg
        {
            AuditItems = auditItems,
            AuditItemsQuery = auditItemsQuery
        };
    }
}
