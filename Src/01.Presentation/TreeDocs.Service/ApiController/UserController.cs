using Apps.Sdk.DependencyInjection;
using Apps.Sdk.Exceptions;
using Auth.Domain.RequestsResponses;
using Auth.Domain.Handlers;
using Domain.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Net;
using System.Threading.Tasks;
using TreeDocs.Service.Authorization;
using Apps.Sdk.Helpers;
using TreeDocs.Service.Filters;
using MediatR;
using System.Threading;

namespace TreeDocs.Service.ApiController
{
    [Route("api/User")]    
    [Authorize, AuthenticationRequired]
    public class UserController : ApiBaseController
    {
        protected override ILogger Logger => _logger ??= SdkDI.Resolve<ILogger<UserController>>();

        public UserController()
        {
        }

        [HttpGet]
        [Route("Authenticated")]
        [Authorize]
        public Contracts.Authentication.AuthenticatedUser Authenticated() => base.AuthenticatedUser;

        [HttpGet]
        [Route("{userEmail}")]
        public async Task<IActionResult> GetUser(string userEmail)
        {
            try
            {
                var result = await Mediator.Send(new GetUserRequest {Email=userEmail});

                return ToActionResult(result);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "There was an error while getting user information");
                return StatusCode((int)HttpStatusCode.InternalServerError, "Unexpected error while getting user information. Error=" + ex.Message);
            }
        }

        [HttpGet]
        [Route("test/{userEmail}")]
        public async Task<IActionResult> GetTestUser(string userEmail)
        {
            try
            {
                // Sample of how to make background jobs running while request is finished
                BackgroundWorker.RunWorkAsync(async (scope) =>
                {
                    await Task.Delay(2000);
                    var mediator = scope.Resolve<IMediator>();
                    var result = await Mediator.Send(new GetUserRequest { Email = userEmail });
                }
                );

                return ToActionResult(new RequestResult<String>() { _Data = "Teste"} );
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "There was an error while getting user information");
                return StatusCode((int)HttpStatusCode.InternalServerError, "Unexpected error while getting user information. Error=" + ex.Message);
            }
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> CreateUser([FromBody]CreateUserRequest createUserRequest)
        {
            try
            {
                var result = await Mediator.Send(createUserRequest);

                if (result._Result != RequestResultType.Success)
                    return ToActionResult(result);

                return Created("", result._Data);
            }
            catch(Exception ex)
            {
                Logger.LogError(ex, "There was an error while processing a user creation");
                return StatusCode( (int)HttpStatusCode.InternalServerError, "Unexpected error while creating user. Error=" + ex.Message);
            }
        }

        [HttpPut]
        public async Task<IActionResult> UpdateUser([FromBody] UpdateUserDataRequest updateUserRequest)
        {
            try
            {
                var result = await Mediator.Send(updateUserRequest);

                return ToActionResult(result);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "There was an error while processing a user update");
                return StatusCode((int)HttpStatusCode.InternalServerError, "Unexpected error while updating user. Error=" + ex.Message);
            }
        }

        [HttpPut]
        [Route("ChangePassword")]
        public async Task<IActionResult> ChangeUserPassword([FromBody] UpdateUserPasswordRequest updateUserPasswordRequest)
        {
            try
            {
                var result = await Mediator.Send(updateUserPasswordRequest);

                return ToActionResult(result);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "There was an error while processing user update");
                return StatusCode((int)HttpStatusCode.InternalServerError, "Unexpected error while updating user. Error=" + ex.Message);
            }
        }

        [HttpPost]
        [Route("Login")]
        [AllowAnonymous]
        public async Task<IActionResult> AuthenticateUser([FromBody] AuthenticateUserRequest authenticateUserRequest)
        {
            try
            {
                var result = await Mediator.Send(authenticateUserRequest);

                if( result._Result == RequestResultType.Success)
                {
                    var authenticatedUser = new Contracts.Authentication.AuthenticatedUser
                    {
                        Id = result._Data.Id,
                        Email = result._Data.Email,
                        Username = result._Data.Name,
                        Roles = result._Data.Roles.ToArray()
                    };
                    var secretKey = TokenService.GetSecretKey(Configuration);
                    var encryptKey = TokenService.GetEncryptionKey(Configuration);
                    var token = TokenService.GenerateToken(authenticatedUser, secretKey, encryptKey, TimeSpan.FromDays(7));
                    authenticatedUser.Token = token.Token;
                    authenticatedUser.TokenExpiration = token.Expiration;

                    return Ok(authenticatedUser);
                }

                return ToActionResult(result);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "There was an error while processing a user authentication");
                return StatusCode((int)HttpStatusCode.InternalServerError, "Unexpected error while authenticating user. Error=" + ex.Message);
            }
        }

    }
}
