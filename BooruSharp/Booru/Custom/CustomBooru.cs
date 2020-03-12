namespace BooruSharp.Booru.Custom
{
    public class CustomBooru : Booru
    {
        public CustomBooru(string baseUrl, UrlFormat format, BooruAuth auth = null, params BooruOptions[] options) : base(baseUrl, auth, format, options)
        {
            isSafe = false;
        }

        public override bool IsSafe()
            => isSafe;

        public void SetIsSafe(bool value)
            => isSafe = value;

        private bool isSafe;
    }
}
