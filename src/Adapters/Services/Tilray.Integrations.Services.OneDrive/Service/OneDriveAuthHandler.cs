using Newtonsoft.Json;
using System.Net;

namespace Tilray.Integrations.Services.OneDrive
{
    internal sealed class OneDriveAuthHandler : DelegatingHandler
    {
        private readonly OneDriveSettings _settings;
        private readonly HttpClient _httpClient;
        private string _accessToken;

        public OneDriveAuthHandler(OneDriveSettings settings, HttpClient httpClient)
        {
            _settings = settings;
            _httpClient = httpClient;
        }

        private async Task<string> GetAccessTokenAsync()
        {
            if (string.IsNullOrEmpty(_accessToken))
            {
                await RefreshTokenAsync();
            }
            return _accessToken;
        }

        private async Task RefreshTokenAsync()
        {
            var tokenHost = _settings.TokenHost;
            var request = new HttpRequestMessage(HttpMethod.Post, $"{tokenHost}/{_settings.TenantId}/oauth2/v2.0/token")
            {
                Content = new FormUrlEncodedContent(new Dictionary<string, string>
                {
                    { "grant_type", "client_credentials" },
                    { "client_id", _settings.ClientId },
                    { "client_secret", _settings.ClientSecret },
                    { "scope", "https://graph.microsoft.com/.default" }
                })
            };

            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var tokenResponse = JsonConvert.DeserializeObject<Dictionary<string, string>>(content);

            _accessToken = tokenResponse["access_token"];
        }


        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var accessToken = await GetAccessTokenAsync();
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            return await base.SendAsync(request, cancellationToken);
        }
    }
}
