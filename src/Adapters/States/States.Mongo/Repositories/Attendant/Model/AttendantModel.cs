using Optimus.Core.Domain.Aggregates.Attendant;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Optimus.States.Mongo.Repositories.Attendant.Model;
#pragma warning disable
public class AttendantModel
{
    [BsonId]
    [BsonElement("_id")]
    public Object _Id { get; set; }

    [BsonRepresentation(BsonType.String)]
    public string Id { get; set; }

    [BsonRepresentation(BsonType.String)]
    public string Name { get; private set; }

    [BsonRepresentation(BsonType.String)]
    public string CardId { get; private set; }

    public DateTime CreatedDate { get; private set; }

    public DateTime? DisableDate { get; private set; }

    internal static AttendantModel FromDomain(AttendantAgg attendant)
    {
        return new AttendantModel
        {
            Id = attendant.Id,
            CardId = attendant.CardId,
            CreatedDate = attendant.CreatedDate,
            DisableDate = attendant.DisabledDate
        };
    }
    internal static Result<AttendantAgg> ToDomain(AttendantModel model)
    {
        if (model is null)
            return Result.Fail<AttendantAgg>("Attendant not found");

        Result<AttendantAgg>? result = AttendantAgg.Create(model.Id, model.Name, model.CardId, model.CreatedDate, model.DisableDate);
        return result;
    }

}
