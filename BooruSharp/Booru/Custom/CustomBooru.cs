using System;
using System.Linq;

namespace BooruSharp.Booru.Custom
{
    public class CustomBooru : Booru
    {
        public CustomBooru(string baseUrl, UrlFormat format, int? maxLimit = null, params BooruOptions[] options) : base(baseUrl, format, maxLimit, (options.Contains(BooruOptions.useHttp)))
        {
            if (!options.Contains(BooruOptions.ignoreCheck))
            {
                try
                {
                    GetNbImage();
                }
                catch (Exception ex)
                {
                    throw new InvalidBooru(imageUrl, ex);
                }
            }
        }
    }
}
