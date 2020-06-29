using System;
using System.Threading.Tasks;
using Esynctraining.Core;
using Esynctraining.Core.Logging;
using Esynctraining.Zoom.ApiWrapper;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace Esynctraining.Lti.Zoom.Controllers
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
                    ViewBag.Message = exception.Message;
                    return View("~/Views/Lti/LtiError.cshtml");
                }
                else if (exception is ZoomApiException)
                {
                    var ex = exception as ZoomApiException;
                    _logger.Error($"[ZoomApiException] Status:{ex.StatusDescription}, Content:{ex.Content}, ErrorMessage: {ex.ErrorMessage}", exception);
                    ViewBag.Message = $"Error received from Zoom Api: {ex.Content}";
                    return View("~/Views/Lti/LtiError.cshtml");
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

            ViewBag.Message = $"Unexpected error occurred. Please contact support";
            return View("~/Views/Lti/LtiError.cshtml");

        }
    }
}