using EventSourcingCQRS.Entities;

namespace EventSourcingCQRS.Events
{
    public class BaseEvent
    {
        internal readonly QueryContext _ctx;

        public BaseEvent(QueryContext ctx ) { _ctx = ctx; }
    }
}
