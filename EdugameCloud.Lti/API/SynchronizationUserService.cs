using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using EdugameCloud.Lti.API.AdobeConnect;
using EdugameCloud.Lti.Core.Business.Models;
using EdugameCloud.Lti.Core.Domain.Entities;
using EdugameCloud.Lti.Domain.Entities;
using EdugameCloud.Lti.DTO;
using EdugameCloud.Lti.Extensions;
using Esynctraining.AC.Provider.Entities;
using Esynctraining.AdobeConnect;
using Esynctraining.Core.Logging;
using Esynctraining.Core.Utils;

namespace EdugameCloud.Lti.API
{
    public class SynchronizationUserService : ISynchronizationUserService
    {
        private readonly LmsFactory lmsFactory;
        private readonly AdobeConnect.IAdobeConnectAccountService acAccountService;
        private readonly UsersSetup usersSetup;
        private readonly LmsUserModel lmsUserModel;
        private readonly LmsCompanyModel lmsCompanyModel;
        private readonly LmsCourseMeetingModel lmsCourseMeetingModel;
        private readonly IAdobeConnectUserService acUserService;
        private readonly ILogger logger;


        public SynchronizationUserService(LmsFactory lmsFactory, AdobeConnect.IAdobeConnectAccountService acAccountService, UsersSetup usersSetup,
            LmsUserModel lmsUserModel, LmsCompanyModel lmsCompanyModel,
            LmsCourseMeetingModel lmsCourseMeetingModel, IAdobeConnectUserService acUserService, ILogger logger)
        {
            this.lmsFactory = lmsFactory;
            this.acAccountService = acAccountService;
            this.usersSetup = usersSetup;
            this.lmsUserModel = lmsUserModel;
            this.lmsCompanyModel = lmsCompanyModel;
            this.lmsCourseMeetingModel = lmsCourseMeetingModel;
            this.acUserService = acUserService;
            this.logger = logger;
        }


        public void SynchronizeUsers(ILmsLicense lmsCompany, bool syncACUsers, IEnumerable<int> meetingIds = null)
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
//            var meetings = lmsCompany.LmsCourseMeetings.ToList();
            var scoIds = new HashSet<string>(lmsCompany.LmsCourseMeetings.Select(x => x.GetMeetingScoId())).ToList();
            var scos = acProvider.ReportBulkObjects(scoIds).Values;
            var settings = lmsCompany.Settings.ToList(); //to avoid nhibernate errors
            var groupedMeetings = lmsCompany.LmsCourseMeetings
                .Where(x =>
                    (meetingIds == null || meetingIds.Any(m => m == x.Id))
                    && scos.Any(s => s.ScoId == x.ScoId))
                .GroupBy(y => y.CourseId);
            List<LmsUser> users = lmsCompany.LmsUsers.ToList();//meetingIds == null ? lmsCompany.LmsUsers.ToList() : null;

            object localLockObject = new object();
            Dictionary<int, IEnumerable<LmsUserDTO>> licenseUsers = new Dictionary<int, IEnumerable<LmsUserDTO>>();
//            var input = meetings.Select(x => new Tuple<LmsCourseMeeting, string>(x, x.GetMeetingScoId())).ToList();
            var timer = Stopwatch.StartNew();
//synchronous version
            foreach (var groupedMeeting in groupedMeetings)
            {
                var courseId = groupedMeeting.Key;
                var opResult = service.GetUsers(lmsCompany, courseId);
                if (opResult.IsSuccess)
                {
                    licenseUsers.Add(courseId, opResult.Data);
                }
                else
                {
                    licenseUsers.Add(courseId, new List<LmsUserDTO>());
                }

            }
//parallel version
            //            Parallel.ForEach<int, Dictionary<int, IEnumerable<LmsUserDTO>>>(
            //                  groupedMeetings.Select(x => x.Key),
            //                  () => new Dictionary<int, IEnumerable<LmsUserDTO>>(),
            //                  (courseId, state, localDictionary) =>
            //                  {
            //                  var opResult = service.GetUsers(lmsCompany, lmsCompany.AdminUser, courseId);
            //                      if (opResult.IsSuccess)
            //                      {
            //                          localDictionary.Add(courseId, opResult.Data);
            //                      }
            //                      else
            //                      {
            //                          localDictionary.Add(courseId, new List<LmsUserDTO>());
            //                      }
            //
            //                      return localDictionary;
            //                  },
            //                  (finalResult) =>
            //                  {
            //                      lock (localLockObject)
            //                          foreach (var item in finalResult)
            //                          {
            //                              licenseUsers.Add(item.Key, item.Value);
            //                          }
            //                  }
            //            );
            timer.Stop();
            logger.Warn($"Users from API elapsed seconds:{timer.Elapsed.ToString()}");
            acProvider = acAccountService.GetProvider(lmsCompany); //users retrieve can take more than session timeout

