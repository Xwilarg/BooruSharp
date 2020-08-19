namespace BooruSharp.Booru
{
    public sealed class E926 : Template.E621
    {
        public E926() : base("e926.net")
        { }

        /// <summary>
        /// E926 can provide images with an explicit rating, but the URL of the file will be null
        /// </summary>
        /// <returns></returns>
        public override bool IsSafe() => true;
    }
}
