using BooruSharp.Search;
using Newtonsoft.Json.Linq;

namespace BooruSharp.Booru
{
    public sealed class Atfbooru : Template.Danbooru
    {
        public Atfbooru() : base("booru.allthefallen.moe")
        { }

        public override bool IsSafe()
            => false;

        protected internal override Search.Post.SearchResult GetPostSearchResult(JToken elem)
        {
            var success = elem["success"];
            if (success != null && success.Value<bool>() == false)
                throw new InvalidTags();
            return base.GetPostSearchResult(elem);
        }

        protected internal override Search.Post.SearchResult[] GetPostsSearchResult(object json)
        {
            var arr = json as JArray;
            if (arr == null)
            {
                var token = (JToken)json;
                if (token == null)
                    return new Search.Post.SearchResult[0];
                var success = token["success"];
                if (success != null && success.Value<bool>() == false)
                    return new Search.Post.SearchResult[0];
                return new Search.Post.SearchResult[1] { GetPostSearchResult(((JToken)json)["post"]) };
            }
            Search.Post.SearchResult[] res = new Search.Post.SearchResult[arr.Count];
            int i = 0;
            foreach (var elem in arr)
            {
                res[i] = GetPostSearchResult(elem);
                i++;
            }
            return res;
        }
    }
}
