using Apps.Sdk.DependencyInjection;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TreeNotes.Domain.Services;

namespace TreeNotes.Domain
{
    public static class DependencyInjection
    {
        public static void RegisterDependencies(ISdkContainerBuilder builder, IConfigurationRoot config)
        {
            builder.RegisterTransient<IUserServices, UserServices>();
        }

    }
}
