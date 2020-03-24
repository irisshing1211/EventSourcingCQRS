using System.Linq;
using EventSourcingCQRS.Entities;
using EventSourcingCQRS.Entities.Relational;
using EventSourcingCQRS.Events;

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
        }

        public void Rollback(RollbackCommand cmd)
        {
            var logs = _eventSourceCtx.EventLogs.Where(a => a.Time >= cmd.Time).OrderByDescending(a => a.Time).ToList();
            new RollbackEvent(_queryCtx).Push(logs);
            _eventSourceCtx.EventLogs.RemoveRange(logs);
            _eventSourceCtx.SaveChanges();
        }
    }
}
