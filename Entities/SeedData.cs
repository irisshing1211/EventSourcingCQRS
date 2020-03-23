using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using System.Threading.Tasks;
using EventSourcingCQRS.Entities.NoSql;
using EventSourcingCQRS.Entities.Relational;
using EventSourcingCQRS.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Newtonsoft.Json;

namespace EventSourcingCQRS.Entities
{
    public static class SeedData
    {
        public static void Initialize(IServiceProvider serviceProvider, DbSettings setting)
        {
            using (var scope = serviceProvider.CreateScope())
            {
                var services = scope.ServiceProvider;

                try
                {
                    using (var context = new EventSourceContext(
                        services.GetRequiredService<DbContextOptions<EventSourceContext>>()))
                    {
                        context.Database.Migrate();

                        if (context.EventLogs.Any())
                        {
                            SeedMongo(setting, context);

                            return; // DB has been seeded
                        }

                        var item1 = new CountItem {Id = Guid.NewGuid(), ItemName = "Item 1", Count = 0};
                        var item2 = new CountItem {Id = Guid.NewGuid(), ItemName = "Item 2", Count = 1};

                        context.EventLogs.Add(new EventLog(item1.Id,
                                                           "",
                                                           JsonConvert.SerializeObject(item1),
                                                           LogAction.Insert));

                        context.EventLogs.Add(new EventLog(item2.Id,
                                                           "",
                                                           JsonConvert.SerializeObject(item2),
                                                           LogAction.Insert));

                        context.SaveChanges();
                        SeedMongo(setting, context);
                    }
                }
                catch (Exception ex)
                {
                    //  Log.Error(ex.Message);
                }
            }
        }

        private static void SeedMongo(DbSettings settings, EventSourceContext eventSourceContext)
        {
            var client = new MongoClient(settings.NoSqlConnString);

            if (client != null)
            {
                client.DropDatabase(settings.DatabaseName);
                var queryDb = new QueryContext(settings);
                var eventList = eventSourceContext.EventLogs.AsQueryable().OrderBy(a => a.Time).ToList();

                foreach (var log in eventList)
                {
                  

                    switch (log.Action)
                    {
                        case LogAction.Insert:  
                            var item = LogParser.ConvertStringToObject<CountItem>(log.NewValue);
                            queryDb.CountItems.InsertOne(item);

                            break;
                        case LogAction.Update:
                            var old = queryDb.CountItems.Find(a => a.Id == log.ItemId).FirstOrDefault();
                            var update = LogParser.UpdateObjectByData(old, log.NewValue);
                            queryDb.CountItems.ReplaceOne(a => a.Id == log.ItemId, update);

                            break;
                        case LogAction.Delete:
                            queryDb.CountItems.DeleteOne(a => a.Id == log.ItemId);

                            break;
                    }
                }
            }
        }
    }
}
