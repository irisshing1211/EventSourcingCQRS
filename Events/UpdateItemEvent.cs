using EventSourcingCQRS.Entities;
using EventSourcingCQRS.Entities.NoSql;
using MongoDB.Driver;

namespace EventSourcingCQRS.Events
{
    public class UpdateItemEvent : BaseEvent
    {
        public UpdateItemEvent(QueryContext ctx) : base(ctx) {}

        public void Push(CountItem item) { _ctx.CountItems.ReplaceOne(a => a.Id == item.Id, item); }

    }
}
