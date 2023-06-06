using Apps.Sdk.Extensions;
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
using Auth.Domain.Entities;
using Auth.Domain.RequestsResponses;
using Apps.Sdk.DependencyInjection;

namespace Auth.Domain.Handlers
{
    public class UpdateUserPasswordHandler : Notifiable, IRequestHandler<UpdateUserPasswordRequest, RequestResult<UpdateUserPasswordResponse>>
    {
        IAuthDatabase _authDb;
        ILoggedUserService _loggedUserService;

        public UpdateUserPasswordHandler(IAuthDatabase authDb, ILoggedUserService loggedUserService)
        {
            _authDb = authDb;
            _loggedUserService = loggedUserService;
        }

        public async Task<RequestResult<UpdateUserPasswordResponse>> Handle(UpdateUserPasswordRequest request, CancellationToken cancellationToken)
        {           
            User user;
            
            if( !request.UserId.IsNullOrEmpty())            
                user = await _authDb.Users.GetUserById(request.UserId);
            else
                user = await _authDb.Users.GetUserByEmail(request.UserEmail);

            if (user == null)
                return new RequestResult<UpdateUserPasswordResponse>(Resource.ErrUserNotFound.Format(request.UserId), RequestResultType.ObjectNotFound);

            if( user.Email.Address == "demouser@gmail.com")
                return new RequestResult<UpdateUserPasswordResponse>("This user cannot be modified");

            if (_loggedUserService.LoggedUser?.Id == user.Id)
            {
                user.ClearNotifications();
                if (!user.ValidatePassword(request.OldPassword))
                    AddNotificationsWithNewName("OldPassword", user);
            }
            else
                if( !_loggedUserService.HasPrivilege(Privilege.UpdateUserPassword))
                    return new RequestResult<UpdateUserPasswordResponse>(Resource.ErrNotAuthorizedOperation.Format(Privilege.UpdateUser.ToString()), RequestResultType.Unnauthorized);

            user.ClearNotifications();
            if( !user.SetPassword(request.NewPassword) )
                AddNotificationsWithNewName("NewPassword", user);

            if ( this.HasNotifications)
                return new RequestResult<UpdateUserPasswordResponse>(Notifications);

            await _authDb.Users.UpdateUserPassword(user.Id, user.HashedPassword);

            var response = new UpdateUserPasswordResponse();

            await _authDb.SaveChangesAsync();

            SdkDI.Resolve<IAuditTrail>().InsertEntry("User password changed", user.Name, user.Id, request.UserIP);

            return new RequestResult<UpdateUserPasswordResponse>(response);
        }
    }
}
