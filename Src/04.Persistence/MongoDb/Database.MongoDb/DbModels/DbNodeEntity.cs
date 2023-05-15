using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace Repository.MongoDb.DbModels
{
    public class DbNodeEntity
    {
        [BsonId]
        public ObjectId Id { get; set; }

        public string OwnerId { get; set; }

        public Nullable<ObjectId> ParentId { get; set; }

        [BsonDateTimeOptions(Kind = DateTimeKind.Utc, DateOnly = false)]
        public DateTime CreatedOn { get; set; }
        
        [BsonDateTimeOptions(Kind = DateTimeKind.Utc, DateOnly = false)]
        public DateTime UpdatedOn { get; set; }
    }
}
