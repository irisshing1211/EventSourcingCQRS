using System;

namespace EventSourcingCQRS.Models
{
    public class UpdateCountModel
    {
        public Guid Id { get; set; }
        public int Value { get; set; }
    }
}
