using Apps.Sdk.Extensions;
using Apps.Sdk.Helpers;
using Domain.Shared.Validations;
using MediatR;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Auth.Domain.Repository;
using Auth.Domain.Services;
using Domain.Shared;
using Auth.Domain.RequestsResponses;

namespace Auth.Domain.Handlers
{
    public class GetUserHandler : Notifiable, IRequestHandler<GetUserRequest, RequestResult<GetUserResponse>>
    {
        IAuthDatabase _authDb;
        ILoggedUserService _loggedUserService;

        public GetUserHandler(IAuthDatabase authDb, ILoggedUserService loggedUserService)
        {
            this._authDb = authDb;
            _loggedUserService = loggedUserService;
        }

        public async Task<RequestResult<GetUserResponse>> Handle(GetUserRequest request, CancellationToken cancellationToken)
        {
            if (!_loggedUserService.HasPrivilege(Privilege.GetAnyUserData) &&
                _loggedUserService.LoggedUser?.Email?.Address?.EqualsIgnoreCase(request.Email)!=true)
                return new RequestResult<GetUserResponse>(Resource.ErrNotAuthorizedOperation.Format(Privilege.GetAnyUserData.ToString()), RequestResultType.Unauthorized);

            if (request.Email.IsNullOrEmpty())
                return new RequestResult<GetUserResponse>(Resource.ErrInvalidUserEmail, RequestResultType.InvalidRequest);

            var user = await _authDb.Users.GetUserByEmail(request.Email.ToLowerInvariant());

            if( user == null)
                return new RequestResult<GetUserResponse>(Resource.ErrUserNotFound.Format(request.Email), RequestResultType.ObjectNotFound);

            var response = new GetUserResponse(user.Id, user.Name, user.Email.Address, user.Roles.ToArray());

            await _authDb.SaveChangesAsync();

            return new RequestResult<GetUserResponse>(response);
        }


    }
}
