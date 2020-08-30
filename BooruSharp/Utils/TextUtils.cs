using System;
using System.Collections.Generic;
using System.Linq;

namespace BooruSharp.Utils
{
    internal static class TextUtils
    {
        public static string JoinAndEscape(
            IEnumerable<string> stringsToJoin, string separator = " ", bool forceLowerCase = true)
        {
            if (separator == null)
                throw new ArgumentNullException(nameof(separator));

            if (stringsToJoin != null && stringsToJoin.Any())
            {
                var joined = string.Join(separator, stringsToJoin);
                var escaped = Uri.EscapeDataString(
                    forceLowerCase ? joined.ToLowerInvariant() : joined);

                return escaped;
            }

            return "";
        }
    }
}
