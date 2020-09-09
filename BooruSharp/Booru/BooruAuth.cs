using System;
using System.Diagnostics;

namespace BooruSharp.Booru
{
    /// <summary>
    /// Represents authentication credentials.
    /// </summary>
    [DebuggerDisplay("ID: {UserId} Password: {PasswordHash}")]
    public class BooruAuth : IEquatable<BooruAuth>
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

        /// <inheritdoc/>
        public override bool Equals(object obj)
        {
            return Equals(obj as BooruAuth);
        }

        /// <inheritdoc/>
        public bool Equals(BooruAuth other)
        {
            return other != null && UserId == other.UserId && PasswordHash == other.PasswordHash;
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            int hashCode = -1555372705;
            hashCode = hashCode * -1521134295 + UserId.GetHashCode();
            hashCode = hashCode * -1521134295 + PasswordHash.GetHashCode();
            return hashCode;
        }
    }
}
