using Microsoft.Extensions.Logging;
using System.Net.Http.Json;
using Tilray.Integrations.Services.Rootstock.Startup;
using Tilray.Integrations.Services.SAPConcur.Service.Model;

namespace Tilray.Integrations.Services.Rootstock.Service
{
    /// <summary>
    /// A <see cref="DelegatingHandler"/> which adds the Authentication header onto outgoing Rootstock HTTP requests.
    /// </summary>
    public class RootstockAuthHandler : DelegatingHandler
    {
        private readonly RootstockSettings _rootstockSettings;
        private readonly HttpClient _httpClient;
        private readonly ILogger<RootstockAuthHandler> _logger;
        private string authenticationToken = "";

        public RootstockAuthHandler(RootstockSettings rootstockSettings, ILogger<RootstockAuthHandler> logger, HttpClient httpClient)
        {
            _rootstockSettings = rootstockSettings;
            _logger = logger;
            _httpClient = httpClient;
        }

        /// <inheritdoc cref="DelegatingHandler"/>
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            if (!request.Headers.Contains("Authorization"))
            {
                if (string.IsNullOrEmpty(authenticationToken))
                    authenticationToken = $"Bearer {GetAuthenticationToken()}";

                request.Headers.Add("Authorization", authenticationToken);
            }

            return base.SendAsync(request, cancellationToken);
        }

        private string GetAuthenticationToken()
        {
            try
            {
                var tokenRequest = new HttpRequestMessage(HttpMethod.Post, $"{_rootstockSettings.BaseUrl}/services/oauth2/token")
                {
                    Content = new FormUrlEncodedContent(
                    [
                        new KeyValuePair<string, string>("grant_type", "client_credentials"),
                    new KeyValuePair<string, string>("client_id", _rootstockSettings.ClientId),
                    new KeyValuePair<string, string>("client_secret", _rootstockSettings.ClientSecret)
                    ])
                };

                var response = _httpClient.SendAsync(tokenRequest).Result;

                if (response.IsSuccessStatusCode)
                {

                    var data = response.Content.ReadFromJsonAsync<RootstockTokenResponse>().Result;
                    return data.access_token;
                }
                else
                {
                    string errorMessage = response.Content.ReadAsStringAsync().Result;
                    throw new HttpRequestException($"Error on Rootstock authentication - {errorMessage}", null, response.StatusCode);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while processing the GetAccessToken for Rootstock");
                throw;
            }
        }
    }
}
