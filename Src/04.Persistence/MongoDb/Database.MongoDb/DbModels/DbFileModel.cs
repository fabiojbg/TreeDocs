using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Repository.MongoDb.DbModels
{
    public class DbFileModel : DbNodeEntity
    {
        public string Name { get; set; }
        public byte[] Contents { get; set; }
        public string ContentsHash { get; set; }
    }
}
