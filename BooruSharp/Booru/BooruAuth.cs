namespace BooruSharp.Booru
{
    public class BooruAuth
    {
        public BooruAuth(string login, string passwordHash)
        {
            Login = login;
            PasswordHash = passwordHash;
        }

        public string Login { private set; get; }
        public string PasswordHash { private set; get; }
    }
}
