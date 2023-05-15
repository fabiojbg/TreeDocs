using Apps.Sdk;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;
using Auth.Domain.Services;
using Apps.Sdk.DependencyInjection;

namespace Auth.Domain
{
    public static class DependencyInjection
    {
        public static void RegisterDependencies(ISdkContainerBuilder builder, IConfigurationRoot config)
        {
            builder.RegisterScoped<ILoggedUserService, LoggedUserService>();
        }

    }
}
