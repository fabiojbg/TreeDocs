using Microsoft.AspNetCore.Http;
using Apps.Sdk;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Apps.Sdk.Helpers
{
    public static class LanguageHelper
    {
        public static string GetLanguageFromRequest(HttpRequest request)
        {
            string languageHeader = "Accept-Language";

            if (!request.Headers.ContainsKey(languageHeader))
                return "pt-BR";

            var language =request.Headers[languageHeader].First();

            if (language.Contains(","))
            {
                var languages = language.Split(',');
                return languages[0];
            }
            return language;
        }

        public static string SetCurrentLanguage( string userLanguage )
        {
            var previousLanguage = Thread.CurrentThread.CurrentCulture;

            if (string.IsNullOrEmpty(userLanguage.Trim()))
                return previousLanguage.Name;

            if (userLanguage != null)
            {
                var language = CultureInfo.GetCultureInfo(userLanguage.Trim());
                Thread.CurrentThread.CurrentCulture = language;
                Thread.CurrentThread.CurrentUICulture = language;
                CultureInfo.DefaultThreadCurrentCulture = language;
                CultureInfo.DefaultThreadCurrentUICulture = language;
            }
            return previousLanguage.Name;
        }

        public static string GetTextForCurrentLang(Dictionary<string, string> localizedtext)
        {
            string result;

            if (localizedtext?.Any() != true)
                return String.Empty;

            if (localizedtext.Count == 1)
            {
                result = localizedtext.First().Value;
                return result;
            }

            var currentLanguage = Thread.CurrentThread.CurrentCulture.Name;
            var foundText = localizedtext.Where(x => x.Key == currentLanguage);

            if (!foundText.Any())
            {
                currentLanguage = Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName;
                foundText = localizedtext.Where(x => x.Key.ToLower().StartsWith(currentLanguage));
                if (!foundText.Any())
                {
                    foundText = localizedtext.Where(x => x.Key.ToLower() == "default");
                    if (!foundText.Any())
                    {
                        result = localizedtext.First().Value;
                        return result;
                    }
                }
            }
            result = foundText.First().Value;
            return result;
        }

        public static Dictionary<string, string> CreateFromParams(params string[] options)
        {
            var result = new Dictionary<string, string>();
            int i = 0;
            do
            {
                result.Add(options[i], options[i + 1]);
                i = i + 2;
            } while (i < options.Length);
            return result;
        }

    }
}
