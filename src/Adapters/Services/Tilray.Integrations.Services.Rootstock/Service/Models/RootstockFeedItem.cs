namespace Tilray.Integrations.Services.Rootstock.Service.Models;

public class RootstockFeedItem
{
    public string Body { get; set; }
}

public class RootstockChatterFeedItem
{
    public ChatterMessageBody Body { get; set; }
    public string FeedElementType { get; set; }
    public string SubjectId { get; set; }
    public static RootstockChatterFeedItem Create(string subjectId, string message)
    {
        return new RootstockChatterFeedItem
        {
            Body = new ChatterMessageBody
            {
                MessageSegments =
                [
                    new
                    {
                        type = "Text",
                        text = message
                    }
                ]
            },
            FeedElementType = "FeedItem",
            SubjectId = subjectId
        };
    }
}

public class ChatterMessageBody
{
    public object[] MessageSegments { get; set; }
}
