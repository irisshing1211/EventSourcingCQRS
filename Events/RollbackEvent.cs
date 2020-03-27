using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using EventSourcingCQRS.Entities;
using EventSourcingCQRS.Entities.NoSql;
using EventSourcingCQRS.Entities.Relational;
using EventSourcingCQRS.Query;
using Microsoft.EntityFrameworkCore.Internal;
using MongoDB.Driver;

namespace EventSourcingCQRS.Events
{
    public class RollbackEvent : BaseEvent
    {
        private EventSourceContext _eventSource;

        public RollbackEvent(QueryContext ctx, EventSourceContext eventSourceContext) : base(ctx)
        {
            _eventSource = eventSourceContext;
        }

        public void Push(List<EventLog> logs)
        {
            var query = new ItemQueryService(_ctx);

            var grp = logs.GroupBy(a => a.ItemId)
                          .Select(g => new {id = g.Key, logs = g.OrderByDescending(a => a.Time).ToList()})
                          .ToList();

            foreach (var g in grp)
            {
                var events = g.logs;

                if (events.Any(a => a.Action == LogAction.Insert) && events.Any(a => a.Action == LogAction.Delete))
                {
                    continue;
                }

                // if item has been created
                else if (events.Any(a => a.Action == LogAction.Insert))
                {
                    // delete item 
                    _ctx.CountItems.DeleteOne(a => a.Id == g.id);
                    events.Clear();

                    continue;
                }

                // if item has been deleted
                else if (events.Any(a => a.Action == LogAction.Delete))

                    // get the latest snapshot / add event then apply events again
                {
                    EventLog snapshot = _eventSource
                                        .EventLogs.Where(
                                            a => a.Action == LogAction.Snapshot || a.Action == LogAction.Insert)
                                        .OrderByDescending(a => a.Time)
                                        .FirstOrDefault();

                    var item = LogParser.ConvertStringToObject<CountItem>(snapshot.NewValue);

                    events = _eventSource
                             .EventLogs.Where(a => a.ItemId == item.Id &&
                                                   a.Time > snapshot.Time &&
                                                   a.Action == LogAction.Update)
                             .ToList();
                }

                var current = query.GetById(g.id);

                foreach (var log in events) { current = LogParser.UpdateObjectByData(current, log.NewValue); }

            }
        }
    }
}
