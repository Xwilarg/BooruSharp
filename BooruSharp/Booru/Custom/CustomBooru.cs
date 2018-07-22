using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BooruSharp.Booru.Custom
{
    public class CustomBooru : Booru
    {
        public CustomBooru(string baseUrl, UrlFormat format, int? maxLimit = null, params BooruOptions[] options) : base(baseUrl, format, maxLimit, options)
        { }

        public static async Task<List<BooruOptions> > GetOptions(string baseUrl, UrlFormat format)
        {
            CustomBooru booru = new CustomBooru(baseUrl, format, null);
            List<BooruOptions> options = new List<BooruOptions>();
            Search.Post.SearchResult result;
            try
            {
                result = await booru.GetImage(0);
            } catch (Exception) {
                booru = new CustomBooru(baseUrl, format, null, BooruOptions.useHttp);
                try
                {
                    result = await booru.GetImage(0);
                } catch (Exception ex) {
                    throw new InvalidBooru(baseUrl, ex);
                }
                options.Add(BooruOptions.useHttp);
            }
            uint id = (await booru.GetTag(result.tags[0])).id;
            try
            {
                await booru.GetComment((int)result.id);
            } catch (Exception) {
                options.Add(BooruOptions.noComment);
            }
            try
            {
                await booru.GetWiki(result.tags[0]);
            } catch (Exception) {
                try
                {
                    booru = new CustomBooru(baseUrl, format, null, ((options.Contains(BooruOptions.useHttp)) ? (BooruOptions.useHttp) : (BooruOptions.noComment)), BooruOptions.wikiSearchUseTitle);
                    await booru.GetWiki(result.tags[0]);
                    options.Add(BooruOptions.wikiSearchUseTitle);
                } catch (Exception) {
                    options.Add(BooruOptions.noWiki);
                }
            }
            try
            {
                await booru.GetTag((int)id);
            } catch (Exception) {
                options.Add(BooruOptions.noTagById);
            }
            try
            {
                if ((await booru.GetRelated(result.tags[0]))[0].name != result.tags[0])
                    options.Add(BooruOptions.noRelated);
            } catch (Exception) {
                options.Add(BooruOptions.noRelated);
            }
            return (options);
        }
    }
}
