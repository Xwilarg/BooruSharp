using System;

namespace BooruSharp.Booru.Custom
{
    public class CustomBooru : Booru
    {
        public CustomBooru(string baseUrl, UrlFormat format, int? maxLimit = null, bool ignoreCheck = false) : base(baseUrl, format, maxLimit)
        {
            if (!ignoreCheck)
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
