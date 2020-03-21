using EventSourcingCQRS.Entities.Relational;
using Microsoft.EntityFrameworkCore;

namespace EventSourcingCQRS.Entities
{
    public class EventSourceContext : DbContext
    {
        public DbSet<EventLog> EventLogs { get; set; }
    }
}
