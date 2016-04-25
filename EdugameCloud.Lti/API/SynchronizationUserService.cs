using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using Esynctraining.Core.Logging;
using EdugameCloud.Lti.API.AdobeConnect;
using EdugameCloud.Lti.Core.Business.Models;
using EdugameCloud.Lti.Core.Domain.Entities;
using EdugameCloud.Lti.Domain.Entities;
using EdugameCloud.Lti.DTO;
using EdugameCloud.Lti.Extensions;
using Esynctraining.AC.Provider.Entities;
using Esynctraining.Core.Utils;

namespace EdugameCloud.Lti.API
{
    public class SynchronizationUserService : ISynchronizationUserService
    {
//        public static int SyncUsersCountLimit
//        {
//            get
//            {
//                int limit = Lti.Core.Utils.Constants.SyncUsersCountLimit;
//                int.TryParse(ConfigurationManager.AppSettings["SyncUsersCountLimit"], out limit);
//                return limit;
//            }
//        }

        private readonly LmsFactory lmsFactory;
        private readonly IAdobeConnectAccountService acAccountService;
        private readonly UsersSetup usersSetup;
        private readonly LmsUserModel lmsUserModel;
        private readonly LmsCourseMeetingModel lmsCourseMeetingModel;
        private readonly IAdobeConnectUserService acUserService;
        private readonly ILogger logger;

        public SynchronizationUserService(LmsFactory lmsFactory, IAdobeConnectAccountService acAccountService, UsersSetup usersSetup,
            LmsUserModel lmsUserModel, LmsCourseMeetingModel lmsCourseMeetingModel, IAdobeConnectUserService acUserService, ILogger logger)
        {
            this.lmsFactory = lmsFactory;
            this.acAccountService = acAccountService;
            this.usersSetup = usersSetup;
            this.lmsUserModel = lmsUserModel;
            this.lmsCourseMeetingModel = lmsCourseMeetingModel;
            this.acUserService = acUserService;
            this.logger = logger;
        }

        public void SynchronizeUsers(LmsCompany lmsCompany, bool syncACUsers, IEnumerable<int> meetingIds = null)
        {
            LmsUserServiceBase service = null;
            if ((LmsProviderEnum)lmsCompany.LmsProviderId == LmsProviderEnum.Desire2Learn)
            {
                service = IoC.Resolve<LmsUserServiceBase>(LmsProviderEnum.Desire2Learn.ToString() + "_Sync");
            }
            else
            {
                service = lmsFactory.GetUserService((LmsProviderEnum)lmsCompany.LmsProviderId);
            }

            var acProvider = acAccountService.GetProvider(lmsCompany);
            var groupedMeetings = lmsCompany.LmsCourseMeetings
                .Where(x =>
                    (meetingIds == null || meetingIds.Any(m => m == x.Id))
                    && acProvider.GetScoInfo(x.GetMeetingScoId()).Status.Code == StatusCodes.ok)
                .GroupBy(y => y.CourseId);
            foreach (var courseGroup in groupedMeetings)
            {
                logger.InfoFormat("Retrieving users for LmsCompanyId={0}, LmsProvider={1}, CourseId={2}; MeetingIds:{3}",
                    lmsCompany.Id, (LmsProviderEnum)lmsCompany.LmsProviderId, courseGroup.Key, String.Join(",", courseGroup.Select(x=>x.Id)));
                try
                {
                    //todo: set extra data param
                    var opResult = service.GetUsers(lmsCompany, lmsCompany.AdminUser, courseGroup.Key);
                    if (opResult.IsSuccess)
                    {
                        var usersCount = opResult.Data.Count;
                        if (usersCount == 0)
                        {
                            //todo: take all users (meeting.Users) and make foreach trying to retrieve
                            logger.WarnFormat("Couldn't retrieve users from API for LmsCompanyId={0}, LmsProvider={1}, CourseId={2}",
                                lmsCompany.Id, (LmsProviderEnum)lmsCompany.LmsProviderId, courseGroup.Key);
                        }
                        else if (usersCount > Core.Utils.Constants.SyncUsersCountLimit)
                        {
                            logger.WarnFormat("Course contains {0} users that is more than limit for users sync ({1}). LmsCompanyId={2}, LmsProvider={3}, CourseId={4}",
                                usersCount, Core.Utils.Constants.SyncUsersCountLimit, lmsCompany.Id, (LmsProviderEnum)lmsCompany.LmsProviderId, courseGroup.Key);
                            foreach (var meeting in courseGroup)
                            {
                                if (!meeting.EnableDynamicProvisioning)
                                {
                                    meeting.EnableDynamicProvisioning = true;
                                    lmsCourseMeetingModel.RegisterSave(meeting, true);
                                }
                            }
                        }
                        else
                        {
                            var userIds = opResult.Data.Select(x => x.lti_id ?? x.id);
                            logger.InfoFormat("API user ids: {0}", String.Join(",", userIds));
                            var existedDbUsers =
                                lmsUserModel.GetByUserIdAndCompanyLms(userIds.ToArray(),
                                    lmsCompany.Id).GroupBy(x => x.UserId).Select(x=> x.OrderBy(u=> u.Id).First());

                            var newUsers = UpdateDbUsers(opResult.Data, lmsCompany, existedDbUsers, acProvider);
                                
                            // merge results;
                            foreach (var meeting in courseGroup)
                            {
                                var userRolesToDelete =
                                    meeting.MeetingRoles.Where(x => existedDbUsers.All(u => u.Id != x.User.Id)).ToList();

                                var usersToAddToMeeting = new List<LmsUser>(newUsers);
                                usersToAddToMeeting.AddRange(
                                    existedDbUsers.Where(
                                        x => meeting.MeetingRoles.Select(mr => mr.User).All(u => u.Id != x.Id)).ToList());

                                if (userRolesToDelete.Any())
                                {
                                    logger.InfoFormat(
                                        "LmsUser ids to delete from meetingId={0}, courseId={1}: {2}",
                                        meeting.Id, meeting.CourseId,
                                        String.Join(", ", userRolesToDelete.Select(x => x.User.Id)));
                                }
                                if (usersToAddToMeeting.Any())
                                {
                                    logger.InfoFormat(
                                        "LmsUser ids to add to meetingId={0}, courseId={1}: {2}",
                                        meeting.Id, meeting.CourseId,
                                        String.Join(", ", usersToAddToMeeting.Select(x => x.UserId)));
                                }

                                userRolesToDelete.ForEach(x => meeting.MeetingRoles.Remove(x));
                                usersToAddToMeeting.ForEach(x => meeting.MeetingRoles.Add(new LmsUserMeetingRole
                                {
                                    Meeting = meeting,
                                    User = x,
                                    LmsRole = opResult.Data.First(dto => x.UserId == (dto.lti_id ?? dto.id)).lms_role
                                }));
                                meeting.EnableDynamicProvisioning = false;
                                lmsCourseMeetingModel.RegisterSave(meeting, true);
                                // todo: optimize condition, probably refresh roles not for all users
                                var dbPrincipalIds = new HashSet<string>(
                                    meeting.MeetingRoles.Where(x => x.User.PrincipalId != null).Select(x => x.User.PrincipalId));
                                List<PermissionInfo> enrollments = usersSetup.GetMeetingAttendees(acProvider, meeting.GetMeetingScoId());
                                var acPrincipalIds = new HashSet<string>(enrollments.Select(e => e.PrincipalId));

                                if (syncACUsers 
                                    && (meeting.LmsMeetingType == (int)LmsMeetingType.Meeting || meeting.LmsMeetingType == (int)LmsMeetingType.Seminar)
                                    &&
                                    (dbPrincipalIds.Count != meeting.MeetingRoles.Count 
                                    || dbPrincipalIds.Count != acPrincipalIds.Count
                                    || dbPrincipalIds.Any(x => acPrincipalIds.All(p => p != x))))
                                {
                                    logger.InfoFormat("Synchronizing AC for meetingId={0}, courseId={1}", meeting.Id, meeting.CourseId);
                                    UpdateACRoles(lmsCompany, meeting, acProvider, enrollments);
                                }
                            }
                        }
                    }
                }

                catch (Exception e)
                {
                    var message = String.Format(
                        "Error happened when tried to update users for LmsCompany with id={0}",
                        lmsCompany.Id);
                    logger.Error(message, e);
                }
            }
        }

