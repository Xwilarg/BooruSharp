using System;

namespace BooruSharp.Search
{
    public class AuthentificationRequired : Exception
    {
        public AuthentificationRequired() : base("Authentification is required for this feature")
        { }
    }
}
