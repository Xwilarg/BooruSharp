using System;

namespace BooruSharp.Booru
{
    public class Atfbooru : Booru
    {
        public Atfbooru(BooruAuth auth = null) : base("booru.allthefallen.moe", auth, UrlFormat.danbooru, 1000, BooruOptions.wikiSearchUseTitle, BooruOptions.noComment, BooruOptions.noRelated)
        { }

        public override bool IsSafe()
            => false;
    }
}
