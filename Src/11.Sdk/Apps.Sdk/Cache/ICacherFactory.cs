using Apps.Sdk.SdkServices;
using System;

namespace Apps.Sdk
{
    public enum CacheStrategy
    {
        Default,
        ProcessMemory,  // use this one with caution cause it is not a distributed cache
        Redis,
        ReliableDictionary
    }

    public interface ICacherFactory
    {
        ICacher GetCacherFor(string tenant);

        ICacher GetCacherFor(CacheStrategy cacheStrategy, string tenant);

        ICacher GetCacherFor(CacheStrategy cacheStrategy, string tenant, string serviceName);

        IMemoryFallbackCache GetCacherWithMemoryFallbackFor(CacheStrategy cacheStrategy, string tenant, string serviceName, TimeSpan memoryFallbackTime);

        IMemoryFallbackCache GetCacherWithMemoryFallbackFor(string tenant, TimeSpan memoryFallbackTime);
    }
}