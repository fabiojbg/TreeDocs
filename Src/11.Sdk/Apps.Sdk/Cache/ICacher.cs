using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Apps.Sdk
{

    public interface ICacher
    {
        void SetCacheParameters(string tenant, string clientService);

        void Set<T>(string key, T @object, TimeSpan timeSpan);
        Task SetAsync<T>(string key, T @object, TimeSpan timeSpan);

        T Get<T>(string key);
        Task<T> GetAsync<T>(string key);

        T GetOrDefaultValue<T>(string key, T defaultValue);
        Task<T> GetOrDefaultValueAsync<T>(string key, T defaultValue);

        void Delete(string key);
        Task DeleteAsync(string key);

        Task<IEnumerable<string>> GetKeysByPatternAsync(string pattern);

        IEnumerable<string> GetKeysByPattern(string pattern);
    }
}