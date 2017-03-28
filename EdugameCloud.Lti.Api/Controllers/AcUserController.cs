using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using EdugameCloud.Lti.Api.Filters;
using EdugameCloud.Lti.API.AdobeConnect;
using EdugameCloud.Lti.Core.Business.Models;
using EdugameCloud.Lti.Core.Constants;
using EdugameCloud.Lti.Core.DTO;
using EdugameCloud.Lti.Domain.Entities;
using EdugameCloud.Lti.DTO;
using EdugameCloud.Lti.Extensions;
using EdugameCloud.Lti.Resources;
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
using Microsoft.AspNetCore.Mvc;

namespace EdugameCloud.Lti.Api.Controllers
{
    public class AcUserController : BaseApiController
    {
        // HACK: not great solution (
        [DataContract]
        public sealed class AddUserDto : PrincipalInputDto
        {
            [Required]
            [DataMember]
            public int MeetingId { get; set; }

        }

        private LmsCourseMeetingModel LmsCourseMeetingModel => IoC.Resolve<LmsCourseMeetingModel>();

        private MeetingSetup MeetingSetup => IoC.Resolve<MeetingSetup>();

        #region Constructors and Destructors

        public AcUserController(
            API.AdobeConnect.IAdobeConnectAccountService acAccountService, 
            ApplicationSettingsProvider settings, 
            ILogger logger, ICache cache)
            : base(acAccountService, settings, logger, cache)
        {
        }

        #endregion

        [Route("acNewUser")]
        [HttpPost]
        [LmsAuthorizeBase]
        public virtual OperationResultWithData<LmsUserDTO> AddNewUser([FromBody]AddUserDto request)
        {
            try
            {
                // TODO: use FeatureName but this settings has true as default value
                if (!LmsCompany.GetSetting<bool>(LmsCompanySettingNames.EnableAddGuest, true))
                    return OperationResultWithData<LmsUserDTO>.Error("Operation is not enabled.");
                var provider = GetAdminProvider();
                Principal principal;

                try
                {
                    if (string.IsNullOrWhiteSpace(request.PrincipalId))
                        principal = CreatePrincipal(request, LmsCompany, provider);
                    else
                        principal = provider.GetOneByPrincipalId(request.PrincipalId).PrincipalInfo.Principal;
                }
                catch (Core.WarningMessageException ex)
                {
                    return OperationResultWithData<LmsUserDTO>.Error(ex.Message);
                }

                // HACK: LmsMeetingType.Meeting param - not in use here
                LmsCourseMeeting meeting = MeetingSetup.GetCourseMeeting(LmsCompany, CourseId, request.MeetingId, LmsMeetingType.Meeting);

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
                    return OperationResultWithData<LmsUserDTO>.Error(Messages.UserIsAlreadyParticipant);
                }

                AcRole role = AcRole.GetById(request.MeetingRole);
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
                    AcRole = request.MeetingRole,
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
        [LmsAuthorizeBase]
        public virtual OperationResultWithData<IEnumerable<PrincipalDto>> SearchExistingUser([FromBody]SearchRequestDto request)
        {
            try
            {
                var provider = GetAdminProvider();

                var result = new List<Principal>();
                PrincipalCollectionResult byLogin = provider.GetAllByFieldLike("login", request.SearchTerm);
                if (byLogin.Success)
                {
                    result.AddRange(byLogin.Values);
                }
                else
                {
                    // TODO: log error and return error!!
                }

                PrincipalCollectionResult byName = provider.GetAllByFieldLike("name", request.SearchTerm);
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


        private static Principal CreatePrincipal(PrincipalInputDto user, ILmsLicense credentials, IAdobeConnectProxy provider)
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
                    throw new Core.WarningMessageException(string.Format(Messages.PrincipalValidateAlreadyInAc, login ?? user.Email));
                }

                if (pu.Status.InvalidField == "name" && pu.Status.SubCode == StatusSubCodes.range)
                {
                    throw new Core.WarningMessageException(Messages.PrincipalValidateNameLength);
                }

                if (pu.Status.InvalidField == "email" && pu.Status.SubCode == StatusSubCodes.range)
                {
                    throw new Core.WarningMessageException(Messages.PrincipalValidateEmailLength);
                }

                if (pu.Status.InvalidField == "login" && pu.Status.SubCode == StatusSubCodes.range)
                {
                    throw new Core.WarningMessageException(Messages.PrincipalValidateLoginLength);
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