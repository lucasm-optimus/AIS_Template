namespace Tilray.Integrations.Core.Domain.Aggregates.Expenses;

public enum ExpenseType
{
    Cash,
    Company
}

public class Expense
{
    public string LineType { get; set; }
    public string BatchID { get; set; }
    public DateTime? BatchDate { get; set; }
    public string EmployeeLastName { get; set; }
    public string EmployeeFirstName { get; set; }
    public string EmployeeGroupID { get; set; }
    public string ReportID { get; set; }
    public string ReportDescription { get; set; }
    public string ReportTotalApprovedID { get; set; }
    public DateTime? ReportEntryTransactionDate { get; set; }
    public string ReportEntryCurrencyAlphaCode { get; set; }
    public string ReportEntryDescription { get; set; }
    public string ReportEntryVendorDescription { get; set; }
    public string CompanyCode { get; set; }
    public string ERP { get; set; }
    public string Department { get; set; }
    public string ExpenseCode { get; set; }
    public string ManAttention { get; set; }
    public string Region { get; set; }
    public string Location { get; set; }
    public string Brand { get; set; }
    public string Facility { get; set; }
    public string PaymentTypeCode { get; set; }
    public string PaymentCode { get; set; }
    public string ReportEntryLocationCountryCode { get; set; }
    public string ReportEntryLocationCountrySubCode { get; set; }
    public string JournalPayerPaymentTypeName { get; set; }
    public string JournalDebitOrCredit { get; set; }
    public decimal? JournalAmount { get; set; }
    public decimal? JournalNetOfTotalAdjustedReclaimTax { get; set; }
    public decimal? CreditCardTransactionAmount { get; set; }
    public decimal? CreditCardTransactionPostedAmount { get; set; }
    public string TaxAuthorityName { get; set; }
    public string TaxAuthorityLabel { get; set; }
    public decimal? ReportEntryTaxTransactionAmount { get; set; }
    public decimal? ReportEntryTaxReclaimTransactionAmount { get; set; }
    public string CleanCompanyCode => CompanyCode?.Replace("'", "") ?? string.Empty;
    public bool IsNegative => JournalAmount < 0;
    public bool IsTaxExpense => new[] { "HST", "GST", "PST" }.Contains(TaxAuthorityLabel);
    public bool IsQstsExpense => TaxAuthorityLabel == "QSTS";
    public bool IsCashExpense => PaymentTypeCode?.ToLower() == "cash";
    public bool IsCompanyExpense => PaymentTypeCode?.ToLower() != "cash";
}

public class ExpenseDetails
{
    public string StopTime { get; set; }
    public IEnumerable<Expense> Expenses { get; set; }
    public static ExpenseDetails Create(string stopTime, IEnumerable<Expense> expenses)
    {
        return new ExpenseDetails
        {
            StopTime = stopTime,
            Expenses = expenses
        };
    }
}
