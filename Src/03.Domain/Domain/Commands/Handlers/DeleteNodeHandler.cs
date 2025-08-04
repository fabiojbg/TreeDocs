using Domain.Shared;
using Domain.Shared.Validations;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TreeNotes.Domain.Repositories;
using TreeNotes.Domain.RequestsResponses;
using TreeNotes.Domain.Services;

namespace TreeNotes.Domain.Commands.Handlers
{


    public class DeleteNodeDataHandler : Notifiable, IRequestHandler<DeleteNodeRequest, RequestResult<DeleteNodeResponse>>
    {
        IUserNodeRepository _nodeRep;
        IUserServices _userServices;

        public DeleteNodeDataHandler(IUserNodeRepository nodeRep, IUserServices userServices)
        {
            _nodeRep = nodeRep;
            _userServices = userServices;
        }

        public async Task<RequestResult<DeleteNodeResponse>> Handle(DeleteNodeRequest request, CancellationToken cancellationToken)
        {
            if (!request.Validate())
                return new RequestResult<DeleteNodeResponse>(request.Notifications);

            var dbNode = await _nodeRep.GetByIdAsync(request.NodeId);
            if (dbNode == null)
                return new RequestResult<DeleteNodeResponse>(DomainResources.ErrNodeNotFound, RequestResultType.ObjectNotFound);

            if (_userServices.LoggedUserId == null || dbNode.OwnerId != _userServices.LoggedUserId)
                return new RequestResult<DeleteNodeResponse>("Access Denied", RequestResultType.Unauthorized);

            var nodesToDelete = new List<(string Id, string Name)>();
            var notDeletedNodes = new List<string>();
            await getAllChildrenToDelete(dbNode.Id, dbNode.Name, nodesToDelete);
            foreach (var nodeToDelete in nodesToDelete)
            {
                try
                {
                    await _nodeRep.DeleteAsync(nodeToDelete.Id);
                }
                catch (Exception ex)
                {
                    notDeletedNodes.Add( nodeToDelete.Id );
                    AddNotification($"Error deleting node {nodeToDelete.Name}({nodeToDelete.Id}). Error = {ex.Message}");
                }
            }
            if (notDeletedNodes.Any())
            {
                if (notDeletedNodes.Contains(request.NodeId))
                {
                    if( notDeletedNodes.Count > 1)
                        return new RequestResult<DeleteNodeResponse>(this.Notifications, DomainResources.ErrSomeChildrenCouldNotBeRemoved, RequestResultType.OperationError);
                    else
                        return new RequestResult<DeleteNodeResponse>(this.Notifications, DomainResources.ErrNoteCouldNotBeRemoved, RequestResultType.OperationError);
                }
            }
            else
                await _nodeRep.SaveChanges();

            return new RequestResult<DeleteNodeResponse>(new DeleteNodeResponse());
        }

        private async Task getAllChildrenToDelete(string parentNodeId, string parentNodeName, List<(string Id, string Name)> nodesList)
        {
            var childNodes = await _nodeRep.GetChildrenWithoutContentsAsync(parentNodeId);
            foreach (var child in childNodes)
                await getAllChildrenToDelete(child.Id, child.Name, nodesList);

            nodesList.AddRange(childNodes.Select( x => (x.Id, x.Name))); // this must be done last in order to the child nodes be inserted first in the list
            nodesList.Add((Id: parentNodeId, Name: parentNodeName));
        }
    }
}
