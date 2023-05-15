using Apps.Sdk.Extensions;
using Domain.Shared.Validations;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TreeDocs.Domain.Entities;
using TreeDocs.Domain.Repositories;
using Domain.Shared;
using TreeDocs.Domain.RequestsResponses;
using Auth.Domain.Services;

namespace TreeDocs.Domain.Handlers
{
    public class CreateNodeHandler : Notifiable,  IRequestHandler<CreateNodeRequest, RequestResult<CreateNodeResponse>>
    {
        IUserNodeRepository _nodeRep;
        IAppUserService _appUserService;

        public CreateNodeHandler(IUserNodeRepository nodeRep, IAppUserService appUserService)
        {
            _nodeRep = nodeRep;
            _appUserService = appUserService;
        }

        public async Task<RequestResult<CreateNodeResponse>> Handle(CreateNodeRequest request, CancellationToken cancellationToken)
        {
            var userNode = new UserNode(request.ParentId, request.Name, request.NodeType, _appUserService.GetLoggedUserId(), request.Contents);
            if (userNode.Invalid)
            {
                AddNotifications(request);
                return new RequestResult<CreateNodeResponse>(this.Notifications);
            }

            if( request.ParentId == null)
                return new RequestResult<CreateNodeResponse>(DomainResources.ErrNodeMustHaveParent);

            var parent = await _nodeRep.GetByIdAsync(request.ParentId);
            if (parent == null)
                AddNotification(DomainResources.ErrFolderNotFound);
            else
            {
                if (parent.OwnerId != userNode.OwnerId)
                    AddNotification(DomainResources.ErrCannotCreateNodeInAnotherUserNode);
            }

            if ( Valid && await _nodeRep.GetNodeByName(_appUserService.GetLoggedUserId(), request.ParentId, request.Name)!=null)
                AddNotification(DomainResources.ErrNodeAlreadyExists);

            if( Invalid )
                return new RequestResult<CreateNodeResponse>(this.Notifications);

            userNode.OwnerId = _appUserService.GetLoggedUserId();
            var nodeId = await _nodeRep.CreateAsync(userNode);

            var response = new CreateNodeResponse
            {
                Id = nodeId
            };

            await _nodeRep.SaveChanges();

            return new RequestResult<CreateNodeResponse>(response, DomainResources.MsgNodeCreationSuccess.Format(request.Name));
        }

    }
}
