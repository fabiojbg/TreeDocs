using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Apps.Sdk
{
    public interface IHttpAgent : IDisposable
    {
        T Get<T>(string url);
        Task<T> GetAsync<T>(string url);
        Task<string> GetAsync(string url);
        Task<int> GetStatusCodeAsync(string url);
        Task<string> GetAsStringAsync(string url);
        Task<HttpResponseMessage> PatchAsync(string url, object value);
        Task<HttpResponseMessage> PostAsync(string url, object value);
        Task<HttpResponseMessage> PutAsync(string url, object value);
        Task<T> PatchAsync<T>(string url, object value);
        Task<T> PostAsync<T>(string url, object value);
        Task<T> PutAsync<T>(string url, object value);
        Task<HttpResponseMessage> SendAsync(HttpRequestMessage request);
        Task<HttpResponseMessage> DeleteAsync(string url);
        void AddHeader(string key, string value);
        void RemoveHeader(string key);
        void AddAuthorizationHeaders(string authorization, string language=null);
        void SetTimeout(TimeSpan timeout);
    }
}
