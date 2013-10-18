namespace Esynctraining.Core.Authentication
{
    using System.Security.Principal;
    using System.ServiceModel.Web;

    /// <summary>
    ///     The AuthenticationValidator interface.
    /// </summary>
    public interface IAuthenticationValidator
    {
        #region Public Methods and Operators

        /// <summary>
        /// The validate.
        /// </summary>
        /// <param name="request">
        /// The identity string container.
        /// </param>
        /// <param name="isExpired">
        /// The is expired.
        /// </param>
        /// <returns>
        /// The <see cref="IIdentity"/>.
        /// </returns>
        IIdentity Validate(IncomingWebRequestContext request, out bool isExpired);

        #endregion
    }
}