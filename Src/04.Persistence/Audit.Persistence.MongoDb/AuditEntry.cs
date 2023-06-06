using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Audit.Persistence.MongoDb
{
    public class AuditEntry
    {
        [BsonId]
        public ObjectId Id { get; set; }

        [BsonDateTimeOptions(Kind = DateTimeKind.Utc, DateOnly = false)]
        public DateTime CreatedOn { get; set; }

        public string? UserName { get; set; }
        public string? UserId { get; set; }
        public string? UserIP { get; set; }
        public string? Message { get; set; }
        public string? MessageDetails { get; set; }
    }
}
