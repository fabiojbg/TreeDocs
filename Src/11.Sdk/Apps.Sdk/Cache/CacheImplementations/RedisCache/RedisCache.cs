using Apps.Sdk.Helpers;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using StackExchange.Redis;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Apps.Sdk.SdkServices.Cache
{
    class RedisCache : IDistributedCacher
    {
        private static string NO_TENANT => "";
        private static string NO_OWNER => "";
        private ILogger _logger;

        private static Lazy<ConnectionMultiplexer> _lazyConnection;
        private static ConnectionMultiplexer Connection => _lazyConnection.Value;

        private string _tenant = null;
        private string _clientService = null;

        private ConnectionMultiplexer _redisConnection => _lazyConnection.Value;

        public RedisCache(IConfiguration appSettings, ILogger<RedisCache> logger)
        {
            _logger = logger;
            var redisConfigOptions = appSettings["redis.configuration"];
            _tenant = NO_TENANT;
            _clientService = NO_OWNER;

            if (_lazyConnection == null)
                _lazyConnection = new Lazy<ConnectionMultiplexer>(() => ConnectionMultiplexer.Connect(redisConfigOptions));
        }

        public void SetCacheParameters(string tenant, string clientService)
        {
            _tenant = tenant ?? NO_TENANT;
            _clientService = clientService ?? NO_OWNER;
        }

        IDatabase getTenantDatabase(string tenant)
        {
            if (tenant == NO_TENANT)
                return _redisConnection.GetDatabase(0);

            if (!_redisConnection.IsConnected)
                throw new Exception($"None of the Redis Servers are connected. Connection status: {_redisConnection.GetStatus()}");

            var tenantHash = HashingHelper.ToSHA1Hash(tenant).ToUpper();
            var tenantDbNumber = Convert.ToInt32(tenantHash[0].ToString(), 16);
;
            return _redisConnection.GetDatabase(tenantDbNumber);
        }

        private string getKeyName(string key)
        {
            return $"{_tenant}_{_clientService}_{key}";
        }

        public void Set<T>(string key, T @object, TimeSpan timeSpan)
        {
            var keyName = getKeyName(key);
            try
            {
                var redisDatabase = getTenantDatabase(_tenant);

                if (EqualityComparer<T>.Default.Equals(@object, default(T)))
                {
                    Delete(key);
                    return;
                }
                var serializedObject = JsonConvert.SerializeObject(@object, Formatting.None, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });

                redisDatabase.StringSet(keyName, serializedObject, timeSpan);
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, $"({nameof(RedisCache)}) Error setting key {keyName}");
            }
        }

        public Task SetAsync<T>(string key, T @object, TimeSpan timeSpan)
        { 
            //Botelho> aviso-> Como as funções Async do redis(stack exchange) possuem um bug de travamento, simplesmente converto as funções sincronas para assincronas
            var task = Task.Run(() =>
            {
                Set(key, @object, timeSpan);
            });
            return task;
        }

        public T Get<T>(string key)
        {
            var keyName = getKeyName(key);
            try
            {
                var redisDatabase = getTenantDatabase(_tenant);

                var value = redisDatabase.StringGet(keyName);
                if (!value.HasValue)
                    return default(T);

                T deserializedObject = JsonConvert.DeserializeObject<T>(value);

                return deserializedObject;
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, $"({nameof(RedisCache)}) Error getting key {keyName}");
            }

            return default(T);
        }

        public Task<T> GetAsync<T>(string key)
        {
            //Botelho> aviso-> Como as funções Async do redis(stack exchange) possuem um bug de travamento, simplesmente converto as funções sincronas para assincronas
            var task = Task.Run(() =>
            {
                return Get<T>(key);
            });
            return task;
        }

        public T GetOrDefaultValue<T>(string key, T defaultValue)
        {
            T value = Get<T>(key);
            if (EqualityComparer<T>.Default.Equals(value, default(T)))
            {
                return defaultValue;
            }

            return value;
        }

        public async Task<T> GetOrDefaultValueAsync<T>(string key, T defaultValue)
        {
            T value = await GetAsync<T>(key);
            if (EqualityComparer<T>.Default.Equals(value, default(T)))
            {
                return defaultValue;
            }

            return value;
        }

        public void Delete(string key)
        {
            var keyName = getKeyName(key);

            try
            {
                var redisDatabase = getTenantDatabase(_tenant);

                redisDatabase.KeyDelete(keyName);
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, $"({nameof(RedisCache)}) Error deleting key {keyName}");
            }
        }

        public Task DeleteAsync(string key)
        {
            //Botelho> aviso-> Como as funções Async do redis(stack exchange) possuem um bug de travamento, simplesmente converto as funções sincronas para assincronas
            var task = Task.Run(() =>
            {
                Delete(key);
            });
            return task;
        }

#pragma warning disable CS1998
        public async Task<IEnumerable<string>> GetKeysByPatternAsync(string pattern)
        {
            return GetKeysByPattern(pattern);
        }
#pragma warning restore

        public IEnumerable<string> GetKeysByPattern(string pattern)
        {
            var result = new List<string>();
            foreach (var serverEndpoint in _redisConnection.GetEndPoints())
            {
                var server = _redisConnection.GetServer(serverEndpoint);
                var keys = server.Keys(pattern: pattern);
                if (keys?.Any() == true)
                    result.AddRange(keys.Select(x => x.ToString()));
            }
            return result;
        }

    }
}
