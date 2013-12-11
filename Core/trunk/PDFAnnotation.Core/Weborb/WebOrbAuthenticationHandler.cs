namespace PDFAnnotation.Core.Weborb
{
    using System.Security.Principal;

    using Esynctraining.Core.Authentication;
    using Esynctraining.Core.Utils;

    using global::Weborb.Message;
    using global::Weborb.Security;

    using PDFAnnotation.Core.Authentication;
    using PDFAnnotation.Core.Business.Models;
    using PDFAnnotation.Core.Domain.Entities;

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
            Contact user;
            if (!string.IsNullOrWhiteSpace(email)
                && !string.IsNullOrWhiteSpace(passwordHash)
                && (user = IoC.Resolve<ContactModel>().GetOneByEmail(email).Value) != null
                && (user.ValidatePasswordHash(passwordHash) || user.ValidatePassword(passwordHash)))
            {
                return new WebOrbPrincipal(new PDFAnnotationIdentity(user));
            }

            throw new WebORBAuthenticationException("Invalid username or password");
        }
    }
}
