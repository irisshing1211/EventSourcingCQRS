using System;

namespace EventSourcingCQRS.Commands
{
    public class RollbackCommand
    {
        public  DateTime Time { get; set; }
    }
}
