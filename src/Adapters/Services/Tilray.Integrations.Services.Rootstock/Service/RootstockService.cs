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

    private async Task<Result<string>> GetChatterGroupIdAsync(string groupName)
    {
        var response = await httpClient.GetAsync($"{QueryUrl}?q={string.Format(RootstockQueries.GetChatterGroupIdQuery, groupName)}");

        if (!response.IsSuccessStatusCode)
        {
            string errorMessage = Helpers.GetErrorFromResponse(response);
            logger.LogError($"Failed to fetch Chatter Group ID. Error: {errorMessage}");
            return Result.Fail<string>(errorMessage);
        }

        var jsonResponse = await response.Content.ReadAsStringAsync();
        if (string.IsNullOrWhiteSpace(jsonResponse))
            return Result.Fail<string>("Failed to fetch Chatter Group ID");

        var records = JObject.Parse(jsonResponse)?["records"] as JArray;
        var groupId = records?.FirstOrDefault()?["Id"]?.ToString();

        return string.IsNullOrEmpty(groupId)
            ? Result.Fail<string>("Chatter Group ID not found")
            : Result.Ok(groupId);
    }

    private async Task<Result> PostChatterMessageAsync(string message, string groupName)
    {
        var groupResult = await GetChatterGroupIdAsync(groupName);
        if (groupResult.IsFailed)
        {
            logger.LogError("Failed to post to Chatter group {GroupName}: {Errors}", groupName, groupResult.Errors);
            return groupResult.ToResult();
        }

        var content = new
        {
            body = new
            {
                messageSegments = new object[] {
                    new { type = "Text", text = $"{message}\n" },
                    new { type = "Mention", id = groupResult.Value }
                }
            },
            feedElementType = "FeedItem",
            subjectId = groupResult.Value
        };

        var response = await httpClient.PostAsync(
            "/services/data/v59.0/chatter/feed-elements",
            Helpers.CreateStringContent(content)
        );

        if (!response.IsSuccessStatusCode)
        {
            var error = Helpers.GetErrorFromResponse(response);
            logger.LogError("Chatter post failed for {GroupName}: {Error}", groupName, error);
            return Result.Fail(error);
        }

        logger.LogInformation("Posted message to Chatter group {GroupName}", groupName);
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
            var errorMessage = $"The following customers could not be found: {string.Join(", ", invalidCustomers)}";
            logger.LogError(errorMessage);
            return Result.Fail(errorMessage);
        }

        return Result.Ok();
    }

    private async Task<Result<IEnumerable<string>>> ValidateItems(List<string> items)
    {
        if (items.Count == 0)
        {
            var errorMessage = $"No Line Items are provided";
            logger.LogError(errorMessage);
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
            var errorMessage = $"The following items could not be found: {string.Join(", ", invalidItems)}";
            logger.LogError(errorMessage);
            return Result.Fail(errorMessage);
        }

        return Result.Ok();
    }

    private async Task<Result<IEnumerable<string>>> ValidateUploadGroup(IEnumerable<string> uploadGroups)
    {
        var nullUploadGroups = uploadGroups
            .Where(string.IsNullOrWhiteSpace)
            .ToList();

        if (nullUploadGroups.Any())
        {
            var errorMessage = $"Some Upload Groups are null or empty";
            logger.LogError(errorMessage);
            return Result.Fail(errorMessage);
        }

        var validUploadGroups = uploadGroups.Where(ug => !string.IsNullOrWhiteSpace(ug));

        var tasks = validUploadGroups.Select(async uploadGroup =>
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
            string errorMessage = $"{response.StatusCode} (Details: '{responseContent}')"; ;
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

    private async Task ProcessExpensesAsync(IEnumerable<Expense> expenses, CompanyReference company, ExpensesProcessed expensesProcessed)
    {
        foreach (var expense in expenses)
        {
            var (debit, credit) = CreateJournalPair(company, expense);
            await ProcessJournalEntryAsync(debit, expense, company, expensesProcessed);
            await ProcessJournalEntryAsync(credit, expense, company, expensesProcessed);
        }
    }

    private async Task ProcessJournalEntryAsync(RootstockJournalEntry journalEntry, Expense expense, CompanyReference company, ExpensesProcessed expensesProcessed)
    {
        var result = await CreateAsync(journalEntry, "rstkf__jeato__c");
        if (result.IsFailed)
        {
            logger.LogError("Failed to create Journal Entry: {Errors}", Helpers.GetErrorMessage(result.Errors));
            expensesProcessed.ExpenseErrors.Add(ExpenseError.Create(company.Rootstock_Company__c, company.Company_Name__c, journalEntry.rstkf__jeato_date__c, journalEntry.rstkf__jeato_desc__c,
                $"{expense.ReportEntryDescription} {expense.JournalPayerPaymentTypeName}", journalEntry.rstkf__jeato_acct__c, journalEntry.rstkf__jeato_dramt__c, journalEntry.rstkf__jeato_cramt__c,
                journalEntry.rstkf__jeato_uploadgroup__c, Helpers.GetErrorMessage(result.Errors)));
        }
    }
    public async Task<Result<ExpensesProcessed>> CreateJournalEntryAsync(IEnumerable<Expense> expenses)
    {
        var expensesProcessed = new ExpensesProcessed();
        var companyResult = await GetAllCompanyReferencesAsync();

        if (companyResult.IsFailed)
            return Result.Fail<ExpensesProcessed>(companyResult.Errors);

        foreach (var company in companyResult.Value)
        {
            expensesProcessed.CompanyReference = company;
            var companyExpenses = expenses.Where(e => e.CleanCompanyCode == company.Concur_Company__c).ToList();

            if (company.CanProcessExpenses && companyExpenses.Count > 0)
                await ProcessExpensesAsync(companyExpenses, company, expensesProcessed);
        }

        return Result.Ok(expensesProcessed);
    }

    public async Task<Result> PostExpensesChatterMessageAsync(string companyNumber, int errorCount)
    {
        var message = $"The latest Journal Entry Upload for Expenses produced {errorCount} errors.";
        var groupName = $"{rootstockSettings.JournalEntryChatterGroupPrefix}{companyNumber}";
        return await PostChatterMessageAsync(message, groupName);
    }

    #endregion
}
