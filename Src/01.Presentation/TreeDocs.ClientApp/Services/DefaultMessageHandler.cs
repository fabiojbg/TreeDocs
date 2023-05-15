using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.WebAssembly.Http;

namespace TreeDocs.ClientApp.Services
{
    public class DefaultMessageHandler : DelegatingHandler
    {
        public BrowserRequestCache? DefaultBrowserRequestCache { get; set; }
        public BrowserRequestCredentials? DefaultBrowserRequestCredentials { get; set; }
        public BrowserRequestMode? DefaultBrowserRequestMode { get; set; }

        public DefaultMessageHandler(HttpMessageHandler innerHandler)
        {
            InnerHandler = innerHandler;
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            // Get the existing options to not override them if set explicitly

            if( DefaultBrowserRequestCache.HasValue)
                request.SetBrowserRequestCache(DefaultBrowserRequestCache.Value);

            if (DefaultBrowserRequestCredentials.HasValue)
                request.SetBrowserRequestCredentials(DefaultBrowserRequestCredentials.Value);

            if (DefaultBrowserRequestMode.HasValue)
                request.SetBrowserRequestMode(DefaultBrowserRequestMode.Value);


            //IDictionary<string, object> existingProperties = null;
            //if (request.Properties.TryGetValue("WebAssemblyFetchOptions", out object fetchOptions))
            //{
            //    existingProperties = (IDictionary<string, object>)fetchOptions;
            //}

            //if (existingProperties?.ContainsKey("cache") != true)
            //{
            //    request.SetBrowserRequestCache(DefaultBrowserRequestCache);
            //}

            //if (existingProperties?.ContainsKey("credentials") != true)
            //{
            //    request.SetBrowserRequestCredentials(DefaultBrowserRequestCredentials);
            //}

            //if (existingProperties?.ContainsKey("mode") != true)
            //{
            //    request.SetBrowserRequestMode(DefaultBrowserRequestMode);
            //}

            return base.SendAsync(request, cancellationToken);
        }

    }
}
