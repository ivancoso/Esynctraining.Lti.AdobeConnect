using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Runtime.Caching;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using EdugameCloud.Core.Business;
using EdugameCloud.Core.Business.Models;
using EdugameCloud.Lti.API.AdobeConnect;
using EdugameCloud.Lti.Core.Business.Models;
using EdugameCloud.Lti.Core.Utils;
using EdugameCloud.Lti.Domain.Entities;
using Esynctraining.AdobeConnect;
using Esynctraining.Core;
using Esynctraining.Core.Domain;
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
        private LmsUserSession _session;

        #endregion

        protected dynamic Settings { get; private set; }

        protected ILogger Logger { get; private set; }

        protected IAdobeConnectAccountService AcAccountService { get; private set; }

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

        internal LmsUserSession Session
        {
            get
            {
                if (_session == null)
                    throw new WarningMessageException(Resources.Messages.SessionTimeOut);
                return _session;
            }
            set
            {
                _session = value;
            }
        }

        internal ILmsLicense LmsCompany
        {
            get { return Session.LmsCompany; }
        }

        #region Constructors and Destructors

        public BaseController(
            LmsUserSessionModel userSessionModel,
            IAdobeConnectAccountService acAccountService,
            ApplicationSettingsProvider settings,
            ILogger logger)
        {
            this.userSessionModel = userSessionModel;
            this.AcAccountService = acAccountService;
            this.Settings = settings;
            this.Logger = logger;
        }

        #endregion
        
        protected IAdobeConnectProxy GetUserProvider()
        {
            string cacheKey = CachePolicies.Keys.UserAdobeConnectProxy(LmsCompany.Id, Session.LtiSession.LtiParam.lms_user_id);
            var provider = _cache.Get(cacheKey) as IAdobeConnectProxy;

            if (provider == null)
            {
                string breezeSession = LoginCurrentUser(Session);
                provider = AcAccountService.GetProvider2(new AdobeConnectAccess2(new Uri(LmsCompany.AcServer), breezeSession));

                var sessionTimeout = AcAccountService.GetAccountDetails(GetAdminProvider(LmsCompany)).SessionTimeout - 1; //-1 is to be sure 
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

                var ac = GetAdminProvider(lmsCompany);
                var registeredUser = ac.GetOneByPrincipalId(lmsUser.PrincipalId).PrincipalInfo.Principal;

                var MeetingSetup = IoC.Resolve<MeetingSetup>();
                string breezeToken = MeetingSetup.ACLogin(lmsCompany, param, lmsUser, registeredUser, ac);

                return breezeToken;
            }
            catch (Exception ex)
            {
                string errorMessage = GetOutputErrorMessage("ContentApi-LoginCurrentUser", ex);
                throw;
            }
        }

        private IAdobeConnectProxy GetAdminProvider(ILmsLicense lmsCompany)
        {
            string cacheKey = CachePolicies.Keys.CompanyLmsAdobeConnectProxy(lmsCompany.Id);

            var provider = _cache.Get(cacheKey) as IAdobeConnectProxy;
            if (provider == null)
            {
                provider = AcAccountService.GetProvider(new AdobeConnectAccess(new Uri(lmsCompany.AcServer), lmsCompany.AcUsername, lmsCompany.AcPassword), true);
                var sessionTimeout = AcAccountService.GetAccountDetails(provider).SessionTimeout - 1; //-1 is to be sure 
                _cache.Set(cacheKey, provider, DateTimeOffset.Now.AddMinutes(sessionTimeout));
            }

            return provider;
        }

        protected string GetOutputErrorMessage(string methodName, Exception ex)
        {
            string lmsInfo = (LmsCompany != null)
                ? string.Format(" LmsCompany ID: {0}. Lms License Title: {1}. Lms Domain: {2}. AC Server: {3}.", 
                LmsCompany.Id, LmsCompany.Title, LmsCompany.LmsDomain, LmsCompany.AcServer)
                : string.Empty;

            Logger.Error(methodName + lmsInfo, ex);

            var forcePassMessage = ex as IUserMessageException;
            if (forcePassMessage != null)
                return ex.Message;

            return IsDebug
                ? Resources.Messages.ExceptionOccured + ex.ToString()
                : Resources.Messages.ExceptionMessage;
        }

    }

    internal class LmsAuthorizeBaseAttribute : ActionFilterAttribute
    {
        private static readonly string HeaderName = "Authorization";
        private static readonly string ltiAuthScheme = "lti ";
        private static readonly string apiAuthScheme = "ltiapi ";

        private readonly LmsUserSessionModel _userSessionModel;
        private readonly ILogger _logger;
        
        private LanguageModel LanguageModel => IoC.Resolve<LanguageModel>();


        public LmsAuthorizeBaseAttribute()
        {
            _userSessionModel = IoC.Resolve<LmsUserSessionModel>();
            _logger = IoC.Resolve<ILogger>();
        }


        public override void OnActionExecuting(HttpActionContext filterContext)
        {
            string mode;
            Guid sessionKey = FetchToken(filterContext.ControllerContext.Request, out mode);

            if (sessionKey != Guid.Empty)
            {
                LmsUserSession session = GetReadOnlySession(sessionKey);
                if (session == null)
                {
                    filterContext.Response =
                    filterContext.ControllerContext.Request.CreateResponse<OperationResult>(OperationResult.Error(Resources.Messages.SessionTimeOut));
                }
                else
                {
                    HttpResponseMessage notAllowedResult;
                    var allowed = IsAllowed(session, out notAllowedResult);

                    if (!allowed)
                    {
                        filterContext.Response = notAllowedResult;
                    }
                    else
                    {
                        var api = filterContext.ControllerContext.Controller as BaseController;
                        api.Session = session;
                    }
                }
            }
            else
            {
                filterContext.Response =
                    filterContext.ControllerContext.Request.CreateResponse<OperationResult>(OperationResult.Error("Necessary arguments were not provided."));
            }
            base.OnActionExecuting(filterContext);
        }


        protected virtual bool IsAllowed(LmsUserSession session, out HttpResponseMessage notAllowedResult)
        {
            notAllowedResult = null;
            return true;
        }

        protected LmsUserSession GetReadOnlySession(Guid key)
        {
            var session = _userSessionModel.GetByIdWithRelated(key).Value;
            if (session == null)
            {
                _logger.WarnFormat("LmsUserSession not found. Key: {0}.", key);
                return null;
            }
            System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo(LanguageModel.GetById(session.LmsCompany.LanguageId).TwoLetterCode);
            return session;
        }

        private static Guid FetchToken(HttpRequestMessage req, out string mode)
        {
            string authHeader = null;
            IEnumerable<string> values;
            if (req.Headers.TryGetValues(HeaderName, out values))
            {
                authHeader = values.First();
            }

            if ((authHeader != null) && authHeader.StartsWith(ltiAuthScheme, StringComparison.OrdinalIgnoreCase))
            {
                string token = authHeader.Substring(ltiAuthScheme.Length).Trim();
                Guid uid;
                if (Guid.TryParse(token, out uid))
                {
                    mode = ltiAuthScheme;
                    return uid;
                }
            }

            //if ((authHeader != null) && authHeader.StartsWith(apiAuthScheme, StringComparison.OrdinalIgnoreCase))
            //{
            //    string token = authHeader.Substring(apiAuthScheme.Length).Trim();

            //    Guid uid;
            //    if (Guid.TryParse(token, out uid))
            //    {
            //        mode = apiAuthScheme;
            //        return uid;
            //    }
            //}

            mode = null;
            return Guid.Empty;
        }

    }

}
