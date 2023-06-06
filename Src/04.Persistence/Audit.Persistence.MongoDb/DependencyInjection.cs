using Apps.Sdk.DependencyInjection;
using Domain.Shared;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Audit.Persistence.MongoDb
{
    public static class DependencyInjection
    {
        public static void RegisterDependencies(ISdkContainerBuilder builder, IConfigurationRoot config)
        {
            builder.RegisterTransient<IAuditTrail, AuditTrail>();
        }

    }
}
