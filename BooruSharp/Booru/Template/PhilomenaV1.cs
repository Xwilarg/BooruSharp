namespace BooruSharp.Booru.Template
{
    public abstract class PhilomenaV1 : Philomena
    {
        protected PhilomenaV1(string domain, BooruOptions options = BooruOptions.None) : base(domain, UrlFormat.PhilomenaV1, options)
        { }

        public override string PostsKeyName => "images";
    }
}
