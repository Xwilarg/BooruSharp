using System;

namespace BooruSharp.Search
{
    /// <summary>
    /// Represents errors that occur whenever a method that requires
    /// certain booru features (such as tag wiki API) is called.
    /// </summary>
    public class FeatureUnavailable : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FeatureUnavailable"/> class.
        /// </summary>
        public FeatureUnavailable() : base("This feature isn't available for this Booru")
        { }
    }
}
