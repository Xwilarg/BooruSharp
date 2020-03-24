namespace BooruSharp.Search
{
    public class TooManyTags : System.ArgumentException
    {
        public TooManyTags() : base("You can't have more than 2 tags for a search with this booru")
        { }
    }
}
