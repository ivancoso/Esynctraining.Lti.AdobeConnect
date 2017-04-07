using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using CompanyAcDomainsNamespace;
using CompanyEventsServiceNamespace;
//using System.Web;
//using System.Web.Mvc;
using EdugameCloud.ACEvents.Web.Models;
using EmailServiceNamespace;
using Esynctraining.AC.Provider;
using Esynctraining.AC.Provider.DataObjects;
using Esynctraining.AC.Provider.Entities;
using Esynctraining.AdobeConnect;
using Esynctraining.Core.Logging;
using Esynctraining.Core.Providers;
using Esynctraining.Core.Utils;
using LookupServiceNamespace;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using StatusCodes = Esynctraining.AC.Provider.Entities.StatusCodes;

namespace EdugameCloud.ACEvents.Web.Controllers
{
    public class EventsController : Controller
    {
        private readonly ILogger _logger;
        private readonly IEmailService _emailService;
        private readonly ILookupService _lookupService;
        private readonly ICompanyEventsService _companyEventsService;
        private readonly ICompanyAcDomainsService _companyAcDomainsService;
        private readonly IHttpContextAccessor _context;
        private readonly IAdobeConnectAccountService _acService;

        public EventsController(ILogger logger, IEmailService emailService,
            ICompanyEventsService companyEventsService, ILookupService lookupService,
            ICompanyAcDomainsService companyAcDomainsService, IHttpContextAccessor context, IAdobeConnectAccountService acService)
        {
            _acService = acService;
            _context = context;
            _companyAcDomainsService = companyAcDomainsService;
            _companyEventsService = companyEventsService;
            _lookupService = lookupService;
            _emailService = emailService;
            _logger = logger;
        }

        public IActionResult Signup()
        {
            if (string.IsNullOrEmpty(_context.HttpContext.Request.Query["eventQuizMappingId"]))
                throw new InvalidOperationException("You should pass eventQuizMappingId");

            var eventQuizMappingId = _context.HttpContext.Request.Query["eventQuizMappingId"];
            var eventQuizMappingIdGuid = Guid.Parse(eventQuizMappingId);
            var companyEventsService = _companyEventsService;
            var eventMapping = companyEventsService.GetByGuidAsync(eventQuizMappingIdGuid).Result;
            if (eventMapping == null)
                throw new InvalidOperationException("No eventQuizMapping with this id");
            var acService = _companyAcDomainsService;
            var acDomain = acService.GetByIdAsync(eventMapping.companyAcDomainId).Result;
            var acUrl = acDomain.path;
            var login = acDomain.user;
            var pass = acDomain.password;
            var eventScoId = eventMapping.acEventScoId;
            var apiUrl = new Uri(acUrl);
            _logger.Info("Signup started");
            var proxy = _acService.GetProvider(new AdobeConnectAccess(apiUrl, login, pass), true);
            var eventInfo = proxy.GetScoInfo(eventScoId);
            if (!eventInfo.Success)
                throw new InvalidOperationException("Error getting event info");
            var model = new EventModel
            {
                EventName = eventInfo.ScoInfo.Name,
                StartDate = eventInfo.ScoInfo.BeginDate,
                EndDate = eventInfo.ScoInfo.EndDate,
                EventQuizMappingId = eventMapping.eventQuizMappingId
            };

            var lookupWebService = _lookupService;
            var states = lookupWebService.GetStatesAsync().Result;

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

        public IActionResult GetSchoolPerState(string stateCode)
        {
            _logger.Info("Gettings states");
            var lookupWebService = _lookupService;
            //_logger.Info("lookup url is " + lookupWebService.Endpoint.Address.Uri);
            var schools = lookupWebService.GetSchoolsAsync().Result.Where(x => x.State.stateCode == stateCode);
            var stateSchools = schools.OrderBy(x => x.AccountName);
            return Json(stateSchools);
        }
        
        [HttpPost]
        public IActionResult EventRegister(EventModel eventModel)
        {
            if (!ModelState.IsValid) //Check for validation errors
            {
                return Json(new { IsSuccess = false, Message = "You didn't pass form validation" });
            }
            var eventQuizMapping = _companyEventsService;
            var companyQuizEventMappingDto = eventQuizMapping.GetByIdAsync(eventModel.EventQuizMappingId).Result;
            var acService = _companyAcDomainsService;
            var acDomain = acService.GetByIdAsync(companyQuizEventMappingDto.companyAcDomainId).Result;
            var acUrl = acDomain.path;
            var apiUrl = new Uri(acUrl);
            var login = acDomain.user;
            var pass = acDomain.password;
            var scoId = companyQuizEventMappingDto.acEventScoId;
            var proxy = _acService.GetProvider(new AdobeConnectAccess(apiUrl, login, pass), true);
            
            var eventInfo = proxy.GetScoInfo(scoId);
            if (!eventInfo.Success)
                throw new InvalidOperationException("Can't get event info");

            if (eventInfo.ScoInfo.EventGuestPolicy == "guest")
            {
                return Json(new { IsSuccess = false, Message = "Registration should not be allowed to guests. Please contact system administrator" });
            }

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
                try
                {
                    // AA: after you register to event you should login again
                    var loginResult = proxy.Login(new UserCredentials(login, pass));
                    if (!loginResult.Success)
                        throw new InvalidOperationException($"Can't login to AC url {apiUrl} user {acDomain.user}");

                    var byLogin = proxy.GetAllByLogin(eventModel.Email);
                    if (byLogin.Success)
                    {
                        var principalId = byLogin.Values.ToList()[0].PrincipalId;

                        proxy.UpdateScoPermissions(new List<IPermissionUpdateTrio>
                        {
                            new MeetingPermissionUpdateTrio
                            {
                                PermissionId = MeetingPermissionId.view,
                                PrincipalId = principalId,
                                ScoId = scoId
                            }
                        });
                    }
                }
                catch (Exception e)
                {
                    _logger.Error("Error on adding user as a participant to an event", e);
                }

                if (status.Code != StatusCodes.ok)
                {
                    var message = $"{status.Code}  {status.InnerXml} {status.InvalidField} {status.UnderlyingExceptionInfo} ";
                    _logger.Error(message);
                    return Json(new { IsSuccess = false, Message = status.UnderlyingExceptionInfo });
                }

                var eventRegistrationDto = new EventRegistrationDTO()
                {
                    Email = eventModel.Email,
                    FirstName = eventModel.FirstName,
                    LastName = eventModel.LastName,
                    EventEndDate = eventModel.EndDate,
                    EventName = eventModel.EventName,
                    EventStartDate = eventModel.StartDate,
                    eventQuizMappingId = eventModel.EventQuizMappingId,
                };
                var sendingResult = _emailService.SendRegistrationEmailAsync(eventRegistrationDto).Result;
            }
            catch (Exception e)
            {
                var message = e.Message;
                if (message.Contains("duplicate"))
                    message = "There is already a user registered with this email";
                if (message.Contains("already-registered"))
                    message = "You have already registered to the event!";
                if (message.Contains("denied"))
                    message = "The registration cannot be completed. Please contact your Administrator.";
                if (message.Contains("sco_expired"))
                    message = "The registration to the event is expired. Please contact your Administrator.";
                _logger.Error(message);
                return Json(new { IsSuccess = false, Message = message });
            }
            return Json(new { IsSuccess = true, Message = "You've successfully signed up for an event!", RedirectUrl = Url.Action("SuccessPage") });
        }

        public IActionResult SuccessPage()
        {
            return View();
        }

    }
}