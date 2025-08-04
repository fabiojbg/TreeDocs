using Domain.Shared;
using Domain.Shared.Validations;
using MediatR;
using System.Collections.Generic;

namespace TreeNotes.Domain.RequestsResponses
{ 
    public class UpdateUserNodeDataRequest : ValidatableRequest, IRequest<RequestResult<UpdateUserNodeDataResponse>>
    {
        public string NodeId { get; set; }
        public string ParentId { get; set; }
        public string Name { get; set; }
        public string NodeContents { get; set; }
        public List<string> ChildrenOrder { get; set; }

        public override bool Validate()
        {
            AddNotifications(new Validator().IsNotNullOrWhiteSpace(NodeId, nameof(NodeId), "NodeId is required"));

            return Valid;
        }
    }
}
