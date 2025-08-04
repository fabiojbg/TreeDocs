using Apps.Sdk.Helpers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Net.Http.Headers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TreeNotes.Service.Authorization
{
    public class JwtCryptedBearerEvents : JwtBearerEvents
    {
        public override Task MessageReceived(MessageReceivedContext context)
        {
            string authorization = context.Request.Headers[HeaderNames.Authorization];
            if (authorization?.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase) == true)
            {
                context.Token = TokenService.DecodeToken(authorization.Substring("Bearer ".Length).Trim());
            }
            return Task.CompletedTask;
        }
    }
}
