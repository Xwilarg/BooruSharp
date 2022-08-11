using BooruSharp.Booru.Template;

namespace BooruSharp.Booru
{
    public class Derpibooru : Philomena
    {
        public Derpibooru() : base("derpibooru.org")
        { }

        public override bool IsSafe => false;
    }
}
