using System;

using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace RequestDropper.Mongo.Models
{
    public class MongoDropCounter
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string Key { get; set; }
        public int Count { get; set; }
        public DateTimeOffset TimeStamp { get; set; }
    }
}
