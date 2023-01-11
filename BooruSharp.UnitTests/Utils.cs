using BooruSharp.Booru;
using BooruSharp.Search.Post;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace BooruSharp.UnitTests
{
    public static class Utils
    {
        public static IEnumerable<object[]> BooruParams { get; } = new object[][]
        {
            new object[] {
                new BooruTestData()
                {
                    BooruType = typeof(Atfbooru),
                    Tags = new[] { "kantai_collection", "blue_eyes", "outdoors" },
                    ValidPostId = 419308
                }
            },
            new object[] {
                new BooruTestData()
                {
                    BooruType = typeof(DanbooruDonmai),
                    Tags = new[] { "kantai_collection", "blue_eyes", "outdoors" },
                    ValidPostId = 5972904
                }
            },
            new object[] {
                new BooruTestData()
                {
                    BooruType = typeof(Derpibooru),
                    Tags = new[] { "pegasus", "raised_tail", "lying_down" },
                    ValidPostId = 3021165
                }
            },
            new object[] {
                new BooruTestData()
                {
                    BooruType = typeof(E621),
                    Tags = new[] { "kantai_collection", "blue_eyes", "outside" },
                    ValidPostId = 1329650
                }
            },
            new object[] {
                new BooruTestData()
                {
                    BooruType = typeof(E926),
                    Tags = new[] { "kantai_collection", "blue_eyes", "outside" },
                    ValidPostId = 1329650
                }
            },
            new object[] {
                new BooruTestData()
                {
                    BooruType = typeof(Gelbooru),
                    Tags = new[] { "kantai_collection", "blue_eyes", "outdoors" },
                    ValidPostId = 8113595
                }
            },
            new object[] {
                new BooruTestData()
                {
                    BooruType = typeof(Konachan),
                    Tags = new[] { "kantai_collection", "blue_eyes", "hat" },
                    ValidPostId = 351127
                }
            },
            new object[] {
                new BooruTestData()
                {
                    BooruType = typeof(Lolibooru),
                    Tags = new[] { "kantai_collection", "blue_eyes", "outdoors" },
                    ValidPostId = 516532
                }
            },
            new object[] {
                new BooruTestData()
                {
                    BooruType = typeof(Ponybooru),
                    Tags = new[] { "pegasus", "raised_tail", "lying_down" },
                    ValidPostId = 3262026
                }
            },
            new object[] {
                new BooruTestData()
                {
                    BooruType = typeof(Realbooru),
                    Tags = new[] { "kantai_collection", "asian", "swimsuit" },
                    ValidPostId = 809254
                }
            },
            new object[] {
                new BooruTestData()
                {
                    BooruType = typeof(Rule34),
                    Tags = new[] { "kantai_collection", "outdoors", "swimsuit" },
                    ValidPostId = 139056
                }
            },
            new object[] {
                new BooruTestData()
                {
                    BooruType = typeof(Safebooru),
                    Tags = new[] { "kantai_collection", "outdoors", "swimsuit" },
                    ValidPostId = 4248721
                }
            },
            new object[] {
                new BooruTestData()
                {
                    BooruType = typeof(Sakugabooru),
                    Tags = new[] { "kantai_collection", "explosions", "smoke" },
                    ValidPostId = 157179
                }
            },
            new object[] {
                new BooruTestData()
                {
                    BooruType = typeof(SankakuComplex),
                    Tags = new[] { "kantai_collection", "blue_eyes", "outdoors" },
                    ValidPostId = 32458685
                }
            },
            new object[] {
                new BooruTestData()
                {
                    BooruType = typeof(Twibooru),
                    Tags = new[] { "pegasus", "raised_tail", "lying_down" },
                    ValidPostId = 2869409
                }
            },
            new object[] {
                new BooruTestData()
                {
                    BooruType = typeof(Xbooru),
                    Tags = new[] { "kantai_collection", "blue_eyes", "outside" },
                    ValidPostId = 830871
                }
            },
            new object[] {
                new BooruTestData()
                {
                    BooruType = typeof(Yandere),
                    Tags = new[] { "kantai_collection", "sweater", "bra" },
                    ValidPostId = 1052829
                }
            },
            //new BooruTestData() { BooruType = typeof(Pixiv) }
        };

        public static HttpClient Client;

        private static readonly Dictionary<Type, Task<ABooru>> _boorus = new();

        public static Task<ABooru> GetAsync(Type type)
        {
            lock (_boorus)
            {
                if (!_boorus.TryGetValue(type, out var booruTask))
                {
                    booruTask = Task.Run(() => CreateBooruAsync(type));
                    _boorus[type] = booruTask;
                }

                return booruTask;
            }
        }

        private static async Task<ABooru> CreateBooruAsync(Type type)
        {
            var booru = (ABooru)Activator.CreateInstance(type);

            /*if (booru is Pixiv pixiv)
            {
                string refresh = Environment.GetEnvironmentVariable("PIXIV_REFRESH_TOKEN");

                Skip.If(refresh == null, "Pixiv tokens aren't set.");

                await pixiv.LoginAsync(refresh);
            }*/

            return booru;
        }

        private static async Task<bool> ValidateUrlAsync(string url)
        {
            if (string.IsNullOrEmpty(url))
            {
                return false;
            }

            var response = await Client.SendAsync(new HttpRequestMessage(HttpMethod.Head, url));
            return response.IsSuccessStatusCode
                || response.StatusCode == HttpStatusCode.UnavailableForLegalReasons; // Takedown request on danbooru returns 451 code
        }

        public static async Task<T> DoWebRequest<T>(Func<Task<T>> method)
        {
            try
            {
                return await method();
            }
            catch (HttpRequestException hre)
            {
                Skip.If(hre.StatusCode == HttpStatusCode.ServiceUnavailable, "Service returned 503 error");
                throw;
            }
        }

        public static async Task ValidatePostAsync(PostSearchResult res, string[] inputTags)
        {
            if (res.FileUrl != null) Assert.True(await ValidateUrlAsync(res.FileUrl.AbsoluteUri), $"Invalid URL {res.FileUrl.AbsoluteUri}");
            if (res.PreviewUrl != null) Assert.True(await ValidateUrlAsync(res.PreviewUrl.AbsoluteUri), $"Invalid URL {res.PreviewUrl.AbsoluteUri}");
            if (res.PostUrl != null) Assert.True(await ValidateUrlAsync(res.PostUrl.AbsoluteUri), $"Invalid URL {res.PostUrl.AbsoluteUri}");
            if (res.SampleUri != null) Assert.True(await ValidateUrlAsync(res.SampleUri.AbsoluteUri), $"Invalid URL {res.SampleUri.AbsoluteUri}");
            Assert.InRange(res.Rating, Rating.General, Rating.Explicit);
            Assert.NotEmpty(res.Tags);
            Assert.NotEqual(0, res.ID);
            Assert.NotEqual(0, res.Width);
            Assert.NotEqual(0, res.Height);
            if (res.PreviewWidth != null) Assert.NotEqual(0, res.PreviewWidth);
            if (res.PreviewHeight != null) Assert.NotEqual(0, res.PreviewHeight);
            if (res.Size != null) Assert.NotEqual(0, res.Size);
            foreach (var tag in inputTags)
            {
                Assert.Contains(tag.Replace('_', ' '), res.Tags.Select(x => x.Replace('_', ' ')));
                if (res.DetailedTags != null) Assert.Contains(tag.Replace('_', ' '), res.DetailedTags.Select(x => x.Name.Replace('_', ' ')));
            }
        }
    }
}
