using System;
using System.Collections.Generic;
using TreeNotes.Domain.Entities;
using TreeNotes.Domain.Enums;

namespace TreeNotes.Domain.RequestsResponses
{
    public class Node
    {
        public string Id { get; set; }
        public string ParentId { get; set; }
        public string Name { get; set; }
        public NodeType NodeType { get; set; }
        public string Contents { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime UpdatedOn { get; set; }
        public List<Node> Children { get; set; }
        public List<string> ChildrenOrder { get; set; }

        public Node(UserNode userNode)
        {
            if (userNode == null) return;
            Id = userNode.Id;
            ParentId = userNode.ParentId;
            Name = userNode.Name;
            NodeType = userNode.NodeType;
            CreatedOn = userNode.CreatedOn;
            UpdatedOn = userNode.UpdatedOn;
            Contents = userNode.Contents;
            ChildrenOrder = userNode.ChildrenOrder;
        }

    }
}
