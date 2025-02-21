using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tilray.Integrations.Core.Domain.Aggregates.SOXReport
{
    public  class SOXReportAgg
    {
        public IEnumerable<AuditItem> ReportItems {  get; set; }   
        public string Query { get; set; }   
       
    }
}
