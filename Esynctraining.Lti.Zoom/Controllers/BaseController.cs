using System;
using System.Collections.Generic;
using System.Text;
using Esynctraining.Core.Logging;
using Microsoft.AspNetCore.Mvc;

namespace Esynctraining.Lti.Zoom.Controllers
{

    public class BaseController : Controller
    {
        protected ILogger _logger { get; }

        public BaseController(ILogger logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

    }
}
