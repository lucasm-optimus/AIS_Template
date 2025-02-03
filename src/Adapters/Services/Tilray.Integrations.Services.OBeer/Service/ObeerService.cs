using Tilray.Integrations.Core.Common.Extensions;

namespace Tilray.Integrations.Services.OBeer.Service;

public class ObeerService(HttpClient client, ISnowflakeRepository snowflakeRepository, ObeerSettings obeerSettings,
    ILogger<ObeerService> logger, IMapper mapper) : IObeerService
{
    #region Private methods

    private async Task<Result> CreateInvoiceAsync(ObeerInvoice obeerInvoice)
    {
        string apiUrl = $"api?APICommand={obeerSettings.APICommand}&EncompassID={obeerSettings.EncompassId}&APIToken={obeerSettings.APIToken}";

        var content = new StringContent(JsonConvert.SerializeObject(obeerInvoice), Encoding.UTF8, "application/json");

        HttpResponseMessage response = await client.PostAsync(apiUrl, content);
        if (response.IsSuccessStatusCode)
        {
            logger.LogInformation($"Obeer Invoice created successfully for {obeerInvoice?.Import?.InvoiceHeader?.FirstOrDefault()?.CustomerRefNo}");
            return Result.Ok();
        }

        string errorMessage = $@"Failed to create invoice in Obeer. InvoiceId: {obeerInvoice?.Import?.InvoiceHeader?.FirstOrDefault()?.CustomerRefNo}
            Error: {Helpers.GetErrorFromResponse(response)}";
        logger.LogError(errorMessage);
        return Result.Fail(errorMessage);
    }

    private async Task<IEnumerable<GrpoDetails>> GetGrpoDetails(MatchedPurchaseOrderReceipt grpo)
    {
        return await snowflakeRepository.GetGrpoDetailsAsync(
            grpo.PODocNum, grpo.GrnParts[1], grpo.GrnParts[3], grpo.GRPOLineNum);
    }

    private async Task ProcessGrpoLineItemsAsync(Invoice invoice, IEnumerable<LineItem> grpoLineItems,
        List<GrpoLineItemError> errorsGrpo)
    {
        if (!grpoLineItems.Any()) return;

        List<Item> obeerItems = [];
        foreach (var lineItem in grpoLineItems)
        {
            var remainingQty = lineItem.Quantity;
            var grpoCount = lineItem.MatchedPurchaseOrderReceipts.MatchedPurchaseOrderReceipt.Count;

            foreach (var grpo in lineItem.MatchedPurchaseOrderReceipts.MatchedPurchaseOrderReceipt)
            {
                if (!grpo.IsValidGrn)
                {
                    logger.LogError("Invalid GRN format for GRN {GoodsReceiptNumber}, Line Item {LineItemId}, Invoice {InvoiceId}",
                        grpo.GoodsReceiptNumber, lineItem.LineItemId, invoice.ID);
                    errorsGrpo.Add(GrpoLineItemError.Create(invoice, grpo.GoodsReceiptNumber));
                    continue;
                }

                var details = await GetGrpoDetails(grpo);
                if (details == null || !details.Any())
                {
                    logger.LogError("GRPO details not found for GRN {GoodsReceiptNumber}, line item {LineItemId}, Invoice {InvoiceId}",
                        grpo.GoodsReceiptNumber, lineItem.LineItemId, invoice.ID);
                    errorsGrpo.Add(GrpoLineItemError.Create(invoice, grpo.GoodsReceiptNumber));
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
            logger.LogInformation("No valid GRPO items to post for Invoice {InvoiceId}", invoice.ID);
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
                    GrpoLineItemError.Create(item, obeerInvoice.Import.InvoiceHeader.FirstOrDefault(), string.Join(", ", result.Errors))));
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
                    logger.LogWarning("GL account not found for segments {Segment1}-{Segment2} in lineItem {LineItem} for Invoice {InvoiceId}",
                        segments[0], segments[1], lineItem.LineItemId, invoice.ID);
                }
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

        var obeerInvoice = mapper.Map<ObeerInvoice>((invoice, nonPOLineItems, "dDocument_Service", hasGrpoLines));
        var result = await CreateInvoiceAsync(obeerInvoice);
        if (result.IsFailed)
        {
            errorsNonPO.AddRange(obeerInvoice.Import.Items.Select(item =>
                NonPOLineItemError.Create(item, obeerInvoice.Import.InvoiceHeader.FirstOrDefault(), string.Join(", ", result.Errors))));
        }
    }

    #endregion

    #region Public methods

    public async Task<Result<InvoiceProcessingResult>> CreateInvoicesAsync(List<Invoice> invoices)
    {
        var invoiceProcessingResult = new InvoiceProcessingResult();
        foreach (var invoice in invoices)
        {
            logger.LogInformation("Processing invoice {InvoiceNumber}", invoice.ID);
            var validLineItems = invoice.LineItems.LineItem.Where(li => li.IsValid());
            await SetGlAccountForLineItemsAsync(invoice, validLineItems);
            var grpoLineItems = validLineItems.Where(li => li.HasGrpoMatches());
            var nonPOLineItems = validLineItems.Except(grpoLineItems);

            await ProcessGrpoLineItemsAsync(invoice, grpoLineItems, invoiceProcessingResult.ErrorsGrpo);
            await PostNonPOLineItemsAsync(invoice, nonPOLineItems, grpoLineItems.Any() ? true : false, invoiceProcessingResult.ErrorsNoPo);
        }

        if (invoiceProcessingResult.HasErrors)
        {
            logger.LogWarning(
                "Invoices processing completed with errors. GRPOErrors: {GrpoErrorCount}, NonPOErrors: {NonPoErrorCount}",
                invoiceProcessingResult.ErrorsGrpo.Count,
                invoiceProcessingResult.ErrorsNoPo.Count
            );
            return Result.Fail(invoiceProcessingResult);
        }

        logger.LogInformation("Successfully processed {InvoiceCount} invoices", invoices.Count);
        return Result.Ok(invoiceProcessingResult);
    }

    #endregion
}
