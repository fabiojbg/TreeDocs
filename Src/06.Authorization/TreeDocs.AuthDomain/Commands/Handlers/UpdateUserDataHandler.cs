using Apps.Sdk.Extensions;
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
using Auth.Domain.Entities;
using Auth.Domain.RequestsResponses;
using Apps.Sdk.DependencyInjection;

namespace Auth.Domain.Handlers
{
    public class UpdateUserDataHandler : Notifiable, IRequestHandler<UpdateUserDataRequest, RequestResult<UpdateUserDataResponse>>
    {
        IAuthDatabase _authDb;
        ILoggedUserService _loggedUserService;

        public UpdateUserDataHandler(IAuthDatabase authDb, ILoggedUserService loggedUserService)
        {
            _authDb = authDb;
            _loggedUserService = loggedUserService;
        }

        public async Task<RequestResult<UpdateUserDataResponse>> Handle(UpdateUserDataRequest request, CancellationToken cancellationToken)
        {
            if ( request.Email.IsNullOrEmpty())
                return new RequestResult<UpdateUserDataResponse>(Resource.ErrInvalidUserEmail, RequestResultType.InvalidRequest);

            if (!_loggedUserService.HasPrivilege(Privilege.UpdateUser) &&
                _loggedUserService.LoggedUser?.Id != request.UserId)
                return new RequestResult<UpdateUserDataResponse>(Resource.ErrNotAuthorizedOperation.Format(Privilege.UpdateUser.ToString()), RequestResultType.Unnauthorized);

            var existingUser = await _authDb.Users.GetUserById(request.UserId);
            if (existingUser == null)
                return new RequestResult<UpdateUserDataResponse>(Resource.ErrUserNotFound.Format(request.UserId), RequestResultType.ObjectNotFound);

            if (existingUser.Email.Address == "demouser@gmail.com")
                return new RequestResult<UpdateUserDataResponse>("This user cannot be modified");

            if ( !request.Email.EqualsIgnoreCase(existingUser.Email.Address) )
            {
                var userByEmail = await _authDb.Users.GetUserByEmail(request.Email);
                if (userByEmail != null && !userByEmail.Id.EqualsIgnoreCase(request.UserId))
                    return new RequestResult<UpdateUserDataResponse>(Resource.ErrUserAlreadyExists);
            }

            // only admin users can change roles
            var newRoles = (_loggedUserService.HasRole(Constants.ADMIN_USER_ROLE)) ? (request.Roles ?? existingUser.Roles) : existingUser.Roles;

            //TODO: Botelho>do not let Admin remove last admin from Database

            var user = new User(existingUser.Id, request.Name, request.Email, null, existingUser.HashedPassword, newRoles.ToArray());
            if (user.Invalid)
                AddNotifications(user);

            if (Invalid)
                return new RequestResult<UpdateUserDataResponse>(Notifications, Resource.ErrUpdatingUser);

            await _authDb.Users.UpdateUserData(user);

            var userSaved = await _authDb.Users.GetUserById(user.Id);

            var response = new UpdateUserDataResponse(userSaved.Id, userSaved.Name, userSaved.Email.Address, userSaved.Roles.ToArray());

            await _authDb.SaveChangesAsync();

            SdkDI.Resolve<IAuditTrail>().InsertEntry("User data changed", userSaved.Name, userSaved.Id, request.UserIP, 
                $"Name: {userSaved.Name}\r\nEmail: {userSaved.Email}");

            return new RequestResult<UpdateUserDataResponse>(response);
        }
    }
}
