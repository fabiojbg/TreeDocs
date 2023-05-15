using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Text;

namespace Repository.MongoDb.DbModels
{

    public class DbUserNode : DbNodeEntity
    {
        public string Name { get; set; }
        public byte NodeType { get; set; }
        public string InheritancePath { get; set; }
        public Nullable<ObjectId> ContentId { get; set; }
        public string Contents { get; set; }
        public string[] ChildrenOrder { get; set; }
    }

    public class DbNodeContents : DbFileModel
    {

    }
}
