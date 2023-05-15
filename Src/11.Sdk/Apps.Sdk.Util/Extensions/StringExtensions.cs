using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Apps.Sdk.Extensions
{
    public static class StringExtensions
    {
        public static bool IsNullOrEmpty(this string obj)
        {
            return string.IsNullOrWhiteSpace(obj);
        }

        public static string ToBase64(this string text)
        {
            var bytes = Encoding.UTF8.GetBytes(text);
            return Convert.ToBase64String(bytes);
        }

        public static string[] SplitRemoveEmpty(this string obj, string c)
        {
            return obj.Split(new string[] { c }, StringSplitOptions.RemoveEmptyEntries);
        }

        /// <summary>
        /// Performs a case-insensitive equality
        /// </summary>
        public static bool EqualsIgnoreCase(this string source, string dest)
        {
            return string.Equals(source, dest, StringComparison.InvariantCultureIgnoreCase);
        }

        /// <summary>
        /// Performs a case-insensitive equality
        /// </summary>
        public static bool EqualsCI(this string source, string dest)
        {
            return string.Equals(source, dest, StringComparison.InvariantCultureIgnoreCase);
        }

        public static string ReplaceIgnoringCase(this string input, string search, string replacement)
        {
            string result = Regex.Replace(
                input,
                Regex.Escape(search),
                replacement.Replace("$", "$$"),
                RegexOptions.IgnoreCase
            );
            return result;
        }


        public static string Truncate(this string value, int maxLength, bool? includeEllipses = false)
        {
            if (string.IsNullOrEmpty(value) || value.Length <= maxLength)
                return value;

            if (includeEllipses.Value)
                return value.Substring(0, maxLength - 3) + "...";
            else
                return value.Substring(0, maxLength);
        }

        public static string Format(this string value, params object[] parameters)
        {
            if (value == null)
                return "";

            return String.Format(value, parameters);
        }

        public static string TrimExternalParentesis(this string source)
        {
            source = source.Trim();
            if (source.Length < 2)
                return source;
            var parentesisBalance = source.Count(c => c == ')') - source.Count(c => c == '(');
            if (parentesisBalance != 0 || !source.StartsWith("(") || !source.EndsWith(")"))
                return source;

            var parenOpen = 1;
            for (var i = 1; i < source.Length; i++)
            {
                if (source[i] == ')')   parenOpen--;
                if (source[i] == '(')   parenOpen++;
                if (parenOpen == 0)
                    if (i == (source.Length - 1))
                        return source[1..^1].TrimExternalParentesis();
                    else
                        return source;
            }

            return source;
        }

    }
}
