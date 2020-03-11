using Newtonsoft.Json;

namespace BooruSharp.Search.Wiki
{
#pragma warning disable 0649
    internal class SearchResultJson
    {
        [JsonProperty]
        public int id;

        [JsonProperty]
        public string title;

        [JsonProperty("created_at")]
        public string creation
        {
            set { creation = value; }
            get { return creation ?? creation2; }
        }

        [JsonProperty("created-at")]
        public string creation2;

        [JsonProperty("updated_at")]
        public string lastUpdate
        {
            set { lastUpdate = value; }
            get { return lastUpdate ?? lastUpdate2; }
        }

        [JsonProperty("updated-at")]
        public string lastUpdate2;

        [JsonProperty]
        public string body;
    }
#pragma warning restore 0649
}
