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

        /// <summary>
        /// Retrieves the currently authenticated user's information.
        /// </summary>
        /// <returns>
        /// A <see cref="Contracts.Authentication.AuthenticatedUser"/> object containing the authenticated user's details.
        /// </returns>
        /// <response code="200">Returns the authenticated user's information.</response>
        /// <response code="401">If the user is not authenticated.</response>
        [HttpGet]
        [Route("Authenticated")]
        [Authorize]
        public Contracts.Authentication.AuthenticatedUser Authenticated() => base.AuthenticatedUser;

        /// <summary>
        /// Retrieves user details by email.
        /// </summary>
        /// <param name="userEmail">The email of the user to retrieve.</param>
        /// <returns>
        /// An <see cref="IActionResult"/> representing the result of the operation.
        /// On success, returns <see cref="GetUserResponse"/> containing user details.
        /// </returns>
        /// <response code="200">Returns the user details.</response>
        /// <response code="400">If the request is invalid (e.g., invalid email format).</response>
        /// <response code="401">If the user is not authorized to perform this operation.</response>
        /// <response code="404">If the user is not found.</response>
        /// <response code="500">If an unexpected error occurs.</response>
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

        /// <summary>
        /// Sample endpoint to demonstrate background job execution while the request is finished.
        /// </summary>
        /// <param name="userEmail">The email of the user for testing.</param>
        /// <returns>
        /// An <see cref="IActionResult"/> with a success message.
        /// </returns>
        /// <response code="200">Always returns success with a test message.</response>
        /// <response code="500">If an unexpected error occurs during background processing.</response>
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

        /// <summary>
        /// Creates a new user account. Anonymous access allowed.
        /// </summary>
        /// <param name="createUserRequest">The request object containing user creation details.</param>
        /// <returns>
        /// An <see cref="CreatedResult"/> with the created user's data on success.
        /// </returns>
        /// <remarks>
        /// Sample request:
        ///
        ///     POST /api/User
        ///     {
        ///        "name": "John Doe",
        ///        "email": "john.doe@example.com",
        ///        "password": "Password123",
        ///        "roles": ["User"]
        ///     }
        ///
        /// </remarks>
        /// <response code="201">Returns the newly created user's ID.</response>
        /// <response code="400">If the request is invalid (e.g., validation errors).</response>
        /// <response code="409">If a user with the provided email already exists.</response>
        /// <response code="500">If an unexpected error occurs.</response>
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

        /// <summary>
        /// Updates an existing user's data.
        /// </summary>
        /// <param name="updateUserRequest">The request object containing updated user details.</param>
        /// <returns>
        /// An <see cref="IActionResult"/> representing the result of the operation.
        /// On success, returns <see cref="UpdateUserDataResponse"/> containing updated user details.
        /// </returns>
        /// <remarks>
        /// Sample request:
        ///
        ///     PUT /api/User
        ///     {
        ///        "userId": "some-user-id",
        ///        "name": "Jane Doe",
        ///        "email": "jane.doe@example.com",
        ///        "roles": ["User", "Admin"]
        ///     }
        ///
        /// </remarks>
        /// <response code="200">Successfully updated user data.</response>
        /// <response code="400">If the request is invalid (e.g., validation errors).</response>
        /// <response code="401">If the user is not authorized to perform this operation.</response>
        /// <response code="404">If the user is not found.</response>
        /// <response code="409">If the new email is already in use by another user.</response>
        /// <response code="500">If an unexpected error occurs.</response>
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

        /// <summary>
        /// Changes a user's password.
        /// </summary>
        /// <param name="updateUserPasswordRequest">The request object containing password change details.</param>
        /// <returns>
        /// An <see cref="IActionResult"/> representing the result of the operation.
        /// On success, returns <see cref="UpdateUserPasswordResponse"/>.
        /// </returns>
        /// <remarks>
        /// Sample request:
        ///
        ///     PUT /api/User/ChangePassword
        ///     {
        ///        "userId": "some-user-id", (or "userEmail": "user@example.com")
        ///        "oldPassword": "OldPassword123",
        ///        "newPassword": "NewPassword456"
        ///     }
        ///
        /// </remarks>
        /// <response code="200">Successfully changed user password.</response>
        /// <response code="400">If the request is invalid (e.g., old password mismatch, new password validation errors).</response>
        /// <response code="401">If the user is not authorized to perform this operation.</response>
        /// <response code="404">If the user is not found.</response>
        /// <response code="500">If an unexpected error occurs.</response>
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

        /// <summary>
        /// Authenticates a user and issues a JWT token. Anonymous access allowed.
        /// </summary>
        /// <param name="authenticateUserRequest">The request object containing user authentication details.</param>
        /// <returns>
        /// On success, returns <see cref="Contracts.Authentication.AuthenticatedUser"/> with token.
        /// </returns>
        /// <remarks>
        /// Sample request:
        ///
        ///     POST /api/User/Login
        ///     {
        ///        "userEmail": "user@example.com",
        ///        "password": "Password123"
        ///     }
        ///
        /// </remarks>
        /// <response code="200">Returns the authenticated user details including the JWT token and its expiration.</response>
        /// <response code="400">If the request is invalid (e.g., validation errors).</response>
        /// <response code="401">If authentication fails (e.g., invalid credentials).</response>
        /// <response code="404">If the user is not found.</response>
        /// <response code="500">If an unexpected error occurs.</response>
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
