using System;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.IdGenerators;

namespace EventSourcingCQRS.Entities.NoSql
{
    public class CountItem
    {
        [BsonId(IdGenerator = typeof(GuidGenerator))]
        public Guid Id { get; set; }
        public string ItemName { get; set; }
        public int Count { get; set; }
    }
}
