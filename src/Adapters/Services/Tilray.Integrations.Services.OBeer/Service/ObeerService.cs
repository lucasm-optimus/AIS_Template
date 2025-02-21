namespace Tilray.Integrations.Services.OBeer.Service;

public class ObeerService(HttpClient client, ISnowflakeRepository snowflakeRepository, ObeerSettings obeerSettings,
    ILogger<ObeerService> logger, IMapper mapper) : IObeerService
{
    #region Private methods

    private async Task<Result> CreateInvoiceAsync(ObeerInvoice obeerInvoice)
    {
        string apiUrl = $"api?APICommand={obeerSettings.APICommand}&EncompassID={obeerSettings.EncompassId}&APIToken={obeerSettings.APIToken}";

        var jsonContent = obeerInvoice.ToJsonString();
        logger.LogInformation("Obeer Invoice payload: {ObeerInvoice}", jsonContent);

        var content = Helpers.CreateStringContent(jsonContent);

        HttpResponseMessage response = await client.PostAsync(apiUrl, content);
        if (response.IsSuccessStatusCode)
        {
            logger.LogInformation(@$"Obeer Invoice created successfully. SAPConcurInvoiceId: {obeerInvoice?.Import?.APInvoice?.FirstOrDefault()?.ConcurOrderID},
                SAPConcurInvoiceNumber: {obeerInvoice?.Import?.APInvoice?.FirstOrDefault()?.CustomerRefNo},
                ResponseMessage: {await response.Content.ReadAsStringAsync()}");
            return Result.Ok();
        }

        string errorMessage = $@"Failed to create invoice in Obeer. SAPConcurInvoiceId: {obeerInvoice?.Import?.APInvoice?.FirstOrDefault()?.ConcurOrderID},
            SAPConcurInvoiceNumber: {obeerInvoice?.Import?.APInvoice?.FirstOrDefault()?.CustomerRefNo},
            Error: {Helpers.GetErrorFromResponse(response)}";
        logger.LogError(errorMessage);
        return Result.Fail(Helpers.GetErrorFromResponse(response));
    }

    private async Task<IEnumerable<GrpoDetails>> GetGrpoDetails(MatchedPurchaseOrderReceipt grpo)
    {
        return await snowflakeRepository.GetGrpoDetailsAsync(
            grpo.PODocNum, grpo.GrnParts[1], grpo.GrnParts[3], grpo.GRPOLineNum);
    }

    private async Task ProcessGrpoLineItemsAsync(Invoice invoice, IEnumerable<LineItem> grpoLineItems,
        List<GrpoLineItemError> errorsGrpo)
    {
        if (!grpoLineItems.Any())
        {
            logger.LogInformation("No GRPO line items to process for Invoice {InvoiceId}", invoice.ID);
            return;
        }

        List<Item> obeerItems = [];
        foreach (var lineItem in grpoLineItems)
        {
            var remainingQty = lineItem.Quantity;
            var grpoCount = lineItem.MatchedPurchaseOrderReceipts.MatchedPurchaseOrderReceipt.Count();

            foreach (var grpo in lineItem.MatchedPurchaseOrderReceipts.MatchedPurchaseOrderReceipt)
            {
                if (!grpo.IsValidGrn)
                {
                    logger.LogError("Invalid GRN format for GRN {GoodsReceiptNumber}, Line Item {LineItemId}, Invoice {InvoiceId}",
                        grpo.GoodsReceiptNumber, lineItem.LineItemId, invoice.ID);
                    errorsGrpo.Add(ErrorFactory.CreateGrpoLineItemError(invoice, grpo.GoodsReceiptNumber));
                    continue;
                }

                var details = await GetGrpoDetails(grpo);
                if (details == null || !details.Any())
                {
                    logger.LogError("GRPO details not found for GRN {GoodsReceiptNumber}, line item {LineItemId}, Invoice {InvoiceId}",
                        grpo.GoodsReceiptNumber, lineItem.LineItemId, invoice.ID);
                    errorsGrpo.Add(ErrorFactory.CreateGrpoLineItemError(invoice, grpo.GoodsReceiptNumber));
                    continue;
                }

                grpo.UpdateGrpo(details.First().OpenQty, ref remainingQty, ref grpoCount);
                obeerItems.Add(mapper.Map<Item>((lineItem, grpo)));
            }
        }

        await PostGrpoItemsToObeer(invoice, grpoLineItems, obeerItems, errorsGrpo);
    }

    private async Task PostGrpoItemsToObeer(Invoice invoice, IEnumerable<LineItem> grpoLineItems, IEnumerable<Item> obeerItems, List<GrpoLineItemError> errorsGrpo)
    {
        if (!obeerItems.Any())
        {
            logger.LogInformation("No valid GRPO items to post to Obeer for Invoice {InvoiceId}", invoice.ID);
            return;
        }

        var items = obeerItems.Where(x => x.Type == GRPOType.Item);
        var services = obeerItems.Where(x => x.Type == GRPOType.Service);

        await ProcessGrpoGroup(invoice, grpoLineItems, items, "dDocument_Items", errorsGrpo);
        await ProcessGrpoGroup(invoice, grpoLineItems, services, "dDocument_Service", errorsGrpo);
    }

    private async Task ProcessGrpoGroup(Invoice invoice, IEnumerable<LineItem> grpoLineItems, IEnumerable<Item> obeerItems, string documentType,
        List<GrpoLineItemError> errorsGrpo)
    {
        if (!obeerItems.Any()) return;

        var itemGroups = obeerItems.GroupBy(item => item.PODocNum);
        foreach (var itemGroup in itemGroups)
        {
            logger.LogInformation("Processing PODocNum {PODocNum} with {ItemCount} items for Invoice {InvoiceId}",
                itemGroup.Key, itemGroup.Count(), invoice.ID);

            var subGroupedItems = itemGroup
                .GroupBy(item =>
                    $"{item.GRPODocNum}_{item.GRPOLineNum}_{item.UnitPrice}"
                )
                .Select(Item.CreateFromGroup);

            var obeerInvoice = mapper.Map<ObeerInvoice>((invoice, grpoLineItems, subGroupedItems, documentType));
            var result = await CreateInvoiceAsync(obeerInvoice);
            if (result.IsFailed)
            {
                errorsGrpo.AddRange(obeerInvoice.Import.Items.Select(item =>
                    ErrorFactory.CreateGrpoLineItemError(item, obeerInvoice.Import.APInvoice.FirstOrDefault(), Helpers.GetErrorMessage(result.Errors))));
            }
        }
    }

    private async Task SetGlAccountForLineItemsAsync(Invoice invoice, IEnumerable<LineItem> lineItems)
    {
        foreach (var lineItem in lineItems)
        {
            var segments = lineItem.Custom4?.Split('-');
            if (segments is { Length: >= 2 })
            {
                var result = await snowflakeRepository.GetAcctCodeAsync(
                    segments[0],
                    segments[1]);
                if (result == null)
                {
                    logger.LogWarning("GL account not found for segments {Segment1}-{Segment2} in lineItem {LineItemId} for Invoice {InvoiceId}",
                        segments[0], segments[1], lineItem.LineItemId, invoice.ID);
                }
                else
                    lineItem.SetGlAccount(result);
            }
            else
            {
                logger.LogWarning("Invalid Custom4 format in line item {LineItem} for Invoice {InvoiceId}", lineItem.LineItemId, invoice.ID);
            }
        }
    }

    private async Task PostNonPOLineItemsAsync(Invoice invoice, IEnumerable<LineItem> nonPOLineItems, bool hasGrpoLines, List<NonPOLineItemError> errorsNonPO)
    {
        if (!nonPOLineItems.Any())
        {
            logger.LogInformation("No NonPO line items to process for Invoice {InvoiceId}", invoice.ID);
            return;
        }

        logger.LogInformation("Posting NonPO line items to Obeer for invoice {InvoiceId} with InvoiceNumber {InvoiceNumber}", invoice.ID, invoice.InvoiceNumber);
        var obeerInvoice = mapper.Map<ObeerInvoice>((invoice, nonPOLineItems, "dDocument_Service", hasGrpoLines));
        var result = await CreateInvoiceAsync(obeerInvoice);
        if (result.IsFailed)
        {
            errorsNonPO.AddRange(obeerInvoice.Import.Items.Select(item =>
                ErrorFactory.CreateNonPOLineItemError(item, obeerInvoice.Import.APInvoice.FirstOrDefault(), Helpers.GetErrorMessage(result.Errors))));
        }
    }

    #endregion

    #region Public methods

    public async Task<Result<(List<GrpoLineItemError>, List<NonPOLineItemError>)>> CreateInvoiceAsync(Invoice invoice)
    {
        var nonPOErrors = new List<NonPOLineItemError>();
        var grpoErrors = new List<GrpoLineItemError>();

        if (invoice.IsValid())
        {
            logger.LogInformation("Processing invoice {InvoiceNumber}, InvoiceNumber {InvoiceNumber}", invoice.ID, invoice.InvoiceNumber);

            var validLineItems = invoice.LineItems.ValidLineItems();
            await SetGlAccountForLineItemsAsync(invoice, validLineItems);

            var grpoLineItems = invoice.LineItems.GrpoLineItems();
            var nonPOLineItems = invoice.LineItems.NonPOLineItems();

            logger.LogInformation("Invoice {InvoiceId} has {GRPOCount} GRPO line items and {NonPOCount} NonPO line items",
                invoice.ID, grpoLineItems.Count(), nonPOLineItems.Count());

            await PostNonPOLineItemsAsync(invoice, nonPOLineItems, grpoLineItems.Any(), nonPOErrors);
            await ProcessGrpoLineItemsAsync(invoice, grpoLineItems, grpoErrors);
        }

        return Result.Ok((grpoErrors, nonPOErrors));
    }

    #endregion
}
