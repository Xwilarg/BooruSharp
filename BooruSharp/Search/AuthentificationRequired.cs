using System;

namespace BooruSharp.Search
{
    /// <summary>
    /// Represents errors that occur whenever a method that requires
    /// authentication credentials is called with no credentials.
    /// </summary>
    public class AuthentificationRequired : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AuthentificationRequired"/> class.
        /// </summary>
        public AuthentificationRequired()
            : base("Authentification is required for this feature")
        { }
    }
}
