using System.Net;
using System.Net.Http.Headers;
using Polly;
using Polly.Retry;
using Tilray.Integrations.Services.Salesforce.Satrtup;

namespace Tilray.Integrations.Services.Salesforce.Service
{
    /// <summary>
    /// A <see cref="DelegatingHandler"/> which adds the Authentication header onto outgoing Salesforce HTTP requests.
    /// </summary>
    public class SalesforceAuthHandler : DelegatingHandler
    {
        #region Private members

        private readonly SalesforceSettings _salesforceSettings;
        private readonly HttpClient _httpClient;
        private string _accessToken;
        private readonly AsyncRetryPolicy<HttpResponseMessage> _policy;

        #endregion

        #region Constructors

        public SalesforceAuthHandler(SalesforceSettings salesforceSettings, HttpClient httpClient)
        {
            _salesforceSettings = salesforceSettings;
            _httpClient = httpClient;
            _policy = Policy
                .HandleResult<HttpResponseMessage>(r => r.StatusCode == HttpStatusCode.Unauthorized || r.StatusCode == HttpStatusCode.Forbidden)
                .RetryAsync(async (_, _) => await RefreshTokenAsync());
        }

        #endregion

        #region Private methods

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
            var request = new HttpRequestMessage(HttpMethod.Post, $"{_salesforceSettings.AuthorizationUrl}")
            {
                Content = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("grant_type", "client_credentials"),
                    new KeyValuePair<string, string>("client_id", _salesforceSettings.ClientId),  
                    new KeyValuePair<string, string>("client_secret", _salesforceSettings.ClientSecret)
                })
            };

            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var tokenResponse = JsonConvert.DeserializeObject<Dictionary<string, string>>(content);

            _accessToken = tokenResponse["access_token"];
        }

        #endregion

        #region Protected methods

        /// <inheritdoc cref="DelegatingHandler"/>
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            return await _policy.ExecuteAsync(async () =>
            {
                var accessToken = await GetAccessTokenAsync();
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

                return await base.SendAsync(request, cancellationToken);
            });
        }

        #endregion
    }
}
