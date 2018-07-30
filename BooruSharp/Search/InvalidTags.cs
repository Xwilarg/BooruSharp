namespace BooruSharp.Search
{
    public class InvalidTags : System.ArgumentException
    {
        public InvalidTags() : base("There is nothing available with these tags")
        { }
    }
}
