using EventSourcingCQRS.Entities;
using EventSourcingCQRS.Entities.Relational;
using MongoDB.Driver;

namespace EventSourcingCQRS.Events
{
    public class UpdateCountEvent : BaseEvent
    {
        public UpdateCountEvent(QueryContext ctx) : base(ctx) {}

        public void Push(EventLog log)
        {
            var item = _ctx.CountItems.Find(a => a.Id == log.ItemId).FirstOrDefault();
            item.Count = int.Parse(log.NewValue);
            _ctx.CountItems.ReplaceOne(a => a.Id == log.ItemId, item);
        }
    }
}
