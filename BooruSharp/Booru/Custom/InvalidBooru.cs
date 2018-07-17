using System;

namespace BooruSharp.Booru.Custom
{
    public class InvalidBooru : System.ArgumentException
    {
        public InvalidBooru(string booruUrl, Exception ex) : base(booruUrl + " isn't a valid booru" + Environment.NewLine +
                                                            "Impossible to get posts number" + Environment.NewLine +
                                                            "You must give the base URL to the API endpoint in XML format", ex)
        { }
    }
}
