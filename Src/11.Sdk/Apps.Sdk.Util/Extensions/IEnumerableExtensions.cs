using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Apps.Sdk.Extensions
{
    public static class EnumerableExtensions
    {
        public static IDictionary<T, TResult> AddRange<T, TResult>(this IDictionary<T, TResult> source, IDictionary<T, TResult> items)
        {
            foreach (var key in items.Keys)
                source[key] = items[key];

            return source;
        }

        public static void ForEach<T>(this IEnumerable<T> source, Action<T> action)
        {
            foreach (var item in source)
                action(item);
        }

        public static string JoinStr(this IEnumerable<string> source, string separator)
        {
            if (source?.Any() != true)
                return "";
            return string.Join(separator, source);
        }

        public static bool ContainsIgnoreCase(this IEnumerable<string> source, string text)
        {
            if (source == null) return false;
            return source.Contains(text, StringComparer.InvariantCultureIgnoreCase);
        }
    }
}
