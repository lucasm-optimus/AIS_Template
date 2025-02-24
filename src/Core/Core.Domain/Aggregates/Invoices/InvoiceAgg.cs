namespace Tilray.Integrations.Core.Domain.Aggregates.Invoices
{
    public class InvoiceAgg : AggRoot
    {
        public IEnumerable<APLineItem> APLines { get; private set; }
        public IEnumerable<APATOLineItem> APATOLines { get; private set; }
        public IEnumerable<POAPLineItem> POAPMatchMQs { get; private set; }
        public IEnumerable<APATOMQLineItem> APATOMQs { get; private set; }

        private InvoiceAgg() { }

        public static Result<InvoiceAgg> Create(InvoiceGroup invoiceGroup, GLAccountsSettings glAccounts)
        {
            try
            {
                var apMatchingLines = new List<APLineItem>();
                var poapMatchingMQs = new List<POAPLineItem>();
                var apatoMQs = new List<APATOMQLineItem>();
                var apatoLines = new List<APATOLineItem>();
                var companyName = invoiceGroup.Company.Company_Name__c.Trim();
                var companyNumber = invoiceGroup.Company.Rootstock_Company__c.Trim();

                foreach (var invoice in invoiceGroup.Invoices)
                {
                    var goodsReceipts = invoice.HasGoodsReceipts();
                    if (goodsReceipts.IsSuccess)
                    {
                        var companyInvoicesAPMatchResult = ExtractAPMatchingLines(invoice, companyName, companyNumber, glAccounts);
                        if (companyInvoicesAPMatchResult.IsSuccess)
                        {
                            apMatchingLines.AddRange(companyInvoicesAPMatchResult.Value);
                            if (invoiceGroup.Company.PO_AP_Match_Invoices__c)
                            {
                                poapMatchingMQs.AddRange(companyInvoicesAPMatchResult.Value.Select(x => POAPLineItem.Create(x, companyName, companyNumber)).AsEnumerable());
                            }
                        }
                    }
                    else
                    {
                        var apatoLinesResult = ExtractAPATOLines(invoice, companyNumber, glAccounts);
                        if (apatoLinesResult.IsSuccess)
                        {
                            apatoLines.AddRange(apatoLinesResult.Value);
                            if (invoiceGroup.Company.Non_PO_Invoices__c)
                            {
                                apatoMQs.AddRange(apatoLinesResult.Value.Select(x => APATOMQLineItem.Create(x, companyName, companyNumber)).AsEnumerable());
                            }
                        }
                    }
                }

                return Result.Ok(new InvoiceAgg
                {
                    APLines = apMatchingLines,
                    APATOLines = apatoLines,
                    POAPMatchMQs = poapMatchingMQs,
                    APATOMQs = apatoMQs
                });
            }
            catch (Exception e)
            {
                return Result.Fail<InvoiceAgg>(e.Message);
            }
        }

        private static Result<IEnumerable<APLineItem>> ExtractAPMatchingLines(Invoice invoice, string companyName, string companyNumber, GLAccountsSettings glAccounts)
        {
            var result = Result.Ok();
            var lineItems = new List<APLineItem>();

            var headerResult = APLineItem.Create(invoice);
            result.WithErrors(headerResult.Errors);
            if (result.IsFailed) return result;
            lineItems.Add(headerResult.Value);

            var lineCount = 0;
            foreach (var lineItem in invoice.LineItems.GrpoLineItems())
            {
                var detailResult = APLineItem.Create(invoice, lineItem, lineCount++);
                result.WithErrors(detailResult.Errors);
                lineItems.Add(detailResult.Value);
            }

            AddSpecialLineItems(invoice, companyName, glAccounts, lineItems, ref lineCount, result);

            return result.IsFailed ? result : Result.Ok(lineItems.AsEnumerable());
        }

        private static void AddSpecialLineItems(Invoice invoice, string companyName, GLAccountsSettings glAccounts, List<APLineItem> lineItems, ref int lineCount, Result result)
        {
            if (invoice.ShippingAmount > 0)
            {
                var shippingAmountLine = APLineItem.Create(invoice, lineCount++, companyName, glAccounts);
                result.WithErrors(shippingAmountLine.Errors);
                lineItems.Add(shippingAmountLine.Value);
            }

            if (invoice.VatAmountOne > 0)
            {
                var vatAmountOneLine = APLineItem.Create(invoice, lineCount++, APMatchedLineType.VATAmount1Line, companyName, glAccounts);
                result.WithErrors(vatAmountOneLine.Errors);
                lineItems.Add(vatAmountOneLine.Value);
            }

            if (invoice.VatAmountTwo > 0)
            {
                var vatAmountTwoLine = APLineItem.Create(invoice, lineCount++, APMatchedLineType.VATAmount2Line, companyName, glAccounts);
                result.WithErrors(vatAmountTwoLine.Errors);
                lineItems.Add(vatAmountTwoLine.Value);
            }

            if (invoice.VatAmountThree > 0)
            {
                var vatAmountThreeLine = APLineItem.Create(invoice, lineCount++, APMatchedLineType.VATAmount3Line, companyName, glAccounts);
                result.WithErrors(vatAmountThreeLine.Errors);
                lineItems.Add(vatAmountThreeLine.Value);
            }
        }

        public static Result<IEnumerable<APATOLineItem>> ExtractAPATOLines(Invoice invoice, string companyNumber, GLAccountsSettings glAccounts)
        {
            try
            {
                var result = Result.Ok();
                var invoiceLines = new List<APATOLineItem>();
                var lineCount = 0;

                foreach (var item in invoice.LineItems.LineItem)
                {
                    var lineItem = APATOLineItem.Create(item, invoice, lineCount++, companyNumber, APATOLineType.Line, glAccounts);
                    result.WithErrors(lineItem.Errors);
                    invoiceLines.Add(lineItem.Value);
                }

                AddSpecialAPATOLineItems(invoice, companyNumber, glAccounts, invoiceLines, ref lineCount, result);

                return result.IsFailed ? result : Result.Ok(invoiceLines.AsEnumerable());
            }
            catch (Exception ex)
            {
                return Result.Fail<IEnumerable<APATOLineItem>>(ex.Message);
            }
        }

        private static void AddSpecialAPATOLineItems(Invoice invoice, string companyNumber, GLAccountsSettings glAccounts, List<APATOLineItem> invoiceLines, ref int lineCount, Result result)
        {
            if (invoice.ShippingAmount > 0)
            {
                var shippingAmountLine = APATOLineItem.Create(new LineItem(), invoice, lineCount++, companyNumber, APATOLineType.ShippingLine, glAccounts);
                result.WithErrors(shippingAmountLine.Errors);
                invoiceLines.Add(shippingAmountLine.Value);
            }

            if (invoice.VatAmountOne > 0)
            {
                var vatAmountOneLine = APATOLineItem.Create(new LineItem(), invoice, lineCount++, companyNumber, APATOLineType.VATAmount1Line, glAccounts);
                result.WithErrors(vatAmountOneLine.Errors);
                invoiceLines.Add(vatAmountOneLine.Value);
            }

            if (invoice.VatAmountTwo > 0)
            {
                var vatAmountTwoLine = APATOLineItem.Create(new LineItem(), invoice, lineCount++, companyNumber, APATOLineType.VATAmount2Line, glAccounts);
                result.WithErrors(vatAmountTwoLine.Errors);
                invoiceLines.Add(vatAmountTwoLine.Value);
            }

            if (invoice.VatAmountThree > 0)
            {
                var vatAmountThreeLine = APATOLineItem.Create(new LineItem(), invoice, lineCount++, companyNumber, APATOLineType.VATAmount3Line, glAccounts);
                result.WithErrors(vatAmountThreeLine.Errors);
                invoiceLines.Add(vatAmountThreeLine.Value);
            }
        }
    }
}
