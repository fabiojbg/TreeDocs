using System;
using Microsoft.AspNetCore.Http;
using System.Linq;
using Apps.Sdk.Extensions;

namespace Apps.Sdk.Extensions
{
    public static class HttpRequestExtensions
    {
        #region Authorization
        public const string AUTHORIZATION_HEADER = "Authorization";

        public static string GetAuthorization(this HttpRequest request)
        {
            if (request == null)
                return null;

            if (request.Headers.ContainsKey(AUTHORIZATION_HEADER))
            {
                var authorization = request.Headers[AUTHORIZATION_HEADER].First();
                var data = authorization.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
                return data.Last();
            }

            return null;
        }

        public static string GetHeader(this HttpRequest request, string headerName)
        {
            if (request == null)
                return null;

            if (request.Headers.ContainsKey(headerName))
            {
                var result = request.Headers[headerName].First();
                return result;
            }

            return null;
        }

        #endregion Authorization

        public static string GetQueryStringParameter(this HttpRequest request, string ParameterName, string defaultValue = null)
        {
            var key = request.Query.Keys.FirstOrDefault(x => x.EqualsIgnoreCase(ParameterName));
            if (key != null)
                return request.Query[key];
            else
                return defaultValue;

        }

    }
}
