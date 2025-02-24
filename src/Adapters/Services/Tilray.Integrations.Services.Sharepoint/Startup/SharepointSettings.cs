namespace Tilray.Integrations.Services.Sharepoint.Startup;

/// <summary>
/// This class is responsible for reading the Sharepoint settings from the configuration file.
/// </summary>
public class SharepointSettings
{
    public string SiteName { get; set; }
    public string LibraryName { get; set; }
    public string HostName { get; set; }
    public string ClientId { get; set; }
    public string ClientSecret { get; set; }
    public string TenantId { get; set; }
    public string BasePath { get; set; }
    public string InvoicesFolderPath { get; set; }
    public string InvoicesSubFolderPath { get; set; }
    public string InvoicesNonPOErrorsSubFolderPath { get; set; }
    public string InvoicesGrpoErrorsSubFolderPath { get; set; }
    public string ExpensesErrorsSubFolderPath { get; set; }
    public string ExpensesFolderPath { get; set; }
    public string AuditItemsFolderPath { get; set; }
    public string[] Scopes { get; set; } = ["https://graph.microsoft.com/.default"];
}
