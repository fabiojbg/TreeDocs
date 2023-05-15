using Apps.Sdk.Exceptions;
using Domain.Shared.Validations;
using Domain.Shared;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Apps.Sdk.Extensions;

namespace Application.Common
{
     public class RequestValidateWithExceptionBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    {

        public RequestValidateWithExceptionBehavior()
        {
        }

        public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
        {
            if (request is IValidatable)
            {
                var validatableRequest = request as IValidatable;
                var notifiableRequest = request as Notifiable;

                if (!validatableRequest.Validate())
                {
                    var result = new RequestResult<object>()
                    {
                        _Message = Resource.ErrInvalidRequest,
                        _Notifications = notifiableRequest?.Notifications?.ToList(),
                        _Result = RequestResultType.InvalidRequest
                    };

                    throw new ValidationException(Resource.ErrInvalidRequest, result);
                }
            }

            return await next();
        }
    }
}
