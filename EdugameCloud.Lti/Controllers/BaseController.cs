using System;
using System.Runtime.Caching;
using System.Web.Mvc;
using EdugameCloud.Core.Business;
using EdugameCloud.Core.Business.Models;
using EdugameCloud.Lti.Core.Business.Models;
using EdugameCloud.Lti.Domain.Entities;
using Esynctraining.AdobeConnect;
using Esynctraining.Core;
using Esynctraining.Core.Caching;
using Esynctraining.Core.Logging;
using Esynctraining.Core.Providers;
using Esynctraining.Core.Utils;

namespace EdugameCloud.Lti.Controllers
{
    public class BaseController : Controller
    {
        #region Fields

        private static readonly ObjectCache _acCache = MemoryCache.Default;
        private static bool? isDebug;

        private readonly LmsUserSessionModel userSessionModel;

        #endregion
        
        protected dynamic Settings { get; }

        protected ILogger Logger { get; }

        protected ICache Cache { get; }

        protected API.AdobeConnect.IAdobeConnectAccountService acAccountService { get; }

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
            API.AdobeConnect.IAdobeConnectAccountService acAccountService,
            ApplicationSettingsProvider settings,
            ILogger logger,
            ICache cache)
        {
            this.userSessionModel = userSessionModel;
            this.acAccountService = acAccountService;
            Settings = settings;
            Logger = logger;
            Cache = cache;
        }

        #endregion

        protected override JsonResult Json(object data, string contentType,
                System.Text.Encoding contentEncoding, JsonRequestBehavior behavior)
        {
            return new JsonNetResult
            {
                Data = data,
                ContentType = contentType,
                ContentEncoding = contentEncoding,
                JsonRequestBehavior = behavior,
            };
        }


        protected LmsUserSession GetSession(string key)
        {
            Guid uid;
            var session = Guid.TryParse(key, out uid) ? this.userSessionModel.GetByIdWithRelated(uid).Value : null;

            if (session == null)
            {
                Logger.WarnFormat("LmsUserSession not found. Key: {0}.", key);
                throw new Core.WarningMessageException(Resources.Messages.SessionTimeOut);
            }

            return session;
        }

        protected LmsUserSession GetReadOnlySession(string key)
        {
            Guid uid;
            var session = Guid.TryParse(key, out uid) ? this.userSessionModel.GetByIdWithRelated(uid).Value : null;

            if (session == null)
            {
                Logger.WarnFormat("LmsUserSession not found. Key: {0}.", key);
                throw new Core.WarningMessageException(Resources.Messages.SessionTimeOut);
            }

            System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo(LanguageModel.GetById(session.LmsCompany.LanguageId).TwoLetterCode);

            return session;
        }

        protected IAdobeConnectProxy GetAdminProvider(ILmsLicense lmsCompany)
        {
            string cacheKey = CachePolicies.Keys.CompanyLmsAdobeConnectProxy(lmsCompany.Id);
            var provider = _acCache.Get(cacheKey) as IAdobeConnectProxy;
            if (provider == null)
            {
                provider = acAccountService.GetProvider(lmsCompany, login: true);
                var sessionTimeout = acAccountService.GetAccountDetails(provider, Cache).SessionTimeout - 1; //-1 is to be sure 
                _acCache.Set(cacheKey, provider, DateTimeOffset.Now.AddMinutes(sessionTimeout));
            }

            return provider;
        }

        protected string GetOutputErrorMessage(string methodName, Exception ex)
        {
            if (ex is IUserMessageException)
                return ex.Message;

            Logger.Error(methodName, ex);
            return IsDebug
                ? Resources.Messages.ExceptionOccured + ex.ToString()
                : Resources.Messages.ExceptionMessage;
        }

        protected string GetOutputErrorMessage(string originalErrorMessage)
        {
            Logger.Error(originalErrorMessage);
            return IsDebug
                ? Resources.Messages.ExceptionOccured + originalErrorMessage
                : Resources.Messages.ExceptionMessage;
        }

        protected string GetOutputErrorMessage(string methodName, LmsCompany credentials, Exception ex)
        {
            string lmsInfo = (credentials != null)
                ? string.Format(" LmsCompany ID: {0}. Lms License Title: {1}. Lms Domain: {2}. AC Server: {3}.", credentials.Id, credentials.Title, credentials.LmsDomain, credentials.AcServer)
                : string.Empty;

            Logger.Error(methodName + lmsInfo, ex);

            if (ex is IUserMessageException)
                return ex.Message;

            return IsDebug
                ? Resources.Messages.ExceptionOccured + ex.ToString()
                : Resources.Messages.ExceptionMessage;
        }

    }

}
