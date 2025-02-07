namespace Tilray.Integrations.Services.Rootstock.Service;

/// <summary>
/// A <see cref="DelegatingHandler"/> which adds the Authentication header onto outgoing Rootstock HTTP requests.
/// </summary>
public class RootstockAuthHandler : DelegatingHandler
{
    #region Private members

    private readonly RootstockSettings _rootstockSettings;
    private readonly HttpClient _httpClient;
    private string _accessToken;
    private readonly AsyncRetryPolicy<HttpResponseMessage> _policy;

    #endregion

    #region Constructors

    public RootstockAuthHandler(RootstockSettings rootstockSettings, HttpClient httpClient)
    {
        _rootstockSettings = rootstockSettings;
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
        var request = new HttpRequestMessage(HttpMethod.Post, $"{_rootstockSettings.BaseUrl}/services/oauth2/token")
        {
            Content = new FormUrlEncodedContent(
            [
                new KeyValuePair<string, string>("grant_type", "client_credentials"),
                new KeyValuePair<string, string>("client_id", _rootstockSettings.ClientId),
                new KeyValuePair<string, string>("client_secret", _rootstockSettings.ClientSecret)
            ])
        };

        var response = await _httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync();
        var tokenResponse = JsonConvert.DeserializeObject<TokenResponse>(content);
        _accessToken = tokenResponse.access_token;
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
