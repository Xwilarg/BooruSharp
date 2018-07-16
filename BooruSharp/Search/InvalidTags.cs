namespace BooruSharp.Search
{
    public class InvalidTags : System.ArgumentException
    {
        public InvalidTags() : base("There is no image available with these tags")
        { }
    }
}
