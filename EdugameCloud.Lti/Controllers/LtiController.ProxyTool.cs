namespace EdugameCloud.Lti.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Web.Mvc;
    using System.Xml.Linq;
    using System.Xml.XPath;
    using EdugameCloud.Lti.API.BlackBoard;
    using EdugameCloud.Lti.DTO;
    using EdugameCloud.Lti.Models;
    using Esynctraining.Core.Utils;

    public partial class LtiController : Controller
    {
        #region Public Methods and Operators
        
        [ActionName("register-proxy-tool")]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")]
        [HttpGet]
        public virtual ActionResult RegisterProxyTool(string lmsDomain)
        {
            if (string.IsNullOrWhiteSpace(lmsDomain))
            {
                this.ViewBag.Error = "Blackboard LMS domain is missing";
                return this.View("Error");
            }

            lmsDomain = lmsDomain.TrimEnd(@"\/".ToCharArray());
            var blackBoardProfile = ParseBlackBoardSharedInfo(lmsDomain);
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
        
        [ActionName("register-proxy-tool")]
        [HttpPost]
        public virtual ActionResult RegisterProxyTool(ProxyToolPasswordModel model)
        {
            string error;
            if (!this.TryRegisterEGCTool(model, out error))
            {
                this.ViewBag.Error = error;
                return this.View("Error");
            }

            return this.View("ProxyToolRegistrationSucceeded", model);
        }
        
        #endregion

        #region Methods
        
        /// <summary>
        /// The try register EGC tool.
        /// </summary>
        /// <param name="model">
        /// The model.
        /// </param>
        /// <param name="error">
        /// The error.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        private bool TryRegisterEGCTool(ProxyToolPasswordModel model, out string error)
        {
            var pass = (string)this.Settings.InitialBBPassword;
            var soapApi = IoC.Resolve<IBlackBoardApi>();
            return soapApi.TryRegisterEGCTool(model.LmsDomain, model.RegistrationPassword, pass, out error);
        }

        /// <summary>
        /// The parse black board shared info.
        /// </summary>
        /// <param name="lmsDomain">
        /// The LMS domain.
        /// </param>
        /// <returns>
        /// The <see cref="BBConsumerProfileDTO"/>.
        /// </returns>
        private BBConsumerProfileDTO ParseBlackBoardSharedInfo(string lmsDomain)
        {
            var res = new BBConsumerProfileDTO();
            try
            {
                var uriBuilder = new UriBuilder(lmsDomain + "/webapps/ws/wsadmin/tcprofile");
                ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
                var xmlResponse = new WebClient().DownloadString(uriBuilder.Uri);
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