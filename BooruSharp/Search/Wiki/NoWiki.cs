using System;

namespace BooruSharp.Search.Wiki
{
    public class NoWiki : Exception
    {
        public NoWiki() : base("There is no wiki available for this Booru")
        { }
    }
}
