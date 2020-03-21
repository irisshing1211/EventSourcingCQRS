using MongoDB.Bson.Serialization.Attributes;

namespace EventSourcingCQRS.Entities.NoSql
{
    public class CountItem
    {
        [BsonId]
        public string Id { get; set; }
        public string ItemName { get; set; }
        public int Count { get; set; }
    }
}
