﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Castle.Core.Logging;
using EdugameCloud.Lti.API.AdobeConnect;
using EdugameCloud.Lti.Constants;
using EdugameCloud.Lti.Core.Business.Models;
using EdugameCloud.Lti.Domain.Entities;
using Esynctraining.AC.Provider;
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

        protected MeetingSetup meetingSetup { get; private set; }

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
            MeetingSetup meetingSetup, 
            ApplicationSettingsProvider settings, 
            ILogger logger)
        {
            this.userSessionModel = userSessionModel;
            this.meetingSetup = meetingSetup;
            this.Settings = settings;
            this.logger = logger;
        }

        #endregion

        protected LmsUserSession GetSession(string key)
        {
            Guid uid;
            var session = Guid.TryParse(key, out uid) ? this.userSessionModel.GetByIdWithRelated(uid).Value : null;

            if (this.IsDebug && session == null)
            {
                session = this.userSessionModel.GetByIdWithRelated(Guid.Empty).Value;
            }

            if (session == null)
            {
                this.RedirectToError("Session timed out. Please refresh the page.");
                return null;
            }

            return session;
        }

        protected IAdobeConnectProxy GetAdobeConnectProvider(LmsCompany lmsCompany)
        {
            IAdobeConnectProxy provider = null;
            if (lmsCompany != null)
            {
                provider = this.Session[string.Format(LtiSessionKeys.ProviderSessionKeyPattern, lmsCompany.Id)] as IAdobeConnectProxy;
                if (provider == null)
                {
                    provider = this.meetingSetup.GetProvider(lmsCompany);
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
