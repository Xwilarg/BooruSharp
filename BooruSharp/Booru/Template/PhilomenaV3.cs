namespace BooruSharp.Booru.Template
{
    public abstract class PhilomenaV3 : Philomena
    {
        protected PhilomenaV3(string domain, BooruOptions options = BooruOptions.None) : base(domain, UrlFormat.PhilomenaV3, options)
        { }

        public override string PostsKeyName => "posts";
    }
}
