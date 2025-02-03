namespace Tilray.Integrations.Services.Sharepoint.Service;

public class SharepointService(GraphServiceClient graphServiceClient, IMapper mapper, SharepointSettings sharepointSettings, ILogger<SharepointService> logger) : ISharepointService
{
    #region Private methods

    private string GetSubFolderPath<T>()
    {
        Type type = typeof(T);
        return type switch
        {
            Type invoice when invoice == typeof(Invoice) => sharepointSettings.InvoicesSubFolderPath,
            Type error when error == typeof(NonPOLineItemError) => sharepointSettings.InvoicesNonPOErrorsSubFolderPath,
            Type error when error == typeof(GrpoLineItemError) => sharepointSettings.InvoicesGrpoErrorsSubFolderPath,
            _ => throw new NotSupportedException($"Uploading files of type {type.Name} is not supported."),
        };
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

    #endregion

    #region Public methods

    public async Task<Result> UploadFileAsync<T>(List<T> content, CompanyReference companyReference)
    {
        if (content == null || content.Count == 0)
        {
            logger.LogWarning("UploadFileAsync: No content provided for upload");
            return Result.Fail("UploadFileAsync: No content provided for upload");
        }

        var driveResult = await GetDriveIdAsync();
        if (driveResult.IsFailed) return driveResult.ToResult();

        var fileContent = Helpers.ConvertToCsv(content);
        var uploadPath = $"{sharepointSettings.BasePath?.TrimEnd('/')}/{companyReference.Company_Name__c.Trim()}{sharepointSettings.InvoicesFolderPath?.TrimEnd('/')}/{GetSubFolderPath<T>()}{companyReference.Company_Name__c}_{DateTime.Now:yyyy-MM-dd-HHmmss}.csv";

        using (var memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(fileContent)))
        {
            var uploadedFile = await graphServiceClient
                .Drives[driveResult.Value]
                .Items["root"]
                .ItemWithPath(uploadPath)
                .Content
                .PutAsync(memoryStream);

            if (uploadedFile?.Id == null)
            {
                logger.LogError("UploadFileAsync: File upload failed");
                return Result.Fail("UploadFileAsync: File upload failed");
            }
        }

        return Result.Ok();
    }

    #endregion
}
