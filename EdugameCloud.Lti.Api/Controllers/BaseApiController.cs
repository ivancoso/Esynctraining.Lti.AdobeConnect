﻿using System;
using EdugameCloud.Core.Business;
using EdugameCloud.Core.Business.Models;
using EdugameCloud.Lti.Core.Business.Models;
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

        internal LmsUserSession Session
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
        internal LmsUserSession SessionSave
        {
            get { return _session; }
            set { _session = value; }
        }

        internal ILmsLicense LmsCompany { get; set; }

        internal int CourseId { get; set; }

        #region Constructors and Destructors

        public BaseApiController(
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
