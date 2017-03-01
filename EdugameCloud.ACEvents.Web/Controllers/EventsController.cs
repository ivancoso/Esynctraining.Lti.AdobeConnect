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
using Esynctraining.AC.Provider.Entities;
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
            if (Request.QueryString["eventQuizMappingId"] == null)
                throw new InvalidOperationException("You should pass eventQuizMappingId");

            var eventQuizMappingId = Request.QueryString["eventQuizMappingId"];
            var eventQuizMappingIdInt = int.Parse(eventQuizMappingId);
            var companyEventsService = new edugamecloud.com1.CompanyEventsService();
            var eventMapping = companyEventsService.GetById(eventQuizMappingIdInt, true);
            if (eventMapping == null)
                throw new InvalidOperationException("No eventQuizMapping with this id");
            var acService = new AcDomainsNamespace.CompanyAcDomainsService();
            var acDomain = acService.GetById(eventMapping.companyAcDomainId, true);
            var acUrl = acDomain.path;
            var eventScoId = eventMapping.acEventScoId;
            //var acUrl = "http://esynctraining.adobeconnect.com";
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
                EventQuizMappingId = eventQuizMappingIdInt
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
            _logger.Info("lookup url is " + lookupWebService.Url);
            var schools = lookupWebService.GetSchools().Where(x => x.State.stateCode == stateCode);
            var stateSchools = schools.OrderBy(x => x.AccountName);
            return Json(stateSchools, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult EventRegister(EventModel eventModel)
        {
            if (!ModelState.IsValid) //Check for validation errors
            {
                return Json(new { IsSuccess = false, Message = "You didn't pass form validation" });
                //return new JsonResult { Data = new { IsSuccess = false, Message = "You didn't pass form validation" } };
            }
            //var servicesUrl = _settings.EgcServicesUrl;
            //var acUrl = "http://esynctraining.adobeconnect.com";
            var eventQuizMapping = new edugamecloud.com1.CompanyEventsService();
            var companyQuizEventMappingDto = eventQuizMapping.GetById(eventModel.EventQuizMappingId, true);
            var acService = new AcDomainsNamespace.CompanyAcDomainsService();
            var acDomain = acService.GetById(companyQuizEventMappingDto.companyAcDomainId, true);
            var acUrl = acDomain.path;
            //var acUrl = companyQuizEventMappingDto.path;
            //var acUrl = "http://esynctraining.adobeconnect.com";
            var apiUrl = new Uri(acUrl);
            var logger = IoC.Resolve<ILogger>();

            var scoId = companyQuizEventMappingDto.acEventScoId;
            var proxy = new AdobeConnectProxy(new AdobeConnectProvider(new ConnectionDetails(apiUrl)), logger, apiUrl);
            var additionalFields = proxy.GetEventRegistrationDetails(scoId);
            var fields = new Dictionary<string, string>();
            foreach (var eventRegistrationDetail in additionalFields.EventFields)
            {
                if (string.Equals(eventRegistrationDetail.Description, "state", StringComparison.OrdinalIgnoreCase))
                    fields.Add(eventRegistrationDetail.InteractionId, eventModel.State);
                if (string.Equals(eventRegistrationDetail.Description, "school", StringComparison.OrdinalIgnoreCase))
                    fields.Add(eventRegistrationDetail.InteractionId, eventModel.School);
            }

            try
            {
                var status = proxy.RegisterToEvent(new EventRegistrationFormFields()
                {
                    ScoId = scoId,
                    Password = eventModel.Password,
                    VerifyPassword = eventModel.VerifyPassword,
                    LastName = eventModel.LastName,
                    FirstName = eventModel.FirstName,
                    Email = eventModel.Email,
                    AdditionalFields = fields
                });

                if (status.Code != StatusCodes.ok)
                {
                    var message = $"{status.Code}  {status.InnerXml} {status.InvalidField} {status.UnderlyingExceptionInfo} ";
                    _logger.Error(message);
                    //return message;
                    return Json(new { IsSuccess = false, Message = status.UnderlyingExceptionInfo });
                }

            }
            catch (Exception e)
            {
                var message = e.Message;
                _logger.Error(message);
                //return message;
                return Json(new {IsSuccess = false, Message = message});
                //return new JsonResult { Data = new { IsSuccess = false, Message = message } };
            }
            return Json(new { IsSuccess = true, Message = "You've successfully signed up for an event!" } );
            //return new JsonResult { Data = new { IsSuccess = true, Message = "You've successfully signed up for an event!" } };
        }



    }
}