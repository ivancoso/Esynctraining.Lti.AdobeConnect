using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using EdugameCloud.Lti.Api.Filters;
using EdugameCloud.Lti.Api.Models;
using EdugameCloud.Lti.API;
using EdugameCloud.Lti.API.AdobeConnect;
using EdugameCloud.Lti.Core.Constants;
using EdugameCloud.Lti.Domain.Entities;
using EdugameCloud.Lti.DTO;
using EdugameCloud.Lti.Resources;
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
            public int MeetingId { get; set; }

            [DataMember]
            public LmsUserDTO[] Users { get; set; }

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
        [LmsAuthorizeBase(ApiCallEnabled = true)]
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
                    SynchronizationUserService.SynchronizeUsers(LmsCompany, syncACUsers: false, meetingIds: new[] { request.MeetingId });
                }

                string error;
                IList<LmsUserDTO> users = this.UsersSetup.GetUsers(
                    LmsCompany,
                    GetAdminProvider(),
                    CourseId,
                    // TRICK: used for D2L only! to add admin to meeting. It's OK to pass null for API here.
                    SessionSave?.LtiSession?.LtiParam,
                    request.MeetingId,
                    out error,
                    null);

                CleanUpDto(users);

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
        [LmsAuthorizeBase]
        public OperationResultWithData<IEnumerable<LmsUserDTO>> UpdateUser([FromBody]CourseUsersDto request)
        {
            var credentials = LmsCompany;
            string lastError = null;
            var updatedUsers = new List<LmsUserDTO>();
            foreach (var user in request.Users)
            {
                try
                {
                    // TRICK: client-side passes 'email' but user.GetEmail() expects primary-email
                    if (string.IsNullOrEmpty(user.PrimaryEmail) && !string.IsNullOrEmpty(user.Email))
                        user.PrimaryEmail = user.Email;

                    LmsUserDTO updatedUser = null;
                    string error;
                    if (user.GuestId.HasValue)
                    {
                        updatedUser = this.UsersSetup.UpdateGuest(
                            credentials,
                            this.GetAdminProvider(),
                            Session.LtiSession.LtiParam,
                            user,
                            request.MeetingId,
                            out error);
                    }
                    else
                    {
                        updatedUser = this.UsersSetup.UpdateUser(
                            credentials,
                            this.GetAdminProvider(),
                            Session.LtiSession.LtiParam,
                            user,
                            request.MeetingId,
                            out error);
                    }

                    if (!string.IsNullOrEmpty(error))
                    {
                        Logger.Error($"[UpdateUsers] {error}. UserId={user.Id}, MeetingId={request.MeetingId}");
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
                    Logger.Error($"[RemoveUsers] UserId={user.Id}, MeetingId={request.MeetingId}, {lastError}", ex);
                }
            }

            if (string.IsNullOrEmpty(lastError))
                return updatedUsers.AsEnumerable().ToSuccessResult();

            return new OperationResultWithData<IEnumerable<LmsUserDTO>>
            {
                Message = Messages.UsersCouldNotBeUpdated,
                Data = updatedUsers
            };
        }
        
        // NOTE: id - is userID (Id of the user in LMS) or value like "MeetingSetup.model.User-47" (generated by extJS if id was empty - if user doesn't exists in LMS)
        [Route("removefrommeeting")]
        [HttpPost]
        [LmsAuthorizeBase]
        public OperationResultWithData<IEnumerable<LmsUserDTO>> RemoveFromAcMeeting([FromBody]CourseUsersDto request)
        {
            if (!LmsCompany.GetSetting<bool>(LmsCompanySettingNames.EnableRemoveUser, true))
                return OperationResultWithData<IEnumerable<LmsUserDTO>>.Error("Operation is not enabled.");

            string error;
            string lastError = null;
            List<LmsUserDTO> removedUsers = new List<LmsUserDTO>();
            foreach (var user in request.Users)
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
                            request.MeetingId,
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
                            request.MeetingId,
                            out error);
                    }
                    if (!string.IsNullOrEmpty(error))
                    {
                        Logger.Error($"[RemoveUsers] {error}. UserId={user.Id}, MeetingId={request.MeetingId}");
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
                    Logger.Error($"[RemoveUsers] UserId={user.Id}, MeetingId={request.MeetingId}, {lastError}", ex);
                }
            }

            if (string.IsNullOrEmpty(lastError))
                return removedUsers.AsEnumerable().ToSuccessResult();

            return new OperationResultWithData<IEnumerable<LmsUserDTO>>
            {
                Message = Messages.UsersCouldNotBeRemoved,
                Data = removedUsers,
            };
        }


        // TRICK: we need Login for UNIR only
        private void CleanUpDto(IEnumerable<LmsUserDTO> data)
        {
            if (SessionSave == null)
            {
                foreach (var user in data)
                {
                    user.Login = null;
                }
            }
        }

    }

}