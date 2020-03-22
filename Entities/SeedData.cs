using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using System.Threading.Tasks;
using EventSourcingCQRS.Entities.NoSql;
using EventSourcingCQRS.Entities.Relational;
using Newtonsoft.Json;

namespace EventSourcingCQRS.Entities
{
    public static class SeedData
    {
        public static void Initialize(IServiceProvider serviceProvider)
        {
            using (var context = new EventSourceContext(
                serviceProvider.GetRequiredService<DbContextOptions<EventSourceContext>>()))
            {
                context.Database.Migrate();

                if (context.EventLogs.Any())
                {
                    return; // DB has been seeded
                }

                var item1 = new CountItem {Id = "", ItemName = "Item 1", Count = 0};
                var item2 = new CountItem {Id = "", ItemName = "Item 2", Count = 1};
                context.EventLogs.Add(new EventLog("", "", JsonConvert.SerializeObject(item1), LogAction.Add));
                context.EventLogs.Add(new EventLog("", "", JsonConvert.SerializeObject(item2), LogAction.Add));
                context.SaveChanges();
            }
        }
    }
}
