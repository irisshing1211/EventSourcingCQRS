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

        public void AddCount(AddCountCommand cmd)
        {
            var log = cmd.LogObject;
            AddLog(log);
        }

        public void ReduceCount(ReduceCountCommand cmd)
        {
            var log = cmd.LogObject;
            AddLog(log);
        }

        private void AddLog(EventLog log)
        {
            _eventSourceCtx.EventLogs.Add(log);
            _eventSourceCtx.SaveChanges();
            new UpdateCountEvent(_queryCtx).Push(log);
        }
    }
}
