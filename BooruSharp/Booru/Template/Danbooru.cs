using BooruSharp.Search.Post;
using BooruSharp.Search.Tag;
using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace BooruSharp.Booru.Template
{
    /// <summary>
    /// Template booru based on Danbooru https://github.com/danbooru/danbooru . This class is <see langword="abstract"/>.
    /// </summary>
    public abstract class Danbooru : ABooru
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Danbooru"/> template class.
        /// </summary>
        /// <param name="domain">
        /// The fully qualified domain name. Example domain
        /// name should look like <c>www.google.com</c>.
        /// </param>
        protected Danbooru(string domain) : base(domain)
        { }

        protected override Uri CreateQueryString(string query, string squery = "index")
        {
            if (query == "tag" && squery == "related")
            {
                return new($"{BaseUrl}related_tag.json");
            }
            if (query == "tag" && squery == "wiki")
            {
                return new($"{BaseUrl}wiki_pages.json");
            }
            return new($"{BaseUrl}{query}s.json");
        }

        protected override Task<Uri> CreateRandomPostUriAsync(string[] tags)
        {
            return Task.FromResult(CreateUrl(_imageUrl, "limit=1", "tags=" + string.Join("+", tags.Select(Uri.EscapeDataString)).ToLowerInvariant(), "random=true"));
        }

        /// <inheritdoc/>
        protected override void PreRequest(HttpRequestMessage message)
        {
            if (Auth != null)
            {
                message.Headers.Add("Cookie", "user_id=" + Auth.UserId + ";pass_hash=" + Auth.PasswordHash);
            }
        }

        private protected override async Task<PostSearchResult> GetPostSearchResultAsync(Uri uri)
        {
            var parsingData = await GetDataAsync<SearchResult>(uri);

            return new PostSearchResult(
                fileUrl: parsingData.FileUrl != null ? new Uri(parsingData.FileUrl) : null,
                previewUrl: parsingData.PreviewFileUrl != null ? new Uri(parsingData.PreviewFileUrl) : null,
                postUrl: parsingData.Id != null ? new Uri($"{BaseUrl}posts/{parsingData.Id}") : null,
                sampleUri: parsingData.LargeFileUrl != null ? new Uri(parsingData.LargeFileUrl) : null,
                rating: GetRating(parsingData.Rating[0]),
                tags: parsingData.TagString.Split(),
                detailedTags: parsingData.TagStringGeneral.Split().Select(x => new TagSearchResult(-1, x, TagType.Trivia, -1))
                    .Concat(parsingData.TagStringCharacter.Split().Select(x => new TagSearchResult(-1, x, TagType.Character, -1)))
                    .Concat(parsingData.TagStringCopyright.Split().Select(x => new TagSearchResult(-1, x, TagType.Copyright, -1)))
                    .Concat(parsingData.TagStringArtist.Split().Select(x => new TagSearchResult(-1, x, TagType.Artist, -1)))
                    .Concat(parsingData.TagStringMeta.Split().Select(x => new TagSearchResult(-1, x, TagType.Metadata, -1))),
                id: parsingData.Id ?? 0,
                size: parsingData.FileSize,
                height: parsingData.ImageHeight,
                width: parsingData.ImageWidth,
                previewHeight: null,
                previewWidth: null,
                creation: parsingData.CreatedAt,
                sources: string.IsNullOrEmpty(parsingData.Source) ? Array.Empty<string>() : new[] { parsingData.Source },
                score: parsingData.Score,
                hash: parsingData.Md5
            );
        }

        public class SearchResult
        {
            public string FileUrl { init; get; }
            public string PreviewFileUrl { init; get; }
            public string LargeFileUrl { init; get; }
            public int? Id { init; get; }
            public string Md5 { init; get; }
            public string Rating { init; get; }
            public string TagString { init; get; }
            public string TagStringGeneral { init; get; }
            public string TagStringCharacter { init; get; }
            public string TagStringCopyright { init; get; }
            public string TagStringArtist { init; get; }
            public string TagStringMeta { init; get; }
            public int FileSize { init; get; }
            public int ImageHeight { init; get; }
            public int ImageWidth { init; get; }
            public DateTime CreatedAt { init; get; }
            public string Source { init; get; }
            public int Score { init; get; }
        }

        /*
            var detailedtags = new List<Search.Tag.SearchResult>();
            GetTags("tag_string_general", Search.Tag.TagType.Trivia);
            GetTags("tag_string_character", Search.Tag.TagType.Character);
            GetTags("tag_string_copyright", Search.Tag.TagType.Copyright);
            GetTags("tag_string_meta", Search.Tag.TagType.Metadata);
            GetTags("tag_string_artist", Search.Tag.TagType.Artist);
            
            void GetTags(string objectName, Search.Tag.TagType tagType)
            {
                var obj = elem[objectName];
                if(obj == null)
                    return;
                
                foreach(var x in obj.Value<string>().Split())
                {
                    detailedtags.Add(new Search.Tag.SearchResult(
                        -1,
                        x,
                        tagType,
                        -1));
                }
            }
            
            return new Search.Post.SearchResult(
                    url != null ? new Uri(url.Value<string>()) : null,
                    previewUrl != null ? new Uri(previewUrl.Value<string>()) : null,
                    id.HasValue ? new Uri(BaseUrl + "posts/" + id.Value) : null,
                    sampleUrl != null ? new Uri(sampleUrl.Value<string>()) : null,
                    GetRating(elem["rating"].Value<string>()[0]),
                    elem["tag_string"].Value<string>().Split(' '),
                    detailedtags,
                    id ?? 0,
                    elem["file_size"].Value<int>(),
                    elem["image_height"].Value<int>(),
                    elem["image_width"].Value<int>(),
                    null,
                    null,
                    elem["created_at"].Value<DateTime>(),
                    elem["source"].Value<string>(),
                    elem["score"].Value<int>(),
                    md5?.Value<string>()
                );
        }

        private protected override Search.Post.SearchResult[] GetPostsSearchResult(object json)
        {
            if (json is JArray array)
                return array.Select(GetPostSearchResult).ToArray();
            else if (json is JToken token && token["post"] is JToken post)
                return new[] { GetPostSearchResult(post) };
            else
                return Array.Empty<Search.Post.SearchResult>();
        }

        private protected override Search.Comment.SearchResult GetCommentSearchResult(object json)
        {
            var elem = (JObject)json;
            return new Search.Comment.SearchResult(
                elem["id"].Value<int>(),
                elem["post_id"].Value<int>(),
                elem["creator_id"]?.Value<int?>(),
                elem["created_at"].Value<DateTime>(),
                elem["creator_name"]?.Value<string>(),
                elem["body"]?.Value<string>()
                );
        }

        private protected override Search.Wiki.SearchResult GetWikiSearchResult(object json)
        {
            var elem = (JObject)json;
            return new Search.Wiki.SearchResult(
                elem["id"].Value<int>(),
                elem["title"].Value<string>(),
                elem["created_at"].Value<DateTime>(),
                elem["updated_at"].Value<DateTime>(),
                elem["body"].Value<string>()
                );
        }

        private protected override Search.Tag.SearchResult GetTagSearchResult(object json)
        {
            var elem = (JObject)json;
            return new Search.Tag.SearchResult(
                elem["id"].Value<int>(),
                elem["name"].Value<string>(),
                (Search.Tag.TagType)elem["category"].Value<int>(),
                elem["post_count"].Value<int>()
                );
        }

        private protected override Search.Related.SearchResult GetRelatedSearchResult(object json)
        {
            var elem = (JArray)json;
            return new Search.Related.SearchResult(
                elem[0].Value<string>(),
                elem[1].Value<int>()
                );
        }*/
    }
}
