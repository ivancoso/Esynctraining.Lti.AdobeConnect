using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using EdugameCloud.Lti.API.AdobeConnect;
using EdugameCloud.Lti.Core;
using EdugameCloud.Lti.Core.Business.Models;
using EdugameCloud.Lti.Core.Constants;
using EdugameCloud.Lti.Core.Domain.Entities;
using EdugameCloud.Lti.Core.DTO;
using EdugameCloud.Lti.Domain.Entities;
using EdugameCloud.Lti.DTO;
using EdugameCloud.Lti.Extensions;
using Esynctraining.AC.Provider.DataObjects.Results;
using Esynctraining.AC.Provider.Entities;
using Esynctraining.Core.Domain;
using Esynctraining.Core.Extensions;
using Esynctraining.Core.Logging;
using Esynctraining.Core.Providers;
using Esynctraining.Core.Utils;

namespace EdugameCloud.Lti.Controllers
{
    public class AcUserController : BaseController
    {
        private readonly LmsUserModel lmsUserModel;
        private readonly UsersSetup usersSetup;
        private readonly IAdobeConnectUserService acUserService;

        private LmsCourseMeetingModel LmsCourseMeetingModel
        {
            get
            {
                return IoC.Resolve<LmsCourseMeetingModel>();
            }
        }

        private MeetingSetup MeetingSetup
        {
            get
            {
                return IoC.Resolve<MeetingSetup>();
            }
        }

        #region Constructors and Destructors

        public AcUserController(
            LmsUserSessionModel userSessionModel,
            LmsUserModel lmsUserModel,
            IAdobeConnectAccountService acAccountService, 
            ApplicationSettingsProvider settings, 
            UsersSetup usersSetup,
            IAdobeConnectUserService acUserService,
            ILogger logger)
            : base(userSessionModel, acAccountService, settings, logger)
        {
            this.lmsUserModel = lmsUserModel;
            this.usersSetup = usersSetup;
            this.acUserService = acUserService;
        }

        #endregion

        //lti/acNewUser
        [HttpPost]
        public virtual ActionResult AddNewUser(string lmsProviderName, PrincipalInputDto user, int meetingId)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(lmsProviderName))
                    throw new ArgumentException("Empty lmsProviderName", nameof(lmsProviderName));
                if (user == null)
                    throw new ArgumentNullException(nameof(user));

                var session = this.GetSession(lmsProviderName);
                var credentials = session.LmsCompany;
                
                if (!credentials.GetSetting<bool>(LmsCompanySettingNames.EnableAddGuest))
                    return Json(OperationResult.Error("Operation is not enabled."));

                var provider = GetAdobeConnectProvider(credentials);
                Principal principal;

                try
                {
                    if (string.IsNullOrWhiteSpace(user.principal_id))
                        principal = CreatePrincipal(user, credentials, provider);
                    else
                        principal = provider.GetOneByPrincipalId(user.principal_id).PrincipalInfo.Principal;
                }
                catch (WarningMessageException ex)
                {
                    return Json(OperationResult.Error(ex.Message));
                }

                var param = session.LtiSession.With(x => x.LtiParam);
                // HACK: LmsMeetingType.Meeting param - not in use here
                LmsCourseMeeting meeting = MeetingSetup.GetCourseMeeting(credentials, param.course_id, meetingId, LmsMeetingType.Meeting);

                // TODO: review for user-sync mode
                PermissionCollectionResult meetingEnrollments = provider.GetAllMeetingEnrollments(meeting.GetMeetingScoId());
                if (meetingEnrollments.Status.Code != StatusCodes.ok)
                {
                    string errorMessage = GetOutputErrorMessage(
                        string.Format("AcUserController.AddNewUser.GetAllMeetingEnrollments. Status.Code: {0}, Status.SubCode: {1}.", 
                        meetingEnrollments.Status.Code, 
                        meetingEnrollments.Status.SubCode));

                    return Json(OperationResult.Error(errorMessage));
                }
                if (meetingEnrollments.Values.Any(x => x.PrincipalId == principal.PrincipalId))
                {
                    return Json(OperationResult.Error(Resources.Messages.UserIsAlreadyParticipant));
                }

                AcRole role = AcRole.GetById(user.meetingRole);
                StatusInfo status = provider.UpdateScoPermissionForPrincipal(meeting.GetMeetingScoId(), principal.PrincipalId, role.MeetingPermissionId);

                // Add user as guest to DB
                var guest = new LmsCourseMeetingGuest
                {
                    PrincipalId = principal.PrincipalId,
                    LmsCourseMeeting = meeting,
                };

                meeting.MeetingGuests.Add(guest);
                this.LmsCourseMeetingModel.RegisterSave(meeting, flush: true);

