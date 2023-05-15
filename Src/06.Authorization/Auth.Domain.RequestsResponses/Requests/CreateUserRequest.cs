using System;
using System.Collections.Generic;
using System.Text;
using System.Resources;
using MediatR;
using Domain.Shared;
using Domain.Shared.Validations;

namespace Auth.Domain.RequestsResponses
{
    public class CreateUserRequest : ValidatableRequest, IRequest<RequestResult<CreateUserResponse>>
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string[] Roles { get; set; }

        public override bool Validate()
        {
            ClearNotifications();

            AddNotifications(new Validator()
                             .IsTrue(new Email(Email).Valid, "Email", Resource.ErrInvalidUserEmail)
                             .IsNotNullOrWhiteSpace(Name, "Name", Resource.ErrUserNameRequired)
                             .IsNotNullOrWhiteSpace(Password, "Password", Resource.ErrUserPasswordRequired));

            return Valid;
        }

    }
}
