using Newtonsoft.Json;

namespace BooruSharp.Search.Related
{
#pragma warning disable 0649
    internal class SearchResultJson
    {
        [JsonProperty]
        public string name;

        [JsonProperty]
        public int count;
    }
#pragma warning restore 0649
}
