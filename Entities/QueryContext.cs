using EventSourcingCQRS.Entities.NoSql;
using EventSourcingCQRS.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace EventSourcingCQRS.Entities
{
    public class QueryContext
    {
        private readonly IMongoDatabase _db = null;

        public QueryContext(IOptions<DbSettings> settings)
        {
            var client = new MongoClient(settings.Value.NoSqlConnString);

            if (client != null)
                _db = client.GetDatabase(settings.Value.DatabaseName);
        }

        public IMongoCollection<CountItem> CountItems => _db.GetCollection<CountItem>("CountItems");

    }
}
