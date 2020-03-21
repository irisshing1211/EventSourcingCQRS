using EventSourcingCQRS.Entities;
using EventSourcingCQRS.Entities.Relational;

namespace EventSourcingCQRS.Events
{
    public class AddCountEvent: BaseEvent
    {
        public AddCountEvent(QueryContext ctx): base(ctx)
        {
            
        }

        public void Push(EventLog log)
        {
            
        }
    }
}
