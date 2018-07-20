using System;
using System.Linq;

namespace BooruSharp.Booru.Custom
{
    public class CustomBooru : Booru
    {
        public CustomBooru(string baseUrl, UrlFormat format, int? maxLimit = null, params BooruOptions[] options) : base(baseUrl, format, maxLimit, options)
        {
            if (!options.Contains(BooruOptions.ignoreCheck))
            {
                try
                {
                    GetNbImage().GetAwaiter().GetResult();
                }
                catch (Exception ex)
                {
                    throw new InvalidBooru(imageUrl, ex);
                }
            }
        }
    }
}
