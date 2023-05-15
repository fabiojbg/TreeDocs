using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace Apps.Blazor.Components.Extensions
{
    public static class StringExtensions
    {
        public static bool EqualsCI(this string obj, string compare)
        {
            if (obj == null && compare == null)
                return true;
            if (obj != null && compare == null)
                return false;
            if (obj == null && compare != null)
                return false;

            return obj.Equals(compare, StringComparison.InvariantCultureIgnoreCase);
        }
    }
}
