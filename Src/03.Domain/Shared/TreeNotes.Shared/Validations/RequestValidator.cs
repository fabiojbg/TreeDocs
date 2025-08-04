using Domain.Shared.Validations;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Shared
{
    public abstract class RequestValidator<TRequest> : Notifiable, IValidatable
    {
        protected TRequest request;

        public RequestValidator(TRequest requestToValidate)
        {
            request = requestToValidate;
            Validate();
        }
        public abstract bool Validate();
    }
}
