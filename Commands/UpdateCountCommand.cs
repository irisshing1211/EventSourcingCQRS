using System;
using System.Collections.Generic;
using EventSourcingCQRS.Entities.NoSql;
using EventSourcingCQRS.Entities.Relational;

namespace EventSourcingCQRS.Commands
{
    public class UpdateCountCommand
    { private string updatedField;
        public UpdateCountCommand(CountItem old, int updateCnt)
        {
         //   OldValue = old;
            NewValue = new CountItem {ItemName = old.ItemName, Id = old.Id, Count = old.Count + updateCnt};
            updatedField = LogParser.GetUpdatedField(old, NewValue);
        }

       
       // public CountItem OldValue { get; set; }
        public CountItem NewValue { get; set; }

        public EventLog LogObject =>
            new EventLog(NewValue.Id,
                         "",
                         updatedField,
                         LogAction.Update);
    }
}
