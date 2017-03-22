using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using EdugameCloud.Lti.API;
using EdugameCloud.Lti.API.AdobeConnect;
using EdugameCloud.Lti.Core.Constants;
using EdugameCloud.Lti.Domain.Entities;
using EdugameCloud.Lti.DTO;
using Esynctraining.Core.Caching;
using Esynctraining.Core.Domain;
using Esynctraining.Core.Logging;
using Esynctraining.Core.Providers;
using Esynctraining.Core.Utils;
using Microsoft.AspNetCore.Mvc;

namespace EdugameCloud.Lti.Api.Controllers
{
    [Route("users")]
    public partial class UsersController : BaseApiController
    {
        [DataContract]
        public class MeetingRequestDtoEx : MeetingRequestDto
        {
            [DataMember]
            public bool ForceUpdate { get; set; }

        }

        [DataContract]
        public class CourseUsersDto : RequestDto
        {
            [Required]
            [DataMember]
            public int meetingId { get; set; }

            [DataMember]
            public LmsUserDTO[] users { get; set; }

        }

        private LmsFactory LmsFactory => IoC.Resolve<LmsFactory>();
        private ISynchronizationUserService SynchronizationUserService => IoC.Resolve<ISynchronizationUserService>();
        private UsersSetup UsersSetup => IoC.Resolve<UsersSetup>();


        public UsersController(IAdobeConnectAccountService acAccountService, ApplicationSettingsProvider settings, ILogger logger, ICache cache)
            : base(acAccountService, settings, logger, cache)
        {
        }

        [Route("")]
        [HttpPost]
        [EdugameCloud.Lti.Api.Filters.LmsAuthorizeBase(ApiCallEnabled = true)]
        public virtual OperationResultWithData<IList<LmsUserDTO>> GetUsers([FromBody]MeetingRequestDtoEx request)
        {
            try
            {
                var service = LmsFactory.GetUserService((LmsProviderEnum)LmsCompany.LmsProviderId);

                if (request.ForceUpdate && LmsCompany.UseSynchronizedUsers
                    && service != null
                    && service.CanRetrieveUsersFromApiForCompany(LmsCompany)
                    && LmsCompany.LmsCourseMeetings != null)
                {
                    SynchronizationUserService.SynchronizeUsers(LmsCompany, syncACUsers: false, meetingIds: new[] { request.meetingId });
                }

                string error;
                IList<LmsUserDTO> users = this.UsersSetup.GetUsers(
                    LmsCompany,
                    GetAdminProvider(),
                    CourseId,
                    // TRICK: used for D2L only! to add admin to meeting
                    SessionSave?.LtiSession?.LtiParam,
                    request.meetingId,
                    out error,
                    null);

                if (string.IsNullOrWhiteSpace(error))
                {
                    return users.ToSuccessResult();
                }

                return OperationResultWithData<IList<LmsUserDTO>>.Error(error);
            }
            catch (Exception ex)
            {
                string errorMessage = GetOutputErrorMessage("GetUsers", ex);
                return OperationResultWithData<IList<LmsUserDTO>>.Error(errorMessage);
            }
        }

        [Route("update")]
        [HttpPost]
        [EdugameCloud.Lti.Api.Filters.LmsAuthorizeBase]
        public OperationResultWithData<IEnumerable<LmsUserDTO>> UpdateUser([FromBody]CourseUsersDto request)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            var credentials = LmsCompany;
            string lastError = null;
            var updatedUsers = new List<LmsUserDTO>();
            foreach (var user in request.users)
            {
                try
                {
                    // TRICK: client-side passes 'email' but user.GetEmail() expects primary-email
                    if (string.IsNullOrEmpty(user.PrimaryEmail) && !string.IsNullOrEmpty(user.email))
                        user.PrimaryEmail = user.email;

                    LmsUserDTO updatedUser = null;
                    string error;
                    if (user.GuestId.HasValue)
                    {
                        updatedUser = this.UsersSetup.UpdateGuest(
                            credentials,
                            this.GetAdminProvider(),
                            Session.LtiSession.LtiParam,
                            user,
                            request.meetingId,
                            out error);
                    }
                    else
                    {
                        updatedUser = this.UsersSetup.UpdateUser(
                            credentials,
                            this.GetAdminProvider(),
                            Session.LtiSession.LtiParam,
                            user,
                            request.meetingId,
                            out error);
                    }

                    if (!string.IsNullOrEmpty(error))
                    {
                        Logger.Error($"[UpdateUsers] {error}. UserId={user.Id}, MeetingId={request.meetingId}");
                        lastError = error;
                    }
                    else
                    {
                        // TRICK: if user not found in LMS - return original record from client  (remove user from meeting participant list - user doesn't exist in LMS)
                        updatedUsers.Add(updatedUser ?? user);
                    }
                }
                catch (Exception ex)
                {
                    lastError = GetOutputErrorMessage("UpdateUsers", ex);
                    Logger.Error($"[RemoveUsers] UserId={user.Id}, MeetingId={request.meetingId}, {lastError}", ex);
                }
            }

            if (string.IsNullOrEmpty(lastError))
                return updatedUsers.AsEnumerable().ToSuccessResult();

            return new OperationResultWithData<IEnumerable<LmsUserDTO>>
            {
                Message = Resources.Messages.UsersCouldNotBeUpdated,
                Data = updatedUsers
            };
        }
        
        // NOTE: id - is userID (Id of the user in LMS) or value like "MeetingSetup.model.User-47" (generated by extJS if id was empty - if user doesn't exists in LMS)
        [Route("removefrommeeting")]
        [HttpPost]
        [EdugameCloud.Lti.Api.Filters.LmsAuthorizeBase]
        public OperationResultWithData<IEnumerable<LmsUserDTO>> RemoveFromAcMeeting([FromBody]CourseUsersDto request)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            if (!LmsCompany.GetSetting<bool>(LmsCompanySettingNames.EnableRemoveUser, true))
                return OperationResultWithData<IEnumerable<LmsUserDTO>>.Error("Operation is not enabled.");

            string error;
            string lastError = null;
            List<LmsUserDTO> removedUsers = new List<LmsUserDTO>();
            foreach (var user in request.users)
            {
                try
                {
                    if (user.GuestId.HasValue)
                    {
                        this.UsersSetup.DeleteGuestFromAcMeeting(
                            LmsCompany,
                            this.GetAdminProvider(),
                            Session.LtiSession.LtiParam,
                            user.AcId,
                            request.meetingId,
                            user.GuestId.Value,
                            out error);
                    }
                    else
                    {
                        this.UsersSetup.DeleteUserFromAcMeeting(
                            LmsCompany,
                            this.GetAdminProvider(),
                            Session.LtiSession.LtiParam,
                            user.AcId,
                            request.meetingId,
                            out error);
                    }
                    if (!string.IsNullOrEmpty(error))
                    {
                        Logger.Error($"[RemoveUsers] {error}. UserId={user.Id}, MeetingId={request.meetingId}");
                        lastError = error;
                    }
                    else
                    {
                        removedUsers.Add(user);
                    }
                }
                catch (Exception ex)
                {
                    lastError = GetOutputErrorMessage("RemoveUsers", ex);
                    Logger.Error($"[RemoveUsers] UserId={user.Id}, MeetingId={request.meetingId}, {lastError}", ex);
                }
            }

            if (string.IsNullOrEmpty(lastError))
                return removedUsers.AsEnumerable().ToSuccessResult();

            return new OperationResultWithData<IEnumerable<LmsUserDTO>>
            {
                Message = Resources.Messages.UsersCouldNotBeRemoved,
                Data = removedUsers,
            };
        }

    }

}