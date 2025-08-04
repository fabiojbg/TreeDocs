using Apps.Sdk.DependencyInjection;
using Auth.Domain.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TreeNotes.Service.Filters
{
    public class AuthenticationRequired : ActionFilterAttribute
    {
        public AuthenticationRequired()
        {
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var hasAuthorizeAttribute = context.ActionDescriptor.EndpointMetadata.Any(x => x is AuthorizeAttribute);
            var hasAllowAnonymous = context.ActionDescriptor.EndpointMetadata.Any(x => x is AllowAnonymousAttribute);

            if (hasAuthorizeAttribute && !hasAllowAnonymous)
            {
                var loggedUserService = SdkDI.Resolve<ILoggedUserService>();
                if (loggedUserService.LoggedUser == null)
                    throw new UnauthorizedAccessException();
            }
        }

    }
}
