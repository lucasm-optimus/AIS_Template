using Optimus.Core.Application.Attendant.State;
using Optimus.Core.Domain.Aggregates.Attendant;

namespace Optimus.States.Mongo.Repositories.Attendant;
public class AttendantRepository : MongoRepositoryBase<AttendantAgg>, IAttendantState
{
    public AttendantRepository(IMongoDatabase database) : base(database, "Attendants")
    {
    }

    public async Task<AttendantAgg> GetByCard(string cardId)
    {
        var filter = Builders<AttendantAgg>.Filter.And(
            Builders<AttendantAgg>.Filter.Eq("CardId", cardId)
            //Builders<AttendantAgg>.Filter.Ne<DateTime?>("DisableDate", null)
        );

        return await db.Find(filter).FirstOrDefaultAsync();
    }
    public async Task<IEnumerable<AttendantAgg>> GetAll()
    {
        return await GetMany(x => true);
    }
}
