using Domain.Shared;
using Domain.Shared.Validations;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;
using TreeDocs.Domain.RequestResponses;

namespace TreeDocs.Domain.RequestsResponses
{
    public class GetUserNodeRequest : ValidatableRequest, IRequest<RequestResult<GetUserNodeResponse>>
    {
        public string NodeId { get; set; }

        public override bool Validate()
        {
            AddNotifications(new Validator().IsNotNullOrWhiteSpace(NodeId, nameof(NodeId), DomainResources.ErrUserNodeIdRequired));

            return Valid;
        }
    }
}
