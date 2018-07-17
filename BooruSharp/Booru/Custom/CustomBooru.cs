using System;

namespace BooruSharp.Booru.Custom
{
    public class CustomBooru : Booru
    {
        public CustomBooru(string baseUrl, int? maxLimit = null, bool ignoreCheck = false) : base(baseUrl, maxLimit)
        {
            if (!ignoreCheck)
            {
                try
                {
                    GetNbImage();
                }
                catch (Exception ex)
                {
                    throw new InvalidBooru(baseUrl, ex);
                }
            }
        }
    }
}
