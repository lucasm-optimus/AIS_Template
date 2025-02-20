namespace Tilray.Integrations.Core.Domain.Aggregates.PurchaseOrders
{
    public class ChatterMessage
    {
        public string RecordIDToAddFeedItemTo { get; set; }
        public List<MessagePiece> MessagePieces { get; set; }

        public static ChatterMessage Create(string recordID, string message)
        {
            return new ChatterMessage
            {
                RecordIDToAddFeedItemTo = recordID,
                MessagePieces = new List<MessagePiece>
                {
                    new MessagePiece { Type = "Text", Text = message + "\n" },
                    new MessagePiece { Type = "Mention", Id = recordID }
                }
            };
        }

        public static ChatterMessage CreateForPurchaseOrderSync(string recordID, string erp, int errorCount)
        {
            var message = $"The latest PO Sync between {erp} and Concur produced {errorCount} errors.";
            return Create(recordID, message);
        }
    }

    public class MessagePiece
    {
        public string Type { get; set; }
        public string Text { get; set; }
        public string Id { get; set; }
    }
}