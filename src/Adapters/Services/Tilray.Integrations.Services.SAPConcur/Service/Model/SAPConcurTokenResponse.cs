using Newtonsoft.Json;

namespace Tilray.Integrations.Services.SAPConcur.Service.Model
{
    public class SAPConcurTokenResponse
    {
        [JsonProperty("access_token")]
        public string AccessToken { get; set; }
    }
}
