using Apps.Sdk.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Apps.Sdk.Helpers
{
    public static class HashingHelper
    {
        public static string ToSHA1Hash(string s)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(s);

            var sha1 = SHA1.Create();
            byte[] hashBytes = sha1.ComputeHash(bytes);

            return hashBytes.ToHexString();
        }

        public static string ToSHA1Hash(byte[] s)
        {
            var sha1 = SHA1.Create();
            byte[] hashBytes = sha1.ComputeHash(s);

            return hashBytes.ToHexString();
        }

        public static string ToMD5Hash(string s)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(s);

            var sha1 = MD5.Create();
            byte[] hashBytes = sha1.ComputeHash(bytes);

            return hashBytes.ToHexString();
        }

        public static string ToMD5Hash(byte[] s)
        {
            var sha1 = MD5.Create();
            byte[] hashBytes = sha1.ComputeHash(s);

            return hashBytes.ToHexString();
        }
    }
}
