using Polly.Retry;
using Polly;
using System.Net.Http.Headers;
using System.Net;
using Newtonsoft.Json;
using Tilray.Integrations.Services.SAPConcur.Startup;
using Tilray.Integrations.Services.SAPConcur.Service.Model;

namespace Tilray.Integrations.Services.SAPConcur.Service
{
    /// <summary>
    /// A <see cref="DelegatingHandler"/> which adds the Authentication header onto outgoing SAP Concur HTTP requests.
    /// </summary>
    internal sealed class SAPConcurAuthHandler : DelegatingHandler
    {
        #region Private members

        private readonly SAPConcurSettings _settings;
        private readonly HttpClient _httpClient;
        private string _accessToken;
        private readonly AsyncRetryPolicy<HttpResponseMessage> _policy;

        #endregion

        #region Constructors

        public SAPConcurAuthHandler(SAPConcurSettings settings, HttpClient httpClient)
        {
            _settings = settings;
            _httpClient = httpClient;
            _policy = Policy
                .HandleResult<HttpResponseMessage>(r => r.StatusCode is HttpStatusCode.Unauthorized or HttpStatusCode.Forbidden)
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
            var request = new HttpRequestMessage(HttpMethod.Post, _settings.TokenEndpoint)
            {
                Content = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("client_id", _settings.ClientId),
                    new KeyValuePair<string, string>("client_secret", _settings.ClientSecret),
                    new KeyValuePair<string, string>("grant_type", "refresh_token"),
                    new KeyValuePair<string, string>("refresh_token", _settings.RefreshToken)
                })
            };

            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var tokenResponse = JsonConvert.DeserializeObject<SAPConcurTokenResponse>(content);
            _accessToken = tokenResponse.AccessToken;
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
