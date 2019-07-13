using System;

namespace BooruSharp.Booru
{
    public class Atfbooru : Booru
    {
        [Obsolete("AtfBooru seams to be definitly down, therefore everything related to it won't work anymore.")]
        public Atfbooru(BooruAuth auth = null) : base("atfbooru.ninja", auth, UrlFormat.danbooru, 1000, BooruOptions.wikiSearchUseTitle, BooruOptions.noComment)
        { }

        public override bool IsSafe()
        {
            return (false);
        }
    }
}
