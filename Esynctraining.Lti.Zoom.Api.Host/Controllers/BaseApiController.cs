using System;
using Esynctraining.Core;
using Esynctraining.Core.Logging;
using Esynctraining.Core.Providers;
using Esynctraining.Lti.Lms.Common.Dto;
using Esynctraining.Lti.Zoom.Api.Dto;
using Esynctraining.Lti.Zoom.Domain;
using Microsoft.AspNetCore.Mvc;

namespace Esynctraining.Lti.Zoom.Api.Host.Controllers
{
    public abstract class BaseApiController : ControllerBase
    {
        #region Fields
        private static bool? isDebug;

        private LmsUserSession _session;

        #endregion

        protected dynamic Settings { get; }

        protected ILogger Logger { get; }

        //protected API.AdobeConnect.IAdobeConnectAccountService acAccountService { get; }

        //private readonly Esynctraining.AdobeConnect.IAdobeConnectAccountService BaseAcAccountService = IoC.Resolve<Esynctraining.AdobeConnect.IAdobeConnectAccountService>();

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
                    throw new Exception("SessionTimeOut");
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

        public LmsLicenseDto LmsLicense { get; set; }

        public string CourseId { get; set; }
        public LtiParamDTO Param { get; set; }

        //private MeetingSetup MeetingSetup => IoC.Resolve<MeetingSetup>();

        #region Constructors and Destructors

        protected BaseApiController(
            //API.AdobeConnect.IAdobeConnectAccountService acAccountService,
            ApplicationSettingsProvider settings,
            ILogger logger
        )
        {
            //this.acAccountService = acAccountService;
            Settings = settings;
            Logger = logger;
        }

        #endregion

        //protected IAdobeConnectProxy GetUserProvider()
        //{
        //    string cacheKey = CachePolicies.Keys.UserAdobeConnectProxy(LmsCompany.Id, Session.LtiSession.LtiParam.lms_user_id);
        //    var provider = _acCache.Get(cacheKey) as IAdobeConnectProxy;

        //    if (provider == null)
        //    {
        //        IAdobeConnectProxy adminProvider;
        //        string breezeSession = LoginCurrentUser(out adminProvider);
        //        var acService = BaseAcAccountService;
        //        provider = acService.GetProvider2(new AdobeConnectAccess2(new Uri(LmsCompany.AcServer), breezeSession));

        //        var sessionTimeout = acService.GetAccountDetails(adminProvider).SessionTimeout - 1; //-1 is to be sure 
        //        _acCache.Set(cacheKey, provider, DateTimeOffset.Now.AddMinutes(sessionTimeout));
        //    }

        //    return provider;
        //}

        //private string LoginCurrentUser(out IAdobeConnectProxy adminProvider)
        //{
        //    //try
        //    //{
        //    var param = Session.LtiSession.LtiParam;
        //    var lmsUser = Session.LmsUser;
        //    if (lmsUser.PrincipalId == null)
        //    {
        //        throw new WarningMessageException("User doesn't have account in Adobe Connect.");
        //    }

        //    adminProvider = GetAdminProvider();
        //    string breezeToken = MeetingSetup.ACLogin(LmsCompany, param, lmsUser, adminProvider).BreezeSession;
        //    if (breezeToken == null)
        //    {
        //        throw new WarningMessageException("There is an issue with login to Adobe Connect.");
        //    }

        //    return breezeToken;
        //    //}
        //    //catch (Exception ex)
        //    //{
        //    //    //string errorMessage = GetOutputErrorMessage("ContentApi-LoginCurrentUser", ex);
        //    //    throw;
        //    //}
        //}

        //protected IAdobeConnectProxy GetAdminProvider()
        //{
        //    return GetAdminProvider(LmsCompany);
        //}

        //private IAdobeConnectProxy GetAdminProvider(ILmsLicense lmsCompany)
        //{
        //    string cacheKey = CachePolicies.Keys.CompanyLmsAdobeConnectProxy(lmsCompany.Id);
        //    var provider = _acCache.Get(cacheKey) as IAdobeConnectProxy;
        //    if (provider == null)
        //    {
        //        provider = acAccountService.GetProvider(lmsCompany, login: true);
        //        var sessionTimeout = acAccountService.GetAccountDetails(provider, Cache).SessionTimeout - 1; //-1 is to be sure 
        //        _acCache.Set(cacheKey, provider, DateTimeOffset.Now.AddMinutes(sessionTimeout));
        //    }

        //    return provider;
        //}

        protected string GetOutputErrorMessage(string originalErrorMessage)
        {
            Logger.Error(originalErrorMessage);
            return IsDebug
                ? "ExceptionOccured" + originalErrorMessage
                : "ExceptionMessage";
        }

        protected string GetOutputErrorMessage(string methodName, Exception ex)
        {
            var license = LmsLicense;
            string lmsInfo = (license != null)
                ? $" LmsCompany ID: {license.Id}. Lms Domain: {license.Domain}."
                : string.Empty;

            Logger.Error(methodName + lmsInfo, ex);

            var forcePassMessage = ex as IUserMessageException;
            if (forcePassMessage != null)
                return ex.Message;

            return IsDebug
                ? "ExceptionOccured" + ex.ToString()
                : "ExceptionMessage";
        }
    }
}
