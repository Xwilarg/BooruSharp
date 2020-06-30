namespace BooruSharp.Booru
{
    public class BooruAuth
    {
        public BooruAuth(string userId, string passwordHash)
        {
            UserId = userId;
            PasswordHash = passwordHash;
        }

        public string UserId { private set; get; }
        public string PasswordHash { private set; get; }
    }
}
