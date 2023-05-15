using Domain.Shared.Validations;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Shared
{
    public abstract class ValidatableRequest : Notifiable, IValidatable
    {
        public abstract bool Validate();
    }
}
