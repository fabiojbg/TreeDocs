using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Apps.Sdk.Helpers
{
    public static class LinqExtensions
    {
        //---------------------------------------------------------------------------------------------------------------------------------
        public static IEnumerable<T> DistinctBy<T, TKey>(this IEnumerable<T> items, Func<T, TKey> property)
        {
            return items.GroupBy(property).Select(x => x.First());
        }


        //---------------------------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Obtains the data as a list; if it is *already* a list, the original object is returned without
        /// any duplication; otherwise, ToList() is invoked.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static List<T> AsList<T>(this IEnumerable<T> source)
        {
            return (source == null || source is List<T>) ? (List<T>)source : source.ToList();
        }

        //---------------------------------------------------------------------------------------------------------------------------------
        public static int Replace<T>(this IList<T> source, T oldValue, T newValue)
        {
            if (source == null)
                throw new ArgumentNullException("source parameter cannot be null");

            var index = source.IndexOf(oldValue);
            if (index != -1)
                source[index] = newValue;
            return index;
        }

    }
}
