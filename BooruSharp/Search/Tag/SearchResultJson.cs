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

        private int? countInternal;

        [JsonProperty]
        public int? count
        {
            set { countInternal = value; }
            get { return countInternal ?? count2; }
        }

        [JsonProperty("post-count")]
        public int count2;
    }
#pragma warning restore 0649
}
