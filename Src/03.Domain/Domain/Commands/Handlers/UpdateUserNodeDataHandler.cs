using Apps.Sdk.Extensions;
using Domain.Shared;
using Domain.Shared.Validations;
using MediatR;
using Pipelines.Sockets.Unofficial.Arenas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TreeNotes.Domain.Entities;
using TreeNotes.Domain.Repositories;
using TreeNotes.Domain.RequestResponses;
using TreeNotes.Domain.RequestsResponses;
using TreeNotes.Domain.Services;

namespace TreeNotes.Domain.Commands.Handlers
{

    public class UpdateUserNodeDataHandler : Notifiable, IRequestHandler<UpdateUserNodeDataRequest, RequestResult<UpdateUserNodeDataResponse>>
    {
        IUserNodeRepository _nodeRep;
        IUserServices _userServices;
        public UpdateUserNodeDataHandler(IUserNodeRepository nodeRep, IUserServices userServices)
        {
            _nodeRep = nodeRep;
            _userServices = userServices;
        }

        public async Task<RequestResult<UpdateUserNodeDataResponse>> Handle(UpdateUserNodeDataRequest request, CancellationToken cancellationToken)
        {
            if (!request.Validate())
                return new RequestResult<UpdateUserNodeDataResponse>(request.Notifications);

            var dbNode = await _nodeRep.GetByIdAsync(request.NodeId);
            if( dbNode == null)
                return new RequestResult<UpdateUserNodeDataResponse>(DomainResources.ErrNodeNotFound, RequestResultType.ObjectNotFound);

            var oldParentId = dbNode.ParentId;
            if (request.ParentId!=null && !request.ParentId.EqualsIgnoreCase(oldParentId) )
            {
                var newParent = await _nodeRep.GetByIdAsync(request.ParentId);
                if (newParent == null)
                    AddNotification(DomainResources.ErrFolderNotFound);
                else
                {
                    if (newParent.OwnerId != _userServices.LoggedUserId)
                        AddNotification(DomainResources.ErrCannotCreateNodeInAnotherUserNode);
                    else
                    {
                        if (await newParentIsADescendent(request.NodeId, newParent))
                        {
                            AddNotification(DomainResources.ErrCannotMoveNodeToDescendent);
                            return new RequestResult<UpdateUserNodeDataResponse>(Notifications);
                        }
                        else
                            dbNode.ParentId = request.ParentId;
                    }
                }
            }
            
            if( !request.Name.IsNullOrEmpty() && !request.Name.Trim().EqualsCI(dbNode.Name))
            {
                dbNode.Name = request.Name.Trim();
                if( !dbNode.Validate() )
                    AddNotifications(dbNode);
            }

            if ( request.ChildrenOrder != null)
            {
                var existingChildren = await _nodeRep.GetChildrenWithoutContentsAsync(request.NodeId);
                var newList = new List<string>();
                request.ChildrenOrder.ForEach(x => { // includes all existing childs. Warning: do not use Distinct because it does not guarantee the order
                    if( !newList.Any( y=> y.EqualsCI(x) && existingChildren.Any(y => y.Id.EqualsCI(x))))
                        newList.Add(x);
                });
                var missingChilds = existingChildren.Where(x => !newList.Any(y => y == x.Id)).OrderBy(w => w.Name).Select(z => z.Id).ToList();
                dbNode.ChildrenOrder = newList;// add the existing childs in the order provided and removes the invalid ones
                dbNode.ChildrenOrder.AddRange(missingChilds); // add missing childs
            }

            var existingNode = await _nodeRep.GetNodeByName(_userServices.LoggedUserId, dbNode.ParentId, dbNode.Name);
            if (Valid && existingNode?.Id != null && existingNode?.Id != dbNode.Id )
                AddNotification(nameof(request.ParentId), DomainResources.ErrNodeAlreadyExists);

            if (this.Invalid)
                return new RequestResult<UpdateUserNodeDataResponse>(Notifications);

            if ( request.NodeContents != null)
                dbNode.Contents = String.IsNullOrWhiteSpace(request.NodeContents) ? null : request.NodeContents;

           await _nodeRep.UpdateAsync(dbNode);

           await _nodeRep.SaveChanges();

            var response = new UpdateUserNodeDataResponse { Node = new Node(dbNode) };

            return new RequestResult<UpdateUserNodeDataResponse>(response);

        }

        async Task<bool> newParentIsADescendent(string oldParentId, UserNode newParent)
        {
            var possibleDescendent = newParent;
            do
            {
                if (possibleDescendent.ParentId == null)
                    return false;

                if (possibleDescendent.ParentId == oldParentId)
                    return true;

                possibleDescendent = await _nodeRep.GetByIdAsync(possibleDescendent.ParentId, nameof(UserNode.ParentId));
            }
            while (possibleDescendent != null);

            return false;
        }
    }
}
