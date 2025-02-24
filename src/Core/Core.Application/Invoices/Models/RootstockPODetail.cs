using Newtonsoft.Json.Linq;

namespace Tilray.Integrations.Core.Application.Invoices.Models
{
    public class RootstockPODetail
    {
        public string? Id { get; private set; }
        public string? Name { get; private set; }
        public string? rstk__poline_longdescr__c { get; private set; }

        private RootstockPODetail() { }

        public static Result<RootstockPODetail> Create(JArray payload)
        {
            try
            {
                var rootstockPODetail = new RootstockPODetail
                {
                    Id = payload.FirstOrDefault()?["Id"]?.ToString(),
                    Name = payload.FirstOrDefault()?["Name"]?.ToString(),
                    rstk__poline_longdescr__c = payload.FirstOrDefault()?["rstk__porcptap_poline__r"]?["rstk__poline_longdescr__c"]?.ToString()
                };

                return Result.Ok(rootstockPODetail);
            }
            catch (Exception e)
            {
                return Result.Fail<RootstockPODetail>(e.Message);
            }
        }
    }
}
