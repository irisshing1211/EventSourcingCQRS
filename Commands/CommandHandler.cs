using EventSourcingCQRS.Entities;
using EventSourcingCQRS.Entities.Relational;

namespace EventSourcingCQRS.Commands
{
    public class CommandHandler
    {
        private readonly EventSourceContext _ctx;
        public CommandHandler(EventSourceContext ctx) { _ctx = ctx; }

        public void AddCount(AddCountCommand cmd)
        {
            _ctx.EventLogs.Add(cmd.LogObject);
            _ctx.SaveChanges();
        }

        public void ReduceCount(ReduceCountCommand cmd)
        {
            _ctx.EventLogs.Add(cmd.LogObject);
            _ctx.SaveChanges();
        }
    }
}
