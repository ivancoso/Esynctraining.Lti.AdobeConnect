using System;
using EdugameCloud.Core.Business;
using EdugameCloud.Lti.API.AdobeConnect;
using EdugameCloud.Lti.Domain.Entities;
using EdugameCloud.Lti.Resources;
using Esynctraining.AdobeConnect;
using Esynctraining.Core;
using Esynctraining.Core.Caching;
using Esynctraining.Core.Logging;
using Esynctraining.Core.Providers;
using Esynctraining.Core.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace EdugameCloud.Lti.Api.Controllers
{
    public abstract class BaseApiController : ControllerBase
    {
        #region Fields
        private static readonly IMemoryCache _acCache = IoC.Resolve<IMemoryCache>();
        private static bool? isDebug;

        private LmsUserSession _session;

        #endregion

        protected dynamic Settings { get; }

        protected ILogger Logger { get; }

        protected ICache Cache { get; }

        protected API.AdobeConnect.IAdobeConnectAccountService acAccountService { get; }

        private readonly Esynctraining.AdobeConnect.IAdobeConnectAccountService BaseAcAccountService = IoC.Resolve<Esynctraining.AdobeConnect.IAdobeConnectAccountService>();

        protected bool IsDebug
        {
            get
            {
                if (isDebug.HasValue)
                {
                    return isDebug.Value;
                }

                bool val;
                isDebug = bool.TryParse(Settings.IsDebug, out val) && val;
                return isDebug.Value;
            }
        }

        public LmsUserSession Session
        {
            get
            {
                if (_session == null)
                    throw new WarningMessageException(Messages.SessionTimeOut);
                return _session;
            }
            set
            {
                _session = value;
            }
        }

        /// <summary>
        /// TRICK: do not use in most of your code.
        /// </summary>
        protected LmsUserSession SessionSave
        {
            get { return _session; }
            set { _session = value; }
        }

        public ILmsLicense LmsCompany { get; set; }

        public int CourseId { get; set; }

        private MeetingSetup MeetingSetup => IoC.Resolve<MeetingSetup>();

        #region Constructors and Destructors

        protected BaseApiController(
            API.AdobeConnect.IAdobeConnectAccountService acAccountService,
            ApplicationSettingsProvider settings,
            ILogger logger, 
            ICache cache
        )
        {
            this.acAccountService = acAccountService;
            Settings = settings;
            Logger = logger;
            Cache = cache;
        }

        #endregion

        protected IAdobeConnectProxy GetUserProvider()
        {
            string cacheKey = CachePolicies.Keys.UserAdobeConnectProxy(LmsCompany.Id, Session.LtiSession.LtiParam.lms_user_id);
            var provider = _acCache.Get(cacheKey) as IAdobeConnectProxy;

            if (provider == null)
            {
                IAdobeConnectProxy adminProvider;
                string breezeSession = LoginCurrentUser(out adminProvider);
                var acService = BaseAcAccountService;
                provider = acService.GetProvider2(new AdobeConnectAccess2(new Uri(LmsCompany.AcServer), breezeSession));

                var sessionTimeout = acService.GetAccountDetails(adminProvider).SessionTimeout - 1; //-1 is to be sure 
                _acCache.Set(cacheKey, provider, DateTimeOffset.Now.AddMinutes(sessionTimeout));
            }

            return provider;
        }

        private string LoginCurrentUser(out IAdobeConnectProxy adminProvider)
        {
            try
            {
                var param = Session.LtiSession.LtiParam;
                var lmsUser = Session.LmsUser;
                if (lmsUser.PrincipalId == null)
                {
                    throw new WarningMessageException("User doesn't have account in Adobe Connect.");
                }

                adminProvider = GetAdminProvider();
                string breezeToken = MeetingSetup.ACLogin(LmsCompany, param, lmsUser, adminProvider).BreezeSession;

                return breezeToken;
            }
            catch (Exception ex)
            {
                //string errorMessage = GetOutputErrorMessage("ContentApi-LoginCurrentUser", ex);
                throw;
            }
        }

        protected IAdobeConnectProxy GetAdminProvider()
        {
            return GetAdminProvider(LmsCompany);
        }

        private IAdobeConnectProxy GetAdminProvider(ILmsLicense lmsCompany)
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
        
        protected string GetOutputErrorMessage(string originalErrorMessage)
        {
            Logger.Error(originalErrorMessage);
            return IsDebug
                ? Messages.ExceptionOccured + originalErrorMessage
                : Messages.ExceptionMessage;
        }

        protected string GetOutputErrorMessage(string methodName, Exception ex)
        {
            var credentials = LmsCompany;
            string lmsInfo = (credentials != null)
                ? string.Format(" LmsCompany ID: {0}. Lms License Title: {1}. Lms Domain: {2}. AC Server: {3}.", credentials.Id, credentials.Title, credentials.LmsDomain, credentials.AcServer)
                : string.Empty;

            Logger.Error(methodName + lmsInfo, ex);

            var forcePassMessage = ex as IUserMessageException;
            if (forcePassMessage != null)
                return ex.Message;

            return IsDebug
                ? Messages.ExceptionOccured + ex.ToString()
                : Messages.ExceptionMessage;
        }

    }

}
