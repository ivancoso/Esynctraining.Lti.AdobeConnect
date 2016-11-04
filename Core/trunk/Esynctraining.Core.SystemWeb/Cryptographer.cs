namespace Esynctraining.Core.Utils
{
    using System;
    using System.Security.Cryptography;
    using System.Text;
    using System.Web.Configuration;
    using System.Web.Security;

    /// <summary>
    /// The cryptographer.
    /// </summary>
    public static class Cryptographer
    {
        #region Constants

        /// <summary>
        /// The salt length.
        /// </summary>
        private const int SaltLength = 16;

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The generate password salt.
        /// </summary>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public static string GeneratePasswordSalt()
        {
            var rng = new RNGCryptoServiceProvider();
            var salt = new byte[SaltLength];
            rng.GetBytes(salt);

            return Convert.ToBase64String(salt);
        }

        /// <summary>
        /// Generates secure hash for the specified string. Use this method whenever you need one
        /// way encryption in the system.
        /// </summary>
        /// <param name="plainString">The plain string to generate hash for.</param>
        /// <returns>Hash for the specified string.</returns>
        public static byte[] GenerateHash(string plainString)
        {
            SHA256 hash = SHA256.Create();

            try
            {
                return hash.ComputeHash(Encoding.UTF8.GetBytes(plainString));
            }
            finally
            {
                hash.Dispose();
            }
        }

        /// <summary>
        /// The hash password.
        /// </summary>
        /// <param name="password">
        /// The password.
        /// </param>
        /// <param name="passwordSalt">
        /// The password salt.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public static string HashPassword(string password, string passwordSalt)
        {
            return FormsAuthentication.HashPasswordForStoringInConfigFile(
                passwordSalt + password, FormsAuthPasswordFormat.SHA1.ToString());
        }

        #endregion
    }
}