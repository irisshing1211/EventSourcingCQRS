using System.Collections.Generic;
using EventSourcingCQRS.Entities;
using EventSourcingCQRS.Entities.NoSql;
using EventSourcingCQRS.Entities.Relational;
using MongoDB.Driver;

namespace EventSourcingCQRS.Events
{
    public class RollbackEvent : BaseEvent
    {
        public RollbackEvent(QueryContext ctx) : base(ctx) {}

        public void Push(List<EventLog> logs)
        {
            foreach (var log in logs)
            {
                var old = LogParser.ConvertStringToObject<CountItem>(log.OldValue);
                _ctx.CountItems.ReplaceOne(a => a.Id == log.ItemId, old);
            }
        }
    }
}
