using System;
using System.Collections.Generic;
using EventSourcingCQRS.Entities.NoSql;

namespace EventSourcingCQRS.Models
{
    public class IndexModel
    {
        public List<CountItem> Items { get; set; }
        public List<DateTime> Logs { get; set; }
    }
}
