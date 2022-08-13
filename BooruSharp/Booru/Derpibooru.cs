using BooruSharp.Booru.Template;

namespace BooruSharp.Booru
{
    public class Derpibooru : PhilomenaV1
    {
        public Derpibooru() : base("derpibooru.org")
        { }

        public override bool IsSafe => false;
    }
}
