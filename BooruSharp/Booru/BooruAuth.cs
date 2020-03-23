namespace BooruSharp.Booru
{
    public class BooruAuth
    {
        public static BooruAuth CreateFromApiKey(string login, string apiKey)
        {
            return new BooruAuth(login, apiKey, null);
        }

        public static BooruAuth CreateFromPasswordHash(string login, string passwordHash)
        {
            return new BooruAuth(login, null, passwordHash);
        }

        private BooruAuth(string login, string apiKey, string passwordHash)
        {
            Login = login;
            ApiKey = apiKey;
            PasswordHash = passwordHash;
        }

        public string Login { private set; get; }
        public string ApiKey { private set; get; }
        public string PasswordHash { private set; get; }
    }
}
