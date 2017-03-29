using System;
using System.Web.Mvc;
using EdugameCloud.Core.Business.Models;
using EdugameCloud.Lti.Core.Business.Models;
using EdugameCloud.Lti.Domain.Entities;
using Esynctraining.AdobeConnect;
using Esynctraining.Core;
using Esynctraining.Core.Logging;
using Esynctraining.Core.Providers;
using Esynctraining.Core.Utils;

namespace EdugameCloud.Lti.Host.Areas.Reports.Controllers
{
    public abstract class BaseController : Controller
    {
        private static bool? isDebug;

        private readonly LmsUserSessionModel userSessionModel;


        protected dynamic Settings { get; private set; }

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

        private LanguageModel LanguageModel => IoC.Resolve<LanguageModel>();

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
        

        protected LmsUserSession GetReadOnlySession(string key)
        {
            Guid uid;
            var session = Guid.TryParse(key, out uid) ? this.userSessionModel.GetByIdWithRelated(uid).Value : null;

            if (session == null)
            {
                logger.WarnFormat("LmsUserSession not found. Key: {0}.", key);
                throw new Core.WarningMessageException(Resources.Messages.SessionTimeOut);
            }

            System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo(LanguageModel.GetById(session.LmsCompany.LanguageId).TwoLetterCode);

            return session;
        }

        protected IAdobeConnectProxy GetAdobeConnectProvider(ILmsLicense lmsCompany)
        {
            IAdobeConnectProxy provider = null;
            if (lmsCompany != null)
            {
                provider = acAccountService.GetProvider(new AdobeConnectAccess(new Uri(lmsCompany.AcServer), lmsCompany.AcUsername, lmsCompany.AcPassword), true);
            }

            return provider;
        }

        //protected string GetOutputErrorMessage(string methodName, Exception ex)
        //{
        //    logger.Error(methodName, ex);
        //    return IsDebug
        //        ? Resources.Messages.ExceptionOccured + ex.ToString()
        //        : Resources.Messages.ExceptionMessage;
        //}

        protected string GetOutputErrorMessage(string originalErrorMessage)
        {
            logger.Error(originalErrorMessage);
            return IsDebug
                ? Resources.Messages.ExceptionOccured + originalErrorMessage
                : Resources.Messages.ExceptionMessage;
        }

        protected string GetOutputErrorMessage(string methodName, LmsCompany credentials, Exception ex)
        {
            string lmsInfo = (credentials != null)
                ? string.Format(" LmsCompany ID: {0}. Lms License Title: {1}. Lms Domain: {2}. AC Server: {3}.", credentials.Id, credentials.Title, credentials.LmsDomain, credentials.AcServer)
                : string.Empty;

            logger.Error(methodName + lmsInfo, ex);

            var forcePassMessage = ex as IUserMessageException;
            if (forcePassMessage != null)
                return ex.Message;

            return IsDebug
                ? Resources.Messages.ExceptionOccured + ex.ToString()
                : Resources.Messages.ExceptionMessage;
        }

    }

}
