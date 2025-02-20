
using Newtonsoft.Json;
using Tilray.Integrations.Core.Domain.Aggregates;

namespace Tilray.Integrations.Services.Salesforce.Service.Models
{
    public class SOXReport
    {
        [JsonProperty("totalSize")]
        public int TotalSize { get; set; }

        [JsonProperty("done")]
        public bool Done { get; set; }

        [JsonProperty("nextRecordsUrl")]
        public string NextRecordsUrl { get; set; }

        [JsonProperty("records")]
        public List<AuditItem> Records { get; set; }
    }
}
