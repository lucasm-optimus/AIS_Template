using Tilray.Integrations.Services.Rootstock.Startup;

namespace Tilray.Integrations.Services.Rootstock.Service.Models;

public class RootstockJournalEntry
{
    public string rstkf__jeato_acct__c { get; set; }
    public string rstkf__jeato_company__c { get; set; }
    public decimal rstkf__jeato_cramt__c { get; set; }
    public decimal rstkf__jeato_dramt__c { get; set; }
    public string rstkf__jeato_div__c { get; set; }
    public string rstkf__jeato_desc__c { get; set; }
    public RootstockFinancialCompany rstkf__jeato_glcmp__r { get; set; } = new();
    public string rstkf__jeato_jeno__c { get; set; }
    public DateTime? rstkf__jeato_date__c { get; set; }
    public string rstkf__jeato_gljh__c { get; set; }
    public RootstockFinancialSystemUser rstkf__jeato_owner__r { get; set; } = new();
    public string rstkf__jeato_uploadgroup__c { get; set; }
    public string rstkf__jeato_vatclass__c { get; set; }
    public RootstockGLAccount rstkf__jeato_glacct__r { get; set; } = new();
    public RootstockFinancialDivision rstkf__jeato_gldiv__r { get; set; } = new();

    public static RootstockJournalEntry CreateBase(CompanyReference company, Expense expense, string integrationUserName)
    {
        return new RootstockJournalEntry
        {
            rstkf__jeato_company__c = company.Rootstock_Company__c,
            rstkf__jeato_div__c = company.Rootstock_Company__c,
            rstkf__jeato_date__c = expense.BatchDate,
            rstkf__jeato_desc__c = $"{expense.ReportDescription} - {expense.BatchID} - {expense.ReportID}",
            rstkf__jeato_uploadgroup__c = expense.ReportID,
            rstkf__jeato_glcmp__r = new RootstockFinancialCompany { rstkf__externalid__c = company.Rootstock_Company__c },
            rstkf__jeato_owner__r = new RootstockFinancialSystemUser { Name = integrationUserName },
            rstkf__jeato_gldiv__r = new RootstockFinancialDivision { rstkf__externalid__c = $"{company.Rootstock_Company__c}_{company.Rootstock_Company__c}" },
            rstkf__jeato_jeno__c = string.Empty,
            rstkf__jeato_vatclass__c = string.Empty
        };
    }

    public static (RootstockJournalEntry Debit, RootstockJournalEntry Credit) CreatePair(CompanyReference company, Expense expense,
        bool isNegative, string account, decimal amount, string integrationUserName, RootstockGLAccountsSettings glAccountsSettings)
    {
        var debit = CreateBase(company, expense, integrationUserName);
        var credit = CreateBase(company, expense, integrationUserName);
        var cashAccount = GetCashAccount(company.Rootstock_Company__c, expense, glAccountsSettings);

        if (isNegative)
        {
            debit.SetDebitAccount(company.Rootstock_Company__c, cashAccount, amount * -1);
            credit.SetCreditAccount(company.Rootstock_Company__c, account, amount * -1);
        }
        else
        {
            debit.SetDebitAccount(company.Rootstock_Company__c, account, amount);
            credit.SetCreditAccount(company.Rootstock_Company__c, cashAccount, amount);
        }

        return (debit, credit);
    }

    public static (RootstockJournalEntry Debit, RootstockJournalEntry Credit) CreateTaxPair(CompanyReference company, Expense expense,
        bool isNegative, string integrationUserName, RootstockGLAccountsSettings glAccountsSettings)
    {
        var taxAccount = company.Rootstock_Company__c == "003" ? glAccountsSettings.SWB.Cash : glAccountsSettings.A1.GST;
        return CreatePair(company, expense, isNegative, taxAccount, expense.JournalAmount ?? 0, integrationUserName, glAccountsSettings);
    }

    public static (RootstockJournalEntry Debit, RootstockJournalEntry Credit) CreateQSTSPair(CompanyReference company, Expense expense,
        bool isNegative, string integrationUserName, RootstockGLAccountsSettings glAccountsSettings)
    {
        var taxAccount = company.Rootstock_Company__c == "003" ? glAccountsSettings.SWB.Cash : glAccountsSettings.A1.QST;
        return CreatePair(company, expense, isNegative, taxAccount, expense.JournalAmount ?? 0, integrationUserName, glAccountsSettings);
    }

    public static (RootstockJournalEntry Debit, RootstockJournalEntry Credit) CreateExpensePair(
        CompanyReference company, Expense expense, bool isNegative, string integrationUserName, RootstockGLAccountsSettings glAccountsSettings)
    {
        var expenseAccount = GetExpenseAccount(company.Rootstock_Company__c, expense);
        var amount = expense.JournalNetOfTotalAdjustedReclaimTax ?? 0;
        return CreatePair(company, expense, isNegative, expenseAccount, amount, integrationUserName, glAccountsSettings);
    }

    private static string GetCashAccount(string companyCode, Expense expense, RootstockGLAccountsSettings glAccountsSettings)
    {
        var paymentType = expense.PaymentTypeCode.ToLower();
        return companyCode switch
        {
            "003" => paymentType == "cash" ? glAccountsSettings.SWB.Cash : glAccountsSettings.SWB.Company,
            "001" => paymentType == "cash" ? glAccountsSettings.A1.Cash : glAccountsSettings.A1.Company,
            _ => string.Empty
        };
    }

    private static string GetExpenseAccount(string companyCode, Expense expense)
    {
        var department = expense.Department;
        var expenseCode = expense.ExpenseCode;

        return companyCode == "003"
            ? department == "0" ? $"000-{expenseCode}" : $"{department}-{expenseCode}"
            : expenseCode;
    }

    private void SetDebitAccount(string companyNumber, string account, decimal amount)
    {
        rstkf__jeato_glacct__r = new RootstockGLAccount { rstkf__externalid__c = $"{companyNumber}_{account}" };
        rstkf__jeato_acct__c = account;
        rstkf__jeato_dramt__c = amount;
        rstkf__jeato_cramt__c = 0;
    }

    private void SetCreditAccount(string companyNumber, string account, decimal amount)
    {
        rstkf__jeato_glacct__r = new RootstockGLAccount { rstkf__externalid__c = $"{companyNumber}_{account}" };
        rstkf__jeato_acct__c = account;
        rstkf__jeato_cramt__c = amount;
        rstkf__jeato_dramt__c = 0;
    }
}

public class RootstockFinancialCompany
{
    public string rstkf__externalid__c { get; set; }
}

public class RootstockFinancialSystemUser
{
    public string Name { get; set; }
}

public class RootstockFinancialDivision
{
    public string rstkf__externalid__c { get; set; }
}

public class RootstockGLAccount
{
    public string rstkf__externalid__c { get; set; }
}
