using BooruSharp.Booru;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BooruSharp.Utils
{
    internal static class TextUtils
    {
        public static string JoinAndEscape(
            IEnumerable<string> stringsToJoin, string separator = " ", bool forceLowerCase = true)
        {
            if (separator is null)
                throw new ArgumentNullException(nameof(separator));

            if (!(stringsToJoin is null) && stringsToJoin.Any())
            {
                var joined = string.Join(separator, stringsToJoin);
                var escaped = Uri.EscapeDataString(
                    forceLowerCase ? joined.ToLowerInvariant() : joined);

                return escaped;
            }

            return "";
        }

        // According to MDN ( https://developer.mozilla.org/en-US/docs/Web/HTTP/Headers/User-Agent#Syntax ),
        // user agent string should look like this: "<product> / <product-version> <comment>".
        // This method will create user agent string similar to that one,
        // for example "BooruSharp/2.10.0" or "BooruSharp.MyPostfix/2.10.0".
        public static string GetUserAgent(string postfix = null)
        {
            var assembly = typeof(ABooru).Assembly.GetName();
            var stringBuilder = new StringBuilder(assembly.Name);

            if (!string.IsNullOrWhiteSpace(postfix))
            {
                stringBuilder
                    .Append('.')
                    .Append(postfix);
            }

            return stringBuilder
                .Append('/')
                // 3 here means we only want the first 3 version numbers (for example "1.0.0").
                .Append(assembly.Version.ToString(3))
                .ToString();
        }
    }
}
