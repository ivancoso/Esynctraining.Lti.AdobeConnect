using System;
using System.Web.Mvc;
using Esynctraining.Core.Logging;
using EdugameCloud.Lti.API.AdobeConnect;
using EdugameCloud.Lti.Constants;
using EdugameCloud.Lti.Core;
using EdugameCloud.Lti.Core.Business.Models;
using EdugameCloud.Lti.Domain.Entities;
using Esynctraining.Core.Providers;

namespace EdugameCloud.Lti.Controllers
{
    public class BaseController : Controller
    {
        protected const string ExceptionMessage = "An exception is occured. Try again later or contact your administrator.";


        #region Fields

        private static bool? isDebug;

        private readonly LmsUserSessionModel userSessionModel;
        
        #endregion

        /// <summary>
        ///   Gets the settings.
        /// </summary>
        public dynamic Settings { get; private set; }

        protected ILogger logger { get; private set; }

        protected IAdobeConnectAccountService acAccountService { get; private set; }

        protected bool IsDebug
        {
            get
            {
                if (isDebug.HasValue)
                {
                    return isDebug.Value;
                }

                bool val;
                isDebug = bool.TryParse(this.Settings.IsDebug, out val) && val;
                return isDebug.Value;
            }
        }

        #region Constructors and Destructors

        public BaseController(
            LmsUserSessionModel userSessionModel,
            IAdobeConnectAccountService acAccountService, 
            ApplicationSettingsProvider settings, 
            ILogger logger)
        {
            this.userSessionModel = userSessionModel;
            this.acAccountService = acAccountService;
            this.Settings = settings;
            this.logger = logger;
        }

        #endregion

        protected LmsUserSession GetSession(string key)
        {
            Guid uid;
            var session = Guid.TryParse(key, out uid) ? this.userSessionModel.GetByIdWithRelated(uid).Value : null;

            if (session == null)
            {
                logger.WarnFormat("LmsUserSession not found. Key: {0}.", key);
                throw new WarningMessageException("Session timed out. Please refresh the page.");
            }

            return session;
        }

        protected IAdobeConnectProxy GetAdobeConnectProvider(ILmsLicense lmsCompany)
        {
            IAdobeConnectProxy provider = null;
            if (lmsCompany != null)
            {
                provider = this.Session[string.Format(LtiSessionKeys.ProviderSessionKeyPattern, lmsCompany.Id)] as IAdobeConnectProxy;
                if (provider == null)
                {
                    provider = acAccountService.GetProvider(lmsCompany);
                    this.Session[string.Format(LtiSessionKeys.ProviderSessionKeyPattern, lmsCompany.Id)] = provider;
                }
            }

            return provider;
        }

        protected string GetOutputErrorMessage(string methodName, Exception ex)
        {
            logger.Error(methodName, ex);
            return IsDebug
                ? "An exception is occured. " + ex.ToString()
                : ExceptionMessage;
        }

        protected string GetOutputErrorMessage(string originalErrorMessage)
        {
            logger.Error(originalErrorMessage);
            return IsDebug
                ? "An exception is occured. " + originalErrorMessage
                : ExceptionMessage;
        }

        // TODO: check!!
        private void RedirectToError(string errorText)
        {
            this.Response.Clear();
            this.Response.Write(string.Format("{{ \"isSuccess\": \"false\", \"message\": \"{0}\" }}", errorText));
            this.Response.End();
        }

    }

}
