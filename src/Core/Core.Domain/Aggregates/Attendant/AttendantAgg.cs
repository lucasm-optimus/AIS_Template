using Optimus.Core.Domain.Aggregates.Attendant.Commands;
using Optimus.Core.Domain.Aggregates.Attendant.Events;

namespace Optimus.Core.Domain.Aggregates.Attendant
{
#pragma warning disable
    public class AttendantAgg : AggRoot
    {
        public string Id { get; internal set; }
        public string Name { get; internal set; }
        public string CardId { get; internal set; }
        public DateTime CreatedDate { get; internal set; }
        public DateTime? DisabledDate { get; internal set; }
        public bool IsEnabled { get { return DisabledDate == null; } }

        internal AttendantAgg() { }
        private AttendantAgg(CreateAttendantCommand cmd)
        {
            Id = Guid.NewGuid().ToString();
            Name = cmd.Name;
            CardId = cmd.CardId;
            CreatedDate = DateTime.Now;
        }

        public static Result<AttendantCreated> Create(CreateAttendantCommand cmd)
        {
            var result = Result.Ok();
            var agg = new AttendantAgg(cmd);
            return result.ToResult(agg.Created());
        }

        public static Result<AttendantAgg> Create(string id,
                                                  string name,
                                                  string cardId,
                                                  DateTime createdDate,
                                                  DateTime? disabledDate)
        {
            AttendantAgg attendantAgg = new AttendantAgg();
            attendantAgg.Id = id;
            attendantAgg.Name = name;
            attendantAgg.CardId = cardId;
            attendantAgg.CreatedDate = createdDate;
            attendantAgg.DisabledDate = disabledDate;
            return Result.Ok(attendantAgg);
        }

        public AttendantCreated Created()
        {
            return AttendantCreated.Create(this);
        }

        public Result<AttendantDisabled> Disable()
        {
            DisabledDate = DateTime.Now;
            return Result.Ok(AttendantDisabled.CreateDisabled(this));
        }

        public Result<AttendantEnabled> Enable()
        {
            DisabledDate = null;
            return Result.Ok(AttendantEnabled.CreateEnabled(this));
        }

        public AttendantAgg ChangeCard(string newCard)
        {
            CardId = newCard;
            return this;
        }

    }
}
