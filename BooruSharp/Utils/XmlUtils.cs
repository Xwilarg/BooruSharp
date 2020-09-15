using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace BooruSharp.Utils
{
    internal static class XmlUtils
    {
        private static readonly ReadOnlyDictionary<string, string> _xmlEntities =
           new ReadOnlyDictionary<string, string>(new Dictionary<string, string>
           {
               // https://www.key-shortcut.com/en/all-html-entities/all-entities/
               ["&Agrave;"] = "\x00C0",
               ["&Aacute;"] = "\x00C1",
               ["&Auml;"] = "\x00C4",
               ["&Acirc;"] = "\x00C2",
               ["&Atilde;"] = "\x00C3",
               ["&Aring;"] = "\x00C5",
               ["&AElig;"] = "\x00C6",
               ["&auml;"] = "\x00E4",
               ["&agrave;"] = "\x00E0",
               ["&aacute;"] = "\x00E1",
               ["&acirc;"] = "\x00E2",
               ["&atilde;"] = "\x00E3",
               ["&aring;"] = "\x00E5",
               ["&aelig;"] = "\x00E6",
               ["&Ccedil;"] = "\x00C7",
               ["&ccedil;"] = "\x00E7",
               ["&ETH;"] = "\x00D0",
               ["&eth;"] = "\x00F0",
               ["&Euml;"] = "\x00CB",
               ["&Egrave;"] = "\x00C8",
               ["&Eacute;"] = "\x00C9",
               ["&Ecirc;"] = "\x00CA",
               ["&euml;"] = "\x00EB",
               ["&egrave;"] = "\x00E8",
               ["&eacute;"] = "\x00E9",
               ["&ecirc;"] = "\x00EA",
               ["&Iuml;"] = "\x00CF",
               ["&Igrave;"] = "\x00CC",
               ["&Iacute;"] = "\x00CD",
               ["&Icirc;"] = "\x00CE",
               ["&iuml;"] = "\x00EF",
               ["&igrave;"] = "\x00EC",
               ["&iacute;"] = "\x00ED",
               ["&icirc;"] = "\x00EE",
               ["&Ntilde;"] = "\x00D1",
               ["&ntilde;"] = "\x00F1",
               ["&Ouml;"] = "\x00D6",
               ["&Ograve;"] = "\x00D2",
               ["&Oacute;"] = "\x00D3",
               ["&Ocirc;"] = "\x00D4",
               ["&Otilde;"] = "\x00D5",
               ["&Oslash;"] = "\x00D8",
               ["&OElig;"] = "\x0152",
               ["&ouml;"] = "\x00F6",
               ["&ograve;"] = "\x00F2",
               ["&oacute;"] = "\x00F3",
               ["&ocirc;"] = "\x00F4",
               ["&otilde;"] = "\x00F5",
               ["&oslash;"] = "\x00F8",
               ["&oelig;"] = "\x0153",
               ["&Scaron;"] = "\x0160",
               ["&szlig;"] = "\x00DF",
               ["&scaron;"] = "\x0161",
               ["&THORN;"] = "\x00DE",
               ["&thorn;"] = "\x00FE",
               ["&Uuml;"] = "\x00DC",
               ["&Ugrave;"] = "\x00D9",
               ["&Uacute;"] = "\x00DA",
               ["&Ucirc;"] = "\x00DB",
               ["&uuml;"] = "\x00FC",
               ["&ugrave;"] = "\x00F9",
               ["&uacute;"] = "\x00FA",
               ["&ucirc;"] = "\x00FB",
               ["&Yuml;"] = "\x0178",
               ["&Yacute;"] = "\x00DD",
               ["&yuml;"] = "\x00FF",
               ["&yacute;"] = "\x00FD",
           });

        public static string ReplaceHtmlEntities(string originalXml)
        {
            var builder = new StringBuilder(originalXml);

            foreach (var pair in _xmlEntities)
            {
                builder.Replace(pair.Key, pair.Value);
            }

            return builder.ToString();
        }
    }
}
