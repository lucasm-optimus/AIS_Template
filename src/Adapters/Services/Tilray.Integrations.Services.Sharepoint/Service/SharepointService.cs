using Tilray.Integrations.Core.Domain.AuditItems;
using Tilray.Integrations.Services.Sharepoint.Startup;

namespace Tilray.Integrations.Services.Sharepoint.Service;

public class SharepointService(GraphServiceClient graphServiceClient, IMapper mapper, SharepointSettings sharepointSettings, ILogger<SharepointService> logger)
    : ISharepointService
{
    #region Private methods

    private string GetSubFolderPath<T>()
    {
        return typeof(T) switch
        {
            Type invoice when invoice == typeof(SharepointInvoice) => sharepointSettings.InvoicesSubFolderPath,
            Type error when error == typeof(NonPOLineItemError) => sharepointSettings.InvoicesNonPOErrorsSubFolderPath,
            Type error when error == typeof(GrpoLineItemError) => sharepointSettings.InvoicesGrpoErrorsSubFolderPath,
            Type error when error == typeof(ExpenseError) => sharepointSettings.ExpensesErrorsSubFolderPath,
            _ => string.Empty,
        };
    }

    private string GetExpensesUploadPath(CompanyReference companyReference, ExpenseType? expenseType, string stopTime)
    {
        var companyName = companyReference?.Company_Name__c?.Trim() ?? "";
        var basePath = sharepointSettings.BasePath?.TrimEnd('/');
        var expensesFolder = sharepointSettings.ExpensesFolderPath?.TrimEnd('/');

        return expenseType switch
        {
            ExpenseType.Cash => $"{basePath}/{companyName}/{expensesFolder}/Expenses_Cash_{companyName}_{DateTime.Parse(stopTime):yyyy-MM-dd-HHmmss}.csv",
            ExpenseType.Company => $"{basePath}/{companyName}/{expensesFolder}/Expenses_Company_{companyName}_{DateTime.Parse(stopTime):yyyy-MM-dd-HHmmss}.csv",
            _ => $"{basePath}/General/Expenses/Expenses_{DateTime.Parse(stopTime):yyyy-MM-dd-HHmmss}.csv"
        };
    }

    private string GetAuditDataUploadPath(string fileExtension)
    {
        var basePath = sharepointSettings.BasePath?.TrimEnd('/');
        var soxReportFolder = sharepointSettings.AuditItemsFolderPath?.TrimEnd('/') ?? "SOXReport";

        string formattedDate = DateTime.UtcNow.ToString("yyyy-MM-dd");

        return $"{basePath}/{soxReportFolder}/SF_SOX_Report_{formattedDate}{fileExtension}";
    }

    private string GetUploadPath<T>(CompanyReference companyReference)
    {
        string subFolderPath = GetSubFolderPath<T>();
        string basePath = sharepointSettings.BasePath?.TrimEnd('/') ?? "";
        string fileName = $"{companyReference?.Company_Name__c}_{DateTime.Now:yyyy-MM-dd-HHmmss}.csv";
        string companyName = companyReference?.Company_Name__c?.Trim() ?? "";

        if (typeof(T) == typeof(ExpenseError))
        {
            return $"{basePath}/{companyName}/{sharepointSettings.ExpensesFolderPath?.TrimEnd('/')}/{subFolderPath}{fileName}";
        }

        return $"{basePath}/{companyName}/{sharepointSettings.InvoicesFolderPath?.TrimEnd('/')}/{subFolderPath}{fileName}";
    }

    private async Task<Result<string>> GetSiteIdAsync()
    {
        var site = await graphServiceClient.Sites[$"{sharepointSettings.HostName}:/sites/{sharepointSettings.SiteName}"]
            .GetAsync();

        if (site?.Id == null)
        {
            string errorMessage = $"GetSiteIdAsync: Failed to retrieve Sharepoint site information for site {sharepointSettings.SiteName}";
            logger.LogError(errorMessage);
            return Result.Fail<string>(errorMessage);
        }

        return Result.Ok(site.Id);
    }

    private async Task<Result<string>> GetDriveIdAsync()
    {
        var siteResult = await GetSiteIdAsync();
        if (siteResult.IsFailed) return siteResult.ToResult();

        var drives = await graphServiceClient.Sites[siteResult.Value]
            .Drives
            .GetAsync();

        if (drives?.Value == null)
        {
            logger.LogError("GetDriveIdAsync: No drives found in SharePoint site");
            return Result.Fail<string>("GetDriveIdAsync: No drives found in SharePoint site");
        }

        var drive = drives.Value.FirstOrDefault(d =>
            d.Name?.Equals(sharepointSettings.LibraryName, StringComparison.OrdinalIgnoreCase) ?? false);

        if (drive?.Id == null)
        {
            logger.LogError("GetDriveIdAsync: Drive '{LibraryName}' not found", sharepointSettings.LibraryName);
            return Result.Fail<string>($"GetDriveIdAsync: Drive '{sharepointSettings.LibraryName}' not found");
        }

        return Result.Ok(drive.Id);
    }

    private async Task<Result> UploadAsync(string content, string uploadPath)
    {
        var driveResult = await GetDriveIdAsync();
        if (driveResult.IsFailed) return driveResult.ToResult();

        using var memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(content));

        var uploadedFile = await graphServiceClient
            .Drives[driveResult.Value]
            .Items["root"]
            .ItemWithPath(uploadPath)
            .Content
            .PutAsync(memoryStream);

        if (uploadedFile?.Id == null)
        {
            string errorMessage = $"UploadAsync: File upload failed for {uploadPath}";
            logger.LogError(errorMessage);
            return Result.Fail(errorMessage);
        }

        return Result.Ok();
    }

    #endregion

    #region Public methods

    public async Task<Result> UploadInvoicesAsync(IEnumerable<Invoice> invoices, CompanyReference companyReference)
    {
        var sharepointInvoices = invoices
            .Where(invoice => invoice?.LineItems?.LineItem != null)
            .SelectMany(invoice =>
                invoice.LineItems.LineItem.Select((lineItem, index) =>
                new { invoice, lineItem, LineItemNumber = index + 1 })
            )
            .Select(x => mapper.Map<SharepointInvoice>((x.invoice, x.lineItem, x.LineItemNumber))).ToList();


        return await UploadFileAsync(sharepointInvoices, companyReference);
    }

    public async Task<Result> UploadExpensesAsync(IEnumerable<Expense> expenses, string stopTime, CompanyReference companyReference = null,
        ExpenseType? expenseType = null)
    {
        var uploadPath = GetExpensesUploadPath(companyReference, expenseType, stopTime);
        string[] ignoredProperties = { "LineType", "CleanCompanyCode", "IsNegative", "IsTaxExpense",
            "IsQstsExpense", "IsCashExpense", "IsCompanyExpense" };

        return await UploadFileAsync(expenses, uploadPath: uploadPath, ignoredProperties: ignoredProperties);
    }

    public async Task<Result> UploadFileAsync<T>(IEnumerable<T> content, CompanyReference companyReference = null,
        string uploadPath = null, string[] ignoredProperties = null)
    {
        if (content == null || !content.Any())
        {
            logger.LogWarning("UploadFileAsync: No content provided for upload");
            return Result.Fail("UploadFileAsync: No content provided for upload");
        }

        var driveResult = await GetDriveIdAsync();
        if (driveResult.IsFailed) return driveResult.ToResult();


        var fileContent = Helpers.ConvertToCsv(content, ignoredProperties);
        uploadPath ??= GetUploadPath<T>(companyReference);

        var result = await UploadAsync(fileContent, uploadPath);
        if(result.IsFailed)
        {
            logger.LogError("UploadFileAsync: Failed to upload {ObjectName} to Sharepoint", typeof(T).Name);
            return result;
        }

        logger.LogInformation("UploadFileAsync: {ObjectName} uploaded. Upload Path {UploadPath}", typeof(T).Name, uploadPath);
        return Result.Ok();
    }

    public async Task<Result> UploadAuditItemsAsync(IEnumerable<AuditItem> auditItems)
    {
        var uploadPath = GetAuditDataUploadPath(".csv");
        string[] ignoredProperties = { "Attributes.Url" };

        return await UploadFileAsync(auditItems, uploadPath: uploadPath, ignoredProperties: ignoredProperties);
    }

    public async Task<Result> UploadAuditItemsQueryAsync(string query)
    {
        if (query == null)
        {
            logger.LogWarning("UploadAuditItemsQueryAsync: No query provided for upload");
            return Result.Fail("UploadAuditItemsQueryAsync: No query provided for upload");
        }

        var uploadPath = GetAuditDataUploadPath(".txt");
        var result = await UploadAsync(query, uploadPath);
        if (result.IsFailed)
        {
            logger.LogError("UploadAuditItemsQueryAsync: Failed to upload audit items query to Sharepoint");
            return result;
        }

        logger.LogInformation("UploadAuditItemsQueryAsync: audit items query uploaded. Upload Path {UploadPath}", uploadPath);
        return Result.Ok();
    }

    #endregion
}
