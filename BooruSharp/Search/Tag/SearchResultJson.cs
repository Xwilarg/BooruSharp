using Newtonsoft.Json;

namespace BooruSharp.Search.Tag
{
#pragma warning disable 0649
    internal class SearchResultJson
    {
        [JsonProperty]
        public int id;

        [JsonProperty]
        public string name;

        [JsonProperty]
        public int type;

        [JsonProperty]
        public int count
        {
            set { count = value; }
            get { return count == 0 ? count2 : count; }
        }

        [JsonProperty("post-count")]
        public int count2;
    }
#pragma warning restore 0649
}
