using System;

namespace BooruSharp.Search
{
    /// <summary>
    /// Represents errors that occur whenever a method that requires
    /// authentication credentials is called with invalid credentials.
    /// </summary>
    public class AuthentificationInvalid : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AuthentificationInvalid"/> class.
        /// </summary>
        public AuthentificationInvalid() : base("Your credentials are invalid")
        { }
    }
}
