using Apps.Sdk.Extensions;
using Domain.Shared;
using Domain.Shared.Validations;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TreeNotes.Domain.Entities;
using TreeNotes.Domain.Repositories;
using TreeNotes.Domain.RequestsResponses;
using TreeNotes.Domain.Services;

namespace TreeNotes.Domain.Handlers
{
    public class GetUserNodeHandler : Notifiable, IRequestHandler<GetUserNodeRequest, RequestResult<GetUserNodeResponse>>
    {
        IAppDatabase _repository;
        IUserServices _userServices;

        public GetUserNodeHandler(IAppDatabase repository, IUserServices userServices)
        {
            _repository = repository;
            _userServices = userServices;
        }

        public async Task<RequestResult<GetUserNodeResponse>> Handle(GetUserNodeRequest request, CancellationToken cancellationToken)
        {
            if (!request.Validate())
                return new RequestResult<GetUserNodeResponse>(request.Notifications);

            var userNode = await _repository.UserNodes.GetByIdAsync(request.NodeId);

            if( userNode == null)
                return new RequestResult<GetUserNodeResponse>(DomainResources.ErrNodeNotFound, RequestResultType.ObjectNotFound);

            if ( _userServices.LoggedUserId == null || (userNode.OwnerId != _userServices.LoggedUserId && !_userServices.HasPrivilege(Privilege.EditAnotherUserNodes)))
                return new RequestResult<GetUserNodeResponse>(DomainResources.ErrCannotCreateNodeInAnotherUserNode, RequestResultType.Unauthorized);

            var children = await _repository.UserNodes.GetChildrenWithoutContentsAsync(userNode.Id);
            if( userNode.ChildrenOrder?.Any() != true)
            {
                userNode.ChildrenOrder = children.OrderBy(x => x.Name).Select( x => x.Id).ToList();
            }
            else
            {
                var childrenOrder = userNode.ChildrenOrder.Where(x => children.Any(y => y.Id.Equals(x))).ToList();
                var missingChilds = children.Where(x => !childrenOrder.Any(y => y.Equals(x.Id))).OrderBy( x => x.Name).Select( x => x.Id).ToList();
                userNode.ChildrenOrder.Clear();
                userNode.ChildrenOrder.AddRange(childrenOrder);
                userNode.ChildrenOrder.AddRange(missingChilds);
            }

            var response = new GetUserNodeResponse
            {
                Node = new Node(userNode)
            };

            return new RequestResult<GetUserNodeResponse>(response);
        }

    }
}
