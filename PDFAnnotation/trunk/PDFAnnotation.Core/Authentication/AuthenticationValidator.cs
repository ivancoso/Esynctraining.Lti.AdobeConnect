namespace PDFAnnotation.Core.Authentication
{
    using System.IdentityModel.Selectors;
    using System.IdentityModel.Tokens;
    using System.Web;

   // using Esynctraining.Core.Authentication;
    using Esynctraining.Core.Utils;

    using PDFAnnotation.Core.Business.Models;
    using PDFAnnotation.Core.Domain.Entities;

    /// <summary>
    ///     The default authentication validator.
    /// </summary>
    public class AuthenticationValidator : UserNamePasswordValidator
    {
        #region Public Methods and Operators

        /// <summary>
        /// The validate.
        /// </summary>
        /// <param name="userName">
        /// The user name.
        /// </param>
        /// <param name="password">
        /// The password.
        /// </param>
        public override void Validate(string userName, string password)
        {
            Contact user;
            if (!string.IsNullOrWhiteSpace(userName)
                && !string.IsNullOrWhiteSpace(password)
                && (user = IoC.Resolve<ContactModel>().GetOneByEmail(userName).Value) != null
                && (user.ValidatePasswordHash(password) || user.ValidatePassword(password)))
            {
                HttpContext.Current.User = new WebOrbPrincipal(new PDFAnnotationIdentity(user));
                return;
            }

            throw new SecurityTokenValidationException();
        }

        #endregion
    }
}