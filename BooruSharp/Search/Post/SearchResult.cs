namespace BooruSharp.Search.Post
{
    public struct SearchResult
    {
        public SearchResult(string fileUrl, string previewUrl, Rating rating, string[] tags, uint id)
        {
            this.fileUrl = fileUrl;
            this.previewUrl = previewUrl;
            this.rating = rating;
            this.tags = tags;
            this.id = id;
        }
        public readonly string fileUrl;
        public readonly string previewUrl;
        public readonly Rating rating;
        public readonly string[] tags;
        public readonly uint id;
    }
}
