using System;

namespace EventSourcingCQRS.Commands
{
    public class RollbackSpecificCommand
    {
        public Guid Id { get; set; }
        public DateTime Time { get; set; }
    }
}
