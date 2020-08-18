namespace BooruSharp.Search
{
    public class InvalidPostId : System.ArgumentException
    {
        public InvalidPostId(string message) : base(message)
        { }

        public InvalidPostId() : base("There is no post with this id")
        { }
    }
}
