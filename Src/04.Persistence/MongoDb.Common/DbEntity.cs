using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace MongoDb.Common
{
    public class DbEntity
    {
        [BsonId]
        public ObjectId Id { get; set; }

        [BsonDateTimeOptions(Kind = DateTimeKind.Utc, DateOnly = false)]
        public DateTime CreatedOn { get; set; }
        
        [BsonDateTimeOptions(Kind = DateTimeKind.Utc, DateOnly = false)]
        public DateTime UpdatedOn { get; set; }
    }
}
