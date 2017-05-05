namespace EdugameCloud.Lti.API.AdobeConnect
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using EdugameCloud.Lti.Core.Business;
    using EdugameCloud.Lti.Core.Business.Models;
    using EdugameCloud.Lti.Core.Domain.Entities;
    using EdugameCloud.Lti.Domain.Entities;
    using EdugameCloud.Lti.DTO;
    using EdugameCloud.Lti.Extensions;
    using Esynctraining.AC.Provider.DataObjects.Results;
    using Esynctraining.AC.Provider.Entities;
    using Esynctraining.AdobeConnect;
    using Esynctraining.AdobeConnect.Api.Meeting;
    using Esynctraining.Core;
    using Esynctraining.Core.Extensions;
    using Esynctraining.Core.Logging;
    using Esynctraining.Core.Providers;
    using Esynctraining.Core.Utils;
    using Esynctraining.Crypto;

    public sealed class UsersSetup : IUsersSetup
    {
        #region Inner Class: MeetingAttendees

        private sealed class MeetingAttendees
        {
            public List<MeetingPermissionInfo> Hosts { get; set; }

            public List<MeetingPermissionInfo> Presenters { get; set; }

            public List<MeetingPermissionInfo> Participants { get; set; }

            public bool Contains(string principalId)
            {
                return Hosts.Any(x => x.PrincipalId == principalId)
                       || Presenters.Any(x => x.PrincipalId == principalId)
                       || Participants.Any(x => x.PrincipalId == principalId);
            }

        }

        #endregion Inner Class: MeetingAttendees

        #region Fields
        
        private readonly dynamic settings;
        private readonly IAdobeConnectUserService acUserService;
        private readonly IAdobeConnectAccountService acAccountService;
        private readonly ILogger logger;
        private LmsFactory lmsFactory;

        #endregion

        private LmsFactory LmsFactory
        {
            get
            {
                return lmsFactory ?? (lmsFactory = IoC.Resolve<LmsFactory>());
            }
        }

        private LmsCompanyModel LmsCompanyModel => IoC.Resolve<LmsCompanyModel>();

        #region Constructors and Destructors

        public UsersSetup(
            ApplicationSettingsProvider settings,
            IAdobeConnectUserService acUserService,
            IAdobeConnectAccountService acAccountService,
            ILogger logger)
        {
            this.settings = settings;
            this.acUserService = acUserService;
            this.acAccountService = acAccountService;
            this.logger = logger;
        }

        #endregion

        #region Properties
        
        private LmsCourseMeetingModel LmsCourseMeetingModel
        {
            get { return IoC.Resolve<LmsCourseMeetingModel>(); }
        }
        
        private LmsUserModel LmsUserModel
        {
            get { return IoC.Resolve<LmsUserModel>(); }
        }

        #endregion

        #region Public Methods and Operators

        public void AddUsersToMeetingHostsGroup(IAdobeConnectProxy provider, IEnumerable<string> principalIds, PrincipalType principalType = PrincipalType.live_admins)
        {
            if (principalIds.Any())
            {
                var groupPrincipal = provider.GetGroupsByType(principalType).Values.Single();
                var groupParticipants = provider.GetGroupUsers(groupPrincipal.PrincipalId);
                var usersToAdd = principalIds.Where(x => groupParticipants.Values.All(gp => gp.PrincipalId != x));
                try
                {
                    if(usersToAdd.Any())
                        provider.AddToGroupByType(usersToAdd, principalType);
                }
                catch (AdobeConnectException ex)
                {
                    if (ex.Status.Code == StatusCodes.invalid && ex.Status.SubCode == StatusSubCodes.missing && ex.Status.InvalidField == "name")
                    {
                        logger.Warn("AC failed to add some users to MeetingHosts group. Trying to add them one-by-one...");
                        foreach (string principal in usersToAdd)
                        {
                            try
                            {
                                provider.AddToGroupByType(principal, principalType);
                            }
                            catch (AdobeConnectException)
                            {
                                // TRICK: do nothing. exception was logged by IAdobeConnectProxy
                            }
                        }
                    }
                    else
                    {
                        throw;
                    }
                }
            }
        }

        public bool SetACPassword(IAdobeConnectProxy provider, ILmsLicense lmsCompany, 
            LmsUser lmsUser, LtiParamDTO param, string adobeConnectPassword)
        {
            if (!string.IsNullOrWhiteSpace(adobeConnectPassword))
            {
                Principal registeredUser = null;
                if (lmsUser.PrincipalId != null)
                {

                    var principalInfo = !string.IsNullOrWhiteSpace(lmsUser.PrincipalId)
                        ? provider.GetOneByPrincipalId(lmsUser.PrincipalId).PrincipalInfo
                        : null;
                    registeredUser = principalInfo != null ? principalInfo.Principal : null;
                }

                if (registeredUser != null)
                {
                    var loginResult = acAccountService.LoginIntoAC(lmsCompany, param, registeredUser, 
                        adobeConnectPassword, provider, updateAcUser: false);

                    if (loginResult != null)
                    {
                        ResetUserACPassword(lmsUser, adobeConnectPassword);
                        return true;
                    }
                }
            }

            return false;
        }

        public void ResetUserACPassword(LmsUser lmsUser, string password)
        {
            if (!string.IsNullOrEmpty(password))
            {
                var sharedKey = AESGCM.NewKey();
                lmsUser.SharedKey = Convert.ToBase64String(sharedKey);
                lmsUser.ACPasswordData = AESGCM.SimpleEncrypt(password, sharedKey);
                LmsUserModel.RegisterSave(lmsUser);
            }
        }

        public List<LmsUserDTO> GetLMSUsers(
            ILmsLicense lmsCompany, 
            LmsCourseMeeting meeting, 
            int courseId, 
            out string error,
            LtiParamDTO extraData = null)
        {
            if (lmsCompany.UseSynchronizedUsers && meeting != null && meeting.MeetingRoles != null
                && !meeting.EnableDynamicProvisioning)
            {
                var dbUserMeetingRoles = GetUserMeetingRoles(meeting);
                var userDtos = dbUserMeetingRoles.Select(x => new LmsUserDTO
                {
                    AcId = x.User.PrincipalId,
                    Id = x.User.UserIdExtended ?? x.User.UserId,
                    LtiId = x.User.UserId,
                    Login = x.User.Username,
                    Name = x.User.Name,
                    PrimaryEmail = x.User.Email,
                    LmsRole = x.LmsRole,
                });

                if (userDtos.Any())
                {
                    error = null;
                    return userDtos.ToList();
                }
            }
            var service = LmsFactory.GetUserService((LmsProviderEnum)lmsCompany.LmsProviderId);
            var serviceResult = service.GetUsers(lmsCompany, courseId, extraData);
            if (serviceResult.IsSuccess)
            {
                error = null;
                return serviceResult.Data;
            }
            logger.WarnFormat("[GetLMSUsers] Running old style retrieve method. LmsCompanyId={0}, MeetingId={1}, " +
                "courseId={2}", lmsCompany.Id, meeting.Return(x=>x.Id, 0), courseId);
            var users = service.GetUsersOldStyle(lmsCompany, courseId, out error, extraData);
            return error == null ? users : new List<LmsUserDTO>();
        }

        public IEnumerable<LmsUserMeetingRole> GetUserMeetingRoles(LmsCourseMeeting meeting)
        {
            return meeting.MeetingRoles
                    .GroupBy(x => x.User.UserId).Select(x => x.OrderBy(u => u.User.Id).First());
        }
        
        public List<MeetingPermissionInfo> GetMeetingAttendees(IAdobeConnectProxy provider, string meetingSco)
        {
            var alreadyAdded = new HashSet<string>();
            MeetingPermissionCollectionResult allMeetingEnrollments = provider.GetAllMeetingEnrollments(meetingSco);
            List<MeetingPermissionInfo> allValues = allMeetingEnrollments.Values.Return(
                x => x.ToList(), 
                new List<MeetingPermissionInfo>());
            foreach (MeetingPermissionInfo g in allValues)
            {
                alreadyAdded.Add(g.PrincipalId);
            }

            return ProcessACMeetingAttendees(new HashSet<string>(), provider, allValues, alreadyAdded);
        }

        public string GetParamLogin(LtiParamDTO param, LmsCompany lmsCompany)
        {
            var login = param.lms_user_login;
            if (string.IsNullOrWhiteSpace(login) && !string.IsNullOrEmpty(param.lms_user_id))
            {
                // TODO: for D2L more effective would be to get WhoIAm and UserInfo information from their API
                string error;
                var lmsUserService = LmsFactory.GetUserService((LmsProviderEnum) lmsCompany.LmsProviderId);
                //d2l service returns not-empty error parameter in some cases, but it's acceptable here
                LmsUserDTO user = lmsUserService.GetUser(lmsCompany,
                    param.lms_user_id,
                    param.course_id,
                    out error,
                    param);

                if (user != null)
                {
                    login = user.Login;
                }
            }

            return login;
        }

        public IList<LmsUserDTO> GetUsers(
            ILmsLicense lmsCompany,
            IAdobeConnectProxy provider,
            int courseId,
            LtiParamDTO param, 
            long id, 
            out string error,
            List<LmsUserDTO> users = null)
        {
            LmsCourseMeeting meeting = this.LmsCourseMeetingModel.GetOneByCourseAndId(
                lmsCompany.Id,
                courseId, 
                id);

            if (meeting == null)
            {
                logger.ErrorFormat("[GetUsers] Meeting not found in DB. LmsCompanyID: {0}. CourseID: {1}. ID: {2}.", 
                    lmsCompany.Id,
                    courseId,
                    id);

                error = Resources.Messages.MeetingNotFound;
                return new List<LmsUserDTO>();
            }

            // TRICK: not to have nhibernate 'no session or session was closed' error later in the method
            if (meeting.MeetingGuests != null)
            {
                 var guests = meeting.MeetingGuests.ToList();
            }
            if (users == null || !users.Any())
            {
                users = this.GetLMSUsers(
                lmsCompany,
                meeting,
                courseId,
                out error,
                param).ToList();

                if (error != null)
                {
                    throw new InvalidOperationException("Lms users");
                }

            }
               
            var nonEditable = new HashSet<string>();
            MeetingAttendees attendees = GetMeetingAttendees(
                provider, 
                meeting.GetMeetingScoId(),
                nonEditable);

            string[] userIds = users.Select(user => user.LtiId ?? user.Id).ToArray();
            IEnumerable<LmsUser> lmsUsers = null;
            if (lmsCompany.UseSynchronizedUsers && meeting.MeetingRoles != null)
            {
                lmsUsers = GetUserMeetingRoles(meeting).Select(x => x.User);
                //when users where not synchronized yet
                if (!lmsUsers.Any())
                {
                    lmsUsers = this.LmsUserModel.GetByUserIdAndCompanyLms(userIds, lmsCompany.Id);
                }
            }
            else
            {
                lmsUsers = this.LmsUserModel.GetByUserIdAndCompanyLms(userIds, lmsCompany.Id);
            }

            var usersCount = users.Count;
            // Debug.Assert(userIds.Length == lmsUsers.Count(), "Should return single user by userId+lmsCompany.Id");

            // NOTE: sometimes we have no users here - for example due any security issue in LMS service (BlackBoard)
            // So skip this step for better performance
            if (usersCount > 0)
            {
                IEnumerable<Principal> principalCache = this.GetAllPrincipals(lmsCompany, provider, users);

                bool uncommitedChangesInLms = false;

                string message;
                List<LmsUserDTO> usersToAddToMeeting = GetUsersToAddToMeeting(lmsCompany, users, out message);

                var company = LmsCompanyModel.GetOneById(lmsCompany.Id).Value;

                foreach (LmsUserDTO user in users)
                {
                    string login = user.GetLogin();
                    LmsUser lmsUser = lmsUsers.FirstOrDefault(u => u.UserId == (user.LtiId ?? user.Id));
                    if (usersCount <= EdugameCloud.Lti.Core.Utils.Constants.SyncUsersCountLimit)
                    {
                        if (lmsUser == null)
                        {
                            lmsUser = new LmsUser
                            {
                                LmsCompany = company,
                                Username = login,
                                UserId = user.LtiId ?? user.Id,
                            };
                        }

                        if (string.IsNullOrEmpty(lmsUser.PrincipalId)
                            && usersToAddToMeeting.Contains(user))
                        {
                            // NOTE: we create Principals during Users/GetAll to have ability to join office hours meeting for all course participants.
                            Principal principal = null;
                            try
                            {
                                principal = acUserService.GetOrCreatePrincipal2(
                                    provider,
                                    login,
                                    user.GetEmail(),
                                    user.GetFirstName(),
                                    user.GetLastName(),
                                    lmsCompany,
                                    principalCache);
                            }
                            catch (Exception ex)
                            {
                                logger.Error("GetUsers - GetOrCreatePrincipal", ex);
                            }
                            if (principal != null)
                            {
                                lmsUser.PrincipalId = principal.PrincipalId;
                                this.LmsUserModel.RegisterSave(lmsUser, flush: false);
                                uncommitedChangesInLms = true;
                            }
                        }
                    }
                    //var principalInfo = provider.GetOneByPrincipalId(lmsUser.PrincipalId);

                    if (string.IsNullOrEmpty(lmsUser?.PrincipalId))
                    {
                        new RoleMappingService().CheckAndSetNoneACMapping(user, lmsCompany);
                        continue;
                    }

                    user.AcId = lmsUser.PrincipalId;
                    user.IsEditable = !nonEditable.Contains(user.AcId);

                    if (attendees.Hosts.Any(v => v.PrincipalId == user.AcId))
                    {
                        user.AcRole = AcRole.Host.Id;
                        attendees.Hosts = attendees.Hosts.Where(v => v.PrincipalId != user.AcId).ToList();
                    }
                    else if (attendees.Presenters.Any(v => v.PrincipalId == user.AcId))
                    {
                        user.AcRole = AcRole.Presenter.Id;
                        attendees.Presenters = attendees.Presenters.Where(v => v.PrincipalId != user.AcId).ToList();
                    }
                    else if (attendees.Participants.Any(v => v.PrincipalId == user.AcId))
                    {
                        user.AcRole = AcRole.Participant.Id;
                        attendees.Participants = attendees.Participants.Where(v => v.PrincipalId != user.AcId).ToList();
                    }
                    else
                    {
                        new RoleMappingService().CheckAndSetNoneACMapping(user, lmsCompany);
                    }
                }

                if (uncommitedChangesInLms)
                {
                    this.LmsUserModel.Flush();
                }
            }

            ProcessGuests(users, meeting, attendees.Hosts.Where(h => !h.HasChildren), AcRole.Host);
            ProcessGuests(users, meeting, attendees.Presenters.Where(h => !h.HasChildren), AcRole.Presenter);
            ProcessGuests(users, meeting, attendees.Participants.Where(h => !h.HasChildren), AcRole.Participant);

            //TRICK: we need email on client side only if AC uses emails as login!!
            // we use emails to check they are not empty and are unique within a course
            if (lmsCompany.ACUsesEmailAsLogin.GetValueOrDefault())
            {
                users.ForEach(x => 
                {
                    x.Email = x.PrimaryEmail;
                });
            }
            
            error = null;
            return users;
        }
        
        private static void ProcessGuests(IList<LmsUserDTO> users, LmsCourseMeeting meeting, IEnumerable<MeetingPermissionInfo> permissions, AcRole role)
        {
            foreach (MeetingPermissionInfo permissionInfo in permissions)
            {

                LmsCourseMeetingGuest guest = meeting.MeetingGuests.FirstOrDefault(x => x.PrincipalId == permissionInfo.PrincipalId);
                if (guest != null)
                {
                    users.Add(
                        new LmsUserDTO
                        {
                            GuestId = guest.Id,
                            AcId = guest.PrincipalId,
                            Name = permissionInfo.Name,
                            AcRole = role.Id,
                        });
                }
                else
                {
                    users.Add(
                        new LmsUserDTO
                        {
                            AcId = permissionInfo.PrincipalId,
                            Name = permissionInfo.Name,
                            AcRole = role.Id,
                        });
                }
            }
        }

        public LmsUserDTO GetOrCreateUserWithAcRole(
            ILmsLicense lmsCompany,
            IAdobeConnectProxy provider,
            LtiParamDTO param,
            LmsCourseMeeting meeting,
            out string error,
            string lmsUserId = null)
        {
            var service = LmsFactory.GetUserService((LmsProviderEnum)lmsCompany.LmsProviderId);
            //todo: not param for BrainHoney
            var lmsUser = service.GetUser(lmsCompany, lmsUserId, param.course_id, out error, param);

            if (meeting == null)
            {
                return lmsUser;
            }

            if (lmsUser != null)
            {
                var lmsDbUser = this.LmsUserModel.GetOneByUserIdAndCompanyLms(lmsUser.LtiId ?? lmsUser.Id, lmsCompany.Id).Value;
                var nonEditable = new HashSet<string>();
                MeetingAttendees attendees = GetMeetingAttendees(
                    provider,
                    meeting.GetMeetingScoId(),
                    nonEditable);
                IEnumerable<Principal> principalCache = GetAllPrincipals(lmsCompany, provider, 
                    new List<LmsUserDTO>{lmsUser});

                bool uncommitedChangesInLms = false;
                ProcessLmsUserDtoAcInfo(lmsUser, lmsDbUser, lmsCompany, principalCache, provider, ref uncommitedChangesInLms,
                    nonEditable, attendees);

                if (uncommitedChangesInLms)
                {
                    LmsUserModel.Flush();
                }

                return lmsUser;
            }

            return null;
        }

        private void ProcessLmsUserDtoAcInfo(LmsUserDTO user, LmsUser lmsDbUser, ILmsLicense lmsCompany,
            IEnumerable<Principal> principalCache, IAdobeConnectProxy provider, ref bool uncommitedChangesInLms,
            HashSet<string> nonEditable, MeetingAttendees attendees)
        {
            string login = user.GetLogin();
            lmsDbUser = lmsDbUser ?? new LmsUser
            {
                LmsCompany = LmsCompanyModel.GetOneById(lmsCompany.Id).Value,
                Username = login,
                UserId = user.LtiId ?? user.Id,
            };

            if (string.IsNullOrEmpty(lmsDbUser.PrincipalId))
            {
                Principal principal = acUserService.GetOrCreatePrincipal2(
                    provider,
                    login,
                    user.GetEmail(),
                    user.GetFirstName(),
                    user.GetLastName(),
                    lmsCompany,
                    principalCache);

                if (principal != null)
                {
                    lmsDbUser.PrincipalId = principal.PrincipalId;
                    this.LmsUserModel.RegisterSave(lmsDbUser, flush: false);
                    uncommitedChangesInLms = true;
                }
                else
                {
                    return;
                }
            }

            user.AcId = lmsDbUser.PrincipalId;
            user.IsEditable = !nonEditable.Contains(user.AcId);

            if (attendees.Hosts.Any(v => v.PrincipalId == user.AcId))
            {
                user.AcRole = AcRole.Host.Id;
            }
            else if (attendees.Presenters.Any(v => v.PrincipalId == user.AcId))
            {
                user.AcRole = AcRole.Presenter.Id;
            }
            else if (attendees.Participants.Any(v => v.PrincipalId == user.AcId))
            {
                user.AcRole = AcRole.Participant.Id;
            }
        }

        /// <summary>
        /// The is teacher.
        /// </summary>
        /// <param name="param">
        /// The parameter.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool IsTeacher(LtiParamDTO param)
        {
            return new LmsRoleService(this.settings).IsTeacher(param);
        }

        public List<LmsUserDTO> SetDefaultRolesForNonParticipants(
            ILmsLicense lmsCompany,
            IAdobeConnectProxy provider,
            LmsCourseMeeting meeting,
            IEnumerable<LmsUserDTO> users,
            IEnumerable<LmsUser> lmsDbUsers,
            List<MeetingPermissionInfo> enrollments,
            ref string error)
        {
            string meetingSco = meeting.GetMeetingScoId();

            bool denyACUserCreation = lmsCompany.DenyACUserCreation;

            var meetingPermissions = new List<MeetingPermissionUpdateTrio>();
            var hostPrincipals = new List<string>();

            var principalIds = new HashSet<string>(enrollments.Select(e => e.PrincipalId));
            
            List<LmsUserDTO> usersToAddToMeeting = GetUsersToAddToMeeting(lmsCompany, users, out error);

            var company = LmsCompanyModel.GetOneById(lmsCompany.Id).Value;

            foreach (LmsUserDTO lmsUserDto in usersToAddToMeeting)
            {
                // TRICK: we can filter by 'UserId' - cause we sync it in 'getUsersByLmsCompanyId' SP
                string id = lmsUserDto.LtiId ?? lmsUserDto.Id;
                LmsUser dbUser = lmsDbUsers.FirstOrDefault(x => x.UserId == id)
                    ?? new LmsUser { LmsCompany = company, Username = lmsUserDto.GetLogin(), UserId = id, };

                if (string.IsNullOrEmpty(dbUser.PrincipalId))
                {
                    string err;
                    Principal principal = CreatePrincipalAndUpdateLmsUserPrincipalId(provider, lmsUserDto, dbUser, lmsCompany, out err);
                    if (!string.IsNullOrWhiteSpace(err))
                    {
                        error = err;
                    }
                    if (principal == null && denyACUserCreation)
                    {
                        if (!error.Contains(Resources.Messages.CreateAcPrincipalManually))
                            error += " " + Resources.Messages.CreateAcPrincipalManually;
                        continue;
                    }
                }

                // TRICK: dbUser.PrincipalId can be updated within CreatePrincipalAndUpdateLmsUserPrincipalId
                lmsUserDto.AcId = dbUser.PrincipalId;

                MeetingPermissionInfo enrollment = enrollments.FirstOrDefault(e => e.PrincipalId.Equals(dbUser.PrincipalId));

                if (enrollment != null)
                {
                    new RoleMappingService().CheckAndSetNoneACMapping(lmsUserDto, lmsCompany);
                    if(lmsUserDto.AcRole == AcRole.None.Id)
                    {
                        meetingPermissions.Add(new MeetingPermissionUpdateTrio
                        {
                            ScoId = meetingSco,
                            PrincipalId = dbUser.PrincipalId,
                            PermissionId = AcRole.None.MeetingPermissionId
                        });
                    }
                    else
                    {
                        lmsUserDto.AcRole = AcRole.GetRoleId(enrollment.PermissionId);
                    }

                    principalIds.Remove(dbUser.PrincipalId);
                }
                else if (!string.IsNullOrEmpty(dbUser.PrincipalId))
                {
                    // NOTE: check that Principal is in AC yet
                    var principalInfo = provider.GetOneByPrincipalId(dbUser.PrincipalId);
                    if (!principalInfo.Success)
                    {
                        string err;
                        var principal = CreatePrincipalAndUpdateLmsUserPrincipalId(provider, lmsUserDto, dbUser, lmsCompany, out err);
                        if (!string.IsNullOrWhiteSpace(err))
                        {
                            error = err;
                        }

                        if (principal == null && denyACUserCreation)
                        {
                            if (!error.Contains(Resources.Messages.CreateAcPrincipalManually))
                                error += " " + Resources.Messages.CreateAcPrincipalManually;
                            continue;
                        }
                    }
                    this.SetLMSUserDefaultACPermissions2(
                        lmsCompany,
                        meetingSco,
                        lmsUserDto,
                        dbUser.PrincipalId,
                        meetingPermissions,
                        hostPrincipals);
                }
            }

            foreach (var chunk in meetingPermissions.Chunk(provider.GetPermissionChunk()))
            {
                StatusInfo status = provider.UpdateScoPermissions(chunk);
                if (status.Code != StatusCodes.ok)
                {
                    string errorMsg = string.Format("SetDefaultRolesForNonParticipants > UpdateScoPermissionForPrincipal. Status.Code:{0}, Status.SubCode:{1}.", 
                        status.Code.ToString(), 
                        status.SubCode
                        );
                    throw new InvalidOperationException(errorMsg);
                }
            }

            var result = users.ToList();

            var guestsToDelete = new List<LmsCourseMeetingGuest>();
            foreach (LmsCourseMeetingGuest guest in meeting.MeetingGuests)
            {
                principalIds.Remove(guest.PrincipalId);

                MeetingPermissionInfo guestEnrollment = enrollments.FirstOrDefault(x => x.PrincipalId == guest.PrincipalId);
                if (guestEnrollment == null)
                {
                    guestsToDelete.Add(guest);
                }
                else
                {
                    result.Add(new LmsUserDTO
                    {
                        GuestId = guest.Id,
                        AcId = guest.PrincipalId,
                        Name = guestEnrollment.Name,
                        AcRole = AcRole.GetRoleId(guestEnrollment.PermissionId),
                    });
                }
            }

            foreach (LmsCourseMeetingGuest guestToDelete in guestsToDelete)
                meeting.MeetingGuests.Remove(guestToDelete);
            // TRICK: not to have nhibernate 'no session or session was closed' error later in the method
            LmsCourseMeetingModel.Refresh(ref meeting);
            LmsCourseMeetingModel.RegisterSave(meeting, flush: true);

            if (principalIds.Any())
            {
                // TRICK: do not delete participants if meeting is ReUsed
                // TRICK: do not delete participants if meeting is source for any ReUsed meeting
                bool skipSyncAcUsers = lmsCompany.EnableMeetingReuse && 
                    ((meeting.Reused.HasValue && meeting.Reused.Value)
                        || LmsCourseMeetingModel.ContainsByCompanyAndScoId(lmsCompany, meeting.GetMeetingScoId(), meeting.Id));

                // NOTE: if it is reused meeting - skip delete principals from meeting in AC
                // + return them to client side
                if (skipSyncAcUsers)
                {
                    result.AddRange(principalIds.Select(
                        principalId =>
                            new LmsUserDTO
                            {
                                AcId = principalId,
                                Name = enrollments.Single(x => x.PrincipalId == principalId).Name,
                                AcRole = AcRole.GetRoleId(enrollments.Single(x => x.PrincipalId == principalId).PermissionId),
                            }));
                }
                else
                {
                    foreach (var chunk in principalIds.Select(principalId =>
                        new MeetingPermissionUpdateTrio
                        {
                            ScoId = meetingSco,
                            PrincipalId = principalId,
                            PermissionId = MeetingPermissionId.remove,
                        }).Chunk(provider.GetPermissionChunk()))
                    {
                        StatusInfo status = provider.UpdateScoPermissions(chunk);
                        if (status.Code != StatusCodes.ok)
                        {
                            string errorMsg = string.Format("SetDefaultRolesForNonParticipants > UpdateScoPermissionForPrincipal. Status.Code:{0}, Status.SubCode:{1}.",
                                status.Code.ToString(),
                                status.SubCode
                                );
                            throw new InvalidOperationException(errorMsg);
                        }
                    }
                }
            }

            // NOTE: add to meeting-hosts *only* user meeting created.
            //var hostGroup = MeetingTypeFactory.HostGroup((LmsMeetingType) meeting.LmsMeetingType);
            //this.AddUsersToMeetingHostsGroup(provider, hostPrincipals, hostGroup);

            return result;
        }

        private Principal CreatePrincipalAndUpdateLmsUserPrincipalId(IAdobeConnectProxy provider,
            LmsUserDTO lmsUserDto, LmsUser dbUser, ILmsLicense lmsCompany, out string error)
        {
            error = string.Empty;
            Principal principal = null;
            try
            {
                principal = acUserService.GetOrCreatePrincipal(
                    provider,
                    lmsUserDto.GetLogin(),
                    lmsUserDto.GetEmail(),
                    lmsUserDto.GetFirstName(),
                    lmsUserDto.GetLastName(),
                    lmsCompany);
            }
            catch (Exception ex)
            {
                if (ex is IUserMessageException)
                {
                    error = ex.Message;
                }
                logger.Error("CreatePrincipalAndUpdateLmsUserPrincipalId", ex);
            }

            if (principal != null)
            {
                lmsUserDto.AcId = principal.PrincipalId;
                dbUser.PrincipalId = principal.PrincipalId;
                this.LmsUserModel.RegisterSave(dbUser);
            }

            return principal;
        }

        public List<LmsUserDTO> SetDefaultRolesForNonParticipants(
            ILmsLicense lmsCompany,
            IAdobeConnectProxy provider, 
            LtiParamDTO param, 
            int id, 
            out string error)
        {
            LmsCourseMeeting meeting = this.LmsCourseMeetingModel.GetOneByCourseAndId(
                lmsCompany.Id, 
                param.course_id, 
                id);

            // TRICK: not to have nhibernate 'no session or session was closed' error later in the method
            if (meeting.MeetingGuests != null)
            {
                var guestsTmp = meeting.MeetingGuests.ToList();
            }

            List<LmsUserDTO> users = this.GetLMSUsers(
                lmsCompany, 
                meeting, 
                param.course_id, 
                out error, 
                param);

            if (meeting.LmsMeetingType != (int)LmsMeetingType.StudyGroup && lmsCompany.UseSynchronizedUsers 
                && (meeting.EnableDynamicProvisioning || users.Count > EdugameCloud.Lti.Core.Utils.Constants.SyncUsersCountLimit))
            {
                return users;
            }

            string[] userIds = users.Select(user => user.LtiId ?? user.Id).ToArray();
            IEnumerable<LmsUser> lmsDbUsers = null;
            if (lmsCompany.UseSynchronizedUsers && meeting.MeetingRoles != null)
            {
                lmsDbUsers = GetUserMeetingRoles(meeting).Select(x => x.User);
                //when users where not synchronized yet
                if (!lmsDbUsers.Any())
                {
                    lmsDbUsers = this.LmsUserModel.GetByUserIdAndCompanyLms(userIds, lmsCompany.Id);
                }
            }
            else
            {
                lmsDbUsers = this.LmsUserModel.GetByUserIdAndCompanyLms(userIds, lmsCompany.Id);
            }
            List<MeetingPermissionInfo> enrollments = this.GetMeetingAttendees(provider, meeting.GetMeetingScoId());
            return SetDefaultRolesForNonParticipants(lmsCompany, provider, meeting, users, lmsDbUsers, enrollments, ref error);
        }

        public List<LmsUserDTO> GetUsersToAddToMeeting(ILmsLicense lmsCompany, IEnumerable<LmsUserDTO> lmsUsers, out string message)
        {
            message = string.Empty;
            List<LmsUserDTO> usersToAddToMeeting = lmsUsers.ToList();

            if (lmsCompany.ACUsesEmailAsLogin.GetValueOrDefault())
            {
                bool containsEmptyEmails = lmsUsers.Any(x => string.IsNullOrWhiteSpace(x.PrimaryEmail));
                bool containsDuplicateEmails = lmsUsers.Select(x => x.PrimaryEmail).Count() != lmsUsers.Select(x => x.PrimaryEmail).Distinct().Count();
                if (containsEmptyEmails || containsDuplicateEmails)
                {
                    message += Resources.Messages.UsersCannotBeSync;
                    if (containsEmptyEmails)
                        message += Resources.Messages.UsersEmptyEmail;
                    if (containsDuplicateEmails)
                        message += Resources.Messages.UsersDuplicateEmail;
                    // TRICK: ugly string processing )
                    message = message.Substring(0, message.Length - 2);
                    message += ". ";
                }

                var duplicateEmails = lmsUsers.GroupBy(x => x.PrimaryEmail)
                    .Where(g => g.Count() > 1)
                    .Select(y => y.Key)
                    .ToList();

                // NOTE: process ONLY VALID users
                usersToAddToMeeting = lmsUsers.Where(x => !string.IsNullOrWhiteSpace(x.PrimaryEmail) && !duplicateEmails.Contains(x.PrimaryEmail)).ToList();
            }

            //TRICK: we need email on client side only if AC uses emails as login!!
            // we use emails to check they are not empty and are unique within a course
            if (lmsCompany.ACUsesEmailAsLogin.GetValueOrDefault())
            {
                usersToAddToMeeting.ForEach(x =>
                {
                    x.Email = x.PrimaryEmail;
                });
            }

            return usersToAddToMeeting;
        }

        public void SetDefaultUsers(
            ILmsLicense lmsCompany, 
            LmsCourseMeeting meeting,
            IAdobeConnectProxy provider, 
            string lmsUserId, 
            int courseId, 
            string meetingScoId,
            List<LmsUserDTO> users,
            object extraData = null)
        {
            IEnumerable<Principal> principalCache = this.GetAllPrincipals(lmsCompany, provider, users);
//            string[] userIds = users.Select(user => user.lti_id ?? user.id).ToArray();
            IEnumerable<LmsUser> lmsUsers = null;
            if (lmsCompany.UseSynchronizedUsers && meeting != null && meeting.MeetingRoles != null)
            {
                lmsUsers = GetUserMeetingRoles(meeting).Select(x => x.User);
                //when users where not synchronized yet
                if (!lmsUsers.Any())
                {
                    lmsUsers = this.LmsUserModel.GetByCompanyLms(lmsCompany.Id, users);
                }
            }
            else
            {
                var sw = Stopwatch.StartNew();
                lmsUsers = this.LmsUserModel.GetByCompanyLms(lmsCompany.Id, users);

                sw.Stop();
                logger.InfoFormat("SaveMeeting: SetDefaultUsers.GetByCompanyLms: time: {0}.", sw.Elapsed.ToString());
            }

            var sw2 = Stopwatch.StartNew();
            //check if meeting != null or provide LmsMeetingType as separate parameter
            this.ProcessUsersInAC(lmsCompany, provider, meetingScoId, users, principalCache, lmsUsers, true, (LmsMeetingType)meeting.LmsMeetingType);

            sw2.Stop();
            logger.InfoFormat("SaveMeeting: SetDefaultUsers.ProcessUsersInAC: time: {0}.", sw2.Elapsed.ToString());
        }

        public void SetLMSUserDefaultACPermissions2(
            ILmsLicense lmsCompany,
            string meetingScoId, 
            LmsUserDTO u, 
            string principalId, 
            List<MeetingPermissionUpdateTrio> meetingPermission, 
            List<string> hostPrincipals)
        {
            var permission = new RoleMappingService().SetAcRole(lmsCompany, u);

            if (!string.IsNullOrWhiteSpace(principalId) && !string.IsNullOrWhiteSpace(meetingScoId))
            {
                // TODO: try not use by one
                // provider.UpdateScoPermissionForPrincipal(meetingScoId, principalId, permission);
                meetingPermission.Add(
                    new MeetingPermissionUpdateTrio
                    {
                            ScoId = meetingScoId, 
                            PrincipalId = principalId, 
                            PermissionId = permission,
                    });

                if (permission == MeetingPermissionId.host)
                {
                    // this.AddUserToMeetingHostsGroup(provider, principalId);
                    hostPrincipals.Add(principalId);
                }
            }
        }

        public LmsUserDTO UpdateUser(
            ILmsLicense lmsCompany,
            IAdobeConnectProxy provider,
            LtiParamDTO param,
            LmsUserDTO user,
            int id,
            out string error,
            bool skipReturningUsers = false)
        {
            error = string.Empty;
            LmsCourseMeeting meeting = this.LmsCourseMeetingModel.GetOneByCourseAndId(
                lmsCompany.Id,
                param.course_id,
                id);

            // TODO:  DO WE NEED IT ?
            if (meeting == null)
            {
                return skipReturningUsers
                    ? null
                    : GetOrCreateUserWithAcRole(lmsCompany, provider, param, meeting, out error, lmsUserId: user.Id);
            }

            // NOTE: now we create AC principal within Users/GetAll method. So user will always have ac_id here.
            if (user.AcId == null)
            {
                logger.WarnFormat("[UpdateUser]. ac_id == null. LmsCompanyId:{0}. Id:{1}. UserLogin:{2}.", lmsCompany.Id, id, user.GetLogin());

                // Get all users from COURSE
                // Process if they have empty\duplicate with current user

                Principal principal = acUserService.GetOrCreatePrincipal(
                    provider,
                    user.GetLogin(),
                    user.GetEmail(),
                    user.GetFirstName(),
                    user.GetLastName(),
                    lmsCompany);

                if (principal != null)
                {
                    user.AcId = principal.PrincipalId;
                }
                else if (lmsCompany.DenyACUserCreation)
                {
                    error = Resources.Messages.CreateAcPrincipalManually;
                    return null;
                }
                else
                {
                    error = "Can't create Adobe Connect principal";
                    return null;
                }
            }

            if (!user.AcRole.HasValue)
            {
                throw new InvalidOperationException("Adobe Connect principal role is empty");
            }

            var nonEditable = new HashSet<string>();
            var attendees = GetMeetingAttendees(
                provider,
                meeting.GetMeetingScoId(),
                nonEditable);
            var permission = MeetingPermissionId.view;
            if (attendees.Contains(user.AcId))
            {
                if (user.AcRole.Value == AcRole.Presenter.Id)
                {
                    permission = MeetingPermissionId.mini_host;
                }
                else if (user.AcRole.Value == AcRole.Host.Id)
                {
                    permission = MeetingPermissionId.host;
                }
            }
            else
            {
                permission = new RoleMappingService().SetAcRole(lmsCompany, user, ignoreEmptyACRole: true);
            }

            try
            {
                StatusInfo status = provider.UpdateScoPermissionForPrincipal(meeting.GetMeetingScoId(), user.AcId, permission);
            }
            catch (InvalidOperationException)
            {
                // NOTE: check that Principal is in AC yet
                var principalInfo = provider.GetOneByPrincipalId(user.AcId);
                if (!principalInfo.Success)
                {
                    var dbUser = this.LmsUserModel.GetOneByUserIdAndCompanyLms(user.LtiId ?? user.Id, lmsCompany.Id).Value;

                    //
                    // TODO: CHECK EMAIL IS VALID (UNIQUE AND NON-EMPTY) if AC uses Emails-as-Logins
                    //
                    string err;
                    var principal = CreatePrincipalAndUpdateLmsUserPrincipalId(provider, user, dbUser, lmsCompany, out err);
                    if (!string.IsNullOrWhiteSpace(err))
                    {
                        error = err;
                    }
                    if (principal == null && lmsCompany.DenyACUserCreation)
                    {
                        if (!error.Contains(Resources.Messages.CreateAcPrincipalManually))
                            error += " " + Resources.Messages.CreateAcPrincipalManually;
                        return null;
                    }
                }

                // NOTE: try again
                StatusInfo status = provider.UpdateScoPermissionForPrincipal(meeting.GetMeetingScoId(), user.AcId, permission);
            }

            // NOTE: add to meeting-hosts *only* user meeting created.
            //if (permission == MeetingPermissionId.host)
            //{
            //    var hostGroup = MeetingTypeFactory.HostGroup((LmsMeetingType) meeting.LmsMeetingType);
            //    this.AddUsersToMeetingHostsGroup(provider, new[] { user.AcId }, hostGroup);
            //}

            return skipReturningUsers
                ? null
                : GetOrCreateUserWithAcRole(lmsCompany, provider, param, meeting, out error, lmsUserId: user.Id);
        }

        public void DeleteUserFromAcMeeting(
            ILmsLicense lmsCompany,
            IAdobeConnectProxy provider,
            LtiParamDTO param,
            string principalId,
            int meetingId,
            out string error)
        {
            error = null;
            LmsCourseMeeting meeting = LmsCourseMeetingModel.GetOneByCourseAndId(
                lmsCompany.Id,
                param.course_id,
                meetingId);

            if (meeting == null)
            {
                error = Resources.Messages.MeetingNotFoundInAC;
                return;
            }

            if (string.IsNullOrEmpty(principalId))
            {
                error = Resources.Messages.UserNotInAdobeConnect;
                return;
            }

            provider.UpdateScoPermissionForPrincipal(
                meeting.GetMeetingScoId(),
                principalId,
                MeetingPermissionId.remove);            
        }

        public LmsUserDTO UpdateGuest(
            ILmsLicense lmsCompany,
            IAdobeConnectProxy provider,
            LtiParamDTO param,
            LmsUserDTO user,
            int id,
            out string error)
        {
            error = null;
            LmsCourseMeeting meeting = this.LmsCourseMeetingModel.GetOneByCourseAndId(
                lmsCompany.Id,
                param.course_id,
                id);

            if (meeting == null)
            {
                logger.ErrorFormat("Meeting not found. LmsCompanyId: {0}, CourseId: {1}, ID: {2}.", lmsCompany.Id, param.course_id, id);
                error = Resources.Messages.MeetingNotFound;
                return null;
            }

            if (user.AcId == null)
            {
                error = Resources.Messages.UserNotInAdobeConnect;
                return null;
            }

            if (!user.AcRole.HasValue)
            {
                throw new InvalidOperationException("AdobeConnect role is empty");
            }
            
            var permission = AcRole.GetById(user.AcRole.Value).MeetingPermissionId;
            provider.UpdateScoPermissionForPrincipal(meeting.GetMeetingScoId(), user.AcId, permission);

            // NOTE: add to meeting-hosts *only* user meeting created.
            //if (permission == MeetingPermissionId.host)
            //{
            //    var hostGroup = MeetingTypeFactory.HostGroup((LmsMeetingType) meeting.LmsMeetingType);
            //    AddUsersToMeetingHostsGroup(provider, new[] { user.AcId }, hostGroup);
            //}

            return new LmsUserDTO
            {
                Id = user.Id,
                GuestId = user.GuestId,
                AcId = user.AcId,
                Name = user.Name,
                AcRole = user.AcRole,
            };
        }

        public void DeleteGuestFromAcMeeting(
            ILmsLicense lmsCompany,
            IAdobeConnectProxy provider,
            LtiParamDTO param,
            string principalId,
            int meetingId,
            int guestId,
            out string error)
        {
            error = null;
            LmsCourseMeeting meeting = this.LmsCourseMeetingModel.GetOneByCourseAndId(
                lmsCompany.Id,
                param.course_id,
                meetingId);

            if (meeting == null)
            {
                logger.ErrorFormat("Meeting not found. LmsCompanyId: {0}, CourseId: {1}, ID: {2}.", lmsCompany.Id, param.course_id, meetingId);
                error = Resources.Messages.MeetingNotFound;
                return;
            }

            if (string.IsNullOrWhiteSpace(principalId))
            {
                error = Resources.Messages.UserNotInAdobeConnect;
                return;
            }
            
            provider.UpdateScoPermissionForPrincipal(
                meeting.GetMeetingScoId(),
                principalId,
                MeetingPermissionId.remove);

            LmsCourseMeetingGuest guest = meeting.MeetingGuests.FirstOrDefault(x => x.Id == guestId);
            if (guest != null)
            {
                meeting.MeetingGuests.Remove(guest);
                LmsCourseMeetingModel.RegisterSave(meeting, flush: true);
            }            
        }

        #endregion

        #region Methods
        
        ///// <summary>
        ///// The is user synched.
        ///// </summary>
        ///// <param name="enrollments">
        ///// The enrollments.
        ///// </param>
        ///// <param name="lmsUser">
        ///// The LMS user.
        ///// </param>
        ///// <param name="provider">
        ///// The provider.
        ///// </param>
        ///// <returns>
        ///// The <see cref="bool"/>.
        ///// </returns>
        // private bool IsUserSynched(IEnumerable<PermissionInfo> enrollments, LmsUserDTO lmsUser, AdobeConnectProvider provider)
        // {
        // bool isFound = false;
        // foreach (var host in enrollments)
        // {
        // if (this.LmsUserIsAcUser(lmsUser, host, provider))
        // {
        // lmsUser.ac_id = host.PrincipalId;
        // lmsUser.ac_role = this.GetRoleString(host.PermissionId);
        // isFound = true;
        // break;
        // }
        // }

        // if (!isFound)
        // {
        // return false;
        // }

        // return true;
        // }

        ///// <summary>
        ///// The get role string.
        ///// </summary>
        ///// <param name="permissionId">
        ///// The permission id.
        ///// </param>
        ///// <returns>
        ///// The <see cref="string"/>.
        ///// </returns>
        // private string GetRoleString(PermissionId permissionId)
        // {
        // switch (permissionId)
        // {
        // case PermissionId.host:
        // return "Host";
        // case PermissionId.mini_host:
        // return "Presenter";
        // case PermissionId.view:
        // return "Participant";
        // }

        // return "Unknown";
        // }

        ///// <summary>
        ///// The is participant synched.
        ///// </summary>
        ///// <param name="lmsUsers">
        ///// The LMS users.
        ///// </param>
        ///// <param name="participant">
        ///// The participant.
        ///// </param>
        ///// <param name="provider">
        ///// The provider.
        ///// </param>
        ///// <returns>
        ///// The <see cref="bool"/>.
        ///// </returns>
        // private bool IsParticipantSynched(List<LmsUserDTO> lmsUsers, PermissionInfo participant, AdobeConnectProvider provider)
        // {
        // bool found = false;
        // foreach (var lmsUser in lmsUsers)
        // {
        // if (this.LmsUserIsAcUser(lmsUser, participant, provider))
        // {
        // found = true;
        // }
        // }

        // return found;
        // }

        ///// <summary>
        ///// The LMS user is AC user.
        ///// </summary>
        ///// <param name="lmsUser">
        ///// The LMS user.
        ///// </param>
        ///// <param name="participant">
        ///// The participant.
        ///// </param>
        ///// <param name="provider">
        ///// The provider.
        ///// </param>
        ///// <returns>
        ///// The <see cref="bool"/>.
        ///// </returns>
        // private bool LmsUserIsAcUser(LmsUserDTO lmsUser, PermissionInfo participant, AdobeConnectProvider provider)
        // {
        // string email = lmsUser.GetEmail(), login = lmsUser.GetLogin();
        // var isACUser = participant.Login != null && ((email != null && email.Equals(participant.Login, StringComparison.OrdinalIgnoreCase))
        // || (login != null && login.Equals(participant.Login, StringComparison.OrdinalIgnoreCase)));
        // if (isACUser)
        // {
        // return true;
        // }
        // var principal = provider.GetOneByPrincipalId(participant.PrincipalId);
        // if (principal.Success)
        // {
        // isACUser = principal.PrincipalInfo.Principal.Email.Equals(email)
        // || principal.PrincipalInfo.Principal.Email.Equals(login);
        // }

        // return isACUser;
        // }

        /// <summary>
        /// The get group principals.
        /// </summary>
        /// <param name="provider">
        /// The provider.
        /// </param>
        /// <param name="groupIds">
        /// The group ids.
        /// </param>
        /// <returns>
        /// The <see cref="List{Principal}"/>.
        /// </returns>
        private static List<Principal> GetGroupPrincipals(IAdobeConnectProxy provider, IEnumerable<string> groupIds)
        {
            var principals = new List<Principal>();

            foreach (string groupid in groupIds)
            {
                PrincipalCollectionResult groupPrincipals = provider.GetGroupUsers(groupid);
                principals.AddRange(groupPrincipals.Values);
            }

            return principals;
        }

        /// <summary>
        /// The get meeting attendees.
        /// </summary>
        /// <param name="provider">
        /// The provider.
        /// </param>
        /// <param name="meetingSco">
        /// The meeting SCO.
        /// </param>
        /// <param name="nonEditable">
        /// The non editable.
        /// </param>
        private static MeetingAttendees GetMeetingAttendees(
            IAdobeConnectProxy provider, 
            string meetingSco, 
            HashSet<string> nonEditable = null)
        {
            var alreadyAdded = new HashSet<string>();
            MeetingPermissionCollectionResult allEnrollments = provider.GetAllMeetingEnrollments(meetingSco);
            List<MeetingPermissionInfo> hostsResult =
                allEnrollments.Values.Return(
                    x => x.Where(v => v.PermissionId == MeetingPermissionId.host).ToList(), 
                    new List<MeetingPermissionInfo>());
            List<MeetingPermissionInfo> presentersResult =
                allEnrollments.Values.Return(
                    x => x.Where(v => v.PermissionId == MeetingPermissionId.mini_host).ToList(), 
                    new List<MeetingPermissionInfo>());
            List<MeetingPermissionInfo> participantsResult =
                allEnrollments.Values.Return(
                    x => x.Where(v => v.PermissionId == MeetingPermissionId.view).ToList(), 
                    new List<MeetingPermissionInfo>());
            foreach (MeetingPermissionInfo g in hostsResult)
            {
                alreadyAdded.Add(g.PrincipalId);
            }

            foreach (MeetingPermissionInfo g in presentersResult)
            {
                alreadyAdded.Add(g.PrincipalId);
            }

            foreach (MeetingPermissionInfo g in participantsResult)
            {
                alreadyAdded.Add(g.PrincipalId);
            }

            nonEditable = nonEditable ?? new HashSet<string>();

            var hosts = ProcessACMeetingAttendees(nonEditable, provider, hostsResult, alreadyAdded);
            var presenters = ProcessACMeetingAttendees(nonEditable, provider, presentersResult, alreadyAdded);
            var participants = ProcessACMeetingAttendees(nonEditable, provider, participantsResult, alreadyAdded);

            return new MeetingAttendees 
            {
                Hosts = hosts,
                Presenters = presenters,
                Participants = participants,
            };
        }

        /// <summary>
        /// The process ac meeting attendees.
        /// </summary>
        /// <param name="nonEditable">
        /// The non editable.
        /// </param>
        /// <param name="provider">
        /// The provider.
        /// </param>
        /// <param name="values">
        /// The values.
        /// </param>
        /// <param name="alreadyAdded">
        /// The already added.
        /// </param>
        /// <returns>
        /// The <see cref="List{MeetingPermissionInfo}"/>.
        /// </returns>
        private static List<MeetingPermissionInfo> ProcessACMeetingAttendees(
            HashSet<string> nonEditable,
            IAdobeConnectProxy provider, 
            List<MeetingPermissionInfo> values, 
            HashSet<string> alreadyAdded)
        {
            IEnumerable<string> groupPrincipalList = values.Where(x => x.HasChildren).Select(v => v.PrincipalId);
            if (groupPrincipalList.Any())
            {
                List<Principal> groupValues = GetGroupPrincipals(provider, groupPrincipalList);
                foreach (Principal g in groupValues)
                {
                    if (alreadyAdded.Contains(g.PrincipalId))
                    {
                        continue;
                    }

                    values.Add(
                        new MeetingPermissionInfo
                        {
                                PrincipalId = g.PrincipalId, 
                                Name = g.Name, 
                                Login = g.Login, 
                                IsPrimary = g.IsPrimary,
                            });
                    nonEditable.Add(g.PrincipalId);
                    alreadyAdded.Add(g.PrincipalId);
                }
            }

            return values;
        }
        
        private IEnumerable<Principal> GetAllPrincipals(
            ILmsLicense lmsCompany,
            IAdobeConnectProxy provider, 
            List<LmsUserDTO> users)
        {
            var cacheMode = this.settings.Lti_AcUserCache_Mode as string;

            if (string.IsNullOrEmpty(cacheMode) || cacheMode.Equals("DISABLED", StringComparison.OrdinalIgnoreCase))
            {
                return null;
            }
            
            if (cacheMode.Equals("CONNECT", StringComparison.OrdinalIgnoreCase))
            {
                try
                {
                    var result = new List<Principal>();

                    foreach (var chunk in users.Chunk(50))
                    {
                        PrincipalCollectionResult acResult = lmsCompany.ACUsesEmailAsLogin.GetValueOrDefault()
                            ? provider.GetAllByEmail(chunk.Select(x => x.GetEmail()).Where(x => !string.IsNullOrWhiteSpace(x)))
                            : provider.GetAllByLogin(chunk.Select(x => x.GetLogin()).Where(x => !string.IsNullOrWhiteSpace(x)));

                        if (acResult.Success)
                        {
                            result.AddRange(acResult.Values);
                        }
                        else
                        {
                            // TODO: PROCESS!!
                            throw new InvalidOperationException("GetAllPrincipals. Adobe Connect error");
                        }
                    }

                    return result;
                }
                catch (Exception ex)
                {
                    logger.Error("GetAllPrincipals.CONNECT", ex);
                    throw;
                }
            }

            logger.Error("Unsupported cache mode: " + cacheMode);
            return null;
        }

        private void ProcessUsersInAC(
            ILmsLicense lmsCompany,
            IAdobeConnectProxy provider, 
            string meetingScoId, 
            List<LmsUserDTO> users, 
            IEnumerable<Principal> principalCache, 
            IEnumerable<LmsUser> lmsUsers, 
            bool reRunOnError, LmsMeetingType meetingType)
        {
            var meetingPermissions = new List<MeetingPermissionUpdateTrio>();
            var hostPrincipals = new List<string>();

            var company = LmsCompanyModel.GetOneById(lmsCompany.Id).Value;

            foreach (LmsUserDTO u in users)
            {
                string email = u.GetEmail();
                string login = u.GetLogin();
                
                // TODO: do we need this FORCE?
                // YES: !principal.PrincipalId.Equals(lmsUser.PrincipalId) - we use it to refresh PrincipalId
                Principal principal = acUserService.GetOrCreatePrincipal2(
                    provider, 
                    login, 
                    email, 
                    u.GetFirstName(), 
                    u.GetLastName(), 
                    lmsCompany, 
                    principalCache);
                
                if (principal != null)
                {
                    // TRICK: we can filter by 'UserId' - cause we sync it in 'getUsersByLmsCompanyId' SP
                    string id = u.LtiId ?? u.Id;
                    LmsUser lmsUser = lmsUsers.FirstOrDefault(x => x.UserId == id);

                    if (lmsUser == null || !principal.PrincipalId.Equals(lmsUser.PrincipalId))
                    {
                        if (lmsUser == null)
                        {
                            lmsUser = new LmsUser 
                            {
                                LmsCompany = company, 
                                UserId = id, 
                                Username = login, 
                                // TODO: email??
                            };
                        }

                        lmsUser.PrincipalId = principal.PrincipalId;
                        this.LmsUserModel.RegisterSave(lmsUser);
                    }
                    
                    // TODO: see http://help.adobe.com/en_US/connect/9.0/webservices/WS5b3ccc516d4fbf351e63e3d11a171ddf77-7fcb_SP1.html
                    this.SetLMSUserDefaultACPermissions2(
                        lmsCompany,
                        meetingScoId, 
                        u, 
                        principal.PrincipalId, 
                        meetingPermissions, 
                        hostPrincipals);
                }
            }
            //            var sw3 = Stopwatch.StartNew();

            // TRICK: do not move down to chunk part!
            // NOTE: add to meeting-hosts *only* user meeting created.
            //this.AddUsersToMeetingHostsGroup(provider, hostPrincipals, MeetingTypeFactory.HostGroup(meetingType));

            foreach (var chunk in meetingPermissions.Chunk(provider.GetPermissionChunk()))
            {
                StatusInfo status = provider.UpdateScoPermissions(chunk);
                if (status.Code != StatusCodes.ok)
                {
                    if (reRunOnError)
                    {
                        logger.Error("UpdateScoPermissionForPrincipal - 1st try. Status.Code=" + status.Code.ToString());

                        //IoC.Resolve<IPrincipalCache>().RecreatePrincipalCache(IoC.Resolve<LmsCompanyModel>().GetAll());

                        IEnumerable<Principal> refreshedPrincipalCache = this.GetAllPrincipals(
                            lmsCompany, 
                            provider, 
                            users);
                        this.ProcessUsersInAC(
                            lmsCompany, 
                            provider, 
                            meetingScoId, 
                            users, 
                            refreshedPrincipalCache, 
                            lmsUsers, 
                            false,
                            meetingType);
                        return;
                    }
                    else
                    {
                        logger.Error("UpdateScoPermissionForPrincipal - 2nd try. Status.Code=" + status.Code.ToString());
                        throw new InvalidOperationException(
                            "UpdateScoPermissionForPrincipal. Status.Code=" + status.Code.ToString());
                    }
                }
            }
        }

        #endregion

    }

}