namespace BooruSharp.Booru.Custom
{
    public class CustomBooru : Booru
    {
        public CustomBooru(string baseUrl, UrlFormat format, int? maxLimit = null, params BooruOptions[] options) : base(baseUrl, format, maxLimit, options)
        {
            isSafe = false;
        }

        public override bool IsSafe()
        {
            return (isSafe);
        }

        public void SetIsSafe(bool value)
        {
            isSafe = value;
        }

        private bool isSafe;
    }
}