                var result = new LmsUserDTO
                {
                    guest_id = guest.Id,
                    ac_id = guest.PrincipalId,
                    name = principal.Name,
                    ac_role = user.meetingRole,
                };

                return Json(OperationResultWithData<LmsUserDTO>.Success(result));
            }
            catch (Exception ex)
            {
                string errorMessage = GetOutputErrorMessage("AcUserController.AddNewUser", ex);
                return Json(OperationResult.Error(errorMessage));
            }
        }

        [HttpPost]
        public virtual ActionResult SearchExistingUser(string lmsProviderName, string searchTerm)
        {
            try
            {
                var session = this.GetSession(lmsProviderName);
                var credentials = session.LmsCompany;
                var provider = GetAdobeConnectProvider(credentials);

                var result = new List<Principal>();
                PrincipalCollectionResult byLogin = provider.GetAllByFieldLike("login", searchTerm);
                if (byLogin.Success)
                {
                    result.AddRange(byLogin.Values);
                }
                else
                {
                    // TODO: log error and return error!!
                }

                PrincipalCollectionResult byName = provider.GetAllByFieldLike("name", searchTerm);
                if (byName.Success)
                {
                    result.AddRange(byName.Values);
                }
                else
                {
                    // TODO: log error and return error!!
                }

                var foundPrincipals = (from p in result
                                      group p by new { p.Login }
                                      into distinctLoginGroup
                                      select distinctLoginGroup.First());

                if (!foundPrincipals.Any())
                {
                    return Json(OperationResultWithData<IEnumerable<PrincipalDto>>.Success(new PrincipalDto[0]));
                }

                return Json(OperationResultWithData<IEnumerable<PrincipalDto>>.Success(foundPrincipals
                     .GroupBy(p => p.PrincipalId)
                     .Select(g => g.First())
                     .Return(x => x.Select(PrincipalDto.Build), Enumerable.Empty<PrincipalDto>())));
            }
            catch (Exception ex)
            {
                string errorMessage = GetOutputErrorMessage("AcUserController.SearchExistingUser", ex);
                return Json(OperationResult.Error(errorMessage));
            }
        }


        private static Principal CreatePrincipal(PrincipalInputDto user, LmsCompany credentials, IAdobeConnectProxy provider)
        {
            string login = (credentials.ACUsesEmailAsLogin ?? false) ? null : user.login;

            var setup = new PrincipalSetup
            {
                Email = user.email,
                FirstName = user.firstName,
                LastName = user.lastName,
                Login = login,
                SendEmail = user.sendEmail,
                HasChildren = false,
                Type = PrincipalTypes.user,
                Password = user.password,
                //Name = NOTE: name is used for groups ONLY!!
            };
            
            PrincipalResult pu = provider.PrincipalUpdate(setup, false, false);

            if (!pu.Success)
            {
                if (pu.Status.InvalidField == "login" && pu.Status.SubCode == StatusSubCodes.duplicate)
                {
                    throw new WarningMessageException(string.Format(Resources.Messages.PrincipalValidateAlreadyInAc, login ?? user.email));
                }

                if (pu.Status.InvalidField == "name" && pu.Status.SubCode == StatusSubCodes.range)
                {
                    throw new WarningMessageException(Resources.Messages.PrincipalValidateNameLength);
                }

                if (pu.Status.InvalidField == "email" && pu.Status.SubCode == StatusSubCodes.range)
                {
                    throw new WarningMessageException(Resources.Messages.PrincipalValidateEmailLength);
                }

                if (pu.Status.InvalidField == "login" && pu.Status.SubCode == StatusSubCodes.range)
                {
                    throw new WarningMessageException(Resources.Messages.PrincipalValidateLoginLength);
                }

                string additionalData = string.Format("firstName: {0}, lastName: {1}, login: {2}, email: {3}", user.firstName, user.lastName, user.login, user.email);
                if (pu.Status.UnderlyingExceptionInfo != null)
                {
                    throw new InvalidOperationException(string.Format("AC.PrincipalUpdate error. Additional Data: {0}", additionalData), pu.Status.UnderlyingExceptionInfo);
                }

                if (!string.IsNullOrEmpty(pu.Status.InvalidField))
                {
                    throw new InvalidOperationException(string.Format("AC.PrincipalUpdate error. Invalid Field: {0}. Status.SubCode: {1}. Additional Data: {2}", pu.Status.InvalidField, pu.Status.SubCode, additionalData));
                }

                throw new InvalidOperationException(string.Format("AC.PrincipalUpdate error. Status.Code: {0}. Status.SubCode: {1}. Additional Data: {2}", pu.Status.Code, pu.Status.SubCode, additionalData));
            }

            Principal createdPrincipal = pu.Principal;
            return createdPrincipal;
        }

    }

}