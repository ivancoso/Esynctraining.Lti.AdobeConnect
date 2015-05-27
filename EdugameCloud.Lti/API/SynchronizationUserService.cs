using System;
using System.Collections.Generic;
using System.Linq;
using Castle.Core.Logging;
using EdugameCloud.Lti.API.AdobeConnect;
using EdugameCloud.Lti.Core.Business.Models;
using EdugameCloud.Lti.Core.Domain.Entities;
using EdugameCloud.Lti.Domain.Entities;
using EdugameCloud.Lti.DTO;
using EdugameCloud.Lti.Extensions;
using Esynctraining.AC.Provider;
using Esynctraining.AC.Provider.Entities;

namespace EdugameCloud.Lti.API
{
    public class SynchronizationUserService : ISynchronizationUserService
    {
        private readonly LmsFactory lmsFactory;
        private readonly IMeetingSetup meetingSetup;
        private readonly UsersSetup usersSetup;
        private readonly LmsUserModel lmsUserModel;
        private readonly LmsCourseMeetingModel lmsCourseMeetingModel;
        private readonly ILogger logger;

        public SynchronizationUserService(LmsFactory lmsFactory, IMeetingSetup meetingSetup, UsersSetup usersSetup,
            LmsUserModel lmsUserModel, LmsCourseMeetingModel lmsCourseMeetingModel, ILogger logger)
        {
            this.lmsFactory = lmsFactory;
            this.meetingSetup = meetingSetup;
            this.usersSetup = usersSetup;
            this.lmsUserModel = lmsUserModel;
            this.lmsCourseMeetingModel = lmsCourseMeetingModel;
            this.logger = logger;
        }

        public void SynchronizeUsers(LmsCompany lmsCompany, IEnumerable<string> scoIds = null)
        {
            var service = lmsFactory.GetUserService((LmsProviderEnum)lmsCompany.LmsProvider.Id);
            var acProvider = meetingSetup.GetProvider(lmsCompany);
            var groupedMeetings = lmsCompany.LmsCourseMeetings
                .Where(x =>
                    x.LmsMeetingType != (int) LmsMeetingType.OfficeHours
                    && (scoIds == null || scoIds.Any(m => m == x.ScoId))
                    && acProvider.GetScoInfo(x.GetMeetingScoId()).Status.Code == StatusCodes.ok)
                .GroupBy(y => y.CourseId);
            foreach (var courseGroup in groupedMeetings)
            {
                logger.InfoFormat("Retrieving users for LmsCompanyId={0}, LmsProvider={1}, CourseId={2}",
                    lmsCompany.Id, (LmsProviderEnum)lmsCompany.LmsProvider.Id, courseGroup.Key);
                try
                {
                    //todo: set extra data param
                    var opResult = service.GetUsers(lmsCompany, null,
                        lmsCompany.AdminUser, courseGroup.Key);
                    if (opResult.isSuccess)
                    {
                        if (!opResult.data.Any())
                        {
                            //todo: take all users (meeting.Users) and make foreach trying to retrieve
                            logger.Warn("Couldn't retrieve users from API");
                        }
                        else
                        {
                            var userIds = opResult.data.Select(x => x.lti_id ?? x.id);
                            logger.InfoFormat("API user ids: {0}", String.Join(",", userIds));
                            var existedDbUsers =
                                lmsUserModel.GetByUserIdAndCompanyLms(userIds.ToArray(),
                                    lmsCompany.Id);
                            var newUsers = UpdateDbUsers(opResult.data, lmsCompany, existedDbUsers);
                                
                            // merge results;
                            foreach (var meeting in courseGroup)
                            {
                                var info = acProvider.GetScoInfo(meeting.GetMeetingScoId());
                                var userRolesToDelete =
                                    meeting.MeetingRoles
                                        .Where(x => existedDbUsers.All(u => u.Id != x.User.Id)).ToList();

                                var usersToAddToMeeting = new List<LmsUser>(newUsers);
                                usersToAddToMeeting.AddRange(
                                    existedDbUsers.Where(
                                        x => meeting.MeetingRoles.Select(mr => mr.User).All(u => u.Id != x.Id)).ToList());

                                logger.InfoFormat(
                                    "LmsUser ids to delete from meetingId={0}, courseId={1}: {2}",
                                    meeting.Id, meeting.CourseId,
                                    String.Join(",", userRolesToDelete.Select(x => x.User.Id)));
                                logger.InfoFormat(
                                    "LmsUser ids to add to meetingId={0}, courseId={1}: {2}",
                                    meeting.Id, meeting.CourseId,
                                    String.Join(",", usersToAddToMeeting.Select(x => x.UserId)));

                                userRolesToDelete.ForEach(x => meeting.MeetingRoles.Remove(x));
                                usersToAddToMeeting.ForEach(x => meeting.MeetingRoles.Add(new LmsUserMeetingRole
                                {
                                    Meeting = meeting,
                                    User = x,
                                    LmsRole = opResult.data.First(dto => dto.id == x.UserId).lms_role
                                }));
                                lmsCourseMeetingModel.RegisterSave(meeting);
                                lmsCourseMeetingModel.Flush();
                                // todo: optimize condition, probably refresh roles not for all users
                                if (userRolesToDelete.Any() || usersToAddToMeeting.Any())
                                {
                                    UpdateACRoles(lmsCompany, meeting, acProvider);
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

        private IEnumerable<LmsUser> UpdateDbUsers(List<LmsUserDTO> lmsUserDtos, LmsCompany lmsCompany, IEnumerable<LmsUser> existedDbUsers)
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

                    dbUser = new LmsUser
                    {
                        LmsCompany = lmsCompany,
                        Username = login,
                        UserId = lmsUserDto.lti_id ?? lmsUserDto.id
                    };
                    newUsers.Add(dbUser);
                    logger.InfoFormat(
                        "New user to DB: lmsCompanyId={0}, UserId={1}",
                        lmsCompany.Id, dbUser.UserId);
                    lmsUserModel.RegisterSave(dbUser);
                }
                dbUser.Name = lmsUserDto.name;
                dbUser.Email = lmsUserDto.primary_email;
                dbUser.UserIdExtended = lmsUserDto.lti_id;
                // todo: save lmsUserDto.id to dbUser.UserId
            }

            return newUsers;
        }

        private void UpdateACRoles(LmsCompany lmsCompany, LmsCourseMeeting meeting, AdobeConnectProvider acProvider)
        {
            string error = null;
            var dbUsers =
                meeting.MeetingRoles.Select(x => x.User);
            try
            {
                usersSetup.SetDefaultRolesForNonParticipants(
                    lmsCompany,
                    acProvider,
                    meeting,
                    meeting.MeetingRoles.Select(x => new LmsUserDTO
                    {
                        ac_id = x.User.PrincipalId,
                        id = x.User.UserId,
                        lti_id = x.User.UserId,
                        login_id = x.User.Username,
                        name = x.User.Name,
                        primary_email = x.User.Email,
                        lms_role = x.LmsRole
                    }),
                    dbUsers,
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