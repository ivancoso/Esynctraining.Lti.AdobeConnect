namespace EdugameCloud.Core.Authentication
{
    using System.Security.Principal;

    using EdugameCloud.Core.Business.Models;
    using EdugameCloud.Core.Domain.Entities;

    using Esynctraining.Core.Authentication;
    using Esynctraining.Core.Utils;

    using Weborb.Message;
    using Weborb.Security;

    /// <summary>
    /// The web orb authentication handler.
    /// </summary>
    public class WebOrbAuthenticationHandler : IAuthenticationHandler
    {
        /// <summary>
        /// The check credentials.
        /// </summary>
        /// <param name="email">
        /// The email.
        /// </param>
        /// <param name="passwordHash">
        /// The password hash.
        /// </param>
        /// <param name="message">
        /// The message.
        /// </param>
        /// <returns>
        /// The <see cref="IPrincipal"/>.
        /// </returns>
        /// <exception cref="WebORBAuthenticationException">
        /// In case authentication didn't go well
        /// </exception>
        public IPrincipal CheckCredentials(string email, string passwordHash, Request message)
        {
            User user;
            if (!string.IsNullOrWhiteSpace(email)
                && !string.IsNullOrWhiteSpace(passwordHash)
                && (user = IoC.Resolve<UserModel>().GetOneByEmail(email).Value) != null
                && (user.ValidatePasswordHash(passwordHash) || user.ValidatePassword(passwordHash)))
            {
                return new WebOrbPrincipal(new EdugameCloudIdentity(user));
            }

            throw new WebORBAuthenticationException("Invalid username or password");
        }
    }
}
