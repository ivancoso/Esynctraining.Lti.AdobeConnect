namespace EdugameCloud.Core.Authentication
{
    using System.IdentityModel.Selectors;
    using System.IdentityModel.Tokens;
    using System.Web;

    using EdugameCloud.Core.Business.Models;
    using EdugameCloud.Core.Domain.Entities;

    using Esynctraining.Core.Authentication;
    using Esynctraining.Core.Utils;

    /// <summary>
    ///     The default authentication validator.
    /// </summary>
    public class EdugameAuthenticationValidator : UserNamePasswordValidator
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="EdugameAuthenticationValidator"/> class.
        /// </summary>
        public EdugameAuthenticationValidator()
        {
        }

        #endregion

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
            User user;
            if (!string.IsNullOrWhiteSpace(userName)
                && !string.IsNullOrWhiteSpace(password)
                && (user = IoC.Resolve<UserModel>().GetOneByEmail(userName).Value) != null
                && (user.ValidatePasswordHash(password) || user.ValidatePassword(password)))
            {
                HttpContext.Current.User = new WebOrbPrincipal(new EdugameCloudIdentity(user));
                return;
            }

            throw new SecurityTokenValidationException();
        }

        #endregion
    }
}