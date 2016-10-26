﻿using System;
using System.Runtime.Caching;
using System.Web.Http;
using EdugameCloud.Core.Business;
using EdugameCloud.Core.Business.Models;
using EdugameCloud.Lti.Core.Business.Models;
using EdugameCloud.Lti.Domain.Entities;
using Esynctraining.AdobeConnect;
using Esynctraining.Core;
using Esynctraining.Core.Logging;
using Esynctraining.Core.Providers;
using Esynctraining.Core.Utils;

namespace EdugameCloud.Lti.Mp4.Host.Controllers
{
    public abstract class BaseController : ApiController
    {
        #region Fields

        private static bool? isDebug;

        private readonly ObjectCache _cache = MemoryCache.Default;
        private readonly LmsUserSessionModel userSessionModel;

        #endregion

        protected dynamic Settings { get; }

        protected ILogger Logger { get; }

        protected IAdobeConnectAccountService acAccountService { get; }

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
            this.Logger = logger;
        }

        #endregion
        

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

        protected IAdobeConnectProxy GetAdobeConnectProvider(LmsUserSession session)
        {
            var lmsCompany = session.LmsCompany;
            string cacheKey = CachePolicies.Keys.CompanyLmsAdobeConnectProxy(lmsCompany.Id);

            var provider = _cache.Get(cacheKey) as IAdobeConnectProxy;
            if (provider == null)
            {
                provider = acAccountService.GetProvider(new AdobeConnectAccess(lmsCompany.AcServer, lmsCompany.AcUsername, lmsCompany.AcPassword), true);
                var sessionTimeout = acAccountService.GetAccountDetails(provider).SessionTimeout - 1; //-1 is to be sure 
                _cache.Set(cacheKey, provider, DateTimeOffset.Now.AddMinutes(sessionTimeout));
            }

            return provider;
        }

        protected string GetOutputErrorMessage(string methodName, Exception ex)
        {
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

            var forcePassMessage = ex as IUserMessageException;
            if (forcePassMessage != null)
                return ex.Message;

            return IsDebug
                ? Resources.Messages.ExceptionOccured + ex.ToString()
                : Resources.Messages.ExceptionMessage;
        }

    }

}
