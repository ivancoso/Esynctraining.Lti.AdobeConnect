using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using Esynctraining.Lti.Zoom.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Esynctraining.Core.Providers;
using Esynctraining.Core.Utils;
using Esynctraining.Lti.Zoom.Extensions;
using LtiLibrary.NetCore.Common;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using ILogger = Esynctraining.Core.Logging.ILogger;

namespace Esynctraining.Lti.Zoom.Controllers
{
    public partial class LtiController : BaseController
    {

        #region Constructors and Destructors

        public LtiController(ILogger logger, ApplicationSettingsProvider settings) : base(logger, settings)
        {
            
        }

        #endregion

        public virtual async Task<ActionResult> Home()
        {
            ViewData["Message"] = "Your application description page.";
            return this.View("About");
        }

        [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1309:FieldNamesMustNotBeginWithUnderscore", Justification = "Reviewed. Suppression is OK here.")]
        [AllowAnonymous]
        public virtual async Task<ActionResult> AuthenticationCallback(
            // ReSharper disable once InconsistentNaming
            string __provider__,
            // ReSharper disable once InconsistentNaming
            string __sid__ = null,
            string code = null,
            string state = null,
            string session = null)
        {
            ViewData["Message"] = __provider__;
            return this.View("About");
        }

        public virtual async Task<ActionResult> LoginWithProvider(string provider, LtiParamDTO param)
        {
            var methodTime = Stopwatch.StartNew();
            var trace = new StringBuilder();

            try
            {
                // TRICK: to save course_id in DB;
                param.CalculateFields();
                // Parse and validate the request
                Request.CheckForRequiredLtiParameters();
                var sw = Stopwatch.StartNew();



            }
            catch (LtiException ex)
            {
                _logger.Error("Lti exception", ex);
                ViewBag.Message = $"Invalid LTI request. {ex.Message}";
                if (!string.IsNullOrEmpty(param.launch_presentation_return_url))
                {
                    ViewBag.ReturnUrl = param.launch_presentation_return_url;
                }
                return View("~/Views/Lti/LtiError.cshtml");
            }
            //catch (Core.WarningMessageException ex)
            //{
            //    _logger.WarnFormat("[WarningMessageException] param:{0}.",
            //        JsonSerializer.JsonSerialize(param));
            //    this.ViewBag.Message = ex.Message;
            //    return this.View("~/Views/Lti/LtiError.cshtml");
            //}
            catch (Exception ex)
            {
                _logger.ErrorFormat(ex, "LoginWithProvider exception. oauth_consumer_key:{0}.", param.oauth_consumer_key);
                this.ViewBag.DebugError = IsDebug ? (ex.Message + ex.StackTrace) : string.Empty;
                return this.View("~/Views/Lti/LtiError.cshtml");
            }
            {
                methodTime.Stop();
                var time = methodTime.Elapsed;
                if (time > TimeSpan.FromSeconds(int.Parse((string)_settings.Monitoring_MaxLoginTime)))
                {
                    var monitoringLog = IoC.Resolve<ILogger>("Monitoring");

                    monitoringLog.ErrorFormat("LoginWithProvider takes more than {0} seconds. Time: {1}. Details: {2}.",
                        _settings.Monitoring_MaxLoginTime.ToString(),
                        time.ToString(), trace.ToString());
                }
            }

            ViewData["Message"] = provider;
            return this.View("~/Views/Lti/About.cshtml");
        }
    }
}
