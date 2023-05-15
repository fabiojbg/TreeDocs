using System;

namespace TreeDocs.Db.Entities
{
    //public class BaseEntity
    //{
    //    public string Id { get; set; }
    //    public DateTime CreatedOn;
    //    public DateTime UpdatedOn;
    //}

    public class UserNode
    {
        public string Id { get; set; }
        public string ParentId { get; set; }
        public string Name { get; set; }
        public byte NodeType { get; set; }
        public string InheritancePath { get; set; }
        public Child[] Children { get; set; }
        public string Content { get; set; }
        public string LongContentId { get; set; }
        public DateTime CreatedOn;
        public DateTime UpdatedOn;
    }

    public class Child
    {
        public string Id { get; set; }
        public int Order { get; set; }
    }

    public class NodeContents
    {
        public string Id { get; set; }
        public string Contents { get; set; } 
        public string ContentsHash { get; set; }
        public DateTime CreatedOn;
        public DateTime UpdatedOn;
    }
}
