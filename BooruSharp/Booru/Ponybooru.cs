using BooruSharp.Booru.Template;

namespace BooruSharp.Booru
{
    public class Ponybooru : Philomena
    {
        public Ponybooru() : base("ponybooru.org")
        { }

        public override bool IsSafe => false;
    }
}
