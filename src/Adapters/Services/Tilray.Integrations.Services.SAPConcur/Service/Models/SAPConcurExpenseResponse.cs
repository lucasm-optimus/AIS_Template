namespace Tilray.Integrations.Services.SAPConcur.Service.Models;

public class Definition
{
    public string Id { get; set; }

    [JsonProperty("job-link")]
    public string JobLink { get; set; }
    public string Name { get; set; }
}

public class Job
{
    public string Id { get; set; }

    [JsonProperty("status-link")]
    public string StatusLink { get; set; }

    [JsonProperty("start-time")]
    public string StartTime { get; set; }

    [JsonProperty("stop-time")]
    public string StopTime { get; set; }
    public string Status { get; set; }

    [JsonProperty("file-link")]
    public string FileLink { get; set; }
}
