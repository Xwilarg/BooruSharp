﻿using BooruSharp.Booru.Parsing;
using BooruSharp.Search.Post;
using BooruSharp.Search.Tag;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reflection;

namespace BooruSharp.Booru.Template
{
    /// <summary>
    /// Template booru based on Danbooru. This class is <see langword="abstract"/>.
    /// </summary>
    public abstract class Danbooru : ABooru<EmptyParsing, Danbooru.SearchResult, EmptyParsing, EmptyParsing, EmptyParsing>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Danbooru"/> template class.
        /// </summary>
        /// <param name="domain">
        /// The fully qualified domain name. Example domain
        /// name should look like <c>www.google.com</c>.
        /// </param>
        /// <param name="options">
        /// The options to use. Use <c>|</c> (bitwise OR) operator to combine multiple options.
        /// </param>
        protected Danbooru(string domain, BooruOptions options = BooruOptions.None)
            : base(domain, UrlFormat.Danbooru, options | BooruOptions.NoLastComments | BooruOptions.NoPostCount
                  | BooruOptions.NoFavorite)
        { }

        /// <inheritdoc/>
        protected override void PreRequest(HttpRequestMessage message)
        {
            if (Auth != null)
            {
                message.Headers.Add("Cookie", "user_id=" + Auth.UserId + ";pass_hash=" + Auth.PasswordHash);
            }
        }

        private protected override PostSearchResult GetPostSearchResult(SearchResult parsingData)
        {
            return new PostSearchResult(
                fileUrl: parsingData.file_url != null ? new Uri(parsingData.file_url) : null,
                previewUrl: parsingData.preview_file_url != null ? new Uri(parsingData.preview_file_url) : null,
                postUrl: parsingData.id != null ? new Uri(BaseUrl + "posts/" + parsingData.id) : null,
                sampleUri: parsingData.large_file_url != null ? new Uri(parsingData.large_file_url) : null,
                rating: ABooru.GetRating(parsingData.rating[0]),
                tags: parsingData.tag_string.Split(),
                detailedTags: parsingData.tag_string_general.Split().Select(x => new TagSearchResult(-1, x, TagType.Trivia, -1))
                    .Concat(parsingData.tag_string_character.Split().Select(x => new TagSearchResult(-1, x, TagType.Character, -1)))
                    .Concat(parsingData.tag_string_copyright.Split().Select(x => new TagSearchResult(-1, x, TagType.Copyright, -1)))
                    .Concat(parsingData.tag_string_artist.Split().Select(x => new TagSearchResult(-1, x, TagType.Artist, -1)))
                    .Concat(parsingData.tag_string_meta.Split().Select(x => new TagSearchResult(-1, x, TagType.Metadata, -1))),
                id: parsingData.id ?? 0,
                size: parsingData.file_size,
                height: parsingData.image_height,
                width: parsingData.image_width,
                previewHeight: null,
                previewWidth: null,
                creation: parsingData.create_at,
                source: parsingData.source,
                score: parsingData.score,
                md5: parsingData.md5
            );
        }

        public class SearchResult
        {
            public string file_url;
            public string preview_file_url;
            public string large_file_url;
            public int? id;
            public string md5;
            public string rating;
            public string tag_string;
            public string tag_string_general;
            public string tag_string_character;
            public string tag_string_copyright;
            public string tag_string_artist;
            public string tag_string_meta;
            public int file_size;
            public int image_height;
            public int image_width;
            public DateTime create_at;
            public string source;
            public int score;
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
