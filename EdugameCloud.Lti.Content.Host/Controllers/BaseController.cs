using System;
using System.Runtime.Caching;
using System.Web.Http;
using EdugameCloud.Core.Business.Models;
using EdugameCloud.Lti.API.AdobeConnect;
using EdugameCloud.Lti.Core.Business.Models;
using EdugameCloud.Lti.Core.Utils;
using EdugameCloud.Lti.Domain.Entities;
using Esynctraining.AdobeConnect;
using Esynctraining.Core;
using Esynctraining.Core.Logging;
using Esynctraining.Core.Providers;
using Esynctraining.Core.Utils;
using IAdobeConnectAccountService = Esynctraining.AdobeConnect.IAdobeConnectAccountService;
using IAdobeConnectProxy = Esynctraining.AdobeConnect.IAdobeConnectProxy;

namespace EdugameCloud.Lti.Content.Host.Controllers
{
    public abstract class BaseController : ApiController
    {
        #region Fields

        protected readonly ObjectCache _cache = MemoryCache.Default;

        private static bool? isDebug;

        private readonly LmsUserSessionModel userSessionModel;

        #endregion

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

        private LanguageModel LanguageModel
        {
            get { return IoC.Resolve<LanguageModel>(); }
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

        protected IAdobeConnectProxy GetAdobeConnectProvider(LmsUserSession session)
        {
            //IAdobeConnectProxy provider = null;
            //string cacheKey = CacheKeyUtil.GetKey(session);
            //Esynctraining.AdobeConnect.IAdobeConnectProxy provider = _cache.Get(cacheKey) as Esynctraining.AdobeConnect.IAdobeConnectProxy;
            //provider = acAccountService.GetProvider(new AdobeConnectAccess(lmsCompany.AcServer, lmsCompany.AcUsername, lmsCompany.AcPassword), true);
            //return provider;
            
            string cacheKey = CacheKeyUtil.GetKey(session);
            Esynctraining.AdobeConnect.IAdobeConnectProxy provider = _cache.Get(cacheKey) as Esynctraining.AdobeConnect.IAdobeConnectProxy;

            if (provider == null)
            {
                string breezeSession = LoginCurrentUser(session);
                provider = acAccountService.GetProvider2(new AdobeConnectAccess2(session.LmsCompany.AcServer, breezeSession));

                var sessionTimeout = acAccountService.GetAccountDetails(provider).SessionTimeout - 1; //-1 is to be sure 
                _cache.Set(cacheKey, provider, DateTimeOffset.Now.AddMinutes(sessionTimeout));
            }

            return provider;
        }

        private string LoginCurrentUser(LmsUserSession session)
        {
            LmsCompany lmsCompany = null;
            try
            {
                lmsCompany = session.LmsCompany;
                var param = session.LtiSession.LtiParam;
                var LmsUserModel = IoC.Resolve<LmsUserModel>();
                var lmsUser = LmsUserModel.GetOneByUserIdAndCompanyLms(param.lms_user_id, lmsCompany.Id).Value;
                if (lmsUser == null)
                {
                    throw new Core.WarningMessageException($"No user with id {param.lms_user_id} found in the database.");
                }

                if (lmsUser.PrincipalId == null)
                {
                    throw new Core.WarningMessageException("User doesn't have account in Adobe Connect.");
                }

                var ac = this.GetAdobeConnectProvider(session);
                var registeredUser = ac.GetOneByPrincipalId(lmsUser.PrincipalId).PrincipalInfo.Principal;

                var MeetingSetup = IoC.Resolve<MeetingSetup>();
                string breezeToken = MeetingSetup.ACLogin(lmsCompany, param, lmsUser, registeredUser, ac);

                return breezeToken;
            }
            catch (Exception ex)
            {
                string errorMessage = GetOutputErrorMessage("ContentApi-LoginCurrentUser", lmsCompany, ex);
                throw;
            }
        }

        protected string GetOutputErrorMessage(string methodName, Exception ex)
        {
            logger.Error(methodName, ex);
            return IsDebug
                ? Resources.Messages.ExceptionOccured + ex.ToString()
                : Resources.Messages.ExceptionMessage;
        }

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
