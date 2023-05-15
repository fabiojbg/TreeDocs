using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apps.Sdk.Extensions
{
    public class Enum<TEnum> where TEnum : struct, IConvertible
    {
        public static bool Exists(string value, bool ignoreCase = true )
        {
            var result = Enum.TryParse<TEnum>(value, ignoreCase, out _);
            return result;
        }
    }
}
