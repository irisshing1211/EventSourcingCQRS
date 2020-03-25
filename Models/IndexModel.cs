using System;
using System.Collections.Generic;
using EventSourcingCQRS.Entities.NoSql;

namespace EventSourcingCQRS.Models
{
    public class IndexModel
    {
        public List<CountItem> Items { get; set; }
        public List<IndexLogItemsModel> Logs { get; set; }
    }

    public class IndexLogItemsModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public List<IndexLogsModel> Logs { get; set; }
    }

    public class IndexLogsModel
    {
        public DateTime Time { get; set; }
        public LogAction Action { get; set; }
    }
}
