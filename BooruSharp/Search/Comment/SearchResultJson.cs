using Newtonsoft.Json;

namespace BooruSharp.Search.Comment
{
#pragma warning disable 0649
    internal class SearchResultJson
    {
        [JsonProperty("id")]
        public int commentId;

        [JsonProperty("post_id")]
        public int postId;

        [JsonProperty("creator_id")]
        public int authorId;

        [JsonProperty("created_at")]
        public string creation;

        [JsonProperty("creator")]
        public string authorName;

        [JsonProperty("body")]
        public string body;
    }
#pragma warning restore 0649
}
