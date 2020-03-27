using System;

namespace EventSourcingCQRS.Models
{
    public class UpdateItemModel
    {
        public  Guid Id { get; set; }
        public string Name { get; set; }
        public int Count { get; set; }
    }
}
