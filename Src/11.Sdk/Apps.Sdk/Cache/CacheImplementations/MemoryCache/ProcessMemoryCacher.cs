using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apps.Sdk.SdkServices.Cache
{
    public class ProcessMemoryCacher : ICacher // this is not a distribted cache. Is only a interface implementation to service that dont need distributed cache and stil is able to serve some cache dependency to internal Sdk Service (ex: ICachedConfiguration)
    {
        private static string NO_TENANT => "__NO_TENANT__";
        private static string NO_OWNER => "__NO_OWNER__";

        static IMemoryCache _memoryCacher;
        string _tenant;
        string _clientService;

        public ProcessMemoryCacher()
        {
            if( _memoryCacher == null)
                _memoryCacher = new MemoryCache(new MemoryCacheOptions());
        }

        public bool IsReady => true;

        private string getKeyName(string keyName)
        {
            return _tenant + "_" + _clientService + "_" + keyName;
        }
        public void SetCacheParameters(string tenant, string clientService)
        {
            _tenant = tenant ?? NO_TENANT;
            _clientService = clientService ?? NO_OWNER;
        }

        public void Delete(string key)
        {
            _memoryCacher.Remove(getKeyName(key));
        }

        public Task DeleteAsync(string key)
        {
            return Task.Run(() => Delete(key));
        }

        public T Get<T>(string key)
        {
            T savedValue;

            if( !_memoryCacher.TryGetValue<T>(getKeyName(key), out savedValue) )
                return default;

            return savedValue;
        }

        public Task<T> GetAsync<T>(string key)
        {
            return Task.Run(() => Get<T>(key));
        }

        public T GetOrDefaultValue<T>(string key, T defaultValue)
        {
            T value = Get<T>(key);
            if (EqualityComparer<T>.Default.Equals(value, default(T)))
                return defaultValue;

            return value;
        }

        public async Task<T> GetOrDefaultValueAsync<T>(string key, T defaultValue)
        {
            T value = await GetAsync<T>(key);
            if (EqualityComparer<T>.Default.Equals(value, default(T)))
                return defaultValue;

            return value;
        }

        public void Set<T>(string key, T @object, TimeSpan timeSpan)
        {
            if (EqualityComparer<T>.Default.Equals(@object, default(T)))
                Delete(key);

            _memoryCacher.Set<T>(getKeyName(key), @object, timeSpan);
        }

        public Task SetAsync<T>(string key, T @object, TimeSpan timeSpan)
        {
            return Task.Run(() => Set<T>(key, @object, timeSpan));
        }

        public Task<IEnumerable<string>> GetKeysByPatternAsync(string pattern)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<string> GetKeysByPattern(string pattern)
        {
            throw new NotImplementedException();
        }
    }
}
