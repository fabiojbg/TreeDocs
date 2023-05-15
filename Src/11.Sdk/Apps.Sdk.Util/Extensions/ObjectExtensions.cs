using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Text.Json;

namespace Apps.Sdk.Extensions
{
    public static class ObjectExtensions
    {
        public static string ToJson(this object obj) => JsonSerializer.Serialize(obj);

        public static bool IsEmpty(this Guid obj)
        {
            return obj == Guid.Empty;
        }

        public static bool IsNullOrEmpty(this Guid? obj)
        {
            return obj == null || obj == Guid.Empty;
        }

        public static bool IsEmpty(this DateTime obj)
        {
            return obj == DateTime.MinValue;
        }

        public static bool IsNullOrEmpty(this DateTime? obj)
        {
            return obj == null || !obj.HasValue || obj == DateTime.MinValue;
        }

        public static bool IsNull(this double? obj)
        {
            return obj == null || !obj.HasValue;
        }

        public static bool IsNull(this int? obj)
        {
            return obj == null || !obj.HasValue;
        }

        public static bool IsNull(this bool? obj)
        {
            return obj == null || !obj.HasValue;
        }

        public static bool IsNull(this object obj)
        {
            return (obj == null);
        }

        public static bool IsNull(this Array obj)
        {
            return (obj == null);
        }

        public static bool IsNull(this ICollection obj)
        {
            return (obj == null);
        }

        public static bool IsNullOrEmpty(this ICollection obj)
        {
            return obj == null || obj.Count == 0;
        }

        public static bool IsNull(this IEnumerable obj)
        {
            return (obj == null);
        }

        public static string Join(this IEnumerable<string> obj, string pattern)
        {
            return string.Join(pattern, obj);
        }

        public static bool IsNullOrEmpty(this IDictionary<string, string> obj)
        {
            return obj == null || obj.Count == 0;
        }

        public static T TryGetObjectValue<T>(this IDictionary<string, object> settings, string propertyName)
        {
            if (!settings.ContainsKey(propertyName))
                return default;

            var value = settings[propertyName];

            if (value is T t)
                return t;

            var typeConverter = TypeDescriptor.GetConverter(typeof(T));

            if (!typeConverter.CanConvertFrom(value.GetType()))
                return default;

            return (T)typeConverter.ConvertFrom(value);
        }

        public static bool TrySetProperty(this object obj, string propertyName, object value)
        {
            if (obj == null) return false;
            try
            {
                PropertyInfo prop = obj.GetType().GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance);
                if (null != prop && prop.CanWrite)
                    prop.SetValue(obj, value, null);
            }
            catch { return false; }
            
            return true;
        }


    }
}
