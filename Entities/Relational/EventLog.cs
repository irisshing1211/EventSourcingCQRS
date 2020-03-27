﻿using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Security.Principal;
using EventSourcingCQRS.Entities.NoSql;

namespace EventSourcingCQRS.Entities.Relational
{
    public class EventLog
    {
        public EventLog() {}

        public EventLog(Guid id, string oldValue, string newValue, LogAction action)
        {
            ItemId = id;
            NewValue = newValue;
            Action = action;
            Time = DateTime.Now;
        }

        /// <summary>
        /// for snapshot
        /// </summary>
        /// <param name="item">current item state</param>
        public EventLog(CountItem item)
        {
            ItemId = item.Id;
            NewValue = LogParser.ConvertToHistory(item);
            Action = LogAction.Snapshot;
            Time = DateTime.Now;
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public DateTime Time { get; set; }
        public Guid ItemId { get; set; }
        public string NewValue { get; set; }
        public LogAction Action { get; set; }
    }
}
