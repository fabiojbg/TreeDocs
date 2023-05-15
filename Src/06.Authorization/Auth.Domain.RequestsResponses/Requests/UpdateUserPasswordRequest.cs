using System;
using System.Collections.Generic;
using System.Text;
using Domain.Shared;
using Domain.Shared.Validations;
using MediatR;

namespace Auth.Domain.RequestsResponses
{
    public class UpdateUserPasswordRequest : ValidatableRequest, IRequest<RequestResult<UpdateUserPasswordResponse>>
    {
        public string UserId { get; set; }
        public string UserEmail { get; set; }
        public string OldPassword { get; set; }  
        public string NewPassword { get; set; }

        public override bool Validate()
        {
            AddNotifications(new Validator()
                                .IsTrue(!String.IsNullOrEmpty(UserId) || !String.IsNullOrEmpty(UserEmail), "UserId/UserEmail", Resource.ErrUserIdOrUserEmailRequired)
                                .IsNotNullOrWhiteSpace(NewPassword, "NewPassword", Resource.ErrParameterRequired)
                                .HasMinLen(NewPassword, Constants.MIN_PASSWORD_LEN, "NewPassword", Resource.ErrMinPasswordLen)
                                .HasMaxLen(NewPassword, Constants.MAX_PASSWORD_LEN, "NewPassword", Resource.ErrMinPasswordLen)
                                .AreNotEquals(OldPassword, NewPassword, "NewPassword", Resource.ErrOldAndNewPasswordCannotBeEqual)
                                );
            return Valid;
        }

    }
}
