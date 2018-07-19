using System;

namespace BooruSharp.Search
{
    public class FeatureUnavailable : Exception
    {
        public FeatureUnavailable() : base("This feature isn't available for this Booru")
        { }
    }
}
