namespace Tilray.Integrations.Services.Rootstock.Service;

public class RootstockService(HttpClient client, RootstockSettings rootstockSettings, ILogger<RootstockService> logger) : IRootstockService
{
    #region Constants

    private const string QueryUrl = "services/data/v52.0/query";

    #endregion

    #region Private methods

    private async Task<Result<IEnumerable<T>>> GetObjectListAsync<T>(string query, string objectName)
    {
        HttpResponseMessage response = await client.GetAsync($"{QueryUrl}?q={query}");

        if (!response.IsSuccessStatusCode)
        {
            string errorMessage = $"Failed to fetch {objectName}. Error: {Helpers.GetErrorFromResponse(response)}";
            logger.LogError(errorMessage);
            return Result.Fail<IEnumerable<T>>(errorMessage);
        }

        var content = await response.Content.ReadAsStringAsync();
        var result = JsonConvert.DeserializeObject<JObject>(content);

        if (result["records"] is not JArray recordsArray || recordsArray.Count == 0)
        {
            return Result.Ok(Enumerable.Empty<T>());
        }

        logger.LogInformation("Total {TotalRecords} {ObjectName} found", recordsArray.Count, objectName);
        var records = JsonConvert.DeserializeObject<IEnumerable<T>>(recordsArray.ToString());
        return Result.Ok(records);
    }

    #endregion

    #region Public methods

    public async Task<Result<IEnumerable<CompanyReference>>> GetAllCompanyReferencesAsync()
    {
        return await GetObjectListAsync<CompanyReference>(
            RootstockQueries.GetAllCompanyReferenceQuery, "External_Company_Reference__c");
    }

    #endregion
}
