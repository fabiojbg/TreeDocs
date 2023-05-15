using System;
using System.Collections.Generic;
using System.Text;
using Domain.Shared;
using Domain.Shared.Validations;
using MediatR;

namespace Auth.Domain.RequestsResponses
{
    public class UpdateUserDataRequest : ValidatableRequest, IRequest<RequestResult<UpdateUserDataResponse>>
    {
        public string UserId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public List<string> Roles { get; set; }

        public override bool Validate()
        {
            ClearNotifications();

            AddNotifications(new Validator()
                             .IsNotNullOrWhiteSpace(UserId, "UserId", Resource.ErrUserIdRequired)
                             .IsNotNullOrWhiteSpace(Name, "Name", Resource.ErrUserNameRequired)
                             .IsTrue(new Email(Email).Valid, "Email", Resource.ErrInvalidUserEmail)
                             );

            return Valid;
        }


    }
}
