using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Esynctraining.Core.Logging;
using EdugameCloud.Lti.API.AdobeConnect;
using EdugameCloud.Lti.Core.Business.Models;
using EdugameCloud.Lti.Core.DTO;
using Esynctraining.AC.Provider.DataObjects.Results;
using Esynctraining.AC.Provider.Entities;
using Esynctraining.Core.Extensions;
using Esynctraining.Core.Providers;

namespace EdugameCloud.Lti.Controllers
{
    public class AcMeetingController : BaseController
    {
        private readonly IAdobeConnectUserService acUserService;

        #region Constructors and Destructors

        public AcMeetingController(
            LmsUserSessionModel userSessionModel,
            IAdobeConnectAccountService acAccountService, 
            ApplicationSettingsProvider settings,
            IAdobeConnectUserService acUserService,
            ILogger logger)
            : base(userSessionModel, acAccountService, settings, logger)
        {
            this.acUserService = acUserService;
        }

        #endregion

        [HttpPost]
        public virtual ActionResult SearchExistingMeeting(string lmsProviderName, string searchTerm)
        {
            try
            {
                var session = this.GetSession(lmsProviderName);

                if (session.LmsUser == null)
                    return Json(OperationResult.Error("Session doesn't contain LMS user."));
                if (string.IsNullOrWhiteSpace(session.LmsUser.PrincipalId))
                    return Json(OperationResult.Error("You don't have Adobe Connect account."));
                
                var provider = GetAdobeConnectProvider(session.LmsCompany);
                var result = new List<MeetingItem>();
                MeetingItemCollectionResult foundByNameMeetings = provider.ReportMeetingsByName(searchTerm);

                foreach (var meeting in foundByNameMeetings.Values)
                {
                    PermissionCollectionResult prm = provider.GetScoPermissions(meeting.ScoId, session.LmsUser.PrincipalId);// CommandParams.Permissions.Filter.PermissionId.Host);
                    if ((prm.Status.Code == StatusCodes.ok) 
                        && (prm.Values.First().PermissionId == PermissionId.host))
                    {
                        result.Add(meeting);
                    }
                }

                return Json(OperationResult.Success(result.Select(MeetingItemDto.Build)));
            }
            catch (Exception ex)
            {
                string errorMessage = GetOutputErrorMessage("AcMeetingController.SearchExistingMeeting", ex);
                return Json(OperationResult.Error(errorMessage));
            }
        }

    }

}