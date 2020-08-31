using System;

namespace BooruSharp.Booru
{
    /// <summary>
    /// Represents authentication credentials.
    /// </summary>
    public class BooruAuth
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BooruAuth"/> class.
        /// </summary>
        /// <param name="userId">User ID.</param>
        /// <param name="passwordHash">User password hash.</param>
        /// <exception cref="ArgumentNullException"/>
        public BooruAuth(string userId, string passwordHash)
        {
            UserId = userId ?? throw new ArgumentNullException(nameof(userId));
            PasswordHash = passwordHash ?? throw new ArgumentNullException(nameof(passwordHash));
        }

        /// <summary>
        /// Gets the user's ID.
        /// </summary>
        public string UserId { get; }

        /// <summary>
        /// Gets the user's password hash.
        /// </summary>
        public string PasswordHash { get; }
    }
}
