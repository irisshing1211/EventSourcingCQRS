using EventSourcingCQRS.Entities.Relational;
using Microsoft.EntityFrameworkCore;

namespace EventSourcingCQRS.Entities
{
    public class EventSourceContext : DbContext
    {
        public EventSourceContext(DbContextOptions<EventSourceContext> options) : base(options) {}

        public DbSet<EventLog> EventLogs { get; set; }
    }
}
