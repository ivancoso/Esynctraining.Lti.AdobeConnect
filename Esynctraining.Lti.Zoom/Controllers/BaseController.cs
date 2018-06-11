using Esynctraining.Core.Logging;
using Esynctraining.Core.Providers;
using Microsoft.AspNetCore.Mvc;
using System;

namespace Esynctraining.Lti.Zoom.Controllers
{

    public class BaseController : Controller
    {
        protected readonly ILogger _logger;
        protected readonly dynamic _settings;
        private static bool? _isDebug;

        protected bool IsDebug
        {
            get
            {
                if (_isDebug.HasValue)
                {
                    return _isDebug.Value;
                }

                bool val;
                _isDebug = bool.TryParse(_settings.IsDebug, out val) && val;
                return _isDebug.Value;
            }
        }


        public BaseController(ILogger logger, ApplicationSettingsProvider settings)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _settings = settings ?? throw new ArgumentNullException(nameof(settings));
        }

    }
}
