using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Castle.Core.Logging;
using EdugameCloud.Lti.API.AdobeConnect;
using EdugameCloud.Lti.Core.Business.Models;
using EdugameCloud.Lti.Core.Domain.Entities;
using EdugameCloud.Lti.Core.DTO;
using EdugameCloud.Lti.Domain.Entities;
using EdugameCloud.Lti.DTO;
using EdugameCloud.Lti.Extensions;
using Esynctraining.AC.Provider;
using Esynctraining.AC.Provider.DataObjects.Results;
using Esynctraining.AC.Provider.Entities;
using Esynctraining.Core.Extensions;
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
        
        #region Constructors and Destructors

        public AcUserController(
            LmsUserSessionModel userSessionModel,
            LmsUserModel lmsUserModel, 
            MeetingSetup meetingSetup, 
            ApplicationSettingsProvider settings, 
            UsersSetup usersSetup,
            IAdobeConnectUserService acUserService,
            ILogger logger)
            : base(userSessionModel, meetingSetup, settings, logger)
        {
            this.lmsUserModel = lmsUserModel;
            this.usersSetup = usersSetup;
            this.acUserService = acUserService;
        }

        #endregion

        [HttpPost]
        public virtual ActionResult AddNewUser(string lmsProviderName, PrincipalInputDto user, string meetingScoId)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(lmsProviderName))
                    throw new ArgumentException("Empty lmsProviderName", "lmsProviderName");
                if (user == null)
                    throw new ArgumentNullException("user");
                if (string.IsNullOrWhiteSpace(lmsProviderName))
                    throw new ArgumentException("Empty meetingScoId", "meetingScoId");
                
                var session = this.GetSession(lmsProviderName);
                var credentials = session.LmsCompany;                
                var provider = GetAdobeConnectProvider(credentials);
                Principal principal;

                if (string.IsNullOrWhiteSpace(user.principal_id))
                    principal = CreatePrincipal(user, credentials, provider);
                else
                    principal = provider.GetOneByPrincipalId(user.principal_id).PrincipalInfo.Principal;

                var param = session.LtiSession.With(x => x.LtiParam);
                LmsCourseMeeting meeting = meetingSetup.GetLmsCourseMeeting(credentials, param.course_id, meetingScoId, (int)LmsMeetingType.Meeting);

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
                    return Json(OperationResult.Error("This Adobe Connect user is already a participant of the meeting."));
                }

                AcRole role = AcRole.GetByName(user.meetingRole);
                StatusInfo status = provider.UpdateScoPermissionForPrincipal(meetingScoId, principal.PrincipalId, role.MeetingPermissionId);

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

                return Json(OperationResult.Success(result));
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
                string error;

                var provider = GetAdobeConnectProvider(credentials);
                // TODO: IMPLEMENT http://dev.connectextensions.com/api/xml?action=principal-list&filter-like-login=@esynctraining&filter-like-name=sergey
                //PrincipalCollectionResult resultByEmail = provider.GetAllByEmail(searchTerm);


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

                if (result.Count == 0)
                {
                    return Json(OperationResult.Success(new PrincipalDto[0]));
                }

                return Json(OperationResult.Success(result
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


        private static Principal CreatePrincipal(PrincipalInputDto user, LmsCompany credentials, AdobeConnectProvider provider)
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


            PrincipalResult pu = provider.PrincipalUpdate(setup);

            if (!pu.Success)
            {
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