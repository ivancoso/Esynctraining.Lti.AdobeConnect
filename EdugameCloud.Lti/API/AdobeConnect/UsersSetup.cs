namespace EdugameCloud.Lti.API.AdobeConnect
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Security;
    using Castle.Core.Logging;
    using EdugameCloud.Lti.Core.Business;
    using EdugameCloud.Lti.Core.Business.Models;
    using EdugameCloud.Lti.Core.Domain.Entities;
    using EdugameCloud.Lti.Domain.Entities;
    using EdugameCloud.Lti.DTO;
    using EdugameCloud.Lti.Extensions;
    using Esynctraining.AC.Provider.DataObjects.Results;
    using Esynctraining.AC.Provider.Entities;
    using Esynctraining.Core.Extensions;
    using Esynctraining.Core.Providers;
    using Esynctraining.Core.Utils;
    
    public sealed class UsersSetup : IUsersSetup
    {
        #region Inner Class: MeetingAttendees

        private sealed class MeetingAttendees
        {
            public List<PermissionInfo> Hosts { get; set; }

            public List<PermissionInfo> Presenters { get; set; }

            public List<PermissionInfo> Participants { get; set; }

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
        private readonly LmsFactory lmsFactory;
        private IAdobeConnectUserService acUserService;
        private ILogger logger;

        #endregion

        #region Constructors and Destructors

        public UsersSetup(
            ApplicationSettingsProvider settings,
            LmsFactory lmsFactory,
            IAdobeConnectUserService acUserService,
            ILogger logger)
        {
            this.settings = settings;
            this.lmsFactory = lmsFactory;
            this.acUserService = acUserService;
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

        /// <summary>
        /// The add user to meeting hosts group.
        /// </summary>
        /// <param name="provider">
        /// The provider.
        /// </param>
        /// <param name="principalId">
        /// The principal id.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public void AddUserToMeetingHostsGroup(IAdobeConnectProxy provider, string principalId)
        {
            provider.AddToGroupByType(principalId, "live-admins");
        }

        /// <summary>
        /// The add users to meeting hosts group.
        /// </summary>
        /// <param name="provider">
        /// The provider.
        /// </param>
        /// <param name="principalIds">
        /// The principal ids.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public void AddUsersToMeetingHostsGroup(IAdobeConnectProxy provider, IEnumerable<string> principalIds)
        {
            if (principalIds.Any())
                provider.AddToGroupByType(principalIds, "live-admins");
        }
        
        public string GetACPassword(LmsCompany lmsCompany, LmsUserSettingsDTO userSettings, string email, string login)
        {
            var connectionMode = (AcConnectionMode)userSettings.acConnectionMode;
            switch (connectionMode)
            {
                case AcConnectionMode.Overwrite:
                    string password = lmsCompany.AcUsername.Equals(email, StringComparison.OrdinalIgnoreCase)
                                      || lmsCompany.AcUsername.Equals(login, StringComparison.OrdinalIgnoreCase)
                                          ? lmsCompany.AcPassword
                                          : Membership.GeneratePassword(8, 2);
                    return password;
                case AcConnectionMode.DontOverwriteLocalPassword:
                    return userSettings.password;
                default:
                    return null;
            }
        }
        
        public List<LmsUserDTO> GetLMSUsers(
            LmsCompany lmsCompany, 
            LmsCourseMeeting meeting, 
            string lmsUserId, 
            int courseId, 
            out string error, 
            object extraData = null, 
            bool forceUpdate = false)
        {
            if (lmsCompany.UseSynchronizedUsers && meeting != null && meeting.MeetingRoles != null)
            {
                var dbUserMeetingRoles = GetUserMeetingRoles(meeting);
                var userDtos = dbUserMeetingRoles.Select(x => new LmsUserDTO
                {
                    ac_id = x.User.PrincipalId,
                    id = x.User.UserIdExtended ?? x.User.UserId,
                    lti_id = x.User.UserId,
                    login_id = x.User.Username,
                    name = x.User.Name,
                    primary_email = x.User.Email,
                    lms_role = x.LmsRole
                });

                if (userDtos.Any())
                {
                    error = null;
                    return userDtos.ToList();
                }
            }
            var service = lmsFactory.GetUserService((LmsProviderEnum) lmsCompany.LmsProvider.Id);
            LmsUser lmsUser = this.LmsUserModel.GetOneByUserIdAndCompanyLms(lmsUserId, lmsCompany.Id).Value;
            var serviceResult = service.GetUsers(lmsCompany, lmsUser ?? new LmsUser { UserId = lmsUserId }, courseId, extraData, forceUpdate);
            if (serviceResult.isSuccess)
            {
                error = null;
                return serviceResult.data;
            }
            logger.WarnFormat("[GetLMSUsers] Running old style retrieve method. LmsCompanyId={0}, MeetingId={1}, lmsUserId={2}, " +
                "courseId={3}", lmsCompany.Id, meeting.Return(x=>x.Id, 0), lmsUserId, courseId);
            var users = service.GetUsersOldStyle(lmsCompany, lmsUserId, courseId, out error, forceUpdate, extraData);
            return error == null ? users : new List<LmsUserDTO>();
        }

        public IEnumerable<LmsUserMeetingRole> GetUserMeetingRoles(LmsCourseMeeting meeting)
        {
            return meeting.MeetingRoles
                    .GroupBy(x => x.User.UserId).Select(x => x.OrderBy(u => u.User.Id).First());
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
        /// <returns>
        /// The <see cref="List{PermissionInfo}"/>.
        /// </returns>
        public List<PermissionInfo> GetMeetingAttendees(IAdobeConnectProxy provider, string meetingSco)
        {
            var alreadyAdded = new HashSet<string>();
            PermissionCollectionResult allMeetingEnrollments = provider.GetAllMeetingEnrollments(meetingSco);
            List<PermissionInfo> allValues = allMeetingEnrollments.Values.Return(
                x => x.ToList(), 
                new List<PermissionInfo>());
            foreach (PermissionInfo g in allValues)
            {
                alreadyAdded.Add(g.PrincipalId);
            }

            return ProcessACMeetingAttendees(new HashSet<string>(), provider, allValues, alreadyAdded);
        }

        public void GetParamLoginAndEmail(LtiParamDTO param, LmsCompany lmsCompany, out string email, out string login)
        {
            email = param.lis_person_contact_email_primary;
            login = param.lms_user_login;
            if (string.IsNullOrWhiteSpace(login))
            {
                // TODO: for D2L more effective would be to get WhoIAm and UserInfo information from their API
                string error;
                var currentUser = LmsUserModel.GetOneByUserIdAndCompanyLms(param.lms_user_id, lmsCompany.Id).Value;

                var lmsUserService = lmsFactory.GetUserService((LmsProviderEnum) lmsCompany.LmsProvider.Id);
                LmsUserDTO user = lmsUserService.GetUser(lmsCompany,
                    currentUser,
                    param.lms_user_id,
                    param.course_id,
                    out error,
                    param,
                    true);

                if (user != null)
                {
                    login = user.login_id;
                }
            }
        }

        public IList<LmsUserDTO> GetUsers(
            LmsCompany lmsCompany,
            IAdobeConnectProxy provider, 
            LtiParamDTO param, 
            int id, 
            out string error,
            List<LmsUserDTO> users = null,
            bool forceUpdate = false)
        {
            LmsCourseMeeting meeting = this.LmsCourseMeetingModel.GetOneByCourseAndId(
                lmsCompany.Id, 
                param.course_id, 
                id);

            if (meeting == null)
            {
                logger.ErrorFormat("[GetUsers] Meeting not found in DB. LmsCompanyID: {0}. CourseID: {1}. ID: {2}.", 
                    lmsCompany.Id,
                    param.course_id,
                    id);

                error = "Meeting not found";
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
                param.lms_user_id,
                param.course_id,
                out error,
                param,
                forceUpdate).ToList();

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

            string[] userIds = users.Select(user => user.lti_id ?? user.id).ToArray();
            IEnumerable<LmsUser> lmsUsers = null;
            if (lmsCompany.UseSynchronizedUsers && meeting.MeetingRoles != null)
            {
                lmsUsers = GetUserMeetingRoles(meeting).Select(x => x.User);
                //when users where not synchronized yet
                if (!lmsUsers.Any())
                {
                    lmsUsers = this.LmsUserModel.GetByUserIdAndCompanyLms(userIds, lmsCompany.Id).GroupBy(x => x.UserId).Select(x => x.OrderBy(u => u.Id).First());
                }
            }
            else
            {
                lmsUsers = this.LmsUserModel.GetByUserIdAndCompanyLms(userIds, lmsCompany.Id).GroupBy(x => x.UserId).Select(x => x.OrderBy(u => u.Id).First());
            }
            // Debug.Assert(userIds.Length == lmsUsers.Count(), "Should return single user by userId+lmsCompany.Id");

            // NOTE: sometimes we have no users here - for example due any security issue in LMS service (BlackBoard)
            // So skip this step for better performance
            if (users.Count > 0)
            {
                IEnumerable<Principal> principalCache = this.GetAllPrincipals(lmsCompany, provider, users);

                bool uncommitedChangesInLms = false;
                foreach (LmsUserDTO user in users)
                {
                    string login = user.GetLogin();
                    LmsUser lmsUser = lmsUsers.FirstOrDefault(u => u.UserId == (user.lti_id ?? user.id));
                    if (lmsUser == null)
                    {
                        lmsUser = new LmsUser
                        {
                            LmsCompany = lmsCompany,
                            Username = login,
                            UserId = user.lti_id ?? user.id,
                        };
                    }

                    if (string.IsNullOrEmpty(lmsUser.PrincipalId))
                    {
                        // NOTE: we create Principals during Users/GetAll to have ability to join office hours meeting for all course participants.
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
                            lmsUser.PrincipalId = principal.PrincipalId;
                            this.LmsUserModel.RegisterSave(lmsUser, flush: false);
                            uncommitedChangesInLms = true;
                        }
                    }
                    //var principalInfo = provider.GetOneByPrincipalId(lmsUser.PrincipalId);

                    if (string.IsNullOrEmpty(lmsUser.PrincipalId))
                    {
                        continue;
                    }

                    user.ac_id = lmsUser.PrincipalId;
                    user.is_editable = !nonEditable.Contains(user.ac_id);

                    if (attendees.Hosts.Any(v => v.PrincipalId == user.ac_id))
                    {
                        user.ac_role = AcRole.Host.Name;
                        attendees.Hosts = attendees.Hosts.Where(v => v.PrincipalId != user.ac_id).ToList();
                    }
                    else if (attendees.Presenters.Any(v => v.PrincipalId == user.ac_id))
                    {
                        user.ac_role = AcRole.Presenter.Name;
                        attendees.Presenters = attendees.Presenters.Where(v => v.PrincipalId != user.ac_id).ToList();
                    }
                    else if (attendees.Participants.Any(v => v.PrincipalId == user.ac_id))
                    {
                        user.ac_role = AcRole.Participant.Name;
                        attendees.Participants = attendees.Participants.Where(v => v.PrincipalId != user.ac_id).ToList();
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

            error = null;
            return users;
        }
        
        private static void ProcessGuests(IList<LmsUserDTO> users, LmsCourseMeeting meeting, IEnumerable<PermissionInfo> permissions, AcRole role)
        {
            foreach (PermissionInfo permissionInfo in permissions)
            {

                LmsCourseMeetingGuest guest = meeting.MeetingGuests.FirstOrDefault(x => x.PrincipalId == permissionInfo.PrincipalId);
                if (guest != null)
                {
                    users.Add(
                        new LmsUserDTO
                        {
                            guest_id = guest.Id,
                            ac_id = guest.PrincipalId,
                            name = permissionInfo.Name,
                            ac_role = role.Name,
                        });
                }
                else
                {
                    users.Add(
                        new LmsUserDTO
                        {
                            ac_id = permissionInfo.PrincipalId,
                            name = permissionInfo.Name,
                            ac_role = role.Name,
                        });
                }
            }
        }

        public LmsUserDTO GetOrCreateUserWithAcRole(
            LmsCompany lmsCompany,
            IAdobeConnectProxy provider,
            LtiParamDTO param,
            LmsCourseMeeting meeting,
            out string error,
            bool forceUpdate = false, string lmsUserId = null)
        {
            var currentUser = LmsUserModel.GetOneByUserIdAndCompanyLms(param.lms_user_id, lmsCompany.Id).Value;

            var service = lmsFactory.GetUserService((LmsProviderEnum)lmsCompany.LmsProvider.Id);
            //todo: not param for BrainHoney
            var lmsUser = service.GetUser(lmsCompany, currentUser, lmsUserId, param.course_id, out error, param, forceUpdate);

            if (meeting == null)
            {
                return lmsUser;
            }

            if (lmsUser != null)
            {
                var lmsDbUser = this.LmsUserModel.GetOneByUserIdAndCompanyLms(lmsUser.lti_id ?? lmsUser.id, lmsCompany.Id);
                var nonEditable = new HashSet<string>();
                MeetingAttendees attendees = GetMeetingAttendees(
                    provider,
                    meeting.GetMeetingScoId(),
                    nonEditable);
                IEnumerable<Principal> principalCache = GetAllPrincipals(lmsCompany, provider, 
                    new List<LmsUserDTO>{lmsUser});

                bool uncommitedChangesInLms = false;
                ProcessLmsUserDtoAcInfo(lmsUser, lmsDbUser.Value, lmsCompany, principalCache, provider, ref uncommitedChangesInLms,
                    nonEditable, attendees);

                if (uncommitedChangesInLms)
                {
                    LmsUserModel.Flush();
                }

                return lmsUser;
            }

            return null;
        }

        private void ProcessLmsUserDtoAcInfo(LmsUserDTO user, LmsUser lmsDbUser, LmsCompany lmsCompany,
            IEnumerable<Principal> principalCache, IAdobeConnectProxy provider, ref bool uncommitedChangesInLms,
            HashSet<string> nonEditable, MeetingAttendees attendees)
        {
            string login = user.GetLogin();
            lmsDbUser = lmsDbUser ?? new LmsUser
            {
                LmsCompany = lmsCompany,
                Username = login,
                UserId = user.lti_id ?? user.id,
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

            user.ac_id = lmsDbUser.PrincipalId;
            user.is_editable = !nonEditable.Contains(user.ac_id);

            if (attendees.Hosts.Any(v => v.PrincipalId == user.ac_id))
            {
                user.ac_role = AcRole.Host.Name;
            }
            else if (attendees.Presenters.Any(v => v.PrincipalId == user.ac_id))
            {
                user.ac_role = AcRole.Presenter.Name;
            }
            else if (attendees.Participants.Any(v => v.PrincipalId == user.ac_id))
            {
                user.ac_role = AcRole.Participant.Name;
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
            return param.roles != null
                   && (param.roles.Contains("Instructor") 
                   || param.roles.Contains("Administrator")
                   || param.roles.Contains("Course Director")
                   || param.roles.Contains("CourseDirector")
                   || param.roles.Contains("Lecture"));
        }

        /// <summary>
        /// The update user.
        /// </summary>
        /// <param name="lmsCompany">
        /// The lmsCompany.
        /// </param>
        /// <param name="provider">
        /// The provider.
        /// </param>
        /// <param name="param">
        /// The parameter.
        /// </param>
        /// <param name="scoId">
        /// The SCO Id.
        /// </param>
        /// <param name="forceUpdate">
        /// The force update.
        /// </param>
        /// <param name="error">
        /// The error.
        /// </param>
        /// <returns>
        /// The <see cref="List{LmsUserDTO}"/>.
        /// </returns>
        public List<LmsUserDTO> SetDefaultRolesForNonParticipants(
            LmsCompany lmsCompany,
            IAdobeConnectProxy provider,
            LmsCourseMeeting meeting,
            IEnumerable<LmsUserDTO> users,
            IEnumerable<LmsUser> lmsDbUsers,
            List<PermissionInfo> enrollments,
            ref string error)
        {
            string meetingSco = meeting.GetMeetingScoId();

            bool denyACUserCreation = lmsCompany.DenyACUserCreation;

            var meetingPermissions = new List<PermissionUpdateTrio>();
            var hostPrincipals = new List<string>();

            var principalIds = new HashSet<string>(enrollments.Select(e => e.PrincipalId));

            foreach (LmsUserDTO lmsUserDto in users)
            {
                // TRICK: we can filter by 'UserId' - cause we sync it in 'getUsersByLmsCompanyId' SP
                string id = lmsUserDto.lti_id ?? lmsUserDto.id;
                LmsUser dbUser = lmsDbUsers.FirstOrDefault(x => x.UserId == id)
                               ?? new LmsUser { LmsCompany = lmsCompany, Username = lmsUserDto.GetLogin(), UserId = id, };

                if (string.IsNullOrEmpty(dbUser.PrincipalId))
                {
                    var principal = CreatePrincipalAndUpdateLmsUserPrincipalId(provider, lmsUserDto, dbUser, lmsCompany);

                    if (principal == null && denyACUserCreation)
                    {
                        error = "At least one user does not exist in AC database. You must create AC accounts manually";
                        continue;
                    }
                }

                // TRICK: dbUser.PrincipalId can be updated within CreatePrincipalAndUpdateLmsUserPrincipalId
                lmsUserDto.ac_id = dbUser.PrincipalId;

                PermissionInfo enrollment = enrollments.FirstOrDefault(e => e.PrincipalId.Equals(dbUser.PrincipalId));

                if (enrollment != null)
                {
                    lmsUserDto.ac_role = AcRole.GetRoleName(enrollment.PermissionId);
                    principalIds.Remove(dbUser.PrincipalId);
                }
                else if (!string.IsNullOrEmpty(dbUser.PrincipalId))
                {
                    // NOTE: check that Principal is in AC yet
                    var principalInfo = provider.GetOneByPrincipalId(dbUser.PrincipalId);
                    if (!principalInfo.Success)
                    {
                        var principal = CreatePrincipalAndUpdateLmsUserPrincipalId(provider, lmsUserDto, dbUser, lmsCompany);

                        if (principal == null && denyACUserCreation)
                        {
                            error = "At least one user does not exist in AC database. You must create AC accounts manually";
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

            foreach (var chunk in Chunk(meetingPermissions, 50))
            {
                StatusInfo status = provider.UpdateScoPermissionForPrincipal(chunk);
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

                PermissionInfo guestEnrollment = enrollments.FirstOrDefault(x => x.PrincipalId == guest.PrincipalId);
                if (guestEnrollment == null)
                {
                    guestsToDelete.Add(guest);
                }
                else
                {
                    result.Add(new LmsUserDTO
                    {
                        guest_id = guest.Id,
                        ac_id = guest.PrincipalId,
                        name = guestEnrollment.Name,
                        ac_role = AcRole.GetRoleName(guestEnrollment.PermissionId),
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
                if ((meeting.Reused.HasValue && meeting.Reused.Value) 
                    || LmsCourseMeetingModel.GetByCompanyAndScoId(lmsCompany, meeting.GetMeetingScoId()).Any(x => x.Id != meeting.Id))
                {
                    provider.UpdateScoPermissionForPrincipal(
                        principalIds.Select(
                            principalId =>
                                new PermissionUpdateTrio
                                {
                                    ScoId = meetingSco,
                                    PrincipalId = principalId,
                                    PermissionId = MeetingPermissionId.remove,
                                }));
                }
            }

            this.AddUsersToMeetingHostsGroup(provider, hostPrincipals);

            return result;
        }

        private Principal CreatePrincipalAndUpdateLmsUserPrincipalId(IAdobeConnectProxy provider,
            LmsUserDTO lmsUserDto, LmsUser dbUser, LmsCompany lmsCompany)
        {
            Principal principal = acUserService.GetOrCreatePrincipal(
                        provider,
                        lmsUserDto.GetLogin(),
                        lmsUserDto.GetEmail(),
                        lmsUserDto.GetFirstName(),
                        lmsUserDto.GetLastName(),
                        lmsCompany);
            if (principal != null)
            {
                lmsUserDto.ac_id = principal.PrincipalId;
                dbUser.PrincipalId = principal.PrincipalId;
                this.LmsUserModel.RegisterSave(dbUser);
            }

            return principal;
        }

        public List<LmsUserDTO> SetDefaultRolesForNonParticipants(
            LmsCompany lmsCompany,
            IAdobeConnectProxy provider, 
            LtiParamDTO param, 
            int id, 
            bool forceUpdate, 
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
                param.lms_user_id, 
                param.course_id, 
                out error, 
                param, 
                forceUpdate);

            if (meeting == null)
            {
                return users;
            }

            string[] userIds = users.Select(user => user.lti_id ?? user.id).ToArray();
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
            List<PermissionInfo> enrollments = this.GetMeetingAttendees(provider, meeting.GetMeetingScoId());
            return SetDefaultRolesForNonParticipants(lmsCompany, provider, meeting, users, lmsDbUsers, enrollments, ref error);
        }

        /// <summary>
        /// The set default users.
        /// </summary>
        /// <param name="lmsCompany">
        /// The lmsCompany.
        /// </param>
        /// <param name="meeting">
        /// The meeting.
        /// </param>
        /// <param name="provider">
        /// The provider.
        /// </param>
        /// <param name="lmsUserId">
        /// The LMS User Id.
        /// </param>
        /// <param name="courseId">
        /// The LMS course id.
        /// </param>
        /// <param name="meetingScoId">
        /// The meeting SCO id.
        /// </param>
        /// <param name="extraData">
        /// The extra Data.
        /// </param>
        public void SetDefaultUsers(
            LmsCompany lmsCompany, 
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
                lmsUsers = this.LmsUserModel.GetByCompanyLms(lmsCompany.Id, users);
            }

            this.ProcessUsersInAC(lmsCompany, provider, meetingScoId, users, principalCache, lmsUsers, true);
        }

        /// <summary>
        /// The set LMS user default AC permissions.
        /// </summary>
        /// <param name="provider">
        /// The provider.
        /// </param>
        /// <param name="meetingScoId">
        /// The meeting SCO id.
        /// </param>
        /// <param name="u">
        /// The user.
        /// </param>
        /// <param name="principalId">
        /// The principal Id.
        /// </param>
        /// <param name="ignoreAC">
        /// The ignore AC
        /// </param>
        public void SetLMSUserDefaultACPermissions(
            IAdobeConnectProxy provider,
            LmsCompany lmsCompany, 
            string meetingScoId, 
            LmsUserDTO u, 
            string principalId)
        {
            var permission = new RoleMappingService().SetAcRole(lmsCompany, u);

            if (!string.IsNullOrWhiteSpace(principalId) && !string.IsNullOrWhiteSpace(meetingScoId))
            {
                provider.UpdateScoPermissionForPrincipal(meetingScoId, principalId, permission);
                if (permission == MeetingPermissionId.host)
                {
                    this.AddUserToMeetingHostsGroup(provider, principalId);
                }
            }
        }

        /// <summary>
        /// The set lms user default ac permissions 2.
        /// </summary>
        /// <param name="meetingScoId">
        /// The meeting sco id.
        /// </param>
        /// <param name="u">
        /// The u.
        /// </param>
        /// <param name="principalId">
        /// The principal id.
        /// </param>
        /// <param name="meetingPermission">
        /// The meeting permission.
        /// </param>
        /// <param name="hostPrincipals">
        /// The host principals.
        /// </param>
        public void SetLMSUserDefaultACPermissions2(
            LmsCompany lmsCompany,
            string meetingScoId, 
            LmsUserDTO u, 
            string principalId, 
            List<PermissionUpdateTrio> meetingPermission, 
            List<string> hostPrincipals)
        {
            var permission = new RoleMappingService().SetAcRole(lmsCompany, u);

            if (!string.IsNullOrWhiteSpace(principalId) && !string.IsNullOrWhiteSpace(meetingScoId))
            {
                // TODO: try not use by one
                // provider.UpdateScoPermissionForPrincipal(meetingScoId, principalId, permission);
                meetingPermission.Add(
                    new PermissionUpdateTrio
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
            LmsCompany lmsCompany,
            IAdobeConnectProxy provider, 
            LtiParamDTO param, 
            LmsUserDTO user, 
            int id, 
            out string error, 
            bool skipReturningUsers = false)
        {
            error = null;
            LmsCourseMeeting meeting = this.LmsCourseMeetingModel.GetOneByCourseAndId(
                lmsCompany.Id, 
                param.course_id, 
                id);

            // TODO:  DO WE NEED IT ?
            if (meeting == null)
            {
                return skipReturningUsers 
                    ? null 
                    : GetOrCreateUserWithAcRole(lmsCompany, provider, param, meeting, out error, lmsUserId: user.id);
            }

            // NOTE: now we create AC principal within Users/GetAll method. So user will always have ac_id here.
            if (user.ac_id == null)
            {
                logger.WarnFormat("[UpdateUser]. ac_id == null. LmsCompanyId:{0}. Id:{1}. UserLogin:{2}.", lmsCompany.Id, id, user.GetLogin());

                Principal principal = acUserService.GetOrCreatePrincipal(
                    provider,
                    user.GetLogin(),
                    user.GetEmail(),
                    user.GetFirstName(),
                    user.GetLastName(),
                    lmsCompany);

                if (principal != null)
                {
                    user.ac_id = principal.PrincipalId;
                }
                else if (lmsCompany.DenyACUserCreation)
                {
                    error = "User does not exist in AC database. You must create AC accounts manually";
                    return null;
                }
            }
            var nonEditable = new HashSet<string>();

            if (user.ac_role == null)
            {
                provider.UpdateScoPermissionForPrincipal(
                    meeting.GetMeetingScoId(),
                    user.ac_id,
                    MeetingPermissionId.remove);
            }
            else
            {
                var attendees = GetMeetingAttendees(
                    provider,
                    meeting.GetMeetingScoId(),
                    nonEditable);
                var permission = MeetingPermissionId.view;
                if (attendees.Contains(user.ac_id))
                {
                    if (user.ac_role.Equals(AcRole.Presenter.Name, StringComparison.OrdinalIgnoreCase))
                    {
                        permission = MeetingPermissionId.mini_host;
                    }
                    else if (user.ac_role.Equals(AcRole.Host.Name, StringComparison.OrdinalIgnoreCase))
                    {
                        permission = MeetingPermissionId.host;
                    }
                }
                else
                {
                    permission = new RoleMappingService().SetAcRole(lmsCompany, user);
                }

                try
                {
                    StatusInfo status = provider.UpdateScoPermissionForPrincipal(meeting.GetMeetingScoId(), user.ac_id, permission);
                }
                catch (InvalidOperationException)
                {
                    // NOTE: check that Principal is in AC yet
                    var principalInfo = provider.GetOneByPrincipalId(user.ac_id);
                    if (!principalInfo.Success)
                    {
                        var dbUser = this.LmsUserModel.GetOneByUserIdAndCompanyLms(user.lti_id ?? user.id, lmsCompany.Id).Value;

                        var principal = CreatePrincipalAndUpdateLmsUserPrincipalId(provider, user, dbUser, lmsCompany);

                        if (principal == null && lmsCompany.DenyACUserCreation)
                        {
                            error = "User does not exist in AC database. You must create AC accounts manually";
                            return null;
                        }
                    }

                    // NOTE: try again
                    StatusInfo status = provider.UpdateScoPermissionForPrincipal(meeting.GetMeetingScoId(), user.ac_id, permission);
                }

                if (permission == MeetingPermissionId.host)
                {
                    this.AddUserToMeetingHostsGroup(provider, user.ac_id);
                }
            }

            return skipReturningUsers 
                ? null 
                : GetOrCreateUserWithAcRole(lmsCompany, provider, param, meeting, out error, lmsUserId: user.id);
        }

        public LmsUserDTO UpdateGuest(
            LmsCompany lmsCompany,
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
                logger.ErrorFormat("Meeting not found. LmsCompanyId: {}, CourseId: {1}, ID: {2}.", lmsCompany.Id, param.course_id, id);
                error = "Meeting not found";
                return null;
            }

            if (user.ac_id == null)
            {
                error = "Guest user should have Adobe Connect account";
                return null;
            }

            if (user.ac_role == null)
            {
                provider.UpdateScoPermissionForPrincipal(
                    meeting.GetMeetingScoId(),
                    user.ac_id,
                    MeetingPermissionId.remove);
                LmsCourseMeetingGuest guest = meeting.MeetingGuests.FirstOrDefault(x => x.Id == user.guest_id);
                if (guest != null)
                {
                    meeting.MeetingGuests.Remove(guest);
                    LmsCourseMeetingModel.RegisterSave(meeting, flush: true);
                }

                // TRICK: remove id to delete record on client-side
                return new LmsUserDTO
                {
                    id = user.id,
                };
            }
            else
            {
                var permission = AcRole.GetByName(user.ac_role).MeetingPermissionId;
                provider.UpdateScoPermissionForPrincipal(meeting.GetMeetingScoId(), user.ac_id, permission);

                if (permission == MeetingPermissionId.host)
                {
                    AddUserToMeetingHostsGroup(provider, user.ac_id);
                }

                return new LmsUserDTO
                {
                    id = user.id,
                    guest_id = user.guest_id,
                    ac_id = user.ac_id,
                    name = user.name,
                    ac_role = user.ac_role,
                };
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// The chunk.
        /// </summary>
        /// <param name="source">
        /// The source.
        /// </param>
        /// <param name="chunksize">
        /// The chunksize.
        /// </param>
        /// <typeparam name="T">
        /// </typeparam>
        /// <returns>
        /// The <see cref="IEnumerable"/>.
        /// </returns>
        private static IEnumerable<IEnumerable<T>> Chunk<T>(IEnumerable<T> source, int chunksize)
        {
            while (source.Any())
            {
                yield return source.Take(chunksize);
                source = source.Skip(chunksize);
            }
        }

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
        /// <param name="hosts">
        /// The hosts.
        /// </param>
        /// <param name="presenters">
        /// The presenters.
        /// </param>
        /// <param name="participants">
        /// The participants.
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
            PermissionCollectionResult allEnrollments = provider.GetAllMeetingEnrollments(meetingSco);
            List<PermissionInfo> hostsResult =
                allEnrollments.Values.Return(
                    x => x.Where(v => v.PermissionId == PermissionId.host).ToList(), 
                    new List<PermissionInfo>());
            List<PermissionInfo> presentersResult =
                allEnrollments.Values.Return(
                    x => x.Where(v => v.PermissionId == PermissionId.mini_host).ToList(), 
                    new List<PermissionInfo>());
            List<PermissionInfo> participantsResult =
                allEnrollments.Values.Return(
                    x => x.Where(v => v.PermissionId == PermissionId.view).ToList(), 
                    new List<PermissionInfo>());
            foreach (PermissionInfo g in hostsResult)
            {
                alreadyAdded.Add(g.PrincipalId);
            }

            foreach (PermissionInfo g in presentersResult)
            {
                alreadyAdded.Add(g.PrincipalId);
            }

            foreach (PermissionInfo g in participantsResult)
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
        /// The <see cref="List{PermissionInfo}"/>.
        /// </returns>
        private static List<PermissionInfo> ProcessACMeetingAttendees(
            HashSet<string> nonEditable,
            IAdobeConnectProxy provider, 
            List<PermissionInfo> values, 
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
                        new PermissionInfo
                            {
                                PrincipalId = g.PrincipalId, 
                                Name = g.Name, 
                                Login = g.Login, 
                                IsPrimary = g.IsPrimary
                            });
                    nonEditable.Add(g.PrincipalId);
                    alreadyAdded.Add(g.PrincipalId);
                }
            }

            return values;
        }

        /// <summary>
        /// The get all principals.
        /// </summary>
        /// <param name="lmsCompany">
        /// The lms company.
        /// </param>
        /// <param name="provider">
        /// The provider.
        /// </param>
        /// <param name="users">
        /// The users.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable"/>.
        /// </returns>
        /// <exception cref="InvalidOperationException">
        /// </exception>
        private IEnumerable<Principal> GetAllPrincipals(
            LmsCompany lmsCompany,
            IAdobeConnectProxy provider, 
            List<LmsUserDTO> users)
        {
            var cacheMode = this.settings.Lti_AcUserCache_Mode as string;

            if (string.IsNullOrEmpty(cacheMode) || cacheMode.Equals("DISABLED", StringComparison.OrdinalIgnoreCase))
            {
                return null;
            }

            if (cacheMode.Equals("DATABASE", StringComparison.OrdinalIgnoreCase))
            {
                try
                {
                    return
                        IoC.Resolve<AcCachePrincipalModel>()
                            .GetByLmsCompany(lmsCompany.Id, users)
                            .Select(
                                source =>
                                new Principal
                                    {
                                        AccountId = source.AccountId, 
                                        DisplayId = source.DisplayId, 
                                        Email = source.Email, 
                                        FirstName = source.FirstName, 
                                        HasChildren = source.HasChildren.Value, 
                                        IsHidden = source.IsHidden.Value, 
                                        IsPrimary = source.IsPrimary.Value, 
                                        LastName = source.LastName, 
                                        Login = source.Login, 
                                        Name = source.Name, 
                                        PrincipalId = source.PrincipalId, 
                                        Type = source.Type, 
                                    })
                            .ToList();
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException("UsersSetup.GetAllPrincipals.DATABASE error", ex);
                }
            }

            if (cacheMode.Equals("CONNECT", StringComparison.OrdinalIgnoreCase))
            {
                try
                {
                    PrincipalCollectionResult result = provider.GetAllPrincipal();
                    if (result.Success)
                    {
                        return result.Values;
                    }
                    else
                    {
                        // See details: https://helpx.adobe.com/adobe-connect/kb/operation-size-error-connect-enterprise.html
                        bool tooBigPrincipalCount = result.Status.InnerXml.Contains("operation-size-error");
                        if (!tooBigPrincipalCount)
                        {
                            if (result.Status.UnderlyingExceptionInfo != null)
                            {
                                throw new InvalidOperationException(
                                    "UsersSetup.GetAllPrincipals.CONNECT error", 
                                    result.Status.UnderlyingExceptionInfo);
                            }

                            throw new InvalidOperationException("UsersSetup.GetAllPrincipals.CONNECT error");
                        }

                        return null;
                    }
                }
                catch (Exception ex)
                {
                    IoC.Resolve<ILogger>().Error("GetAllPrincipals.CONNECT", ex);
                    throw;
                }
            }

            IoC.Resolve<ILogger>().Error("Unsupported cache mode: " + cacheMode);
            return null;
        }

        private void ProcessUsersInAC(
            LmsCompany lmsCompany,
            IAdobeConnectProxy provider, 
            string meetingScoId, 
            List<LmsUserDTO> users, 
            IEnumerable<Principal> principalCache, 
            IEnumerable<LmsUser> lmsUsers, 
            bool reRunOnError)
        {
            var meetingPermissions = new List<PermissionUpdateTrio>();
            var hostPrincipals = new List<string>();
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
                    string id = u.lti_id ?? u.id;
                    LmsUser lmsUser = lmsUsers.FirstOrDefault(x => x.UserId == id);

                    if (lmsUser == null || !principal.PrincipalId.Equals(lmsUser.PrincipalId))
                    {
                        if (lmsUser == null)
                        {
                            lmsUser = new LmsUser 
                            {
                                LmsCompany = lmsCompany, 
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

            // TRICK: do not move down to chunk part!
            this.AddUsersToMeetingHostsGroup(provider, hostPrincipals);

            foreach (var chunk in Chunk(meetingPermissions, 50))
            {
                StatusInfo status = provider.UpdateScoPermissionForPrincipal(chunk);
                if (status.Code != StatusCodes.ok)
                {
                    if (reRunOnError)
                    {
                        IoC.Resolve<ILogger>()
                            .Error("UpdateScoPermissionForPrincipal - 1st try. Status.Code=" + status.Code.ToString());

                        IoC.Resolve<IPrincipalCache>().RecreatePrincipalCache(IoC.Resolve<LmsCompanyModel>().GetAll());
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
                            false);
                        return;
                    }
                    else
                    {
                        IoC.Resolve<ILogger>()
                            .Error("UpdateScoPermissionForPrincipal - 2nd try. Status.Code=" + status.Code.ToString());
                        throw new InvalidOperationException(
                            "UpdateScoPermissionForPrincipal. Status.Code=" + status.Code.ToString());
                    }
                }
            }
        }

        #endregion
    }
}