using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using EdugameCloud.Lti.Api.Filters;
using EdugameCloud.Lti.Api.Models;
using EdugameCloud.Lti.API.AdobeConnect;
using EdugameCloud.Lti.Core.Constants;
using EdugameCloud.Lti.Core.DTO;
using Esynctraining.AC.Provider.Entities;
using Esynctraining.Core.Caching;
using Esynctraining.Core.Domain;
using Esynctraining.Core.Logging;
using Esynctraining.Core.Providers;
using Esynctraining.Core.Utils;
using Microsoft.AspNetCore.Mvc;

namespace EdugameCloud.Lti.Api.Controllers
{
    public class AcMeetingController : BaseApiController
    {
        private UsersSetup UsersSetup => IoC.Resolve<UsersSetup>();

        #region Constructors and Destructors

        public AcMeetingController(
            API.AdobeConnect.IAdobeConnectAccountService acAccountService,
            ApplicationSettingsProvider settings,
            ILogger logger, ICache cache)
            : base( acAccountService, settings, logger, cache)
        {
        }

        #endregion

        [Route("acSearchMeeting")]
        [HttpPost]
        [TeacherOnly(FeatureName = LmsCompanySettingNames.EnableMeetingReuse)]
        public virtual OperationResultWithData<IEnumerable<MeetingItemDto>> SearchExistingMeeting([FromBody]SearchRequestDto request)
        {
            try
            {
                if (Session.LmsUser == null)
                    return OperationResultWithData<IEnumerable<MeetingItemDto>>.Error("Session doesn't contain LMS user.");
                if (string.IsNullOrWhiteSpace(Session.LmsUser.PrincipalId))
                    return OperationResultWithData<IEnumerable<MeetingItemDto>>.Error("You don't have Adobe Connect account.");
                
                var provider = GetAdminProvider();
                var principal = provider.GetOneByPrincipalId(Session.LmsUser.PrincipalId).PrincipalInfo.Principal;
                var userProvider = GetUserProvider();

                var myMeetings = userProvider.ReportMyMeetings(MeetingPermissionId.host).Values
                    .Where(x => x.MeetingName.IndexOf(request.SearchTerm, StringComparison.OrdinalIgnoreCase) >=0 || x.UrlPath.IndexOf(request.SearchTerm, StringComparison.OrdinalIgnoreCase) >= 0);
                
                return myMeetings.Select(MeetingItemDto.Build).ToSuccessResult();
            }
            catch (Exception ex)
            {
                string errorMessage = GetOutputErrorMessage("AcMeetingController.SearchExistingMeeting", ex);
                return OperationResultWithData<IEnumerable<MeetingItemDto>>.Error(errorMessage);
            }
        }
        
        //private bool IsCurrentUserAcAdministrator(IAdobeConnectProxy ac, string principalId)
        //{
        //    var grp = ac.GetPrimaryGroupsByType(PrincipalType.admins);
        //    if (grp.Status.Code != StatusCodes.ok)
        //    {
        //        return false;
        //    }

        //    string groupPrincipalId = grp.Values.First().PrincipalId;

        //    PrincipalCollectionResult principalInfo = ac.GetGroupPrincipalUsers(groupPrincipalId, principalId);
        //    if (!principalInfo.Success)
        //    {
        //        return false;
        //    }

        //    return principalInfo.Values.Any() && principalInfo.Values.All(x => x.PrincipalId == principalId);
        //}

    }

}