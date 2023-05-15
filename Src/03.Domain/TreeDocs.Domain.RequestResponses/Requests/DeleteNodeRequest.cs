using Domain.Shared;
using Domain.Shared.Validations;
using MediatR;

namespace TreeDocs.Domain.RequestsResponses
{
    public class DeleteNodeRequest : ValidatableRequest, IRequest<RequestResult<DeleteNodeResponse>>
    {
        public string NodeId { get; set; }

        public override bool Validate()
        {
            AddNotifications(new Validator().IsNotNullOrWhiteSpace(NodeId, nameof(NodeId), "Node Id is required"));

            return Valid;
        }
    }
}
