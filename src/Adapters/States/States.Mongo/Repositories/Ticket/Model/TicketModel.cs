//using MongoDB.Bson.Serialization.Attributes;
//using MongoDB.Bson;
//using FluentResults;
//using Optimus.Core.Domain.Aggregates.Ticket;

//namespace States.Mongo.Repositories.Ticket.Model;

//public class TicketModel
//{
//    [BsonId]
//    [BsonElement("_id")]
//    public ObjectId _Id { get; set; }
//    [BsonRepresentation(BsonType.String)]
//    public string ID { get; set; }
//    public string AttendantId { get; private set; }
//    public DateTime CreatedDate { get; private set; }
//    public DateTime? AbandonedDate { get; private set; }
//    public DateTime? ClosedDate { get; private set; }
//    public string Description { get; private set; }

//    internal static TicketModel FromDomain(TicketAgg ticket)
//    {
//        return new TicketModel()
//        {
//            ID = ticket.Id,
//            Description = ticket.Description,
//            CreatedDate = ticket.CreatedDate,
//            AbandonedDate = ticket.AbandonedDate,
//            ClosedDate = ticket.ClosedDate
//        };
//    }

//    internal static Result<TicketAgg> ToDomain(TicketModel model)
//    {
//        if (model is null)
//            return Result.Fail<TicketAgg>("Ticket not found");

//        var ticketResult = TicketAgg.Create(model.ID, model.AttendantId, model.Description, model.CreatedDate, model.AbandonedDate, model.ClosedDate);

//        return ticketResult;
//    }
//}
