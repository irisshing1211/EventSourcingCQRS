using EventSourcingCQRS.Entities.NoSql;
using EventSourcingCQRS.Entities.Relational;
using EventSourcingCQRS.Models;

namespace EventSourcingCQRS.Commands
{
    public class UpdateItemCommand
    {
        private string updatedField;

        public UpdateItemCommand(CountItem old, UpdateItemModel req)
        {
            NewValue = new CountItem {Id = req.Id, ItemName = req.Name, Count = req.Count};
            updatedField = LogParser.GetUpdatedField(old, NewValue);
        }

        public CountItem NewValue { get; set; }
        public EventLog LogObject =>
            new EventLog(NewValue.Id,
                         "",
                         updatedField,
                         LogAction.Update);
    }
}
