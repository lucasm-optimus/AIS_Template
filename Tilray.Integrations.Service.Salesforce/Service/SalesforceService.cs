using FluentResults;
using Microsoft.Extensions.Logging;
using Tilray.Integrations.Services.Salesforce.Service.Models;
using Tilray.Integrations.Services.Salesforce.Service.Queries;

namespace Tilray.Integrations.Services.Salesforce.Service
{
    public class SalesforceService : ISalesforceService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<SalesforceService> _logger;
        public SalesforceService(HttpClient httpClient, ILogger<SalesforceService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<Result<List<AuditItem>>> GetAuditItemsAsync(string reportDate)
        {
            var query = SalesforceQueries.GetSOXReportQuery(reportDate);

            var requestUrl = $"/services/data/v52.0/query?q={query}";
            var request = new HttpRequestMessage(HttpMethod.Get, requestUrl);

            var response = await _httpClient.SendAsync(request);
            var responseBody = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("Failed to fetch auditItems from salesforce. Status: {StatusCode}, Error: {Error}", response.StatusCode, responseBody);
                return Result.Fail<List<AuditItem>>($"Failed to fetch auditItems from salesforce. Error: {responseBody}");
            }

            var auditItems = JsonConvert.DeserializeObject<SOXReport>(responseBody);

            return Result.Ok(auditItems.Records);

        }
    }
}
