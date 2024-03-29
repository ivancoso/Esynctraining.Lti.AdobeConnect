namespace Esynctraining.Core.Business.Models
{
    using System;
    using System.Web;
    using System.Web.Security;
    using Esynctraining.Core.Extensions;

    /// <summary>
    /// The authentication model.
    /// </summary>
    public class AuthenticationModel 
    {
        public static string DefaultUrl => FormsAuthentication.DefaultUrl;

        #region Public Methods and Operators

        [Obsolete]
        public static string CreateRandomPassword()
        {
            return CreateRandomPassword(8);
        }

        [Obsolete]
        public static string CreateRandomPassword(int passwordLength)
        {
            const string AllowedChars = "abcdefghijkmnopqrstuvwxyzABCDEFGHJKLMNOPQRSTUVWXYZ0123456789";
            return CreateRandomPassword(passwordLength, AllowedChars);
        }

        [Obsolete]
        public static string CreateRandomPassword(int passwordLength, string allowedChars)
        {
            var randNum = new Random();
            var chars = new char[passwordLength];
            int allowedCharCount = allowedChars.Length;

            for (int i = 0; i < passwordLength; i++)
            {
                chars[i] = allowedChars[(int)(allowedCharCount * randNum.NextDouble())];
            }

            return new string(chars);
        }

        /// <summary>
        /// The parse cookie value.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <param name="idEmail">
        /// The id email.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public static bool ParseCookieValue(string value, out Tuple<int, string> idEmail)
        {
            idEmail = null;
            var result = value.Split("G".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            if (result.Length == 3)
            {
                int id;
                Guid uid;
                bool idIsValid = int.TryParse(result[0], out id);
                bool guidIsValid = Guid.TryParseExact(result[2], "N", out uid);
                if (idIsValid && guidIsValid)
                {
                    idEmail = new Tuple<int, string>(id, result[1]);
                }

                return idIsValid && guidIsValid;
            }

            return false;
        }

        /// <summary>
        /// The get current user.
        /// </summary>
        /// <param name="getByEmail">
        /// The get By Email.
        /// </param>
        /// <returns>
        /// The <see cref="object"/>.
        /// </returns>
        public virtual object GetCurrentUser(Func<string, object> getByEmail)
        {
            return this.GetCurrentUser(getByEmail, "CurrentUser");
        }

        /// <summary>
        /// The get current user.
        /// </summary>
        /// <param name="getByEmail">
        /// The get By Email.
        /// </param>
        /// <param name="currentUserKey">
        /// The current user key.
        /// </param>
        /// <returns>
        /// The <see cref="Object"/>.
        /// </returns>
        public virtual object GetCurrentUser(Func<string, object> getByEmail, string currentUserKey)
        {
            if (HttpContext.Current != null && HttpContext.Current.User != null
                && HttpContext.Current.User.Identity.IsAuthenticated)
            {
                var currentUser = HttpContext.Current.With(x => x.Items[currentUserKey]);
                if (currentUser == null)
                {
                    string email = HttpContext.Current.User.Identity.Name;
                    currentUser = getByEmail.Invoke(email);
                    HttpContext.Current.Items[currentUserKey] = currentUser;
                }

                if (currentUser != null)
                {
                    return currentUser;
                }

                try
                {
                    this.SignOut();
                }
                catch (Exception)
                {
                }
            }

            return null;
        }

        /// <summary>
        /// The set auth cookie.
        /// </summary>
        /// <param name="userName">
        /// The user name.
        /// </param>
        /// <param name="createPersistentCookie">
        /// The create persistent cookie.
        /// </param>
        public virtual void SetAuthCookie(string userName, bool createPersistentCookie)
        {
            FormsAuthentication.SetAuthCookie(userName, createPersistentCookie);
        }

        /// <summary>
        ///     The sign out.
        /// </summary>
        public virtual void SignOut()
        {
            FormsAuthentication.SignOut();
        }

        #endregion
    }
}