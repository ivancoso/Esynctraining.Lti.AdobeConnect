using System;
using System.Threading.Tasks;
using Esynctraining.Core;
using Esynctraining.Core.Logging;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace Esynctraining.Lti.Zoom.Api.Host.Controllers
{
    public class ErrorController : Controller
    {
        private readonly ILogger _logger;

        public ErrorController(ILogger logger)
        {
            _logger = logger;
        }

        public async Task<ActionResult> Index()
        {
            var exceptionFeature = HttpContext.Features.Get<IExceptionHandlerPathFeature>();

            if (exceptionFeature != null)
            {
                // Get which route the exception occurred at
                string routeWhereExceptionOccurred = exceptionFeature.Path;

                // Get the exception that occurred
                Exception exception = exceptionFeature.Error;
                if (exception is IUserMessageException)
                {
                    return Content(exception.Message);
                }
                else
                {
                    _logger.Error("Unexpected error occurred", exception);
                }

                // TODO: Do something with the exception
                // Log it with Serilog?
                // Send an e-mail, text, fax, or carrier pidgeon?  Maybe all of the above?
                // Whatever you do, be careful to catch any exceptions, otherwise you'll end up with a blank page and throwing a 500
            }

            return Content("Unexpected error occurred. Please contact support");

        }
    }
}