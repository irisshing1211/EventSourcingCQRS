using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using System.Threading.Tasks;
using EventSourcingCQRS.Entities.NoSql;
using EventSourcingCQRS.Entities.Relational;
using EventSourcingCQRS.Models;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
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

                try
                {
                    // get latest snapshot / add / delete event for each item
                    var itemList = eventSourceContext
                                   .EventLogs.Where(a => a.Action == LogAction.Insert ||
                                                         a.Action == LogAction.Snapshot ||
                                                         a.Action == LogAction.Delete).ToList()
                                   .GroupBy(a => a.ItemId)
                                   .ToList();

                    var snapshotList = itemList
                                       .Select(g => new {Log = g.OrderByDescending(a => a.Time).FirstOrDefault()})
                                       .ToList();

                    // filter deleted item
                    var itemGrpList = snapshotList.Where(a => a.Log.Action != LogAction.Delete)
                                                  .Select(a => a.Log)
                                                  .ToList();

                    foreach (var snapshot in itemGrpList)
                    {
                        // get all event after snapshot / add
                        var eventList = eventSourceContext
                                        .EventLogs.Where(a => a.ItemId == snapshot.ItemId && a.Time > snapshot.Time)
                                        .OrderBy(a => a.Time)
                                        .ToList();

                        // parse snapshot item
                        var item = LogParser.ConvertStringToObject<CountItem>(snapshot.NewValue);

                        // update item by event
                        foreach (var log in eventList) { item = LogParser.UpdateObjectByData(item, log.NewValue); }

                        queryDb.CountItems.InsertOne(item);
                    }
                }
                catch (Exception ex) {}
            }
        }
    }
}
