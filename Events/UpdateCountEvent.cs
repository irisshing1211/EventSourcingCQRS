using EventSourcingCQRS.Entities;
using EventSourcingCQRS.Entities.NoSql;
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
            var update = LogParser.UpdateObjectByData<CountItem>(item, log.NewValue);
            _ctx.CountItems.ReplaceOne(a => a.Id == log.ItemId, update);
        }
    }
}
