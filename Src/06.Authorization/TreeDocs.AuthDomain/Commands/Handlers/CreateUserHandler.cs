using Apps.Sdk.Extensions;
using Apps.Sdk.Helpers;
using Domain.Shared.Validations;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Auth.Domain.Repository;
using Auth.Domain.Services;
using Domain.Shared;
using Auth.Domain.RequestsResponses;
using Auth.Domain.Entities;

namespace Auth.Domain.Handlers
{
    public class CreateUserHandler : Notifiable, IRequestHandler<CreateUserRequest, RequestResult<CreateUserResponse>>
    {
        IAuthDatabase _authDb;
        ILoggedUserService _loggedUserService;

        public CreateUserHandler(IAuthDatabase authDb, ILoggedUserService loggedUserService)
        {
            this._authDb = authDb;
            _loggedUserService = loggedUserService;
        }

        public async Task<RequestResult<CreateUserResponse>> Handle(CreateUserRequest request, CancellationToken cancellationToken)
        {
            var newUser = new User(null, request.Name, request.Email, null, request.Password, request.Roles);
            AddNotifications(newUser);

            if (newUser.HasRole(Constants.ADMIN_USER_ROLE) && !_loggedUserService.HasRole(Constants.ADMIN_USER_ROLE))
                return new RequestResult<CreateUserResponse>(Resource.ErrOnlyAdminCanCreateAdmins, RequestResultType.Unnauthorized);

            if (Invalid)
                return new RequestResult<CreateUserResponse>(Notifications, Resource.ErrInvalidUserCreationRequest);

            var existingUser = await _authDb.Users.GetUserByEmail(newUser.Email.Address.ToLowerInvariant());
            if( existingUser != null)
                return new RequestResult<CreateUserResponse>(Resource.ErrUserAlreadyExists);

            var userId = await _authDb.Users.CreateUser(newUser);

            existingUser = await _authDb.Users.GetUserById(userId);

            // é preciso ter o Id do usuário para calcular o hash da senha corretamente.
            await _authDb.Users.UpdateUserPassword(userId, User.GetHashedPassword(userId, request.Password));

            var response = new CreateUserResponse(existingUser.Id, existingUser.Name, existingUser.Email.Address, existingUser.Roles.ToArray());
            
            await _authDb.SaveChangesAsync();
            
            return new RequestResult<CreateUserResponse>(response);
        }
    }
}
