using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace TreeDocs.Auth.IdentityExtensions
{
    public static class IdentityChanger
    {
        /// <summary>
        /// Sets app resouces for identity errors. Must be called after servives.AddIdentity
        /// </summary>
        /// <param name="services"></param>
        public static void SetAppIdentityErrorDescriber(this IServiceCollection services)
        {
            services.RemoveAll(typeof(Microsoft.AspNetCore.Identity.IdentityErrorDescriber));
            services.AddScoped<Microsoft.AspNetCore.Identity.IdentityErrorDescriber, IdentityErrorDescriber>();
        }
    }
}
