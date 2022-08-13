using BooruSharp.Booru.Template;

namespace BooruSharp.Booru
{
    public class Twibooru : PhilomenaV3
    {
        public Twibooru() : base("twibooru.org")
        { }

        public override bool IsSafe => false;
    }
}
