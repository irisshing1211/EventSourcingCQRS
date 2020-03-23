using System;

namespace EventSourcingCQRS.Commands
{
    public class BaseCountCommand
    {
        public Guid Id { get; set; }
        public int NewValue { get; set; }
    }
}
