using Apps.Sdk.Extensions;
using Domain.Shared.Validations;
using System;
using System.Collections.Generic;
using TreeNotes.Domain.Enums;
using TreeNotes.Domain.RequestResponses;

namespace TreeNotes.Domain.Entities
{
    public class UserNode : Entity, IValidatable
    {
        public string ParentId { get; set; }
        public string Name { get; set; }
        public NodeType NodeType { get; set; }
        public string Contents { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime UpdatedOn { get; set; }
        public string OwnerId { get; set; }
        public List<string> ChildrenOrder { get; set; }

        public UserNode(string parentId, string name, NodeType nodeType, string ownerId, string contents)
        {
            ParentId = parentId;
            Name = name;
            NodeType = nodeType;
            Contents = contents;
            OwnerId = ownerId;
            ChildrenOrder = new List<string>();
            Validate();
        }

        public bool Validate()
        {
            AddNotifications(new Validator()
                            .IsNotNullOrWhiteSpace(OwnerId, nameof(OwnerId), "OwnerId parameter is required")
                            .HasMinLen(Name, Constants.MIN_NODE_NAME_LEN, null, DomainResources.ErrUserNameMaxLen.Format(Constants.MIN_NODE_NAME_LEN))
                            .HasMaxLen(Name, Constants.MAX_NODE_NAME_LEN, null, DomainResources.ErrUserNameMinLen.Format(Constants.MAX_NODE_NAME_LEN)));
            return Valid; 
        }

        public override string ToString()
        {
            return $"{Id}({Name})";
        }

    }

}
