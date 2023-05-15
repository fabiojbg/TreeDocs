using Domain.Shared;
using Domain.Shared.Validations;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Auth.Domain.RequestsResponses
{
    public class GetUserRequest : ValidatableRequest, IRequest<RequestResult<GetUserResponse>>
    {
        public string Email { get; set; }

        public override bool Validate()
        {
            ClearNotifications();

            AddNotifications(new Validator()
                             .IsTrue(new Email(Email).Valid, "Email", Resource.ErrInvalidUserEmail));

            return Valid;
        }

    }
}
