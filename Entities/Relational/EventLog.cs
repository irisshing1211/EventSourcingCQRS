using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Security.Principal;

namespace EventSourcingCQRS.Entities.Relational
{
    public class EventLog
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string ItemId { get; set; }
        public string OldValue { get; set; }
        public string NewValue { get; set; }
        public LogAction Action { get; set; }
    }
}
