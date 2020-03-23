using System;

namespace BooruSharp.Search
{
    public class InvalidAuthentificationMethod : Exception
    {
        public InvalidAuthentificationMethod() : base("Invalid authentification method.\nMake sure you are using the right one (API Key / Password Hash)")
        { }
    }
}
