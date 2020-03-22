using EventSourcingCQRS.Entities.Relational;

namespace EventSourcingCQRS.Commands
{
    public class AddCountCommand : BaseCountCommand
    {
      public EventLog LogObject=>new EventLog(Id, (NewValue-1).ToString(), NewValue.ToString(), LogAction.Add);
    }
}
