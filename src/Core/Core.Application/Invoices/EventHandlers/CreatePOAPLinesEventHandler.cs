namespace Tilray.Integrations.Core.Application.Invoices.EventHandlers
{
    public class CreatePOAPLinesEventHandler(IRootstockService rootstockService, IMediator mediator, ILogger<CreatePOAPLinesEventHandler> logger) : IDomainEventHandler<InvoicesAggCreated>
    {
        public async Task Handle(InvoicesAggCreated notification, CancellationToken cancellationToken)
        {
            var apLinesMatchingErrors = new List<APMatchError>();

            foreach (var header in notification.POAPLines.Where(x => x.LineType == "HEADER"))
            {
                if (IsValidHeader(header))
                {
                    var createdSyDataHeaderResult = await rootstockService.CreateSyData(header);
                    if (createdSyDataHeaderResult.IsSuccess)
                    {
                        var detailSummation = GetInvoiceDetailSummation(notification.POAPLines, header.VendorInvoiceNumber!);
                        var rootstockSyDataDetails = await ProcessDetails(detailSummation, header, createdSyDataHeaderResult.Value, apLinesMatchingErrors);

                        if (apLinesMatchingErrors.Count == 0)
                        {
                            var createdSyDataDetailResult = await rootstockService.CreateSyDataDetail(rootstockSyDataDetails);
                            if (createdSyDataDetailResult.IsSuccess)
                            {
                                await rootstockService.UpdateSyDataProcess(createdSyDataHeaderResult.Value, "Ready");
                                logger.LogInformation("SYDATA NOT updated to READY -  {PONumber}", header.PurchaseOrderNumber);
                            }
                            else
                            {
                                apLinesMatchingErrors.Add(AddAPMatchError(header, $"SyDataDetail could not be created. Errors: {string.Join(" | ", createdSyDataDetailResult.Reasons)}"));
                            }
                        }
                    }
                }
                else
                {
                    apLinesMatchingErrors.Add(AddAPMatchError(header, "VendorInvoiceAmount, VendorInvoiceDate, VendorInvoiceNumber are required."));
                }
            }

            if (apLinesMatchingErrors.Any())
            {
                await mediator.Publish(new POAPErrorsGenerated(apLinesMatchingErrors, notification.CompanyName), cancellationToken);
            }
        }

        private static bool IsValidHeader(POAPLineItem header)
        {
            return header.VendorInvoiceAmount != null && header.VendorInvoiceDate != null && header.VendorInvoiceNumber != null;
        }

        private async Task<List<object>> ProcessDetails(IEnumerable<DetailSummation> detailSummation, POAPLineItem header, string syDataHeaderId, List<APMatchError> apLinesMatchingErrors)
        {
            var rootstockSyDataDetails = new List<object>();

            foreach (var detail in detailSummation)
            {
                if (detail.TransactionType == "PO Receipt")
                {
                    await ProcessPOReceiptDetails(detail, header, syDataHeaderId, rootstockSyDataDetails, apLinesMatchingErrors);
                }
                else if (detail.TransactionType == "GL Entry")
                {
                    await ProcessGLEntryDetails(detail, header, syDataHeaderId, rootstockSyDataDetails, apLinesMatchingErrors);
                }
            }

            return rootstockSyDataDetails;
        }

        private async Task ProcessPOReceiptDetails(DetailSummation detail, POAPLineItem header, string syDataHeaderId, List<object> rootstockSyDataDetails, List<APMatchError> apLinesMatchingErrors)
        {
            foreach (var po in detail.PurchaseOrderReceipts)
            {
                var poDetailResult = await rootstockService.GetPODetails(po);
                if (poDetailResult.IsSuccess && poDetailResult.Value.Id != null)
                {
                    rootstockSyDataDetails.Add(new
                    {
                        rstk__sydatad_type__c = "PO Receipt",
                        rstk__sydatad_desc__c = poDetailResult.Value.rstk__poline_longdescr__c,
                        rstk__sydatad_porcptap__c = poDetailResult.Value.Id,
                        rstk__sydatad_sydata__c = syDataHeaderId
                    });
                }
                else
                {
                    apLinesMatchingErrors.Add(AddAPMatchError(header, $"Purchase Order Receipt Transaction could not be found. Errors: {string.Join(" | ", poDetailResult.Reasons)}"));
                }
            }
        }

        private async Task ProcessGLEntryDetails(DetailSummation detail, POAPLineItem header, string syDataHeaderId, List<object> rootstockSyDataDetails, List<APMatchError> apLinesMatchingErrors)
        {
            var errorMessages = ValidateGLEntryDetail(detail);

            if (errorMessages.Count == 0)
            {
                var syaccResult = await rootstockService.GetIdFromExternalColumnReference("rstk__syacc__c", "rstk__externalid__c", detail.SubLedgerAccount!);
                if (syaccResult.IsSuccess && syaccResult.Value != null)
                {
                    rootstockSyDataDetails.Add(new
                    {
                        rstk__sydatad_type__c = "GL Entry",
                        rstk__sydatad_amount__c = detail.LineAmount,
                        rstk__sydatad_desc__c = detail.LineDescription,
                        rstk__sydatad_syacc__c = syaccResult.Value,
                        rstk__sydatad_sydata__c = syDataHeaderId
                    });
                }
                else
                {
                    apLinesMatchingErrors.Add(AddAPMatchError(header, $"GL Entry Transaction is missing required fields. Errors: {string.Join(" | ", syaccResult.Reasons)}"));
                }
            }
            else
            {
                apLinesMatchingErrors.Add(AddAPMatchError(header, $"GL Entry Transaction Errors: {string.Join(" | ", errorMessages)}"));
            }
        }

        private static List<string> ValidateGLEntryDetail(DetailSummation detail)
        {
            var errorMessages = new List<string>();

            if (detail.LineAmount == null)
            {
                errorMessages.Add("Line Amount is blank");
            }
            if (detail.LineDescription == null)
            {
                errorMessages.Add("Line Description is blank");
            }
            if (detail.SubLedgerAccount == null)
            {
                errorMessages.Add("Sub Ledger Account is blank");
            }

            return errorMessages;
        }

        private static APMatchError AddAPMatchError(POAPLineItem header, string message)
        {
            return APMatchError.Create(
                transactionType: header.TransactionType!,
                lineAmount: header.LineAmount.ToString()!,
                lineDescription: header.LineDescription!,
                subLedgerAccount: header.SubLedgerAccount!,
                purchaseOrderReceipt: header.PurchaseOrderReceipt!,
                error: message);
        }

        private static List<DetailSummation> GetInvoiceDetailSummation(IEnumerable<POAPLineItem> poapLines, string vendorInvoiceNumber)
        {
            return poapLines
                .Where(x => x.LineType == "DETAIL" && x.VendorInvoiceNumber == vendorInvoiceNumber)
                .GroupBy(x => $"{x.PurchaseOrderReceipt ?? ""}_{x.DetailType}")
                .Select(g => new DetailSummation
                {
                    TransactionType = g.First().TransactionType,
                    LineAmount = g.Sum(x => x.LineAmount),
                    LineDescription = g.First().LineDescription,
                    SubLedgerAccount = g.First().SubLedgerAccount,
                    PurchaseOrderReceipts = g.First().PurchaseOrderReceipt?.Split('|').ToList()
                })
                .Where(x => x.LineAmount != 0.0)
                .ToList();
        }
    }
}
