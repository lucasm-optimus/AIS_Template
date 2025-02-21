using Tilray.Integrations.Services.Rootstock.Service.Queries;
using Tilray.Integrations.Services.Rootstock.Startup;

namespace Tilray.Integrations.Services.Rootstock.Service;

public class RootstockService(HttpClient httpClient, RootstockSettings rootstockSettings, RootstockGLAccountsSettings glAccountsSettings,
    IMapper mapper, ILogger<RootstockService> logger) : IRootstockService
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
            return Result.Fail<T>(recordsResult.Errors);

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

        if (result["records"] is JArray records && records.Count > 0)
        {
            logger.LogInformation("Fetched {RecordCount} records for {ObjectName}.", records.Count, objectName);
            return Result.Ok(records);
        }
        else
        {
            logger.LogWarning("No records found for {ObjectName}. Returning empty array.", objectName);
            return Result.Ok(new JArray());
        }
    }

    private async Task<Result<string>> CreateAsync<T>(T entity, string objectName)
    {
        var json = Helpers.CreateStringContent(entity);
        var response = await httpClient.PostAsync($"{SObjectUrl}/{objectName}", json);

        if (!response.IsSuccessStatusCode)
        {
            string errorMessage = $"{objectName} creation failed. Error: {Helpers.GetErrorFromResponse(response)}";
            logger.LogError(errorMessage);
            return Result.Fail<string>(Helpers.GetErrorFromResponse(response));
        }

        var responseBody = await response.Content.ReadAsStringAsync();
        var id = JObject.Parse(responseBody)?["id"]?.ToString();
        logger.LogInformation("Successfully created {EntityType} with ID: {ObjectId}",
            objectName, id);

        return Result.Ok(id ?? string.Empty);
    }

    private async Task<Result<bool>> CheckForExistingMessageInChatterGroupAsync(string groupId, string message)
    {
        var result = await GetObjectListAsync<RootstockFeedItem>(string.Format(RootstockQueries.GetChatterBodyQuery, groupId), "FeedItem");
        if (result.IsFailed)
        {
            logger.LogError("Failed to fetch Chatter messages. Error: {Error}", result.Errors);
            return Result.Fail<bool>(result.Errors);
        }

        var hasDuplicate = result.Value?.Any(r => r.Body.Contains(message) == true) ?? false;
        return Result.Ok(hasDuplicate);
    }

    private async Task<Result<string>> GetChatterGroupIdAsync(string groupName)
    {
        var result = await GetObjectByIdAsync<RootstockCollaborationGroup>(groupName, RootstockQueries.GetChatterGroupIdQuery, "CollaborationGroup");
        if (result.IsFailed)
        {
            logger.LogError($"Failed to fetch Chatter Group ID. Error: {Helpers.GetErrorMessage(result.Errors)}");
            return Result.Fail<string>(result.Errors);
        }

        if(result.Value == null)
        {
            string errorMessage = $"Chatter group {groupName} not found.";
            logger.LogWarning(errorMessage);
            return Result.Fail(errorMessage);
        }

        return Result.Ok(result.Value.Id);
    }

    private async Task<Result> PostMessageToChatterAsync(string message, string groupName)
    {
        var groupResult = await GetChatterGroupIdAsync(groupName);
        if (groupResult.IsFailed)
        {
            return groupResult.ToResult();
        }

        var duplicateCheckResult = await CheckForExistingMessageInChatterGroupAsync(groupResult.Value, message);
        if (duplicateCheckResult.IsFailed)
        {
            logger.LogWarning("failed to fetch messages for Chatter group {groupName} with Id {GroupId}. Proceeding with message posting. Error details: {ErrorDetails}",
                    groupName, groupResult.Value, duplicateCheckResult.Errors);
        }
        else if (duplicateCheckResult.Value)
        {
            logger.LogInformation("Duplicate message detected in Chatter group {groupName} with Id {GroupId}. Skipping message posting.", groupName, groupResult.Value);
            return Result.Ok();
        }

        var content = RootstockChatterFeedItem.Create(message, groupResult.Value);
        var response = await httpClient.PostAsync("/services/data/v59.0/chatter/feed-elements", Helpers.CreateStringContent(content));

        if (!response.IsSuccessStatusCode)
        {
            var errorMessage = $"Failed to post message to Chatter group {groupName} with Id {groupResult.Value}. Error: {Helpers.GetErrorFromResponse(response)}";
            logger.LogError(errorMessage);
            return Result.Fail(errorMessage);
        }

        logger.LogInformation("Successfully posted message to Chatter group {groupName} with Id {GroupId}", groupName, groupResult.Value);
        return Result.Ok();
    }

    private async Task<Result<IEnumerable<string>>> ValidateCustomers(IEnumerable<string> customers)
    {
        var invalidCustomers = customers
            .Where(string.IsNullOrWhiteSpace)
            .Select(_ => "Null")
            .ToList();

        var validCustomers = customers.Where(c => !string.IsNullOrWhiteSpace(c));

        var tasks = validCustomers.Select(async customer =>
        {
            var result = await GetObjectByIdAsync<RootstockCustomer>(
                customer,
                RootstockQueries.GetCustomerByIdQuery,
                "rstk__socust__c");

            return result.IsFailed || result.Value == null ? customer : null;
        });

        var results = await Task.WhenAll(tasks);
        invalidCustomers.AddRange(results.Where(c => c != null));

        if (invalidCustomers.Any())
        {
            var errorMessage = $"The following customers could not be found: {Helpers.GetErrorMessage(invalidCustomers)}";
            logger.LogWarning(errorMessage);
            return Result.Fail(errorMessage);
        }

        return Result.Ok();
    }

    private async Task<Result<IEnumerable<string>>> ValidateItems(IEnumerable<string> items)
    {
        if (!items.Any())
        {
            var errorMessage = $"No Line Items are provided";
            logger.LogWarning(errorMessage);
            return Result.Fail(errorMessage);
        }

        var tasks = items.Select(async item =>
        {
            var itemResult = await GetObjectByIdAsync<RootstockItem>(item, RootstockQueries.GetItemByIdQuery, "rstk__peitem__c");
            return itemResult.IsFailed || itemResult.Value == null ? item : null;
        });

        var results = await Task.WhenAll(tasks);
        var invalidItems = results.Where(i => i != null);

        if (invalidItems.Any())
        {
            var errorMessage = $"The following items could not be found: {Helpers.GetErrorMessage(invalidItems)}";
            logger.LogWarning(errorMessage);
            return Result.Fail(errorMessage);
        }

        return Result.Ok();
    }

    private async Task<Result<IEnumerable<string>>> ValidateUploadGroup(IEnumerable<string> uploadGroups)
    {
        var nullUploadGroups = uploadGroups
            .Where(string.IsNullOrWhiteSpace);

        if (nullUploadGroups.Any())
        {
            var errorMessage = $"Some Upload Groups are null or empty";
            logger.LogWarning(errorMessage);
            return Result.Fail(errorMessage);
        }

        var tasks = uploadGroups.Select(async uploadGroup =>
        {
            var uploadGroupResult = await GetObjectByIdAsync<RootstockSalesOrder>(uploadGroup,
                RootstockQueries.GetUploadGroupByIdQuery, "rstk__soapi__c");
            return uploadGroupResult.IsFailed || uploadGroupResult.Value != null ? uploadGroup : null;
        });

        var results = await Task.WhenAll(tasks);
        var invalidUploadGroups = results.Where(g => g != null);

        if (invalidUploadGroups.Any())
        {
            var errorMessage = $"The following upload groups have already been used: {Helpers.GetErrorMessage(invalidUploadGroups)}";
            logger.LogWarning(errorMessage);
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
            : Result.Fail($"Failed to create the following line items: {Helpers.GetErrorMessage(errors)}");
    }

    private async Task<Result<string>> PostRootstockDataAsync(string objectName, object obj)
    {
        var content = new StringContent(JsonConvert.SerializeObject(obj), Encoding.UTF8, "application/json");
        var response = await httpClient.PostAsync($"{SObjectUrl}/{objectName}", content);
        var responseContent = JsonConvert.DeserializeObject<dynamic>(await response.Content.ReadAsStringAsync());

        if (response.IsSuccessStatusCode)
        {
            logger.LogInformation($"[Rootstock Create] Successfully created entry in entity {objectName}.");
            var recordId = responseContent["id"];
            return Result.Ok(recordId.Value);
        }
        else
        {
            string errorMessage = $"{response.StatusCode} (Details: '{responseContent}')";
            logger.LogError($"[Rootstock Create] Failed to create entry in entity {objectName}. Error: {errorMessage}.");
            return Result.Fail(errorMessage);
        }
    }

    private async Task<Result<dynamic>> ExecuteQueryAsync(string query)
    {
        HttpResponseMessage response = await httpClient.GetAsync($"{QueryUrl}?q={query}");
        var responseContent = JsonConvert.DeserializeObject<dynamic>(await response.Content.ReadAsStringAsync());
        if (response.IsSuccessStatusCode)
        {
            logger.LogInformation($"Successfully fetched data. Query: {query}");
            return Result.Ok(responseContent["records"]);
        }
        else
        {
            string errorMessage = $"{response.StatusCode} (Details: '{responseContent}')";
            logger.LogError($"Failed to fetch. Error: {errorMessage}");
            return Result.Fail(errorMessage);
        }
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
        logger.LogInformation($"Validating {salesOrders.Count()} sales orders.");

        var customers = salesOrders.Select(order => order.Customer).Distinct();
        var invalidCustomersResult = await ValidateCustomers(customers);
        if (invalidCustomersResult.IsFailed)
        {
            return Result.Fail(invalidCustomersResult.Errors);
        }

        var distinctItems = salesOrders
            .SelectMany(order => order.LineItems.Select(lineItem => $"{order.Division}_{lineItem.ItemNumber}"))
            .Distinct();

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
        var salesOrderHeaderResult = await CreateSalesOrderHeaderAsync(salesOrder);
        return salesOrderHeaderResult.IsFailed
            ? Result.Fail(salesOrderHeaderResult.Errors)
            : await CreateSalesOrderLinesAsync(salesOrder, salesOrderHeaderResult.Value);
    }

    public async Task<Result<string?>> CreateCustomer(RstkCustomer customer)
    {
        var createdCustomerResult = await PostRootstockDataAsync(Constants.Rootstock.TableNames.Customer, customer);
        return createdCustomerResult.IsFailed
            ? createdCustomerResult
            : RootstockCustomerCreateResponse.GetRecordId(createdCustomerResult.Value);
    }

    public async Task<Result<RstkCustomerInfoResponse?>> GetCustomerInfo(string sfAccountId)
    {
        var formattedQuery = string.Format(RootstockQueries.GetCustomerBySfAccountQuery, sfAccountId);
        var responseResult = await ExecuteQueryAsync(formattedQuery);
        return responseResult.IsFailed
            ? responseResult
            : RstkCustomerInfoResponse.MapFromPayload(responseResult.Value);
    }

    public async Task<Result<string?>> CreateCustomerAddress(RstkCustomerAddress customerAddress)
    {
        return await PostRootstockDataAsync(Constants.Rootstock.TableNames.CustomerAddress, customerAddress);
    }

    public async Task<int?> GetCustomerAddressNextSequence(string customerNo)
    {
        var formattedQuery = string.Format(RootstockQueries.GetMaxAddressSequenceByCustomerNo, customerNo);
        var responseResult = await ExecuteQueryAsync(formattedQuery);
        return RstkCustomerAddressInfoResponse.GetNextSequenceNumber(responseResult.Value);
    }

    public async Task<Result<RstkCustomerAddressInfoResponse>> GetCustomerAddressInfo(string customerNo, string addressType)
    {
        //await string.Format(, customerNo)
        var responseResult = addressType switch
        {
            "ShipTo" => await ExecuteQueryAsync(string.Format(RootstockQueries.GetShipToCustomerAddressInfoQuery, customerNo)),
            "BillTo" => await ExecuteQueryAsync(string.Format(RootstockQueries.GetDefaultBillToCustomerAddressInfoQuery, customerNo)),
            "Acknowledgement" => await ExecuteQueryAsync(string.Format(RootstockQueries.GetAcknowledgementCustomerAddressInfoQuery, customerNo)),
            "Installation" => await ExecuteQueryAsync(string.Format(RootstockQueries.GetInstallationCustomerAddressInfoQuery, customerNo)),
            _ => throw new Exception("Invalid address type.")
        };

        if (responseResult.IsFailed)
        {
            logger.LogError($"Failed to fetch customer address info. Error: {responseResult.Errors}");
            return Result.Fail<RstkCustomerAddressInfoResponse>(responseResult.Errors);
        }

        var result = RstkCustomerAddressInfoResponse.MapFromPayload(responseResult.Value);
        return result.IsSuccess
            ? result
            : Result.Fail<RstkCustomerAddressInfoResponse>(result.Errors);

    }

    public async Task<Result<RstkCustomerAddressInfoResponse>> GetCustomerAddressInfo(string customerNo, string address, string city, string state, string zip)
    {
        string query =
            "SELECT  ID, rstk__externalid__c, Name, External_Customer_Number__c  FROM rstk__socaddr__c " +
            $" WHERE rstk__socaddr_address1__c = '{address.Replace("'", "\\'")}' " +
            $" AND rstk__externalid__c LIKE '{customerNo}_%'" +
            (city != null ? " AND rstk__socaddr_city__c = '" + city.Replace("'", "\\'") + "'" : "") +
            (state != null ? " AND rstk__socaddr_state__c = '" + state + "'" : "") +
            (zip != null ? " AND rstk__socaddr_zip__c = '" + zip + "'" : "") +
            " AND rstk__socaddr_useasshipto__c = true";

        var responseResult = await ExecuteQueryAsync(query);
        return RstkCustomerAddressInfoResponse.MapFromPayload(responseResult.Value);
    }

    public async Task<Result<string?>> CreateSalesOrder(RstkSalesOrder salesOrder)
    {
        return await PostRootstockDataAsync(Constants.Rootstock.TableNames.SalesOrder, salesOrder);
    }

    public async Task<Result<string?>> CreateSalesOrderLineItem(RstkSalesOrderLineItem salesOrderLineItem)
    {
        return await PostRootstockDataAsync(Constants.Rootstock.TableNames.SalesOrder, salesOrderLineItem);
    }

    public async Task<Result<string?>> CreatePrePayment(RstkSalesOrderPrePayment prePayment)
    {
        return await PostRootstockDataAsync(Constants.Rootstock.TableNames.Prepayment, prePayment);
    }

    public async Task<Result<string?>> CreatePrePayment(RstkSyDataPrePayment prePayment)
    {
        return await PostRootstockDataAsync(Constants.Rootstock.TableNames.SyData, prePayment);
    }

    public async Task<bool> SalesOrderExists(string customerReferenceNumber)
    {
        var formattedQuery = string.Format(RootstockQueries.GetListofCustomerReferencesByCustomerReferences, customerReferenceNumber);
        var responseResult = await ExecuteQueryAsync(formattedQuery);
        return responseResult.Value.Count > 0;
    }

    public async Task<Result<string>> GetIdFromExternalColumnReference(string objectName, string externalIdColumnName, string externalId)
    {
        var formattedQuery = string.Format(RootstockQueries.GetIdByExternalReferenceId, objectName, externalIdColumnName, externalId);
        var responseResult = await ExecuteQueryAsync(formattedQuery);
        return responseResult.IsFailed
            ? Result.Fail<string>(responseResult.Errors)
            : Result.Ok(responseResult.Value[0]["Id"].ToString());
    }

    public async Task<Result<string>> GetSoHdr(string soapiId)
    {
        var formattedQuery = string.Format(RootstockQueries.GetSoHdrFromSoApi, soapiId);
        var responseResult = await ExecuteQueryAsync(formattedQuery);
        return responseResult.IsFailed
            ? Result.Fail<string>(responseResult.Errors)
            : Result.Ok(responseResult.Value[0]["rstk__soapi_sohdr__c"].Value);
    }

    private (RootstockJournalEntry Debit, RootstockJournalEntry Credit) CreateJournalPair(CompanyReference company,
        Expense expense)
    {
        return expense switch
        {
            { IsTaxExpense: true } => RootstockJournalEntry.CreateTaxPair(company, expense, expense.IsNegative,
                rootstockSettings.IntegrationUserName, glAccountsSettings),
            { IsQstsExpense: true } => RootstockJournalEntry.CreateQSTSPair(company, expense, expense.IsNegative,
                rootstockSettings.IntegrationUserName, glAccountsSettings),
            _ => RootstockJournalEntry.CreateExpensePair(company, expense, expense.IsNegative,
                rootstockSettings.IntegrationUserName, glAccountsSettings)
        };
    }

    public async Task<Result<List<ExpenseError>>> CreateJournalEntryAsync(Expense expense, CompanyReference company)
    {
        logger.LogInformation($"Expense payload: {expense.ToJsonString()}");

        var (debit, credit) = CreateJournalPair(company, expense);
        var errors = new List<ExpenseError>();

        foreach (var entry in new[] { debit, credit })
        {
            var result = await CreateAsync(entry, "rstkf__jeato__c");
            if (!result.IsFailed) continue;

            errors.Add(ExpenseError.Create(
                company.Rootstock_Company__c,
                company.Company_Name__c,
                entry.rstkf__jeato_date__c,
                entry.rstkf__jeato_desc__c,
                $"{expense.ReportEntryDescription} {expense.JournalPayerPaymentTypeName}",
                entry.rstkf__jeato_acct__c,
                entry.rstkf__jeato_dramt__c,
                entry.rstkf__jeato_cramt__c,
                entry.rstkf__jeato_uploadgroup__c,
                Helpers.GetErrorMessage(result.Errors)));
        }

        return Result.Ok(errors);
    }

    public async Task<Result> PostExpenseMessageToChatterAsync(string companyNumber, int errorCount)
    {
        var message = $"The latest Journal Entry Upload for Expenses produced {errorCount} errors.";
        var groupName = $"{rootstockSettings.JournalEntryChatterGroupPrefix}{companyNumber}";
        return await PostMessageToChatterAsync(message, groupName);
    }

    #endregion
}
