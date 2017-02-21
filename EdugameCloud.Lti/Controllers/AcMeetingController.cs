using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Web.Http;
using System.Web.Security;
using EdugameCloud.Lti.API.AdobeConnect;
using EdugameCloud.Lti.Core.Business.Models;
using EdugameCloud.Lti.Core.Constants;
using EdugameCloud.Lti.Core.DTO;
using EdugameCloud.Lti.Domain.Entities;
using EdugameCloud.Lti.DTO;
using Esynctraining.AC.Provider.DataObjects;
using Esynctraining.AC.Provider.DataObjects.Results;
using Esynctraining.AC.Provider.Entities;
using Esynctraining.Core.Caching;
using Esynctraining.Core.Domain;
using Esynctraining.Core.Logging;
using Esynctraining.Core.Providers;
using Esynctraining.Core.Utils;

namespace EdugameCloud.Lti.Controllers
{
    public class AcMeetingController : BaseApiController
    {
        private readonly IAdobeConnectUserService acUserService;


        private UsersSetup UsersSetup => IoC.Resolve<UsersSetup>();

        #region Constructors and Destructors

        public AcMeetingController(
            LmsUserSessionModel userSessionModel,
            IAdobeConnectAccountService acAccountService, 
            ApplicationSettingsProvider settings,
            IAdobeConnectUserService acUserService,
            ILogger logger, ICache cache)
            : base(userSessionModel, acAccountService, settings, logger, cache)
        {
            this.acUserService = acUserService;
        }

        #endregion

        [Route("acSearchMeeting")]
        [HttpPost]
        public virtual OperationResultWithData<IEnumerable<MeetingItemDto>> SearchExistingMeeting([FromBody]SearchRequestDto request)
        {
            try
            {
                var session = this.GetReadOnlySession(request.lmsProviderName);

                if (session.LmsUser == null)
                    return OperationResultWithData<IEnumerable<MeetingItemDto>>.Error("Session doesn't contain LMS user.");
                if (string.IsNullOrWhiteSpace(session.LmsUser.PrincipalId))
                    return OperationResultWithData<IEnumerable<MeetingItemDto>>.Error("You don't have Adobe Connect account.");

                if (!session.LmsCompany.GetSetting<bool>(LmsCompanySettingNames.EnableMeetingReuse))
                    return OperationResultWithData<IEnumerable<MeetingItemDto>>.Error("Operation is not enabled.");

                var provider = GetAdobeConnectProvider(session.LmsCompany);

                var principal = provider.GetOneByPrincipalId(session.LmsUser.PrincipalId).PrincipalInfo.Principal;
                var userProvider = GetUserProvider(session.LmsCompany, session.LmsUser, principal, provider);

                var myMeetings = userProvider.ReportMyMeetings(MeetingPermissionId.host).Values
                    .Where(x => x.MeetingName.IndexOf(request.searchTerm, StringComparison.OrdinalIgnoreCase) >=0 || x.UrlPath.IndexOf(request.searchTerm, StringComparison.OrdinalIgnoreCase) >= 0);
                
                return myMeetings.Select(MeetingItemDto.Build).ToSuccessResult();
            }
            catch (Exception ex)
            {
                string errorMessage = GetOutputErrorMessage("AcMeetingController.SearchExistingMeeting", ex);
                return OperationResultWithData<IEnumerable<MeetingItemDto>>.Error(errorMessage);
            }
        }

        // copy-pasted from MeetingSetup.LoginToAC. Needs review
        private IAdobeConnectProxy GetUserProvider(LmsCompany lmsCompany, LmsUser lmsUser,
            Principal registeredUser, Esynctraining.AdobeConnect.IAdobeConnectProxy provider)
        {
            IAdobeConnectProxy userProvider = null;
            string generatedPassword = null;
            if (lmsUser.AcConnectionMode == AcConnectionMode.Overwrite && string.IsNullOrEmpty(lmsUser.ACPassword))
            {
                if (lmsCompany.AcUsername.Equals(registeredUser.Login, StringComparison.OrdinalIgnoreCase))
                {
                    generatedPassword = lmsCompany.AcPassword;
                    UsersSetup.ResetUserACPassword(lmsUser, generatedPassword);
                }
                else
                {
                    generatedPassword = Membership.GeneratePassword(8, 2);
                    var resetPasswordResult = provider.PrincipalUpdatePassword(registeredUser.PrincipalId, generatedPassword);
                    if (resetPasswordResult.Success)
                    {
                        UsersSetup.ResetUserACPassword(lmsUser, generatedPassword);
                    }
                }
            }
            var password = lmsUser.ACPassword;
            if (!string.IsNullOrEmpty(password))
            {
                try //todo: GetProvider throws exceptions in case of unsuccess. Here unsuccess is possible, so need to change approach.
                {
                    userProvider = acAccountService.GetProvider(lmsCompany.AcServer,
                        new UserCredentials(registeredUser.Login, password), true);
                }
                catch (Exception e)
                {
                    Logger.Error($"User provider error. server={lmsCompany.AcServer}, login={registeredUser.Login}", e);
                    if (userProvider == null && generatedPassword == null && lmsUser.AcConnectionMode == AcConnectionMode.Overwrite)
                    {
                        // trying to login with new generated password (in case, for example, when user changed his AC password manually)
                        generatedPassword = Membership.GeneratePassword(8, 2);
                        var resetPasswordResult = provider.PrincipalUpdatePassword(registeredUser.PrincipalId, generatedPassword);
                        if (resetPasswordResult.Success)
                        {
                            UsersSetup.ResetUserACPassword(lmsUser, generatedPassword);
                        }

                        userProvider = acAccountService.GetProvider(lmsCompany.AcServer,
                            new UserCredentials(registeredUser.Login, generatedPassword), true);
                    }
                }
                
            }

            return userProvider;
        }

        private bool IsCurrentUserAcAdministrator(IAdobeConnectProxy ac, string principalId)
        {
            var grp = ac.GetPrimaryGroupsByType(PrincipalType.admins);
            if (grp.Status.Code != StatusCodes.ok)
            {
                return false;
            }

            string groupPrincipalId = grp.Values.First().PrincipalId;

            PrincipalCollectionResult principalInfo = ac.GetGroupPrincipalUsers(groupPrincipalId, principalId);
            if (!principalInfo.Success)
            {
                return false;
            }

            return principalInfo.Values.Any() && principalInfo.Values.All(x => x.PrincipalId == principalId);
        }

    }

}