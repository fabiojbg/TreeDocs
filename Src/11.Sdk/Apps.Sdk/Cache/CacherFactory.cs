using Apps.Sdk.Cache;
using Apps.Sdk.DependencyInjection;
using Apps.Sdk.SdkServices;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apps.Sdk
{
    public class CacherFactory : ICacherFactory
    {
        ISdkContainer _dependencyResolver;
        static object objlock = new object();
        static string _NO_SERVICE_NAME_ = "_";

        public CacherFactory(ISdkContainer dependencyResolver)
        {
            _dependencyResolver = dependencyResolver;
        }

        public ICacher GetCacherFor(string tenant)
        {
            return GetCacherFor(CacheStrategy.Default, tenant, _NO_SERVICE_NAME_);
        }

        public ICacher GetCacherFor(CacheStrategy cacheStrategy, string tenant)
        {
            return GetCacherFor(cacheStrategy, tenant, _NO_SERVICE_NAME_);
        }

        public ICacher GetCacherFor(CacheStrategy cacheStrategy, string tenant, string serviceName)
        {
            ICacher newCacher;
            lock (objlock)
            {
                string cacheStrategyName = "";

                if (cacheStrategy == CacheStrategy.Default)
                    cacheStrategy = DefaultCacheStrategy.Value;

                switch (cacheStrategy)
                {
                    case CacheStrategy.Redis: cacheStrategyName = CacheStrategyNames.REDIS; break;
                    case CacheStrategy.ProcessMemory: cacheStrategyName = CacheStrategyNames.MEMORY; break;
                }

                if( cacheStrategy == CacheStrategy.ProcessMemory)
                    newCacher = _dependencyResolver.Resolve<ICacher>(cacheStrategyName);
                else
                    newCacher = _dependencyResolver.Resolve<IDistributedCacher>(cacheStrategyName);

                newCacher?.SetCacheParameters(tenant, serviceName);

                return newCacher;
            }
        }

        public IMemoryFallbackCache GetCacherWithMemoryFallbackFor(CacheStrategy cacheStrategy, string tenant, string serviceName, TimeSpan memoryFallbackTime)
        {
            var memoryFallbackCacher = _dependencyResolver.Resolve<IMemoryFallbackCache>();

            memoryFallbackCacher.SetFallbackParameters(cacheStrategy, memoryFallbackTime);
            memoryFallbackCacher.SetCacheParameters(tenant, serviceName);

            return memoryFallbackCacher;
        }

        public IMemoryFallbackCache GetCacherWithMemoryFallbackFor(string tenant, TimeSpan memoryFallbackTime)
        {
            return GetCacherWithMemoryFallbackFor( CacheStrategy.Default, tenant, _NO_SERVICE_NAME_, memoryFallbackTime );
        }

    }
}
