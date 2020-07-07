namespace BooruSharp.Search
{
    public class InvalidPostId : System.ArgumentException
    {
        public InvalidPostId() : base("There is no post with this id")
        { }
    }
}
