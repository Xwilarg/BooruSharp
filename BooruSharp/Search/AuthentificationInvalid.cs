using System;

namespace BooruSharp.Search
{
    public class AuthentificationInvalid : Exception
    {
        public AuthentificationInvalid() : base("Your credentials are invalid")
        { }
    }
}
