using Apps.Sdk.Cache;
using Apps.Sdk.DependencyInjection;
using Apps.Sdk.SdkServices.Cache;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;

namespace Apps.Sdk.Web
{
    public static class DependencyInjection
    {
        public static void RegisterServices(ISdkContainerBuilder container, IConfigurationRoot configuration)
        {
            container.RegisterInstance<IConfigurationRoot>(configuration);
            container.RegisterTransient<IHttpAgent, HttpAgent>();

            container.RegisterSingleton<IDistributedCacher, RedisCache>();
            container.RegisterSingleton<ICacher, ProcessMemoryCacher>();
            container.RegisterSingleton<ICacherFactory, CacherFactory>();
        }
    }
}
