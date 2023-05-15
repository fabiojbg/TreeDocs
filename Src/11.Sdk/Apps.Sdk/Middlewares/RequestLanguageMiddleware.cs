using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Apps.Sdk.Middlewares
{
    public class RequestLanguageMiddleware
    {
        private readonly RequestDelegate _next;

        public RequestLanguageMiddleware(RequestDelegate next)
        {
            this._next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            var userLanguage = GetUserLanguage(context);

            if (userLanguage != null)
            {
                var language = CultureInfo.GetCultureInfo(userLanguage);
                Thread.CurrentThread.CurrentCulture = language;
                Thread.CurrentThread.CurrentUICulture = language;
                CultureInfo.DefaultThreadCurrentCulture = language;
                CultureInfo.DefaultThreadCurrentUICulture = language;
            }

            await this._next.Invoke(context);
        }

        public string GetUserLanguage(HttpContext context)
        {
            string languageHeader = "Accept-Language";

            if (!context.Request.Headers.ContainsKey(languageHeader))
                return "en-US";

            var language = context.Request.Headers[languageHeader].First();

            if (language.Contains(","))
            {
                var languages = language.Split(',');
                return languages[0];
            }

            return language;
        }

    }
}
