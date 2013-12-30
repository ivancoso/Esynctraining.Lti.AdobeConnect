namespace Esynctraining.Core.Authentication
{
    using System;
    using System.Net;
    using System.Security.Principal;
    using System.ServiceModel.Web;

    using Esynctraining.Core.Utils;

    using WcfRestContrib.ServiceModel.Dispatcher;

    using WebException = WcfRestContrib.ServiceModel.Web.Exceptions.WebException;

    /// <summary>
    /// The WCF authentication handler.
    /// </summary>
    public class WcfAuthenticationHandler : IWebAuthenticationHandler
    {
        #region Public Methods and Operators

        /// <summary>
        /// The authenticate.
        /// </summary>
        /// <param name="request">
        /// The request.
        /// </param>
        /// <param name="response">
        /// The response.
        /// </param>
        /// <param name="parameters">
        /// The parameters.
        /// </param>
        /// <param name="validatorType">
        /// The validator type.
        /// </param>
        /// <param name="secure">
        /// The secure.
        /// </param>
        /// <param name="requiresTransportLayerSecurity">
        /// The requires transport layer security.
        /// </param>
        /// <param name="source">
        /// The source.
        /// </param>
        /// <returns>
        /// The <see cref="IIdentity"/>.
        /// </returns>
        /// <exception cref="WebException">
        /// Exception with the status if authentication fails
        /// </exception>
        public IIdentity Authenticate(
            IncomingWebRequestContext request, 
            OutgoingWebResponseContext response, 
            object[] parameters, 
            Type validatorType, 
            bool secure, 
            bool requiresTransportLayerSecurity, 
            string source)
        {
            var authentivationValidator = IoC.Resolve(validatorType) as IAuthenticationValidator;
            var isExpired = false;
            if (authentivationValidator != null)
            {
                var result = authentivationValidator.Validate(request, out isExpired);
                if (result != null)
                {
                    return result;
                }
            }
            if (isExpired)
            {
                throw new WebException(HttpStatusCode.BadRequest, "Your token is expired, request a new one");
            }
            throw new WebException(HttpStatusCode.Unauthorized, "Your request can not be authorized, sorry");
        }

        #endregion
    }
}