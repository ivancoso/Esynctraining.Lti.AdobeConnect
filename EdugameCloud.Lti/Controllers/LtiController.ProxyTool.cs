using System.Threading.Tasks;
using Esynctraining.HttpClient;
using Esynctraining.Lti.Lms.Common.API.BlackBoard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Xml.Linq;
using System.Xml.XPath;
using Esynctraining.Core.Logging;
using EdugameCloud.Lti.DTO;
using EdugameCloud.Lti.Models;
using Esynctraining.Core.Providers;
using Esynctraining.Core.Utils;

namespace EdugameCloud.Lti.Controllers
{
    public partial class LtiProxyToolController : Controller
    {
        private readonly ILogger logger;
        private readonly dynamic settings;
        private static readonly HttpClientWrapper _httpClientWrapper = new HttpClientWrapper();

        public LtiProxyToolController(
            ApplicationSettingsProvider settings,
            ILogger logger)
        {
            this.settings = settings ?? throw new ArgumentNullException(nameof(settings));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }


        [HttpGet]
        [OutputCache(VaryByParam = "*", NoStore = true, Duration = 0, Location = System.Web.UI.OutputCacheLocation.None)]
        public virtual async Task<ActionResult> RegisterProxyTool(string lmsDomain)
        {
            if (string.IsNullOrWhiteSpace(lmsDomain))
            {
                ViewBag.Error = Resources.Messages.BlackboardDomainMissing;
                return View("Error");
            }

            lmsDomain = lmsDomain.TrimEnd(@"\/".ToCharArray());
            var blackBoardProfile = await ParseBlackBoardSharedInfo(lmsDomain);
            return this.View(
                "ProxyToolPassword",
                new ProxyToolPasswordModel
                {
                    LmsDomain = lmsDomain,
                    BlackBoardTitle =
                    string.IsNullOrWhiteSpace(blackBoardProfile.Name)
                    ? lmsDomain
                    : blackBoardProfile.Name,
                    LtiVersion = string.IsNullOrWhiteSpace(blackBoardProfile.LtiVersion) ? "2.0-July08" : blackBoardProfile.LtiVersion,
                });
        }
        
        [HttpPost]
        public virtual ActionResult RegisterProxyTool(ProxyToolPasswordModel model)
        {
            string error;
            if (!TryRegisterEGCTool(model, out error))
            {
                ViewBag.Error = error;
                return View("Error");
            }

            return View("ProxyToolRegistrationSucceeded", model);
        }

        #region Methods

        private bool TryRegisterEGCTool(ProxyToolPasswordModel model, out string error)
        {
            var pass = (string)settings.InitialBBPassword;
            var soapApi = IoC.Resolve<IBlackBoardApi>();
            return soapApi.TryRegisterEGCTool(model.LmsDomain, model.RegistrationPassword, pass, out error);
        }

        private async Task<BBConsumerProfileDTO> ParseBlackBoardSharedInfo(string lmsDomain)
        {
            var res = new BBConsumerProfileDTO();
            try
            {
                var uriBuilder = new UriBuilder(lmsDomain + "/webapps/ws/wsadmin/tcprofile");
                var xmlResponse = await _httpClientWrapper.DownloadStringAsync(uriBuilder.Uri);
                var response = XElement.Parse(xmlResponse);
                var name = response.XPathEvaluate("string(/tool-consumer-info/name)").ToString();
                res.Name = name;
                var ltiVersion = response.XPathEvaluate("string(/@ltiVersion)").ToString();
                res.LtiVersion = ltiVersion;
                IEnumerable<XElement> services = response.XPathSelectElements("/services-offered/service");
                var servicesList = services.Select(service => service.XPathEvaluate("string(@name)").ToString()).ToList();
                res.Services = servicesList;
            }
                // ReSharper disable once EmptyGeneralCatchClause
            catch (Exception ex)
            {
                logger.Error("ParseBlackBoardSharedInfo", ex);
            }

            return res;
        }
        
        #endregion

    }

}