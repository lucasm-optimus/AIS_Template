using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tilray.Integrations.Core.Domain.Aggregates.Sales
{
    public class ExternalReferenceId
    {
        private ExternalReferenceId() { }
        [JsonIgnore]
        public string type { get; private set; }
        [JsonProperty("name")]
        public string rstk__externalid__c { get; private set; }

        public static ExternalReferenceId Create(string type, string externalId)
        {
            return new ExternalReferenceId
            {
                type = type,
                rstk__externalid__c = externalId
            };
        }
        override public string ToString()
        {
            return JsonConvert.SerializeObject(new { name = rstk__externalid__c });
        }
    }
}
