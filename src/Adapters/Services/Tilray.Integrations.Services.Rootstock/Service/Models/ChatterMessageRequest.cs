namespace Tilray.Integrations.Services.Rootstock.Service.Models;

public class ChatterMessageRequest
{
    [JsonProperty("body")]
    public Body Body { get; set; }

    [JsonProperty("feedElementType")]
    public string FeedElementType { get; set; }

    [JsonProperty("subjectId")]
    public string SubjectId { get; set; }

    public static ChatterMessageRequest CreateFromChatterMessage(ChatterMessage chatterMessage)
    {
        return new ChatterMessageRequest
        {
            Body = new Body
            {
                MessageSegments = chatterMessage?.MessagePieces?.Select(mp => new MessageSegment
                {
                    Type = mp.Type ?? string.Empty,
                    Text = mp.Type == "Text" ? mp.Text : null,
                    Id = mp.Type == "Mention" ? mp.Id : null
                }).ToList() ?? new List<MessageSegment>()
            },
            FeedElementType = "FeedItem",
            SubjectId = chatterMessage?.RecordIDToAddFeedItemTo ?? string.Empty
        };
    }
}

public class Body
{
    [JsonProperty("messageSegments")]
    public List<MessageSegment> MessageSegments { get; set; }
}

public class MessageSegment
{
    [JsonProperty("type")]
    public string Type { get; set; }

    [JsonProperty("text", NullValueHandling = NullValueHandling.Ignore)]
    public string Text { get; set; }

    [JsonProperty("id", NullValueHandling = NullValueHandling.Ignore)]
    public string Id { get; set; }
}