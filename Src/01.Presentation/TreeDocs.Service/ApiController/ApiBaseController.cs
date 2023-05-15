using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Shared;
using TreeDocs.Service.Authorization;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Apps.Sdk.DependencyInjection;
using TreeDocs.Service.Contracts.Authentication;
using Auth.Domain.Entities;
using Auth.Domain.Services;
using System.Net;

namespace TreeDocs.Service.ApiController
{
    [ApiController]
    public class ApiBaseController : ControllerBase
    {
        AuthenticatedUser _authenticatedUser;
        IConfiguration _configuration;
        IMediator _mediator;
        User _loggedUser;
        protected ILogger _logger;

        protected virtual ILogger Logger => _logger ??= SdkDI.Resolve<ILogger<ApiBaseController>>();
        protected AuthenticatedUser AuthenticatedUser => _authenticatedUser ??= TokenService.GetAuthenticatedUser(User);
        protected IMediator Mediator => _mediator ??= SdkDI.Resolve<IMediator>();
        protected IConfiguration Configuration => _configuration ??= SdkDI.Resolve<IConfiguration>();
        protected User LoggedUser => _loggedUser ??= SdkDI.Resolve<ILoggedUserService>().LoggedUser;

        protected bool UserIsValid => LoggedUser != null;

        //Método alternativo de resolução de dependência usando o DI do .Net Core(funciona para os serviços do Sdk também)
        //ILogger Logger => _logger ??= HttpContext.RequestServices.GetService<ILogger>();

        public ApiBaseController()
        {
            
        }

        protected IActionResult ToActionResult<T>(RequestResult<T> result)
        {
            switch( result._Result)
            {
                case RequestResultType.Success:
                    return Ok(result._Data);
                case RequestResultType.Unnauthorized:
                    {
                        Logger.LogInformation("{0} : Unnauthorized. {1}", AuthenticatedUser?.Username??"-", result._Message);
                        return Unauthorized(result);
                    }
                case RequestResultType.ObjectNotFound:
                    return NotFound(result);
                case RequestResultType.OperationError:
                    var actionResult = new ObjectResult(result)
                    {
                        StatusCode = (int)System.Net.HttpStatusCode.InternalServerError
                    };
                    return actionResult;
                default:
                    return BadRequest(result);
            }
        }
    }
}
