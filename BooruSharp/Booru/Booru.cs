using System;

namespace BooruSharp.Booru
{
    public abstract partial class Booru
    {
        protected Booru(string baseUrl, UrlFormat format, int? maxLimit)
        {
            imageUrl = "https://" + baseUrl + "/" + GetImageUrl(format);
            tagUrl = "https://" + baseUrl + "/" + GetTagUrl(format);
            needInterrogation = (imageUrl.EndsWith(".xml"));
            random = new Random();
            this.maxLimit = maxLimit;
        }

        private string GetImageUrl(UrlFormat format)
        {
            switch (format)
            {
                case UrlFormat.postIndexXml:
                    return ("post/index.xml");

                case UrlFormat.indexPhp:
                    return ("index.php?page=dapi&s=post&q=index");

                case UrlFormat.postXml:
                    return ("post.xml");

                default:
                    throw new ArgumentException("Invalid URL format " + format);
            }
        }

        private string GetTagUrl(UrlFormat format)
        {
            switch (format)
            {
                case UrlFormat.postIndexXml:
                    return ("tag/index.xml");

                case UrlFormat.indexPhp:
                    return ("index.php?page=dapi&s=tag&q=index");

                case UrlFormat.postXml:
                    return ("tag.xml");

                default:
                    throw new ArgumentException("Invalid URL format " + format);
            }
        }

        private string CreateUrl(params string[] args)
        {
            return (imageUrl + ((needInterrogation) ? ("?") : ("&")) + String.Join("&", args));
        }

        private string TagsToString(string[] tags)
        {
            return ("tags=" + String.Join("+", tags));
        }

        protected readonly string imageUrl, tagUrl;
        private readonly bool needInterrogation;
        private readonly int? maxLimit;
        private Random random;
    }
}
