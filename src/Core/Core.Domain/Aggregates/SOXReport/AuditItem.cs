using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Tilray.Integrations.Core.Domain.Aggregates.SOXReport
{
    public class AuditItem
    {
        public string? Action { get; set; }
        public CreatedBy? CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string? DelegateUser { get; set; }
        public string? Display { get; set; }
        public string Id { get; set; } = null!;
        public string? ResponsibleNamespacePrefix { get; set; }
        public string? Section { get; set; }
        public AuditItemAttributes? Attributes { get; set; }
    }

    public class CreatedBy
    {
        public string? Username { get; set; }
    }

    public class AuditItemAttributes
    {
        public string? Type { get; set; }
        public string? Url { get; set; }
    }
}
