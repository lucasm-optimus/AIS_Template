namespace Tilray.Integrations.Services.SAPConcur.Startup;

/// <summary>
/// This class is responsible for reading the SAP Concur settings from the configuration file.
/// </summary>
public class SAPConcurSettings
{
    public string BaseUrl { get; set; }
    public string TokenEndpoint { get; set; }
    public string ApprovalStatus { get; set; }
    public string PaymentStatus { get; set; }
    public string ClientId { get; set; }
    public string ClientSecret { get; set; }
    public string RefreshToken { get; set; }
    public int InvoicesFetchDurationInMinutes { get; set; }
    public int ExpensesFetchDurationInMinutes { get; set; }
    public string ExtractDefinitionName { get; set; }
}
