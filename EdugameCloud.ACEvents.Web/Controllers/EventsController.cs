using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using EdugameCloud.ACEvents.Web.Models;
using Esynctraining.AC.Provider;
using Esynctraining.AC.Provider.DataObjects;
using Esynctraining.AdobeConnect;
using Esynctraining.Core.Logging;
using Esynctraining.Core.Providers;
using Esynctraining.Core.Utils;

namespace EdugameCloud.ACEvents.Web.Controllers
{
    public class EventsController : Controller
    {
        //private readonly ILogger _logger;
        private readonly ILogger _logger;
        private readonly dynamic _settings;

        public EventsController(ILogger logger, ApplicationSettingsProvider settings)
        {
            _settings = settings;
            _logger = logger;
            //  _logger = logger;
        }

        public ActionResult Signup()
        {
            //if (Request.QueryString["licenseId"] == null || Request.QueryString["eventScoId"] == null)
            if (Request.QueryString["eventScoId"] == null)
                throw new InvalidOperationException("You should pass licenseId and eventId in queryString");

            var eventScoId = Request.QueryString["eventScoId"];
            var acUrl = "http://esynctraining.adobeconnect.com";
            var apiUrl = new Uri(acUrl);
            _logger.Info("Signup started");
            var proxy = new AdobeConnectProxy(new AdobeConnectProvider(new ConnectionDetails(apiUrl)), _logger, apiUrl);
            var eventInfo = proxy.GetScoInfo(eventScoId);
            if (!eventInfo.Success)
                throw new InvalidOperationException("Error getting event info");
            var model = new EventModel
            {
                EventName = eventInfo.ScoInfo.Name,
                StartDate = eventInfo.ScoInfo.BeginDate,
                EndDate = eventInfo.ScoInfo.EndDate,
                EventScoId = eventScoId
            };

            //var lookupServiceClient = new LookupServiceClient.LookupServiceClient();
            var lookupWebService = new edugamecloud.com.LookupService();
            var states = lookupWebService.GetStates();

            model.States = states.Select(x => new SelectListItem()
            {
                Value = x.stateCode,
                Text = x.stateName
            }).Concat(new List<SelectListItem>()
            {
                new SelectListItem()
                {
                    Text = "Please select a state",
                    Value = "",
                    Selected = true
                }
            });

            model.Schools = new List<SelectListItem>()
            {
                new SelectListItem()
                {
                    Text = "Please select a school",
                    Value = "",
                    Selected = true
                }
            };

            return View(model);
        }

        public ActionResult GetSchoolPerState(string stateCode)
        {
            _logger.Info("Gettings states");
            var lookupWebService = new edugamecloud.com.LookupService();
            _logger.Info("lookup url is "+lookupWebService.Url);
            var schools = lookupWebService.GetSchools().Where(x => x.State.stateCode == stateCode);
            var stateSchools = schools.OrderBy(x => x.AccountName);
            return Json(stateSchools, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public async Task<bool> EventRegister(EventModel eventModel)
        {
            if (!ModelState.IsValid) //Check for validation errors
            {
                return false;
            }
            var servicesUrl = _settings.EgcServicesUrl;
            var acUrl = "http://esynctraining.adobeconnect.com";
            var apiUrl = new Uri(acUrl);
            var logger = IoC.Resolve<ILogger>();
            var proxy = new AdobeConnectProxy(new AdobeConnectProvider(new ConnectionDetails(apiUrl)), logger, apiUrl);

            var httpClient = new HttpClient();
            
            var eventRegisterUrl = acUrl + "/api/xml" + "?action=event-register&sco-id=" + eventModel.EventScoId + "&login=" +
                             Url.Encode(eventModel.Email) + "&password=" + eventModel.Password + "&password-verify=" +
                             eventModel.VerifyPassword +
                             "&first-name=" + eventModel.FirstName + "&last-name=" + eventModel.LastName +
                             "&interaction-id=1763444230&response=" + eventModel.School;
            var result = await httpClient.GetAsync(eventRegisterUrl);
            _logger.Info(result.Content.ToString());
            return true;
        }

      

    }
}