            foreach (var courseGroup in groupedMeetings)
            {
               logger.InfoFormat("Retrieving users for LmsCompanyId={0}, LmsProvider={1}, CourseId={2}; MeetingIds:{3}",
                    lmsCompany.Id, (LmsProviderEnum)lmsCompany.LmsProviderId, courseGroup.Key, String.Join(",", courseGroup.Select(x=>x.Id)));
                try
                {
                    //todo: set extra data param
//                    var opResult = service.GetUsers(lmsCompany, lmsCompany.AdminUser, courseGroup.Key);
//                    if (opResult.IsSuccess)
//                    {
                        var usersCount = licenseUsers[courseGroup.Key].Count();
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
                            var userIds = licenseUsers[courseGroup.Key].Select(x => x.LtiId ?? x.Id);
//                        logger.InfoFormat("API user ids: {0}", String.Join(",", userIds));
                            var existedDbUsers = users?.Where(x => licenseUsers[courseGroup.Key].Any(u => (u.LtiId ?? u.Id) == x.UserId)) 
                                ?? lmsUserModel.GetByUserIdAndCompanyLms(userIds.ToArray(), lmsCompany.Id);

                            var newUsers = UpdateDbUsers(licenseUsers[courseGroup.Key].ToList(), lmsCompany, users, acProvider);
                            users.AddRange(newUsers);
                            // merge results;
                            foreach (var meeting in courseGroup)
                            {
                                var existedUserIds = existedDbUsers.Select(x => x.Id).ToList();
                                var userRolesToDelete =
                                    meeting.MeetingRoles.Where(x => existedUserIds.All(u => u != x.User.Id)).ToList();

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
                                    LmsRole =
                                        licenseUsers[courseGroup.Key].First(dto => x.UserId == (dto.LtiId ?? dto.Id))
                                            .LmsRole
                                }));
                                meeting.EnableDynamicProvisioning = false;
                                lmsCourseMeetingModel.RegisterSave(meeting, true);
                                // todo: optimize condition, probably refresh roles not for all users
                                var dbPrincipalIds = new HashSet<string>(
                                    meeting.MeetingRoles.Where(x => x.User.PrincipalId != null).Select(x => x.User.PrincipalId));
                                List<MeetingPermissionInfo> enrollments = usersSetup.GetMeetingAttendees(acProvider, meeting.GetMeetingScoId());
                                var acPrincipalIds = new HashSet<string>(enrollments.Select(e => e.PrincipalId));

                                if (syncACUsers 
                                    && (meeting.LmsMeetingType == (int)LmsMeetingType.Meeting || meeting.LmsMeetingType == (int)LmsMeetingType.VirtualClassroom || meeting.LmsMeetingType == (int)LmsMeetingType.Seminar)
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
//                    }
                }

                catch (Exception e)
                {
                    var message = String.Format(
                        $"Error happened when tried to update users for LmsCompany with id={lmsCompany.Id}, lmsCourseId={courseGroup.Key}");
                    logger.Error(message, e);
                }
            }
        }


        private IEnumerable<LmsUser> UpdateDbUsers(List<LmsUserDTO> lmsUserDtos, ILmsLicense lmsCompany,
            IEnumerable<LmsUser> existedDbUsers, IAdobeConnectProxy provider)
        {
            var newUsers = new List<LmsUser>();

            var company = lmsCompanyModel.GetOneById(lmsCompany.Id).Value;

            foreach (var lmsUserDto in lmsUserDtos)
            {
                var dbUser = existedDbUsers.FirstOrDefault(u =>
                    (lmsUserDto.LtiId != null && u.UserId == lmsUserDto.LtiId) || u.UserId == lmsUserDto.Id);
                if (dbUser == null)
                {
                    string login = lmsUserDto.GetLogin();
                    Principal principal = null;
                    try
                    {
                        principal = acUserService.GetOrCreatePrincipal(provider, login, lmsUserDto.PrimaryEmail,
                            lmsUserDto.GetFirstName(),
                            lmsUserDto.GetLastName(), lmsCompany);
                    }
                    catch (Exception ex)
                    {
                        logger.Error("SyncUsers - GetOrCreatePrincipal", ex);
                    }

                    var loginLength = login.Length > 50 ? 50 : login.Length;

                    dbUser = new LmsUser
                    {
                        LmsCompany = company,
                        Username = login.Substring(0, loginLength), // hack: to escape GenericADOException when name>50 //todo: review lmsUserDto.GetLogin() and lmsUser.Username usage
                        UserId = lmsUserDto.LtiId ?? lmsUserDto.Id,
                        PrincipalId = principal?.PrincipalId,
                    };
                    newUsers.Add(dbUser);
                    logger.InfoFormat(
                        "New user to DB: lmsCompanyId={0}, UserId={1}, Username={2}, Name={3}, Email={4}",
                        lmsCompany.Id, dbUser.UserId, login, lmsUserDto.Name, lmsUserDto.PrimaryEmail);
                    lmsUserModel.RegisterSave(dbUser);
                }
                dbUser.Name = lmsUserDto.Name;
                dbUser.Email = lmsUserDto.PrimaryEmail;
                dbUser.UserIdExtended = lmsUserDto.LtiId != null ? lmsUserDto.Id : null;
                // todo: save lmsUserDto.id to dbUser.UserId
            }

            return newUsers;
        }

        private void UpdateACRoles(ILmsLicense lmsCompany, LmsCourseMeeting meeting,
            IAdobeConnectProxy acProvider, List<MeetingPermissionInfo> enrollments)
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
                        AcId = x.User.PrincipalId,
                        Id = x.User.UserIdExtended ?? x.User.UserId,
                        LtiId = x.User.UserId,
                        Login = x.User.Username,
                        Name = x.User.Name,
                        PrimaryEmail = x.User.Email,
                        LmsRole = x.LmsRole
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