namespace EdugameCloud.Lti.API.AdobeConnect
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Web.Security;

    using BbWsClient;

    using Castle.Core.Logging;

    using EdugameCloud.Lti.API.BlackBoard;
    using EdugameCloud.Lti.API.BrainHoney;
    using EdugameCloud.Lti.API.Canvas;
    using EdugameCloud.Lti.API.Common;
    using EdugameCloud.Lti.API.Desire2Learn;
    using EdugameCloud.Lti.API.Moodle;
    using EdugameCloud.Lti.Core.Business.Models;
    using EdugameCloud.Lti.Domain.Entities;
    using EdugameCloud.Lti.DTO;
    using EdugameCloud.Lti.Extensions;
    using EdugameCloud.Lti.OAuth.Desire2Learn;

    using Esynctraining.AC.Provider;
    using Esynctraining.AC.Provider.DataObjects.Results;
    using Esynctraining.AC.Provider.Entities;
    using Esynctraining.Core.Extensions;
    using Esynctraining.Core.Providers;
    using Esynctraining.Core.Utils;

    using Newtonsoft.Json;

    /// <summary>
    /// The UsersSetup interface.
    /// </summary>
    public interface IUsersSetup
    {
        #region Public Methods and Operators

        void SetLMSUserDefaultACPermissions(
            AdobeConnectProvider provider, 
            string meetingScoId, 
            LmsUserDTO user, 
            string principalId, 
            bool ignoreAC = false);

        LmsUserDTO UpdateUser(
            LmsCompany lmsCompany, 
            AdobeConnectProvider provider, 
            LtiParamDTO param, 
            LmsUserDTO user, 
            string scoId, 
            out string error, 
            bool skipReturningUsers = false);

        #endregion
    }

    /// <summary>
    ///     The users setup.
    /// </summary>
    public class UsersSetup : IUsersSetup
    {
        #region Fields

        /// <summary>
        /// The settings.
        /// </summary>
        private readonly dynamic settings;

        private readonly LmsFactory lmsFactory;

        private IAdobeConnectUserService acUserService;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="UsersSetup"/> class.
        /// </summary>
        /// <param name="dlapApi">
        /// The dlap api.
        /// </param>
        /// <param name="soapApi">
        /// The soap api.
        /// </param>
        /// <param name="moodleApi">
        /// The moodle api.
        /// </param>
        /// <param name="lti2Api">
        /// The lti 2 api.
        /// </param>
        /// <param name="canvasApi">
        /// The canvas api.
        /// </param>
        /// <param name="settings">
        /// The settings.
        /// </param>
        /// <param name="logger">
        /// The logger.
        /// </param>
        public UsersSetup(
            ApplicationSettingsProvider settings,
            LmsFactory lmsFactory,
            IAdobeConnectUserService acUserService)
        {
            this.settings = settings;
            this.lmsFactory = lmsFactory;
            this.acUserService = acUserService;
        }

        #endregion

        #region Properties

        /// <summary>
        ///     Gets the canvas course meeting model.
        /// </summary>
        private LmsCourseMeetingModel LmsCourseMeetingModel
        {
            get
            {
                return IoC.Resolve<LmsCourseMeetingModel>();
            }
        }

        /// <summary>
        ///     Gets the LMS user parameters.
        /// </summary>
        private LmsUserModel LmsUserModel
        {
            get
            {
                return IoC.Resolve<LmsUserModel>();
            }
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
        public bool AddUserToMeetingHostsGroup(AdobeConnectProvider provider, string principalId)
        {
            bool group = provider.AddToGroupByType(principalId, "live-admins");

            return group;
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
        public bool AddUsersToMeetingHostsGroup(AdobeConnectProvider provider, IEnumerable<string> principalIds)
        {
            bool group = provider.AddToGroupByType(principalIds, "live-admins");

            return group;
        }

        /// <summary>
        /// The get AC password.
        /// </summary>
        /// <param name="lmsCompany">
        /// The lmsCompany.
        /// </param>
        /// <param name="userSettings">
        /// The user settings.
        /// </param>
        /// <param name="email">
        /// The email.
        /// </param>
        /// <param name="login">
        /// The login.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
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

        /// <summary>
        /// The get lms users.
        /// </summary>
        /// <param name="lmsCompany">
        /// The lmsCompany.
        /// </param>
        /// <param name="meeting">
        /// The meeting.
        /// </param>
        /// <param name="lmsUserId">
        /// The lms user id.
        /// </param>
        /// <param name="courseId">
        /// The course id.
        /// </param>
        /// <param name="error">
        /// The error.
        /// </param>
        /// <param name="extraData">
        /// The extra data.
        /// </param>
        /// <param name="forceUpdate">
        /// The force update.
        /// </param>
        /// <returns>
        /// The <see cref="List"/>.
        /// </returns>
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
                var userDtos = meeting.MeetingRoles.Select(x=>new LmsUserDTO
                {
                    ac_id = x.User.PrincipalId,
                    id = x.User.UserId,
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
            var serviceResult = service.GetUsers(lmsCompany, meeting, lmsUser ?? new LmsUser{UserId = lmsUserId}, courseId, extraData, forceUpdate);
            if (serviceResult.isSuccess)
            {
                error = null;
                return serviceResult.data;
            }
            var users = service.GetUsersOldStyle(lmsCompany, meeting, lmsUserId, courseId, out error, forceUpdate, extraData);
            return error == null ? users : new List<LmsUserDTO>();
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
        public List<PermissionInfo> GetMeetingAttendees(AdobeConnectProvider provider, string meetingSco)
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

        /// <summary>
        /// The get or create principal.
        /// </summary>
        /// <param name="provider">
        /// The provider.
        /// </param>
        /// <param name="login">
        /// The login.
        /// </param>
        /// <param name="email">
        /// The email.
        /// </param>
        /// <param name="firstName">
        /// The first name.
        /// </param>
        /// <param name="lastName">
        /// The last name.
        /// </param>
        /// <param name="lmsCompany">
        /// The lms company.
        /// </param>
        /// <returns>
        /// The <see cref="Principal"/>.
        /// </returns>
        /// <exception cref="InvalidOperationException">
        /// </exception>

        /// <summary>
        /// The get or create principal 2.
        /// </summary>
        /// <param name="provider">
        /// The provider.
        /// </param>
        /// <param name="login">
        /// The login.
        /// </param>
        /// <param name="email">
        /// The email.
        /// </param>
        /// <param name="firstName">
        /// The first name.
        /// </param>
        /// <param name="lastName">
        /// The last name.
        /// </param>
        /// <param name="lmsCompany">
        /// The lms company.
        /// </param>
        /// <param name="principalCache">
        /// The principal cache.
        /// </param>
        /// <returns>
        /// The <see cref="Principal"/>.
        /// </returns>


        /// <summary>
        /// The get param login and email.
        /// </summary>
        /// <param name="param">
        /// The param.
        /// </param>
        /// <param name="lmsCompany">
        /// The lms company.
        /// </param>
        /// <param name="email">
        /// The email.
        /// </param>
        /// <param name="login">
        /// The login.
        /// </param>
        public void GetParamLoginAndEmail(LtiParamDTO param, LmsCompany lmsCompany, out string email, out string login)
        {
            email = param.lis_person_contact_email_primary;
            login = param.lms_user_login;
            if (string.IsNullOrEmpty(email) && string.IsNullOrEmpty(login))
            {
                // todo: for D2L more effective would be to get WhoIAm and UserInfo information from their API
                string error;
                LmsCourseMeeting meeting = this.LmsCourseMeetingModel.GetOneByCourseAndScoId(
                    lmsCompany.Id, 
                    param.course_id, 
                    null);
                var currentUser = LmsUserModel.GetOneByUserIdAndCompanyLms(param.lms_user_id, lmsCompany.Id).Value;

                var lmsUserService = lmsFactory.GetUserService((LmsProviderEnum) lmsCompany.LmsProvider.Id);
                LmsUserDTO user = lmsUserService.GetUser(lmsCompany,
                    currentUser,
                    meeting,
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

        /// <summary>
        /// The get AC user.
        /// </summary>
        /// <param name="provider">
        /// The provider.
        /// </param>
        /// <param name="login">
        /// The login.
        /// </param>
        /// <param name="email">
        /// The email.
        /// </param>
        /// <param name="searchByEmailFirst">
        /// The search By Email First.
        /// </param>
        /// <returns>
        /// The <see cref="Principal"/>.
        /// </returns>


        public List<LmsUserDTO> GetUsers(
            LmsCompany lmsCompany, 
            AdobeConnectProvider provider, 
            LtiParamDTO param, 
            string scoId, 
            out string error,
            bool forceUpdate = false)
        {
            LmsCourseMeeting meeting = this.LmsCourseMeetingModel.GetOneByCourseAndScoId(
                lmsCompany.Id, 
                param.course_id, 
                scoId);

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

            List<PermissionInfo> hosts, participants, presenters;
            var nonEditable = new HashSet<string>();
            GetMeetingAttendees(
                provider, 
                meeting.GetMeetingScoId(), 
                out hosts, 
                out presenters, 
                out participants, 
                nonEditable);

            string[] userIds = users.Select(user => user.lti_id ?? user.id).ToArray();
            IEnumerable<LmsUser> lmsUsers = null;
            if (lmsCompany.UseSynchronizedUsers && meeting.MeetingRoles != null)
            {
                lmsUsers = meeting.MeetingRoles.Select(x => x.User);
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
                    LmsUser lmsUser = lmsUsers.FirstOrDefault(u => u.UserId == (user.lti_id ?? user.id))
                                      ?? new LmsUser
                                             {
                                                 LmsCompany = lmsCompany, 
                                                 Username = login, 
                                                 UserId = user.lti_id ?? user.id, 
                                             };

                    if (string.IsNullOrEmpty(lmsUser.PrincipalId))
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
                            lmsUser.PrincipalId = principal.PrincipalId;
                            this.LmsUserModel.RegisterSave(lmsUser, flush: false);
                            uncommitedChangesInLms = true;
                        }
                    }

                    if (string.IsNullOrEmpty(lmsUser.PrincipalId))
                    {
                        continue;
                    }

                    user.ac_id = lmsUser.PrincipalId;
                    user.is_editable = !nonEditable.Contains(user.ac_id);

                    if (hosts.Any(v => v.PrincipalId == user.ac_id))
                    {
                        user.ac_role = "Host";
                        hosts = hosts.Where(v => v.PrincipalId != user.ac_id).ToList();
                    }
                    else if (presenters.Any(v => v.PrincipalId == user.ac_id))
                    {
                        user.ac_role = "Presenter";
                        presenters = presenters.Where(v => v.PrincipalId != user.ac_id).ToList();
                    }
                    else if (participants.Any(v => v.PrincipalId == user.ac_id))
                    {
                        user.ac_role = "Participant";
                        participants = participants.Where(v => v.PrincipalId != user.ac_id).ToList();
                    }
                }

                if (uncommitedChangesInLms)
                {
                    this.LmsUserModel.Flush();
                }
            }

            // ReSharper disable once LoopCanBeConvertedToQuery
            foreach (PermissionInfo permissionInfo in hosts.Where(h => !h.HasChildren))
            {
                if (users.All(x => x.ac_id != permissionInfo.PrincipalId))
                {
                    users.Add(
                        new LmsUserDTO
                        {
                            ac_id = permissionInfo.PrincipalId,
                            name = permissionInfo.Name,
                            ac_role = "Host"
                        });
                }
            }

            // ReSharper disable once LoopCanBeConvertedToQuery
            foreach (PermissionInfo permissionInfo in presenters.Where(h => !h.HasChildren))
            {
                users.Add(
                    new LmsUserDTO
                        {
                            ac_id = permissionInfo.PrincipalId, 
                            name = permissionInfo.Name, 
                            ac_role = "Presenter", 
                        });
            }

            // ReSharper disable once LoopCanBeConvertedToQuery
            foreach (PermissionInfo permissionInfo in participants.Where(h => !h.HasChildren))
            {
                users.Add(
                    new LmsUserDTO
                        {
                            ac_id = permissionInfo.PrincipalId, 
                            name = permissionInfo.Name, 
                            ac_role = "Participant", 
                        });
            }

            return users;
        }

        public LmsUserDTO GetOrCreateUserWithAcRole(
            LmsCompany lmsCompany,
            AdobeConnectProvider provider,
            LtiParamDTO param,
            string scoId,
            out string error,
            bool forceUpdate = false, string lmsUserId = null)
        {
            LmsCourseMeeting meeting = this.LmsCourseMeetingModel.GetOneByCourseAndScoId(
                lmsCompany.Id,
                param.course_id,
                scoId);

            var currentUser = LmsUserModel.GetOneByUserIdAndCompanyLms(param.lms_user_id, lmsCompany.Id).Value;

            var service = lmsFactory.GetUserService((LmsProviderEnum)lmsCompany.LmsProvider.Id);
            //todo: not param for BrainHoney
            var lmsUser = service.GetUser(lmsCompany, currentUser, meeting, lmsUserId, param.course_id, out error, param, forceUpdate);

            if (meeting == null)
            {
                return lmsUser;
            }

            if (lmsUser != null)
            {
                var lmsDbUser = this.LmsUserModel.GetOneByUserIdAndCompanyLms(lmsUser.id, lmsCompany.Id);
                List<PermissionInfo> hosts, participants, presenters;
                var nonEditable = new HashSet<string>();
                GetMeetingAttendees(
                    provider,
                    meeting.GetMeetingScoId(),
                    out hosts,
                    out presenters,
                    out participants,
                    nonEditable);
                IEnumerable<Principal> principalCache = GetAllPrincipals(lmsCompany, provider, 
                    new List<LmsUserDTO>{lmsUser});

                bool uncommitedChangesInLms = false;
                ProcessLmsUserDtoAcInfo(lmsUser, lmsDbUser.Value, lmsCompany, principalCache, provider, ref uncommitedChangesInLms,
                    nonEditable, ref hosts, ref participants, ref presenters);

                if (uncommitedChangesInLms)
                {
                    LmsUserModel.Flush();
                }

                return lmsUser;
            }
            return null;
        }

        private void ProcessLmsUserDtoAcInfo(LmsUserDTO user, LmsUser lmsDbUser, LmsCompany lmsCompany,
            IEnumerable<Principal> principalCache, AdobeConnectProvider provider, ref bool uncommitedChangesInLms,
            HashSet<string> nonEditable, ref List<PermissionInfo> hosts, ref List<PermissionInfo> participants, ref List<PermissionInfo> presenters)
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

            if (hosts.Any(v => v.PrincipalId == user.ac_id))
            {
                user.ac_role = "Host";
                hosts = hosts.Where(v => v.PrincipalId != user.ac_id).ToList();
            }
            else if (presenters.Any(v => v.PrincipalId == user.ac_id))
            {
                user.ac_role = "Presenter";
                presenters = presenters.Where(v => v.PrincipalId != user.ac_id).ToList();
            }
            else if (participants.Any(v => v.PrincipalId == user.ac_id))
            {
                user.ac_role = "Participant";
                participants = participants.Where(v => v.PrincipalId != user.ac_id).ToList();
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
                   && (param.roles.Contains("Instructor") || param.roles.Contains("Administrator")
                       || param.roles.Contains("Course Director") || param.roles.Contains("CourseDirector")
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
            AdobeConnectProvider provider,
            LmsCourseMeeting meeting,
            IEnumerable<LmsUserDTO> users,
            IEnumerable<LmsUser> lmsDbUsers,
            ref string error)
        {
            string meetingSco = meeting.GetMeetingScoId();

            bool denyACUserCreation = lmsCompany.DenyACUserCreation;

            var meetingPermissions = new List<PermissionUpdateTrio>();
            var hostPrincipals = new List<string>();

            List<PermissionInfo> enrollments = this.GetMeetingAttendees(provider, meeting.GetMeetingScoId());
            var principalIds = new HashSet<string>(enrollments.Select(e => e.PrincipalId));

            foreach (LmsUserDTO lmsUserDto in users)
            {
                // TRICK: we can filter by 'UserId' - cause we sync it in 'getUsersByLmsCompanyId' SP
                string id = lmsUserDto.lti_id ?? lmsUserDto.id;
                LmsUser dbUser = lmsDbUsers.FirstOrDefault(x => x.UserId == id)
                               ?? new LmsUser { LmsCompany = lmsCompany, Username = lmsUserDto.GetLogin(), UserId = id, };

                if (string.IsNullOrEmpty(dbUser.PrincipalId))
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
                        dbUser.PrincipalId = principal.PrincipalId;
                        this.LmsUserModel.RegisterSave(dbUser);
                    }
                    else if (denyACUserCreation)
                    {
                        error = "At least one user does not exist in AC database. You must create AC accounts manually";
                        continue;
                    }
                }

                PermissionInfo enrollment = enrollments.FirstOrDefault(e => e.PrincipalId.Equals(dbUser.PrincipalId));

                if (enrollment != null)
                {
                    lmsUserDto.ac_id = dbUser.PrincipalId;
                    lmsUserDto.ac_role = enrollment.PermissionId == PermissionId.host
                                          ? "Host"
                                          : (enrollment.PermissionId == PermissionId.mini_host
                                                 ? "Presenter"
                                                 : "Participant");
                    principalIds.Remove(dbUser.PrincipalId);
                }
                else if (!string.IsNullOrEmpty(dbUser.PrincipalId))
                {
                    this.SetLMSUserDefaultACPermissions2(
                        provider,
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
                    throw new InvalidOperationException(
                        "SetDefaultRolesForNonParticipants > UpdateScoPermissionForPrincipal. Status.Code="
                        + status.Code.ToString());
                }
            }

            provider.UpdateScoPermissionForPrincipal(
                principalIds.Select(
                    principalId =>
                    new PermissionUpdateTrio
                    {
                        ScoId = meetingSco,
                        PrincipalId = principalId,
                        PermissionId = MeetingPermissionId.remove,
                    }));

            this.AddUsersToMeetingHostsGroup(provider, hostPrincipals);

            return users.ToList();
        }
        
        public List<LmsUserDTO> SetDefaultRolesForNonParticipants(
            LmsCompany lmsCompany, 
            AdobeConnectProvider provider, 
            LtiParamDTO param, 
            string scoId, 
            bool forceUpdate, 
            out string error)
        {
            LmsCourseMeeting meeting = this.LmsCourseMeetingModel.GetOneByCourseAndScoId(
                lmsCompany.Id, 
                param.course_id, 
                scoId);

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
                lmsDbUsers = meeting.MeetingRoles.Select(x => x.User);
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

            return SetDefaultRolesForNonParticipants(lmsCompany, provider, meeting, users, lmsDbUsers, ref error);
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
            AdobeConnectProvider provider, 
            string lmsUserId, 
            int courseId, 
            string meetingScoId, 
            object extraData = null)
        {
            string error;
            List<LmsUserDTO> users = this.GetLMSUsers(
                lmsCompany, 
                meeting, 
                lmsUserId, 
                courseId, 
                out error, 
                extraData);

            IEnumerable<Principal> principalCache = this.GetAllPrincipals(lmsCompany, provider, users);
            string[] userIds = users.Select(user => user.lti_id ?? user.id).ToArray();
            IEnumerable<LmsUser> lmsUsers = null;
            if (lmsCompany.UseSynchronizedUsers && meeting != null && meeting.MeetingRoles != null)
            {
                lmsUsers = meeting.MeetingRoles.Select(x => x.User);
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
            AdobeConnectProvider provider, 
            string meetingScoId, 
            LmsUserDTO u, 
            string principalId, 
            bool ignoreAC = false)
        {
            var permission = MeetingPermissionId.view;
            u.ac_role = "Participant";
            string role = u.lms_role != null ? u.lms_role.ToLower() : string.Empty;
            if (string.IsNullOrWhiteSpace(u.id) || u.id.Equals("0"))
            {
                permission = MeetingPermissionId.remove;
                u.ac_role = "Remove";
            }

            if (role.Contains("teacher") || role.Contains("instructor") || role.Contains("owner")
                || role.Contains("admin") || role.Contains("lecture"))
            {
                permission = MeetingPermissionId.host;
                u.ac_role = "Host";
            }
            else if (role.Contains("ta") || role.Contains("designer") || role.Contains("author")
                     || role.Contains("teaching assistant") || role.Contains("course builder")
                     || role.Contains("evaluator") || role == "advisor")
            {
                u.ac_role = "Presenter";
                permission = MeetingPermissionId.mini_host;
            }

            if (ignoreAC)
            {
                return;
            }

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
        /// <param name="provider">
        /// The provider.
        /// </param>
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
            AdobeConnectProvider provider, 
            string meetingScoId, 
            LmsUserDTO u, 
            string principalId, 
            List<PermissionUpdateTrio> meetingPermission, 
            List<string> hostPrincipals)
        {
            var permission = MeetingPermissionId.view;
            u.ac_role = "Participant";
            string role = u.lms_role != null ? u.lms_role.ToLower() : string.Empty;
            if (string.IsNullOrWhiteSpace(u.id) || u.id.Equals("0"))
            {
                permission = MeetingPermissionId.remove;
                u.ac_role = "Remove";
            }

            if (role.Contains("teacher") || role.Contains("instructor") || role.Contains("owner")
                || role.Contains("admin") || role.Contains("lecture"))
            {
                permission = MeetingPermissionId.host;
                u.ac_role = "Host";
            }
            else if (role.Contains("ta") || role.Contains("designer") || role.Contains("author")
                     || role.Contains("teaching assistant") || role.Contains("course builder")
                     || role.Contains("evaluator") || role == "advisor")
            {
                u.ac_role = "Presenter";
                permission = MeetingPermissionId.mini_host;
            }

            if (!string.IsNullOrWhiteSpace(principalId) && !string.IsNullOrWhiteSpace(meetingScoId))
            {
                // TODO: try not use by one
                // provider.UpdateScoPermissionForPrincipal(meetingScoId, principalId, permission);
                meetingPermission.Add(
                    new PermissionUpdateTrio
                        {
                            ScoId = meetingScoId, 
                            PrincipalId = principalId, 
                            PermissionId = permission
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
            AdobeConnectProvider provider, 
            LtiParamDTO param, 
            LmsUserDTO user, 
            string scoId, 
            out string error, 
            bool skipReturningUsers = false)
        {
            error = null;
            LmsCourseMeeting meeting = this.LmsCourseMeetingModel.GetOneByCourseAndScoId(
                lmsCompany.Id, 
                param.course_id, 
                scoId);
            if (meeting == null)
            {
                return skipReturningUsers ? null 
                    : this.GetOrCreateUserWithAcRole(lmsCompany, provider, param, scoId, out error, lmsUserId: user.id);
            }

            if (user.ac_id == null)
            {
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

            if (user.ac_id == null)
            {
                return skipReturningUsers ? null 
                    : GetOrCreateUserWithAcRole(lmsCompany, provider, param, scoId, out error, lmsUserId: user.id);
            }

            if (user.ac_role == null)
            {
                provider.UpdateScoPermissionForPrincipal(
                    meeting.GetMeetingScoId(), 
                    user.ac_id, 
                    MeetingPermissionId.remove);
                return skipReturningUsers ? null 
                    : GetOrCreateUserWithAcRole(lmsCompany, provider, param, scoId, out error, lmsUserId: user.id);
            }

            var permission = MeetingPermissionId.view;
            if (user.ac_role.Equals("presenter", StringComparison.InvariantCultureIgnoreCase))
            {
                permission = MeetingPermissionId.mini_host;
            }
            else if (user.ac_role.Equals("host", StringComparison.InvariantCultureIgnoreCase))
            {
                permission = MeetingPermissionId.host;
            }

            provider.UpdateScoPermissionForPrincipal(meeting.GetMeetingScoId(), user.ac_id, permission);
            if (permission == MeetingPermissionId.host)
            {
                this.AddUserToMeetingHostsGroup(provider, user.ac_id);
            }

            return skipReturningUsers ? null 
                    : GetOrCreateUserWithAcRole(lmsCompany, provider, param, scoId, out error, lmsUserId: user.id);
        }

        #endregion

        #region Methods

        /// <summary>
        /// The check cached users.
        /// </summary>
        /// <param name="meeting">
        /// The meeting.
        /// </param>
        /// <param name="forceUpdate">
        /// The force update.
        /// </param>
        /// <param name="timeout">
        /// The timeout.
        /// </param>
        /// <returns>
        /// The <see cref="List{LmsUserDTO}"/>.
        /// </returns>
        private static List<LmsUserDTO> CheckCachedUsers(LmsCourseMeeting meeting, bool forceUpdate, TimeSpan timeout)
        {
            return forceUpdate ? null : meeting.Return(x => x.CachedUsersParsed(timeout), null);
        }

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
        private static List<Principal> GetGroupPrincipals(AdobeConnectProvider provider, IEnumerable<string> groupIds)
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
        private static void GetMeetingAttendees(
            AdobeConnectProvider provider, 
            string meetingSco, 
            out List<PermissionInfo> hosts, 
            out List<PermissionInfo> presenters, 
            out List<PermissionInfo> participants, 
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

            hosts = ProcessACMeetingAttendees(nonEditable, provider, hostsResult, alreadyAdded);
            presenters = ProcessACMeetingAttendees(nonEditable, provider, presentersResult, alreadyAdded);
            participants = ProcessACMeetingAttendees(nonEditable, provider, participantsResult, alreadyAdded);
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
            AdobeConnectProvider provider, 
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
            AdobeConnectProvider provider, 
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
                    return null;
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
            AdobeConnectProvider provider, 
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
                            lmsUser = new LmsUser { LmsCompany = lmsCompany, UserId = id, Username = login, };
                        }

                        lmsUser.PrincipalId = principal.PrincipalId;
                        this.LmsUserModel.RegisterSave(lmsUser);
                    }

                    // TODO: see http://help.adobe.com/en_US/connect/9.0/webservices/WS5b3ccc516d4fbf351e63e3d11a171ddf77-7fcb_SP1.html
                    this.SetLMSUserDefaultACPermissions2(
                        provider, 
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