using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Castle.Core.Logging;
using EdugameCloud.Lti.API;
using EdugameCloud.Lti.API.AdobeConnect;
using EdugameCloud.Lti.Core.Business.Models;
using EdugameCloud.Lti.Core.Domain.Entities;
using EdugameCloud.Lti.Domain.Entities;
using EdugameCloud.Lti.DTO;
using Esynctraining.Core.Utils;

namespace EdugameCloud.Lti.LmsUserUpdater
{
    class Program
    {
        static void Main(string[] args)
        {
            const string neMutexName = "EdugameCloud.Lti.LmsUserUpdater.BackgroundMutexName";

            // prevent two instances from running
            bool created;

            using (Mutex m = new Mutex(true, neMutexName, out created))
            {
                IoCStart.Init();
                ILogger logger = IoC.Resolve<ILogger>();

                if (!created)
                {
                    logger.Info("The application EdugameCloud.Lti.LmsUserUpdater could not be run because another instance was already running.");
                }

                try
                {
                    LmsFactory lmsFactory = IoC.Resolve<LmsFactory>();
                    var lmsUserModel = IoC.Resolve<LmsUserModel>();
                    var lmsCourseMeetingModel = IoC.Resolve<LmsCourseMeetingModel>();
                    var lmsCompanyModel = IoC.Resolve<LmsCompanyModel>();
                    var meetingSetup = IoC.Resolve<IMeetingSetup>();
                    var userSetup = IoC.Resolve<UsersSetup>();

                    logger.Info("===== Update Lms Users Engine Starts =====");
//                    var meetings = lmsCourseMeetingModel.GetAllWithLmsCompany();
                    var companies = lmsCompanyModel.GetAllWithLmsCourseMeeting();
                    var groupedByCompany = companies.GroupBy(x => x.LmsProvider.Id);//.ToDictionary(x => x.Key, y => y.SelectMany(z=>z.LmsCourseMeetings).GroupBy(c => new CourseCompany { CourseId = c.CourseId, LmsCompanyId = c.LmsCompany.Id }));

                    //todo: Task for each lms if possible
                    foreach (var group in groupedByCompany)
                    {
                        var service = lmsFactory.GetUserService((LmsProviderEnum)group.Key);
                        if (service != null)
                        {
                            foreach (var lmsCompany in group)
                            {
//                                var lmsCompany = lmsCompany.First().LmsCompany;
                                if (lmsCompany.UseSynchronizedUsers && service.CanRetrieveUsersFromApiForCompany(lmsCompany)
                                    && lmsCompany.LmsCourseMeetings != null && lmsCompany.LmsCourseMeetings.Any(x => x.LmsMeetingType != (int)LmsMeetingType.OfficeHours))
                                {
                                    var acProvider = meetingSetup.GetProvider(lmsCompany);
                                    foreach (var courseGroup in lmsCompany.LmsCourseMeetings
                                        .Where(x => x.LmsMeetingType != (int)LmsMeetingType.OfficeHours)
                                        .ToList()
                                        .GroupBy(y => y.CourseId))
                                    {
                                        logger.InfoFormat("Retrieving users for LmsCompanyId={0}, LmsProvider={1}, CourseId={2}",
                                            lmsCompany.Id, (LmsProviderEnum)group.Key, courseGroup.Key);
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
                                                            lmsCompany.Id).ToList();
                                                    var newUsers = new List<LmsUser>();
                                                    foreach (var lmsUserDto in opResult.data)
                                                    {
                                                        var dbUser =
                                                            existedDbUsers.FirstOrDefault(
                                                                u =>
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
                                                        }
//                                                        dbUser.LmsRole = lmsUserDto.lms_role;
                                                        dbUser.Name = lmsUserDto.name;
                                                        dbUser.Email = lmsUserDto.primary_email;
                                                        dbUser.UserIdExtended = lmsUserDto.lti_id;
                                                        // todo: save lmsUserDto.id to dbUser.UserId
                                                    }
                                                    // merge results;
                                                    foreach (var meeting in courseGroup)
                                                    {
                                                        var userRolesToDelete =
                                                            meeting.MeetingRoles//.Select(x => x.User)
                                                                .Where(x => existedDbUsers.All(u => u.Id != x.User.Id)).ToList();

                                                        var usersToAddToMeeting = new List<LmsUser>(newUsers);
                                                        usersToAddToMeeting.AddRange(
                                                            existedDbUsers.Where(
                                                                x => meeting.MeetingRoles.Select(mr => mr.User).All(u => u.Id != x.Id)).ToList());

                                                        logger.InfoFormat(
                                                            "LmsUser ids to delete from meetingId={0}, courseId={1}: {2}",
                                                            meeting.Id, meeting.CourseId,
                                                            String.Join(",", userRolesToDelete.Select(x=>x.User.Id)));
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
                                                        // todo: optimize
                                                        if (userRolesToDelete.Any() || usersToAddToMeeting.Any())
                                                        {
                                                            string error = null;
                                                            var dbUsers =
                                                                meeting.MeetingRoles.Select(x => x.User).ToList();
                                                            userSetup.SetDefaultRolesForNonParticipants(
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
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    string msg = "Unexpected error during execution LmsUserUpdater with message: " + ex.Message;
                    logger.Error(msg, ex);
                }
                finally
                {
                    logger.Info("===== Update Lms Users Engine stops =====");
                }
            }
        }
    }
}
