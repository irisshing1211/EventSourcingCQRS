using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Security.Principal;

namespace EventSourcingCQRS.Entities.Relational
{
    public class EventLog
    {
        public EventLog()
        {
            
        }
        public EventLog(string id, string oldValue, string newValue, LogAction action)
        {
            ItemId = id;
            OldValue = oldValue;
            NewValue = newValue;
            Action = action;
            Time = DateTime.Now;
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public DateTime Time { get; set; }
        public string ItemId { get; set; }
        public string OldValue { get; set; }
        public string NewValue { get; set; }
        public LogAction Action { get; set; }
    }
}
