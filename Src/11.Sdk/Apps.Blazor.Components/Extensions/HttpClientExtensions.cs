using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Apps.Blazor.Components.Extensions
{
    public static class HttpClientExtensions
    {
        public static async Task<TValue> GetJsonAsync<TValue>(this HttpClient client, string url)
        {
            var response = await client.GetStringAsync(url);

            if (response == null)
                return default(TValue);

            return JsonConvert.DeserializeObject<TValue>(response);
        }

        public static async Task<TValue> ReadJsonAsync<TValue>(this HttpContent responseContent)
        {
            var contents = await responseContent.ReadAsStringAsync();
            if (contents == null)
                return default(TValue);

            try
            {
                return JsonConvert.DeserializeObject<TValue>(contents);
            }
            catch
            {
                return default;
            }
        }

        public static void SetAuthorizationHeader(this HttpClient client, string value, string mode="Bearer")
        {
            if (value == null)
                return;

            var headers = client.DefaultRequestHeaders;

            if (headers.Contains("Authorization"))
                headers.Remove("Authorization");
            headers.Add("Authorization", $"{mode} {value}");
        }

        public static void SetHeader(this HttpClient client, string key, string value)
        {
            if (value == null)
                return;

            var headers = client.DefaultRequestHeaders;

            if (headers.Contains(key))
                headers.Remove(key);
            headers.Add(key, value);
        }
    }
}
