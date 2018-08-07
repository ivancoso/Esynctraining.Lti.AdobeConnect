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

        #region Constructors and Destructors

        protected BaseApiController(
            ApplicationSettingsProvider settings,
            ILogger logger
        )
        {
            Settings = settings;
            Logger = logger;
        }

        #endregion

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