        private IEnumerable<LmsUser> UpdateDbUsers(List<LmsUserDTO> lmsUserDtos, LmsCompany lmsCompany,
            IEnumerable<LmsUser> existedDbUsers, IAdobeConnectProxy provider)
        {
            var newUsers = new List<LmsUser>();
            foreach (var lmsUserDto in lmsUserDtos)
            {
                var dbUser = existedDbUsers.FirstOrDefault(u =>
                    (lmsUserDto.lti_id != null &&
                     u.UserId == lmsUserDto.lti_id)
                    || u.UserId == lmsUserDto.id);
                if (dbUser == null)
                {
                    string login = lmsUserDto.GetLogin();
                    Principal principal = null;
                    try
                    {
                        principal = acUserService.GetOrCreatePrincipal(provider, login, lmsUserDto.primary_email,
                            lmsUserDto.GetFirstName(),
                            lmsUserDto.GetLastName(), lmsCompany);
                    }
                    catch (Exception ex)
                    {
                        logger.Error("SyncUsers - GetOrCreatePrincipal", ex);
                    }
                    dbUser = new LmsUser
                    {
                        LmsCompany = lmsCompany,
                        Username = login,
                        UserId = lmsUserDto.lti_id ?? lmsUserDto.id,
                        PrincipalId = principal != null ? principal.PrincipalId : null,
                    };
                    newUsers.Add(dbUser);
                    logger.InfoFormat(
                        "New user to DB: lmsCompanyId={0}, UserId={1}",
                        lmsCompany.Id, dbUser.UserId);
                    lmsUserModel.RegisterSave(dbUser);
                }
                dbUser.Name = lmsUserDto.name;
                dbUser.Email = lmsUserDto.primary_email;
                dbUser.UserIdExtended = lmsUserDto.lti_id != null ? lmsUserDto.id : null;
                // todo: save lmsUserDto.id to dbUser.UserId
            }

            return newUsers;
        }

        private void UpdateACRoles(LmsCompany lmsCompany, LmsCourseMeeting meeting,
            IAdobeConnectProxy acProvider, List<PermissionInfo> enrollments)
        {
            string error = null;
            var meetingRoles = usersSetup.GetUserMeetingRoles(meeting);

            var dbUsers = meetingRoles
                    .Select(x => x.User);
            try
            {
                usersSetup.SetDefaultRolesForNonParticipants(
                    lmsCompany,
                    acProvider,
                    meeting,
                    meetingRoles.Select(x => new LmsUserDTO
                    {
                        ac_id = x.User.PrincipalId,
                        id = x.User.UserIdExtended ?? x.User.UserId,
                        lti_id = x.User.UserId,
                        login_id = x.User.Username,
                        name = x.User.Name,
                        primary_email = x.User.Email,
                        lms_role = x.LmsRole
                    }),
                    dbUsers,
                    enrollments,
                    ref error);
            }
            catch (Exception e)
            {
                var message = String.Format(
                    "Error happened when tried to update users in AC for meetingId={0}, scoId={1}",
                    meeting.Id, meeting.GetMeetingScoId());
                logger.Error(message, e);
            }
        }

    }

}