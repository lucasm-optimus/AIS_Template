using AutoMapper;
using Tilray.Integrations.Core.Domain.Aggregates.SalesOrders;
using Tilray.Integrations.Services.Rootstock.Service.Models;

namespace Tilray.Integrations.Services.Rootstock.Service;

public class RootstockService(HttpClient httpClient, RootstockSettings rootstockSettings, IMapper mapper,
    ILogger<RootstockService> logger) : IRootstockService
{
    #region Constants

    private const string QueryUrl = "services/data/v52.0/query";
    private const string SObjectUrl = "services/data/v59.0/sobjects";

    #endregion

    #region Private methods

    private async Task<Result<T>> GetObjectByIdAsync<T>(string id, string query, string objectName)
    {
        var formattedQuery = string.Format(query, id);
        var recordsResult = await FetchRecordsAsync<T>(formattedQuery, objectName);

        if (recordsResult.IsFailed)
        {
            logger.LogError($"Failed to fetch {objectName} with ID: {id}");
            return Result.Fail<T>($"Failed to fetch {objectName} with ID: {id}");
        }

        if (recordsResult.Value.Count == 0)
        {
            return Result.Ok<T>(default);
        }

        return Result.Ok(recordsResult.Value.First().ToObject<T>());
    }

    private async Task<Result<IEnumerable<T>>> GetObjectListAsync<T>(string query, string objectName)
    {
        var recordsResult = await FetchRecordsAsync<T>(query, objectName);
        return recordsResult.IsSuccess
            ? Result.Ok(recordsResult.Value.ToString().ToObject<IEnumerable<T>>())
            : Result.Fail<IEnumerable<T>>(recordsResult.Errors);
    }

    private async Task<Result<JArray>> FetchRecordsAsync<T>(string query, string objectName)
    {
        var response = await httpClient.GetAsync($"{QueryUrl}?q={query}");
        if (!response.IsSuccessStatusCode)
        {
            string errorMessage = Helpers.GetErrorFromResponse(response);
            logger.LogError($"Failed to fetch {objectName}. Error: {errorMessage}");
            return Result.Fail<JArray>(errorMessage);
        }

        var content = await response.Content.ReadAsStringAsync();
        var result = JObject.Parse(content);

        return result["records"] is JArray records && records.Count > 0
            ? Result.Ok(records)
            : Result.Ok(new JArray());
    }

    private async Task<Result<string>> CreateAsync<T>(T entity, string objectName)
    {
        var json = Helpers.CreateStringContent(entity);
        var response = await httpClient.PostAsync($"{SObjectUrl}/{objectName}", json);

        if (!response.IsSuccessStatusCode)
            return Result.Fail<string>($"{typeof(T).Name} creation failed. Error: {Helpers.GetErrorFromResponse(response)}");

        var responseBody = await response.Content.ReadAsStringAsync();
        var id = JObject.Parse(responseBody)?["id"]?.ToString();

        return Result.Ok(id ?? string.Empty);
    }

    private async Task<Result<IEnumerable<string>>> ValidateCustomers(IEnumerable<string> customers)
    {
        var tasks = customers.Select(async customer =>
        {
            var result = await GetObjectByIdAsync<RootstockCustomer>(
                customer,
                RootstockQueries.GetCustomerByIdQuery,
                "rstk__socust__c");

            return result.IsFailed || result.Value == null ? customer : null;
        });

        var results = await Task.WhenAll(tasks);
        var invalidCustomers = results.Where(c => c != null);

        if (invalidCustomers.Any())
        {
            var errorMessage = $"The following customers could not be found: {string.Join(", ", invalidCustomers)}";
            logger.LogError(errorMessage);
            return Result.Fail(errorMessage);
        }

        return Result.Ok();
    }

    private async Task<Result<IEnumerable<string>>> ValidateItems(List<string> items)
    {
        var tasks = items.Select(async item =>
        {
            var itemResult = await GetObjectByIdAsync<RootstockItem>(item, RootstockQueries.GetItemByIdQuery, "rstk__peitem__c");
            return itemResult.IsFailed || itemResult.Value == null ? item : null;
        });

        var results = await Task.WhenAll(tasks);
        var invalidItems = results.Where(i => i != null);

        if (invalidItems.Any())
        {
            var errorMessage = $"The following items could not be found: {string.Join(", ", invalidItems)}";
            logger.LogError(errorMessage);
            return Result.Fail(errorMessage);
        }

        return Result.Ok();
    }

    private async Task<Result<IEnumerable<string>>> ValidateUploadGroup(IEnumerable<string> uploadGroups)
    {
        var tasks = uploadGroups.Select(async uploadGroup =>
        {
            var uploadGroupResult = await GetObjectByIdAsync<RootstockSalesOrder>(uploadGroup, RootstockQueries.GetUploadGroupByIdQuery, "rstk__soapi__c");
            return uploadGroupResult.IsFailed || uploadGroupResult.Value != null ? uploadGroup : null;
        });

        var results = await Task.WhenAll(tasks);
        var invalidUploadGroups = results.Where(g => g != null);

        if (invalidUploadGroups.Any())
        {
            var errorMessage = $"The following upload groups have already been used: {string.Join(", ", invalidUploadGroups)}";
            logger.LogError(errorMessage);
            return Result.Fail(errorMessage);
        }

        return Result.Ok();
    }

    private async Task<Result<string>> CreateSalesOrderHeaderAsync(SalesOrder salesOrder)
    {
        var rootstockOrder = mapper.Map<RootstockSalesOrder>(salesOrder);
        var result = await CreateAsync(rootstockOrder, "rstk__soapi__c");
        if (result.IsFailed)
        {
            var error = $"Failed to create Sales Order. Error: {result.Errors}. Customer: {salesOrder.Customer}, UploadGroup: {salesOrder.UploadGroup}";
            logger.LogError(error);
            return Result.Fail(error);
        }

        logger.LogInformation($"Sales Order created. SalesOrderId: {result.Value}, Customer: {salesOrder.Customer}, UploadGroup: {salesOrder.UploadGroup}");
        return Result.Ok(result.Value);
    }

    private async Task<Result> CreateSalesOrderLinesAsync(SalesOrder salesOrder, string salesOrderId)
    {
        var rootstockOrderResult = await GetObjectByIdAsync<RootstockSalesOrder>(salesOrderId, RootstockQueries.GetSalesOrderByIdQuery, "rstk__soapi__c");
        if (rootstockOrderResult.IsFailed) { return Result.Fail("Sales Order not found"); }
        var errors = new List<string>();

        foreach (var lineItem in salesOrder.LineItems.Skip(1))
        {
            var orderLine = mapper.Map<RootstockSalesOrder>((lineItem, rootstockOrderResult.Value.SoapiSohdr, salesOrder));
            var result = await CreateAsync(orderLine, "rstk__soapi__c");

            if (result.IsFailed)
            {
                errors.Add($"ItemNumber: {lineItem.ItemNumber}, Error: {result.Errors}");
            }
            else
            {
                logger.LogInformation($"Sales Order Line created. SalesOrderId: {result.Value}, Customer: {salesOrder.Customer}, UploadGroup: {salesOrder.UploadGroup}, Item: {lineItem.ItemNumber}");
            }
        }

        return errors.Count == 0
            ? Result.Ok()
            : Result.Fail($"Failed to create the following line items: {string.Join("; ", errors)}");
    }

    #endregion

    #region Public methods

    public async Task<Result<IEnumerable<CompanyReference>>> GetAllCompanyReferencesAsync()
    {
        return await GetObjectListAsync<CompanyReference>(
            RootstockQueries.GetAllCompanyReferenceQuery, "External_Company_Reference__c");
    }

    public async Task<Result<IEnumerable<SalesOrder>>> ValidateSalesOrders(IEnumerable<SalesOrder> salesOrders)
    {
        var customers = salesOrders.Select(order => order.Customer).Distinct();
        var invalidCustomersResult = await ValidateCustomers(customers);
        if (invalidCustomersResult.IsFailed)
        {
            return Result.Fail(invalidCustomersResult.Errors);
        }

        var distinctItems = salesOrders
            .SelectMany(order => order.LineItems.Select(lineItem => $"{order.Division}_{lineItem.ItemNumber}"))
            .Distinct()
            .ToList();

        var invalidItemsResult = await ValidateItems(distinctItems);
        if (invalidItemsResult.IsFailed)
        {
            return Result.Fail(invalidItemsResult.Errors);
        }

        var uploadGroups = salesOrders.Select(order => order.UploadGroup).Distinct().ToList();
        var invalidUploadGroupsResult = await ValidateUploadGroup(uploadGroups);
        if (invalidUploadGroupsResult.IsFailed)
        {
            return Result.Fail(invalidUploadGroupsResult.Errors);
        }

        return Result.Ok(salesOrders);
    }

    public async Task<Result> CreateSalesOrderAsync(SalesOrder salesOrder)
    {
        var headerResult = await CreateSalesOrderHeaderAsync(salesOrder);
        return headerResult.IsFailed
            ? Result.Fail(headerResult.Errors)
            : await CreateSalesOrderLinesAsync(salesOrder, headerResult.Value);
    }

    #endregion
}
