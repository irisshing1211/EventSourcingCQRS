using System.Collections.Generic;
using System.Linq;
using EventSourcingCQRS.Entities;
using EventSourcingCQRS.Entities.Relational;
using EventSourcingCQRS.Events;
using EventSourcingCQRS.Query;

namespace EventSourcingCQRS.Commands
{
    public class CommandHandler
    {
        private readonly EventSourceContext _eventSourceCtx;
        private readonly QueryContext _queryCtx;

        public CommandHandler(EventSourceContext eventSourceCtx, QueryContext queryCtx)
        {
            _eventSourceCtx = eventSourceCtx;
            _queryCtx = queryCtx;
        }

        public void UpdateCount(UpdateCountCommand cmd)
        {
            var log = cmd.LogObject;
            _eventSourceCtx.EventLogs.Add(log);
            _eventSourceCtx.SaveChanges();
            new UpdateCountEvent(_queryCtx).Push(log);
            SnapShot(log);
        }

        public void UpdateItem(UpdateItemCommand cmd)
        {
            var log = cmd.LogObject;
            _eventSourceCtx.EventLogs.Add(log);
            _eventSourceCtx.SaveChanges();
            new UpdateItemEvent(_queryCtx).Push(cmd.NewValue);
            SnapShot(log);
        }
        public void Rollback(RollbackCommand cmd)
        {
            var logs = _eventSourceCtx.EventLogs.Where(a => a.Time > cmd.Time)
                                      .OrderByDescending(a => a.Time)
                                      .ToList();

            Rollback(logs);
        }

        public void Rollback(RollbackSpecificCommand cmd)
        {
            var logs = _eventSourceCtx
                       .EventLogs.Where(a => a.ItemId == cmd.Id && a.Time > cmd.Time)
                       .OrderByDescending(a => a.Time)
                       .ToList();

            Rollback(logs);
        }

        private void Rollback(List<EventLog> logs)
        {
            new RollbackEvent(_queryCtx, _eventSourceCtx).Push(logs);
            logs.Remove(logs[logs.Count - 1]);
            _eventSourceCtx.EventLogs.RemoveRange(logs);
            _eventSourceCtx.SaveChanges();
        }

        private void SnapShot(EventLog log)
        {
            var logCnt = _eventSourceCtx.EventLogs.Count(a => a.ItemId == log.ItemId && a.Action != LogAction.Snapshot);

            // add snapshot for every 10 events
            if (logCnt % 10 == 0)
            {
                var query = new ItemQueryService(_queryCtx);
                var item = query.GetById(log.ItemId);

                var snapshot = new EventLog(item);
                _eventSourceCtx.EventLogs.Add(snapshot);
                _eventSourceCtx.SaveChanges();
            }
        }
    }
}
