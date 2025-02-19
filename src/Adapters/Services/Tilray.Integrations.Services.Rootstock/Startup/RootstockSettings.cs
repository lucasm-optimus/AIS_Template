namespace Tilray.Integrations.Services.Rootstock.Startup;

/// <summary>
/// This class is responsible for reading the Rootstock settings from the configuration file.
/// </summary>
public class RootstockSettings
{
    public string? BaseUrl { get; set; }
    public string? ClientId { get; set; }
    public string? ClientSecret { get; set; }
    public string JournalEntryChatterGroupPrefix { get; set; }
    public string IntegrationUserName { get; set; }
    public string Division { get; set; } = "001";
    public string PaymentGateway { get; set; } = "Authorize.net";
    public bool CapturedInPaymentGateway { get; set; } = false;
    public string Status { get; set; } = "Payment Completed";
}

public class RootstockGLAccountsSettings
{
    public AccountSettings A1 { get; set; } = new();
    public AccountSettings SWB { get; set; } = new();
}

public class AccountSettings
{
    public string Tax { get; set; }
    public string GST { get; set; }
    public string QST { get; set; }
    public string Cash { get; set; }
    public string Company { get; set; }
}
