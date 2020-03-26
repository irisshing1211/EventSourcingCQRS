using System;
using EventSourcingCQRS.Entities.NoSql;
using EventSourcingCQRS.Entities.Relational;

namespace EventSourcingCQRS.Commands
{
    public class UpdateCountCommand
    {
        public UpdateCountCommand(CountItem old, int updateCnt)
        {
            OldValue = old;
            NewValue = new CountItem {ItemName = old.ItemName, Id = old.Id, Count = old.Count + updateCnt};
        }

        public CountItem OldValue { get; set; }
        public CountItem NewValue { get; set; }

        public EventLog LogObject =>
            new EventLog(OldValue.Id,
                         LogParser.ConvertToHistory(OldValue),
                         LogParser.GetUpdatedField(OldValue, NewValue),
                         LogAction.Update);
    }
}
