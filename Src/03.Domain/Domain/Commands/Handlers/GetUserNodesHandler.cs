using Apps.Sdk.DependencyInjection;
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
    public class GetUserNodesHandler : Notifiable, IRequestHandler<GetUserNodesRequest, RequestResult<GetUserNodesResponse>>
    {
        IAppDatabase _repository;
        IUserServices _userServices;

        public GetUserNodesHandler(IAppDatabase repository, IUserServices userServices)
        {
            _repository = repository;
            _userServices = userServices;
            
        }

        public async Task<RequestResult<GetUserNodesResponse>> Handle(GetUserNodesRequest request, CancellationToken cancellationToken)
        {
            if (!request.Validate())
                return new RequestResult<GetUserNodesResponse>(request.Notifications);

            if ( _userServices.LoggedUserId == null ||(request.UserId != _userServices.LoggedUserId && !_userServices.HasPrivilege(Privilege.EditAnotherUserNodes)))
                return new RequestResult<GetUserNodesResponse>(DomainResources.ErrCannotCreateNodeInAnotherUserNode, RequestResultType.Unauthorized);

            var userNodes = await _repository.UserNodes.GetAllUserNodesWithoutContentsAsync(request.UserId);

            if( !userNodes.Any() )
            {
                var userNode = new UserNode(null, _userServices.LoggedUser.Name, Enums.NodeType.Folder, _userServices.LoggedUserId, null);
                await _repository.UserNodes.CreateAsync(userNode);
                userNodes = await _repository.UserNodes.GetAllUserNodesWithoutContentsAsync(request.UserId);
            }

            var response = new GetUserNodesResponse
            {
                OwnerId = request.UserId,
                Nodes = convertToHierarchicalNodes(userNodes, null)
                //Nodes = fillChildrenOrderFields(userNodes)
                //Nodes = userNodes.Select(x => new Node(x))
            };

            return new RequestResult<GetUserNodesResponse>(response);
        }

        private IEnumerable<Node> convertToHierarchicalNodes(IEnumerable<UserNode> plainNodes, string parentNodeId)
        {
            var result = new List<Node>();
            foreach (var userNode in plainNodes.Where(x => x.ParentId == parentNodeId))
            {
                var node = new Node(userNode);
                var children = convertToHierarchicalNodes(plainNodes, node.Id);

                var childrenOrder = userNode.ChildrenOrder.Where(x => children.Any(y => y.Id.Equals(x))).ToList();
                var missingChilds = children.Where(x => !childrenOrder.Any(y => y.Equals(x.Id))).OrderBy(x => x.Name).Select(x => x.Id).ToList();
                childrenOrder.AddRange(missingChilds);

                node.Children = new List<Node>();
                childrenOrder.ForEach(x => {
                    var item = children.First(y => y.Id == x);
                    node.Children.Add(item);
                    });
                node.ChildrenOrder = childrenOrder;
                result.Add(node);
            }
            return result;
        }

        //private IEnumerable<Node> fillChildrenOrderFields(IEnumerable<UserNode> plainNodes)
        //{
        //    var result = new List<Node>();
        //    foreach (var userNode in plainNodes)
        //    {
        //        var children = plainNodes.Where( x => x.ParentId == userNode.Id).ToList();
        //        if (userNode.ChildrenOrder?.Any() != true)
        //        {
        //            userNode.ChildrenOrder = children.OrderBy(x => x.Name).Select(x => x.Id).ToList();
        //        }
        //        else
        //        {
        //            var childrenOrder = userNode.ChildrenOrder.Where(x => children.Any(y => y.Id.Equals(x))).ToList();
        //            var missingChilds = children.Where(x => !childrenOrder.Any(y => y.Equals(x.Id))).OrderBy(x => x.Name).Select(x => x.Id).ToList();
        //            userNode.ChildrenOrder.Clear();
        //            userNode.ChildrenOrder.AddRange(childrenOrder);
        //            userNode.ChildrenOrder.AddRange(missingChilds);
        //        }

        //        var node = new Node(userNode);
        //        result.Add(node);
        //    }
        //    return result;
        //}

    }
}
