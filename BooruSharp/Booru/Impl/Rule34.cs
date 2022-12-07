using System.Linq;
using System.Threading.Tasks;
using System.Xml;
using System;

namespace BooruSharp.Booru
{
    /// <summary>
    /// Rule 34.
    /// <para>https://rule34.xxx/</para>
    /// </summary>
    public sealed class Rule34 : Template.Gelbooru02
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Rule34"/> class.
        /// </summary>
        public Rule34()
            : base("rule34.xxx") //TODO:, BooruOptions.NoComment
        { }

        protected override async Task<Uri> CreateRandomPostUriAsync(string[] tags)
        {
            if (!tags.Any())
            {
                await base.CreateRandomPostUriAsync(tags); // Nothing change here, let's just call the base class
            }
            var url = CreateUrl(new(_imageUrl.AbsoluteUri.Replace("index.json", "index.xml")), "limit=1", string.Join("+", tags.Select(Uri.EscapeDataString)).ToLowerInvariant());
            XmlDocument xml = await GetXmlAsync(url.AbsoluteUri);
            int max = int.Parse(xml.ChildNodes.Item(1).Attributes[0].InnerXml);

            if (max == 0)
                throw new Search.InvalidTags();

            // The limit is in fact 200000 but search with tags make it incredibly hard to know what is really your pid
            if (max > 20001)
                max = 20001;

            return CreateUrl(_imageUrl, "limit=1", string.Join("+", tags.Select(Uri.EscapeDataString)).ToLowerInvariant(), "pid=" + Random.Next(0, max));
        }

        /// <inheritdoc/>
        public override bool IsSafe => false;
    }
}
