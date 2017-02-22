using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Web.Http;
using EdugameCloud.Lti.API.AdobeConnect;
using EdugameCloud.Lti.Core;
using EdugameCloud.Lti.Core.Business.Models;
using EdugameCloud.Lti.Core.Constants;
using EdugameCloud.Lti.Core.DTO;
using EdugameCloud.Lti.Domain.Entities;
using EdugameCloud.Lti.DTO;
using EdugameCloud.Lti.Extensions;
using Esynctraining.AC.Provider.DataObjects.Results;
using Esynctraining.AC.Provider.Entities;
using Esynctraining.AdobeConnect;
using Esynctraining.AdobeConnect.Api.Meeting;
using Esynctraining.Core.Caching;
using Esynctraining.Core.Domain;
using Esynctraining.Core.Extensions;
using Esynctraining.Core.Logging;
using Esynctraining.Core.Providers;
using Esynctraining.Core.Utils;

namespace EdugameCloud.Lti.Controllers
{
    public class AcUserController : BaseApiController
    {
        [DataContract]
        public sealed class AddUserDto : MeetingRequestDto
        {
            [Required]
            [DataMember]
            public PrincipalInputDto user { get; set; }
            
        }

        private readonly LmsUserModel lmsUserModel;
        private readonly UsersSetup usersSetup;
        private readonly IAdobeConnectUserService acUserService;

        private LmsCourseMeetingModel LmsCourseMeetingModel => IoC.Resolve<LmsCourseMeetingModel>();

        private MeetingSetup MeetingSetup => IoC.Resolve<MeetingSetup>();

        #region Constructors and Destructors

        public AcUserController(
            LmsUserSessionModel userSessionModel,
            LmsUserModel lmsUserModel,
            API.AdobeConnect.IAdobeConnectAccountService acAccountService, 
            ApplicationSettingsProvider settings, 
            UsersSetup usersSetup,
            IAdobeConnectUserService acUserService,
            ILogger logger, ICache cache)
            : base(userSessionModel, acAccountService, settings, logger, cache)
        {
            this.lmsUserModel = lmsUserModel;
            this.usersSetup = usersSetup;
            this.acUserService = acUserService;
        }

        #endregion

        [Route("acNewUser")]
        [HttpPost]
        public virtual OperationResultWithData<LmsUserDTO> AddNewUser(AddUserDto request)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(request.lmsProviderName))
                    throw new ArgumentException("Empty lmsProviderName", nameof(request));
                if (request.user == null)
                    throw new ArgumentNullException(nameof(request));

                var session = this.GetReadOnlySession(request.lmsProviderName);
                var credentials = session.LmsCompany;
                
                if (!credentials.GetSetting<bool>(LmsCompanySettingNames.EnableAddGuest, true))
                    return OperationResultWithData<LmsUserDTO>.Error("Operation is not enabled.");

                var provider = GetAdobeConnectProvider(credentials);
                Principal principal;

                try
                {
                    if (string.IsNullOrWhiteSpace(request.user.PrincipalId))
                        principal = CreatePrincipal(request.user, credentials, provider);
                    else
                        principal = provider.GetOneByPrincipalId(request.user.PrincipalId).PrincipalInfo.Principal;
                }
                catch (Core.WarningMessageException ex)
                {
                    return OperationResultWithData<LmsUserDTO>.Error(ex.Message);
                }

                var param = session.LtiSession.With(x => x.LtiParam);
                // HACK: LmsMeetingType.Meeting param - not in use here
                LmsCourseMeeting meeting = MeetingSetup.GetCourseMeeting(credentials, param.course_id, request.meetingId, LmsMeetingType.Meeting);

