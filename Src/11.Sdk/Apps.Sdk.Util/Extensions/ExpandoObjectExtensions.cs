using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apps.Sdk.Extensions
{
    public static class ExpandoObjectExtensions
    {
        public static object GetFieldValue(this ExpandoObject obj, string keyName) // find a value based on key and ignoring case on key name
        {
            if (!(obj is IDictionary<string, object> dict)) 
                return null;

            if (dict.TryGetValue(keyName, out object result))
                return result;

            var key = dict.Keys.FirstOrDefault(x => x.EqualsIgnoreCase(keyName));
            if (key != null)
                return dict[key];

            return null;
        }
        public static object GetFieldValue(this Dictionary<string,object> dict, string keyName) // find a value based on key and ignoring case on key name
        {
            if (dict == null) return null;

            if (dict.TryGetValue(keyName, out object result))
                return result;

            var key = dict.Keys.FirstOrDefault(x => x.EqualsIgnoreCase(keyName));
            if (key != null)
                return dict[key];

            return null;
        }

    }
}
