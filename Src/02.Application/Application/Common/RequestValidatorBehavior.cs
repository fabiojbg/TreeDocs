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
using Newtonsoft.Json.Serialization;
using Apps.Sdk.DependencyInjection;
using Microsoft.AspNetCore.Http;

namespace Application.Common
{
     public class RequestValidatorBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    {

        public RequestValidatorBehavior()
        {
        }

        public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
        {
            if ((request is ValidatableRequest) && typeof(TResponse).FullName.Contains("RequestResult"))
            {                       
                var validatableRequest = request as ValidatableRequest;
                
                var httpAcessor = SdkDI.Resolve<IHttpContextAccessor>();
                validatableRequest.UserIP = httpAcessor?.HttpContext?.Connection?.RemoteIpAddress?.ToString();

                if (!validatableRequest.Validate())
                {
                    TResponse result;

                    result = (TResponse)Activator.CreateInstance(typeof(TResponse));

                    result.TrySetProperty(nameof(RequestResult<string>._Message), Resource.ErrInvalidRequest);
                    result.TrySetProperty(nameof(RequestResult<string>._Notifications), validatableRequest.Notifications.ToList());
                    result.TrySetProperty(nameof(RequestResult<string>._Result), RequestResultType.InvalidRequest);

                    return result;
                }
            }

            return await next();
        }
    }
}
