using Apps.Sdk.DependencyInjection;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using TreeDocs.Domain.RequestsResponses;
using TreeDocs.Service.Filters;

namespace TreeDocs.Service.ApiController
{
    [Route("api/v1/nodes")]
    [ApiController]
    [Authorize, AuthenticationRequired]
    public class NodesController : ApiBaseController
    {
        protected override ILogger Logger => _logger ??= SdkDI.Resolve<ILogger<NodesController>>();

        public NodesController()
        {
        }

        [HttpGet]
        public async Task<IActionResult> GetUserNodes()
        {
            try
            {
                var userNodesRequest = new GetUserNodesRequest();
                userNodesRequest.UserId = AuthenticatedUser?.Id;

                var result = await Mediator.Send(userNodesRequest);

                return ToActionResult(result);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "There was an error while getting user notes");
                return StatusCode((int)HttpStatusCode.InternalServerError, "Unexpected error while getting user notes. Error=" + ex.Message);
            }
        }

        [HttpGet]
        [Route("{nodeId}")]
        public async Task<IActionResult> GetUserNode(string nodeId = null)
        {
            try
            {
                var userNodeRequest = new GetUserNodeRequest();
                userNodeRequest.NodeId = nodeId;

                var result = await Mediator.Send(userNodeRequest);

                return ToActionResult(result);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "There was an error while getting user notes");
                return StatusCode((int)HttpStatusCode.InternalServerError, "Unexpected error while getting user notes. Error=" + ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateUserNode(CreateNodeRequest request)
        {
            try
            {
                var result = await Mediator.Send(request);

                return ToActionResult(result);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "There was an error while creating user note");
                return StatusCode((int)HttpStatusCode.InternalServerError, "Unexpected error while creating user note. Error=" + ex.Message);
            }
        }


        [HttpPut]
        public async Task<IActionResult> UpdateUserNode(UpdateUserNodeDataRequest request)
        {
            try
            {
                var result = await Mediator.Send(request);

                return ToActionResult(result);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "There was an error while creating user note");
                return StatusCode((int)HttpStatusCode.InternalServerError, "Unexpected error while creating user note. Error=" + ex.Message);
            }
        }

        [HttpDelete]
        [Route("{nodeId}")]
        public async Task<IActionResult> DeleteUserNode(string nodeId)
        {
            try
            {
                var request = new DeleteNodeRequest { NodeId = nodeId };
                var result = await Mediator.Send(request);

                return ToActionResult(result);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "There was an error while creating user note");
                return StatusCode((int)HttpStatusCode.InternalServerError, "Unexpected error while creating user note. Error=" + ex.Message);
            }
        }
    }
}
