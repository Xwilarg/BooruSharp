using BooruSharp.Booru.Template;

namespace BooruSharp.Booru
{
    public class Twibooru : BooruOnRails
    {
        public Twibooru() : base("twibooru.org")
        { }

        public override bool IsSafe => false;
    }
}
