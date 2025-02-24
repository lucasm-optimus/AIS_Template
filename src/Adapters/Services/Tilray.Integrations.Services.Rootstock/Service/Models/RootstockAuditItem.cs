using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tilray.Integrations.Core.Domain.AuditItems;

namespace Tilray.Integrations.Services.Rootstock.Service.Models
{
    public  class RootstockAuditItem
    {
         [JsonProperty("totalSize")]
         public int TotalSize { get; set; }

         [JsonProperty("done")]
         public bool Done { get; set; }

         [JsonProperty("nextRecordsUrl")]
         public string NextRecordsUrl { get; set; }

         [JsonProperty("records")]
         public IEnumerable<AuditItem> Records { get; set; }
        
    }
}
