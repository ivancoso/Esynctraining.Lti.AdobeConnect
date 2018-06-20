using Esynctraining.Core.Logging;
using Esynctraining.Core.Providers;
using Microsoft.AspNetCore.Mvc;
using System;
using Esynctraining.Lti.Zoom.Api.Services;
using Esynctraining.Lti.Zoom.Domain;
using System.Threading.Tasks;

namespace Esynctraining.Lti.Zoom.Controllers
{
    public class BaseController : Controller
    {
        protected readonly ILogger Logger;
        protected readonly dynamic Settings;
        private static bool? _isDebug;
        protected readonly UserSessionService _sessionService;

        protected bool IsDebug
        {
            get
            {
                if (_isDebug.HasValue)
                {
                    return _isDebug.Value;
                }

                bool val;
                _isDebug = bool.TryParse(Settings.IsDebug, out val) && val;
                return _isDebug.Value;
            }
        }


        public BaseController(ILogger logger, ApplicationSettingsProvider settings, UserSessionService sessionService)
        {
            Logger = logger ?? throw new ArgumentNullException(nameof(logger));
            Settings = settings ?? throw new ArgumentNullException(nameof(settings));
            _sessionService = sessionService ?? throw new ArgumentNullException(nameof(settings));
        }

        protected async Task<LmsUserSession> GetSession(string key)
        {
            Guid uid;
            var session = Guid.TryParse(key, out uid) ? await _sessionService.GetSession(uid) : null;

            if (session == null)
            {
                Logger.WarnFormat("LmsUserSession not found. Key: {0}.", key);
                throw new ArgumentException("Session timed out. Please refresh the page.");
            }

            return session;
        }

    }
}
