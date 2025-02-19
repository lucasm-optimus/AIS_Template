namespace Tilray.Integrations.Services.Email.Startup;

public class EmailSettings
{
    public string SmtpHost { get; set; } = "smtp.office365.com";
    public int SmtpPort { get; set; } = 587;
    public string SmtpUser { get; set; } = "mult@apha.com";
    public string SmtpPassword { get; set; } = "s5";
    public string ToEmails { get; set; } = "s5";
    public string CcEmails { get; set; } = "s5";
    public IEnumerable<string> ToEmailsList() =>
            (ToEmails ?? string.Empty).Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
    public IEnumerable<string> CcEmailsList() =>
        (CcEmails ?? string.Empty).Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
}
