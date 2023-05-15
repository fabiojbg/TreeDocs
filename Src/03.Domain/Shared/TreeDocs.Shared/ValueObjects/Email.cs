using System;
using System.Collections.Generic;
using System.Text;
using Domain.Shared.Validations;

namespace Domain.Shared
{
    public class Email : ValueObject
    {
        public string Address { get; private set; }

        public Email(string address)
        {
            Address = address == null ? "" : address.ToLowerInvariant();

            AddNotifications(new Validator().IsEmail(Address, "Email", Resource.ErrInvalidEmail));
        }

    }
}
