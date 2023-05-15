using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apps.Sdk.SdkServices.CacheImplementations
{
    public class MemoryFallbackCache : IMemoryFallbackCache
    {
        ICacher _distributedCacher;
        ICacher _memoryCacher;
        ICacherFactory _cacherFactory;
        TimeSpan _memoryFallbackTime;
        CacheStrategy _cacheStrategy;

        public MemoryFallbackCache(ICacherFactory cacherFactory)
        {
            _cacherFactory = cacherFactory;
            _cacheStrategy = CacheStrategy.Default;
            _memoryFallbackTime = TimeSpan.FromMilliseconds(500); // fallback padrão de 0,5 segs
        }

        public void SetCacheParameters(string tenant, string clientService)
        {
            if (_distributedCacher == null)
                _distributedCacher = _cacherFactory.GetCacherFor(_cacheStrategy, tenant);

            _memoryCacher = _cacherFactory.GetCacherFor(CacheStrategy.ProcessMemory, tenant);

            _distributedCacher.SetCacheParameters(tenant, clientService);
            _memoryCacher.SetCacheParameters(tenant, clientService);
        }

        public void SetFallbackParameters(CacheStrategy cacheStrategy, TimeSpan memoryFallbackTime)
        {
            _cacheStrategy = cacheStrategy;
            _memoryFallbackTime = memoryFallbackTime;
        }

        public T Get<T>(string key)
        {
            T value;

            value = _memoryCacher.Get<T>(key);
            if (value == null)
            {
                value = _distributedCacher.Get<T>(key);
                if (value != null)
                    _memoryCacher.Set(key, value, _memoryFallbackTime);
            }

            return value;
        }

        public async Task<T> GetAsync<T>(string key)
        {
            T value;

            value = _memoryCacher.Get<T>(key);
            if (value == null)
            {
                value = await _distributedCacher.GetAsync<T>(key);
                if (value != null)
                    _memoryCacher.Set(key, value, _memoryFallbackTime);
            }

            return value;
        }

        public T GetOrDefaultValue<T>(string key, T defaultValue)
        {
            T value;

            value = _memoryCacher.GetOrDefaultValue<T>(key, defaultValue);
            if (value == null)
            {
                value = _distributedCacher.GetOrDefaultValue<T>(key, defaultValue);
                if (value != null)
                    _memoryCacher.Set(key, value, _memoryFallbackTime);
            }

            return value;
        }

        public async Task<T> GetOrDefaultValueAsync<T>(string key, T defaultValue)
        {
            T value;

            value = _memoryCacher.GetOrDefaultValue<T>(key, defaultValue);
            if (value == null)
            {
                value = await _distributedCacher.GetOrDefaultValueAsync<T>(key, defaultValue);
                if (value != null)
                    _memoryCacher.Set(key, value, _memoryFallbackTime);
            }

            return value;
        }

        public void Set<T>(string key, T @object, TimeSpan timeSpan)
        {
            TimeSpan fallbackTimeInMiliseconds = (timeSpan.TotalMilliseconds< _memoryFallbackTime.TotalMilliseconds) ? timeSpan : _memoryFallbackTime;

            _memoryCacher.Set(key, @object,fallbackTimeInMiliseconds);

            _distributedCacher.Set(key, @object, timeSpan);            
        }

        public async Task SetAsync<T>(string key, T @object, TimeSpan timeSpan)
        {
            TimeSpan fallbackTimeInMiliseconds = (timeSpan.TotalMilliseconds < _memoryFallbackTime.TotalMilliseconds) ? timeSpan : _memoryFallbackTime;

            _memoryCacher.Set(key, @object, fallbackTimeInMiliseconds);

            await _distributedCacher.SetAsync(key, @object, timeSpan);
        }

        public void Delete(string key)
        {
            _memoryCacher.Delete(key);
            _distributedCacher.Delete(key);
        }

        public async Task DeleteAsync(string key)
        {
            _memoryCacher.Delete(key);
            await _distributedCacher.DeleteAsync(key);
        }

        public IEnumerable<string> GetKeysByPattern(string pattern)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<string>> GetKeysByPatternAsync(string pattern)
        {
            throw new NotImplementedException();
        }
    }
}
