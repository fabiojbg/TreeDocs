using Apps.Sdk.Extensions;
using Apps.Sdk.Helpers;
using Domain.Shared.Validations;
using MediatR;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Auth.Domain.Repository;
using Domain.Shared;
using Auth.Domain.RequestsResponses;
using System.Linq;
using Apps.Sdk.DependencyInjection;

namespace Auth.Domain.Handlers
{
    public class AuthenticationUserHandler : Notifiable, IRequestHandler<AuthenticateUserRequest, RequestResult<AuthenticateUserResponse>>
    {
        IAuthDatabase _authDb;
        ILogger<AuthenticationUserHandler> _logger;

        public AuthenticationUserHandler(IAuthDatabase authDatabase, ILogger<AuthenticationUserHandler> logger)
        {
            this._authDb = authDatabase;
            _logger = logger;
        }

        public async Task<RequestResult<AuthenticateUserResponse>> Handle(AuthenticateUserRequest request, CancellationToken cancellationToken)
        {
            var user = await _authDb.Users.GetUserByEmail(request.UserEmail.ToLowerInvariant());

            if( user == null)
                return new RequestResult<AuthenticateUserResponse>(Resource.ErrUserNotFound.Format(request.UserEmail), RequestResultType.Unnauthorized);

            if (!user.ValidatePassword(request.Password))
            {
                _logger.LogWarning($"Invalid attemp to login with user {request.UserEmail}");
                return new RequestResult<AuthenticateUserResponse>(Resource.ErrInvalidPassword, RequestResultType.Unnauthorized);
            }

            await _authDb.Users.UpdateUserLastLogin(user.Id, DateTime.Now);

            var response = new AuthenticateUserResponse(user.Id, user.Name, user.Email.Address, user.Roles);

            await _authDb.SaveChangesAsync();

            SdkDI.Resolve<IAuditTrail>()?.InsertEntry($"User logged", user.Name, user.Id, request.UserIP);

            return new RequestResult<AuthenticateUserResponse>(response);
        }
    }
}
