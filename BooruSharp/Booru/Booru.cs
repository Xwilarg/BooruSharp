using System;
using System.Net;
using System.Xml;

namespace BooruSharp.Booru
{
    public abstract class Booru
    {
        protected Booru(string url)
        {
            this.url = url;
            needInterrogation = (url.EndsWith(".xml"));
        }

        public uint GetNbImageMax(params string[] tags)
        {
            XmlDocument xml = new XmlDocument();
            using (WebClient wc = new WebClient())
            {
                wc.Headers.Add("User-Agent: BooruSharp");
                xml.LoadXml(wc.DownloadString(CreateUrl("limit=1", TagsToString(tags))));
            }
            return (Convert.ToUInt32(xml.ChildNodes.Item(1).Attributes[0].InnerXml));
        }

        private string CreateUrl(params string[] args)
        {
            return (url + ((needInterrogation) ? ("?") : ("&")) + String.Join("&", args));
        }

        private string TagsToString(string[] tags)
        {
            return ("tags=" + String.Join("+", tags));
        }

        private readonly string url;
        private readonly bool needInterrogation;
    }
}
