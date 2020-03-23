using EventSourcingCQRS.Entities.Relational;

namespace EventSourcingCQRS.Commands
{
    public class ReduceCountCommand : BaseCountCommand
    {
        public EventLog LogObject => new EventLog(Id, (NewValue + 1).ToString(), NewValue.ToString(), LogAction.Update);
    }
}
