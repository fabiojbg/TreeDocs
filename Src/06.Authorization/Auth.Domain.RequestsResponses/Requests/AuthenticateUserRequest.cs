using Domain.Shared;
using Domain.Shared.Validations;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Auth.Domain.RequestsResponses
{
    public class AuthenticateUserRequest : ValidatableRequest, IRequest<RequestResult<AuthenticateUserResponse>>
    {
        public string UserEmail { get; set; }
        public string Password { get; set; }

        public override bool Validate()
        {
            ClearNotifications();

            AddNotifications(new Validator()
                             .IsTrue(new Email(UserEmail).Valid, "UserEmail", Resource.ErrInvalidUserEmail)
                             .HasMinLen(Password, Constants.MIN_PASSWORD_LEN, "Password", Resource.ErrInvalidPassword)
                             .HasMaxLen(Password, Constants.MAX_PASSWORD_LEN, "Password", Resource.ErrInvalidPassword)
                             );

            return Valid;
        }

    }
}