                // TODO: review for user-sync mode
                MeetingPermissionCollectionResult meetingEnrollments = provider.GetAllMeetingEnrollments(meeting.GetMeetingScoId());
                if (meetingEnrollments.Status.Code != StatusCodes.ok)
                {
                    string errorMessage = GetOutputErrorMessage(
                        string.Format("AcUserController.AddNewUser.GetAllMeetingEnrollments. Status.Code: {0}, Status.SubCode: {1}.", 
                        meetingEnrollments.Status.Code, 
                        meetingEnrollments.Status.SubCode));

                    return OperationResultWithData<LmsUserDTO>.Error(errorMessage);
                }
                if (meetingEnrollments.Values.Any(x => x.PrincipalId == principal.PrincipalId))
                {
                    return OperationResultWithData<LmsUserDTO>.Error(Resources.Messages.UserIsAlreadyParticipant);
                }

                AcRole role = AcRole.GetById(request.user.MeetingRole);
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
                    GuestId = guest.Id,
                    AcId = guest.PrincipalId,
                    Name = principal.Name,
                    AcRole = request.user.MeetingRole,
                };

                return result.ToSuccessResult();
            }
            catch (Exception ex)
            {
                string errorMessage = GetOutputErrorMessage("AcUserController.AddNewUser", ex);
                return OperationResultWithData<LmsUserDTO>.Error(errorMessage);
            }
        }

        [Route("acSearchUser")]
        [HttpPost]
        public virtual OperationResultWithData<IEnumerable<PrincipalDto>> SearchExistingUser([FromBody]SearchRequestDto request)
        {
            try
            {
                var session = this.GetReadOnlySession(request.lmsProviderName);
                var credentials = session.LmsCompany;
                var provider = GetAdobeConnectProvider(credentials);

                var result = new List<Principal>();
                PrincipalCollectionResult byLogin = provider.GetAllByFieldLike("login", request.searchTerm);
                if (byLogin.Success)
                {
                    result.AddRange(byLogin.Values);
                }
                else
                {
                    // TODO: log error and return error!!
                }

                PrincipalCollectionResult byName = provider.GetAllByFieldLike("name", request.searchTerm);
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
                    return new PrincipalDto[0].AsEnumerable().ToSuccessResult();
                }

                return foundPrincipals
                     .GroupBy(p => p.PrincipalId)
                     .Select(g => g.First())
                     .Return(x => x.Select(PrincipalDto.Build), Enumerable.Empty<PrincipalDto>())
                     .ToSuccessResult();
            }
            catch (Exception ex)
            {
                string errorMessage = GetOutputErrorMessage("AcUserController.SearchExistingUser", ex);
                return OperationResultWithData<IEnumerable<PrincipalDto>>.Error(errorMessage);
            }
        }


        private static Principal CreatePrincipal(PrincipalInputDto user, LmsCompany credentials, IAdobeConnectProxy provider)
        {
            string login = (credentials.ACUsesEmailAsLogin ?? false) ? null : user.Login;

            var setup = new PrincipalSetup
            {
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Login = login,
                SendEmail = user.SendEmail,
                HasChildren = false,
                Type = PrincipalType.user,
                Password = user.Password,
                //Name = NOTE: name is used for groups ONLY!!
            };
            
            PrincipalResult pu = provider.PrincipalUpdate(setup, false, false);

            if (!pu.Success)
            {
                if (pu.Status.InvalidField == "login" && pu.Status.SubCode == StatusSubCodes.duplicate)
                {
                    throw new Core.WarningMessageException(string.Format(Resources.Messages.PrincipalValidateAlreadyInAc, login ?? user.Email));
                }

                if (pu.Status.InvalidField == "name" && pu.Status.SubCode == StatusSubCodes.range)
                {
                    throw new Core.WarningMessageException(Resources.Messages.PrincipalValidateNameLength);
                }

                if (pu.Status.InvalidField == "email" && pu.Status.SubCode == StatusSubCodes.range)
                {
                    throw new Core.WarningMessageException(Resources.Messages.PrincipalValidateEmailLength);
                }

                if (pu.Status.InvalidField == "login" && pu.Status.SubCode == StatusSubCodes.range)
                {
                    throw new Core.WarningMessageException(Resources.Messages.PrincipalValidateLoginLength);
                }

                string additionalData = string.Format("firstName: {0}, lastName: {1}, login: {2}, email: {3}", user.FirstName, user.LastName, user.Login, user.Email);
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