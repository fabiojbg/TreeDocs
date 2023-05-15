using Apps.Sdk.Extensions;
using Apps.Sdk.Helpers;
using Apps.Sdk.Exceptions;
using Newtonsoft.Json;
using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Apps.Sdk
{
    public class HttpAgent : IHttpAgent
    {
        HttpClient _client;

        public HttpAgent()

        {
            _client = new HttpClient();
            _client.DefaultRequestHeaders.AcceptLanguage.TryParseAdd(Thread.CurrentThread.CurrentCulture.Name);
        }

        public void Dispose()
        {
            _client.Dispose();
        }

        public void SetTimeout(TimeSpan timeout)
        {
            if (timeout.TotalSeconds > 300)
                throw new Exception("Timeout cannot exceed 300 seconds");

            _client.Timeout = timeout;
        }

        protected void AddAuthorization(string token)
        {
            if (!token.IsNullOrEmpty())
                _client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        }

       
        public void AddLanguage(string language)
        {
            try
            {
                _client.DefaultRequestHeaders.Remove("Accept-Language");
            }
            catch { }
            _client.DefaultRequestHeaders.TryAddWithoutValidation("Accept-Language", language);
        }

        public void AddAuthorizationHeaders(string authorization, string language = null)
        {
            if( authorization != null)
                AddAuthorization(authorization);
            if (language != null)
                AddLanguage(language);
        }

        public void AddHeader(string key, string value)
        {
            try
            {
                _client.DefaultRequestHeaders.Remove(key);
            }
            catch { }
            _client.DefaultRequestHeaders.TryAddWithoutValidation(key, value);
        }

        public void RemoveHeader(string key)
        {
            try
            {
                _client.DefaultRequestHeaders.Remove(key);
            }
            catch { }
        }

        public T Get<T>(string url)
        {
            return AsyncHelper.RunSync(() => GetAsync<T>(url));
        }

        public async Task<T> GetAsync<T>(string url)
        {
            var result = await GetAsStringAsync(url);
            return JsonConvert.DeserializeObject<T>(result);
        }

        public Task<string> GetAsync(string url)
        {
            return  GetAsStringAsync(url);
        }

        public async Task<int> GetStatusCodeAsync(string url)
        {
            try
            {
                var result = await _client.GetAsync(url);
                return (int)result.StatusCode;
            }
            catch (HttpException httpEx)
            {
                return (int)httpEx.StatusCode;
            }
            catch (Exception)
            {
                return (int)HttpStatusCode.InternalServerError;
            }
        }

        public string GetAsString(string url)
        {
            return AsyncHelper.RunSync(() => GetAsStringAsync(url));
        }

        public async Task<string> GetAsStringAsync(string url)
        {
            var response = await _client.GetAsync(url);

            if (!response.IsSuccessStatusCode)
                throw new HttpException(response);

            return await response.Content.ReadAsStringAsync();
        }

        public async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request)
        {
            return await _client.SendAsync(request);
        }

        public async Task<HttpResponseMessage> DeleteAsync(string url)
        {          
            var response = await _client.DeleteAsync(url);

            if (!response.IsSuccessStatusCode)
                throw new HttpException(response);

            return response;
        }

        public async Task<HttpResponseMessage> PostAsync(string url, object @object)
        {
            var serializedObject = JsonConvert.SerializeObject(@object, Formatting.None,  
                                                               new JsonSerializerSettings {  NullValueHandling = NullValueHandling.Ignore });

            var content = new StringContent(serializedObject, Encoding.UTF8, "application/json");

            var response = await _client.PostAsync(url, content);

            if (!response.IsSuccessStatusCode)
                throw new HttpException(response);

            return response;
        }

        public async Task<T> PostAsync<T>(string url, object @object)
        {
            var response = await PostAsync(url, @object);

            var serializedResponse = await response.Content.ReadAsStringAsync();

            T returnedObject  = JsonConvert.DeserializeObject<T>(serializedResponse);

            return returnedObject;
        }

        public async Task<HttpResponseMessage> PutAsync(string url, object @object)
        {
            var serializedObject = JsonConvert.SerializeObject(@object, Formatting.None,
                                                               new JsonSerializerSettings { NullValueHandling = NullValueHandling.Include });  // para o put, devo incluir os campos null

            var content = new StringContent(serializedObject, Encoding.UTF8, "application/json");

            var response = await _client.PutAsync(url, content);

            if (!response.IsSuccessStatusCode)
                throw new HttpException(response);

            return response;
        }

        public async Task<T> PutAsync<T>(string url, object @object)
        {
            var response = await PutAsync(url, @object);

            var serializedResponse = await response.Content.ReadAsStringAsync();

            T returnedObject = JsonConvert.DeserializeObject<T>(serializedResponse);

            return returnedObject;
        }


        public async Task<HttpResponseMessage> PatchAsync(string url, object @object)
        {
            var serializedObject = JsonConvert.SerializeObject(@object, Formatting.None,
                                                               new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }); // para o patch, não devemos incluir os campos não preenchidos (null)

            var content = new StringContent(serializedObject, Encoding.UTF8, "application/json");

            var patch = new HttpMethod("PATCH");
            var message = new HttpRequestMessage(patch, url)
            {
                Content = content
            };

            var response = await _client.SendAsync(message);

            if (!response.IsSuccessStatusCode)
                throw new HttpException(response);

            return response;
        }

        public async Task<T> PatchAsync<T>(string url, object @object)
        {
            var response = await PatchAsync(url, @object);

            var serializedResponse = await response.Content.ReadAsStringAsync();

            T returnedObject = JsonConvert.DeserializeObject<T>(serializedResponse);

            return returnedObject;
        }

    }
}
