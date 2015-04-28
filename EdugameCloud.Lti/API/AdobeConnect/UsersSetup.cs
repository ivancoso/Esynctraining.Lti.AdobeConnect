using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
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

namespace EdugameCloud.Lti.API.AdobeConnect
{
    public interface IUsersSetup
    {
        List<LmsUserDTO> UpdateUser(
            LmsCompany lmsCompany,
            AdobeConnectProvider provider,
            LtiParamDTO param,
            LmsUserDTO user,
            string scoId,
            out string error,
            bool skipReturningUsers = false);

        void SetLMSUserDefaultACPermissions(
            AdobeConnectProvider provider,
            string meetingScoId,
            LmsUserDTO u,
            string principalId,
            bool ignoreAC = false);
    }

    /// <summary>
    /// The users setup.
    /// </summary>
    public class UsersSetup : IUsersSetup
    {
        #region Fields

        /// <summary>
        /// The locker.
        /// </summary>
        private static readonly Dictionary<string, object> locker = new Dictionary<string, object>();

        private readonly IBrainHoneyApi dlapApi;
        private readonly IEGCEnabledCanvasAPI canvasApi;
        private readonly IBlackBoardApi soapApi;
        private readonly IMoodleApi moodleApi;
        private readonly LTI2Api lti2Api;
        private readonly dynamic settings;
        private readonly ILogger logger;

        #endregion

        #region Constructors and Destructors

        public UsersSetup(IBrainHoneyApi dlapApi, IBlackBoardApi soapApi, IMoodleApi moodleApi, LTI2Api lti2Api, IEGCEnabledCanvasAPI canvasApi,
            ApplicationSettingsProvider settings, ILogger logger)
        {
            this.dlapApi = dlapApi;
            this.soapApi = soapApi;
            this.moodleApi = moodleApi;
            this.lti2Api = lti2Api;
            this.canvasApi = canvasApi;
            this.settings = settings;
            this.logger = logger;
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

        #region Public Methods

        public void GetParamLoginAndEmail(
            LtiParamDTO param,
            LmsCompany lmsCompany,
            out string email,
            out string login)
        {
            email = param.lis_person_contact_email_primary;
            login = param.lms_user_login;
            if (string.IsNullOrEmpty(email) && string.IsNullOrEmpty(login))
            {
                //todo: for D2L more effective would be to get WhoIAm and UserInfo information from their API
                string error;
                LmsCourseMeeting meeting = this.LmsCourseMeetingModel.GetOneByCourseAndScoId(lmsCompany.Id, param.course_id, null);
                List<LmsUserDTO> users = this.GetLMSUsers(lmsCompany, meeting, param.lms_user_id, param.course_id, out error, param, true);
                var user =
                    users.FirstOrDefault(
                        u => u.lti_id != null && u.lti_id.Equals(param.lms_user_id, StringComparison.InvariantCultureIgnoreCase));
                if (user != null)
                {
                    login = user.login_id;
                }
            }
        }

        /// <summary>
        /// The get users.
        /// </summary>
        /// <param name="lmsCompany">
        /// The credentials.
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
        /// <param name="error">
        /// The error.
        /// </param>
        /// <param name="forceUpdate">
        /// The force Update.
        /// </param>
        /// <returns>
        /// The <see cref="List{LmsUserDTO}"/>.
        /// </returns>
        [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1305:FieldNamesMustNotUseHungarianNotation",
            Justification = "Reviewed. Suppression is OK here.")]
        public List<LmsUserDTO> GetUsers(LmsCompany lmsCompany, AdobeConnectProvider provider, LtiParamDTO param, string scoId, out string error, bool forceUpdate = false)
        {
            LmsCourseMeeting meeting = this.LmsCourseMeetingModel.GetOneByCourseAndScoId(lmsCompany.Id, param.course_id, scoId);
            List<LmsUserDTO> users = this.GetLMSUsers(lmsCompany, meeting, param.lms_user_id, param.course_id, out error, param, forceUpdate);
            if (meeting == null)
            {
                return users;
            }

            List<PermissionInfo> hosts, participants, presenters;
            var nonEditable = new HashSet<string>();
            GetMeetingAttendees(provider, meeting.GetMeetingScoId(), out hosts, out presenters, out participants, nonEditable);

            string[] userIds = users.Select(user => user.lti_id ?? user.id).ToArray();
            IEnumerable<LmsUser> lmsUsers = LmsUserModel.GetByUserIdAndCompanyLms(userIds, lmsCompany.Id);
            //Debug.Assert(userIds.Length == lmsUsers.Count(), "Should return single user by userId+lmsCompany.Id");

            // NOTE: sometimes we have no users here - for example due any security issue in LMS service (BlackBoard)
            // So skip this step for better performance
            if (users.Count > 0)
            {
                IEnumerable<Principal> principalCache = GetAllPrincipals(lmsCompany, provider, users);

                bool uncommitedChangesInLms = false;
                foreach (LmsUserDTO user in users)
                {
                    string login = user.GetLogin();
                    var lmsUser = lmsUsers.FirstOrDefault(u => u.UserId == (user.lti_id ?? user.id))
                        ?? new LmsUser
                        {
                            LmsCompany = lmsCompany,
                            Username = login,
                            UserId = user.lti_id ?? user.id,
                        };

                    if (string.IsNullOrEmpty(lmsUser.PrincipalId))
                    {
                        var principal = this.GetOrCreatePrincipal2(provider,
                            login, user.GetEmail(),
                            user.GetFirstName(), user.GetLastName(),
                            lmsCompany, principalCache);

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
                    this.LmsUserModel.Flush();
            }

            // ReSharper disable once LoopCanBeConvertedToQuery
            foreach (PermissionInfo permissionInfo in hosts.Where(h => !h.HasChildren))
            {
                users.Add(new LmsUserDTO 
                {
                    ac_id = permissionInfo.PrincipalId,
                    name = permissionInfo.Name,
                    ac_role = "Host",
                });
            }

            // ReSharper disable once LoopCanBeConvertedToQuery
            foreach (PermissionInfo permissionInfo in presenters.Where(h => !h.HasChildren))
            {
                users.Add(new LmsUserDTO
                {
                    ac_id = permissionInfo.PrincipalId,
                    name = permissionInfo.Name,
                    ac_role = "Presenter",
                });
            }

            // ReSharper disable once LoopCanBeConvertedToQuery
            foreach (PermissionInfo permissionInfo in participants.Where(h => !h.HasChildren))
            {
                users.Add(new LmsUserDTO
                {
                    ac_id = permissionInfo.PrincipalId,
                    name = permissionInfo.Name,
                    ac_role = "Participant",
                });
            }

            return users;
        }

        /// <summary>
        /// The update user.
        /// </summary>
        /// <param name="lmsCompany">
        /// The credentials.
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
        /// <returns>
        /// The <see cref="List{LmsUserDTO}"/>.
        /// </returns>
        public List<LmsUserDTO> SetDefaultRolesForNonParticipants(
            LmsCompany lmsCompany,
            AdobeConnectProvider provider,
            LtiParamDTO param,
            string scoId,
            bool forceUpdate,
            out string error)
        {
            LmsCourseMeeting meeting = this.LmsCourseMeetingModel.GetOneByCourseAndScoId(lmsCompany.Id, param.course_id, scoId);
       
            List<LmsUserDTO> users = this.GetLMSUsers(lmsCompany, meeting, param.lms_user_id, param.course_id, out error, param, forceUpdate);
            
            if (meeting == null)
            {
                return users;
            }
            string meetingSco = meeting.GetMeetingScoId();

            List<PermissionInfo> enrollments = this.GetMeetingAttendees(provider, meeting.GetMeetingScoId());

            var principalIds = new HashSet<string>(enrollments.Select(e => e.PrincipalId));

            IEnumerable<LmsUser> lmsUsers = this.LmsUserModel.GetByCompanyLms(lmsCompany.Id, users);
            var denyACUserCreation = lmsCompany.DenyACUserCreation;

            var meetingPermissions = new List<PermissionUpdateTrio>();
            var hostPrincipals = new List<string>();

            foreach (LmsUserDTO lmsUser in users)
            {
                // TRICK: we can filter by 'UserId' - cause we sync it in 'getUsersByLmsCompanyId' SP
                string id = lmsUser.lti_id ?? lmsUser.id;
                LmsUser user = lmsUsers.FirstOrDefault(x => x.UserId == id)
                    ?? new LmsUser
                    {
                        LmsCompany = lmsCompany,
                        Username = lmsUser.GetLogin(),
                        UserId = id,
                    };

                if (string.IsNullOrEmpty(user.PrincipalId))
                {
                    var principal = this.GetOrCreatePrincipal(provider, lmsUser.GetLogin(), lmsUser.GetEmail(), lmsUser.GetFirstName(), lmsUser.GetLastName(), lmsCompany);
                    if (principal != null)
                    {
                        user.PrincipalId = principal.PrincipalId;
                        this.LmsUserModel.RegisterSave(user);
                    }
                    else if (denyACUserCreation)
                    {
                        error = "At least one user does not exist in AC database. You must create AC accounts manually";
                        continue;
                    }
                }

                var enrollment = enrollments.FirstOrDefault(e => e.PrincipalId.Equals(user.PrincipalId));

                if (enrollment != null)
                {
                    lmsUser.ac_id = user.PrincipalId;
                    lmsUser.ac_role = enrollment.PermissionId == PermissionId.host
                        ? "Host"
                        : (enrollment.PermissionId == PermissionId.mini_host ? "Presenter" : "Participant");
                    principalIds.Remove(user.PrincipalId);
                }
                else if (!string.IsNullOrEmpty(user.PrincipalId))
                {
                    this.SetLMSUserDefaultACPermissions2(provider, meetingSco, lmsUser, user.PrincipalId, meetingPermissions, hostPrincipals);
                }
            }

            foreach (var chunk in Chunk(meetingPermissions, 50))
            {
                var status = provider.UpdateScoPermissionForPrincipal(chunk);
                if (status.Code != StatusCodes.ok)
                {
                    throw new InvalidOperationException("SetDefaultRolesForNonParticipants > UpdateScoPermissionForPrincipal. Status.Code=" + status.Code.ToString());
                }
            }

            provider.UpdateScoPermissionForPrincipal(principalIds.Select(principalId => new PermissionUpdateTrio
            {
                ScoId = meetingSco,
                PrincipalId = principalId,
                PermissionId = MeetingPermissionId.remove,
            }));

            AddUsersToMeetingHostsGroup(provider, hostPrincipals);

            return users;
        }

        /// <summary>
        /// The update user.
        /// </summary>
        /// <param name="lmsCompany">
        /// The credentials.
        /// </param>
        /// <param name="provider">
        /// The provider.
        /// </param>
        /// <param name="param">
        /// The parameter.
        /// </param>
        /// <param name="user">
        /// The user.
        /// </param>
        /// <param name="scoId">
        /// The SCO Id.
        /// </param>
        /// <param name="error">
        /// The error.
        /// </param>
        /// <param name="skipReturningUsers">
        /// The skip Returning Users.
        /// </param>
        /// <returns>
        /// The <see cref="List{LmsUserDTO}"/>.
        /// </returns>
        public List<LmsUserDTO> UpdateUser(
            LmsCompany lmsCompany,
            AdobeConnectProvider provider,
            LtiParamDTO param,
            LmsUserDTO user,
            string scoId,
            out string error,
            bool skipReturningUsers = false)
        {
            error = null;
            LmsCourseMeeting meeting = this.LmsCourseMeetingModel.GetOneByCourseAndScoId(lmsCompany.Id, param.course_id, scoId);
            if (meeting == null)
            {
                return skipReturningUsers ? null : this.GetUsers(lmsCompany, provider, param, scoId, out error);
            }

            if (user.ac_id == null)
            {
                var principal = this.GetOrCreatePrincipal(
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
                return skipReturningUsers ? null : this.GetUsers(lmsCompany, provider, param, scoId, out error);
            }

            if (user.ac_role == null)
            {
                provider.UpdateScoPermissionForPrincipal(meeting.GetMeetingScoId(), user.ac_id, MeetingPermissionId.remove);
                return skipReturningUsers ? null : this.GetUsers(lmsCompany, provider, param, scoId, out error);
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

            return skipReturningUsers ? null : this.GetUsers(lmsCompany, provider, param, scoId, out error);
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

            if (role.Contains("teacher") || role.Contains("instructor") || role.Contains("owner") || role.Contains("admin"))
            {
                permission = MeetingPermissionId.host;
                u.ac_role = "Host";
            }
            else if (role.Contains("ta") || role.Contains("designer") || role.Contains("author") || role.Contains("teaching assistant")
                || role.Contains("course builder") || role.Contains("evaluator") || role == "advisor")
            {
                u.ac_role = "Presenter";
                permission = MeetingPermissionId.mini_host;
            }

            if (ignoreAC)
            {
                return;
            }

            if (!string.IsNullOrWhiteSpace(principalId)
                && !string.IsNullOrWhiteSpace(meetingScoId))
            {
                provider.UpdateScoPermissionForPrincipal(meetingScoId, principalId, permission);
                if (permission == MeetingPermissionId.host)
                {
                    this.AddUserToMeetingHostsGroup(provider, principalId);
                }
            }
        }

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

            if (role.Contains("teacher") || role.Contains("instructor") || role.Contains("owner") || role.Contains("admin"))
            {
                permission = MeetingPermissionId.host;
                u.ac_role = "Host";
            }
            else if (role.Contains("ta") || role.Contains("designer") || role.Contains("author") || role.Contains("teaching assistant")
                || role.Contains("course builder") || role.Contains("evaluator") || role == "advisor")
            {
                u.ac_role = "Presenter";
                permission = MeetingPermissionId.mini_host;
            }

            if (!string.IsNullOrWhiteSpace(principalId)
                && !string.IsNullOrWhiteSpace(meetingScoId))
            {
                // TODO: try not use by one
                //provider.UpdateScoPermissionForPrincipal(meetingScoId, principalId, permission);
                meetingPermission.Add(new PermissionUpdateTrio { ScoId = meetingScoId, PrincipalId = principalId, PermissionId = permission });

                if (permission == MeetingPermissionId.host)
                {
                    //this.AddUserToMeetingHostsGroup(provider, principalId);
                    hostPrincipals.Add(principalId);
                }
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
            return param.roles != null && (param.roles.Contains("Instructor")
                || param.roles.Contains("Administrator")
                || param.roles.Contains("Course Director")
                || param.roles.Contains("CourseDirector"));
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
        /// <returns>
        /// The <see cref="List{PermissionInfo}"/>.
        /// </returns>
        public List<PermissionInfo> GetMeetingAttendees(
            AdobeConnectProvider provider,
            string meetingSco)
        {
            var alreadyAdded = new HashSet<string>();
            var allMeetingEnrollments = provider.GetAllMeetingEnrollments(meetingSco);
            var allValues = allMeetingEnrollments.Values.Return(x => x.ToList(), new List<PermissionInfo>());
            foreach (var g in allValues)
            {
                alreadyAdded.Add(g.PrincipalId);
            }

            return ProcessACMeetingAttendees(
                new HashSet<string>(),
                provider,
                allValues,
                alreadyAdded);
        }

        /// <summary>
        /// The get AC password.
        /// </summary>
        /// <param name="lmsCompany">
        /// The credentials.
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
        // ReSharper disable once UnusedMethodReturnValue.Local
        public bool AddUserToMeetingHostsGroup(AdobeConnectProvider provider, string principalId)
        {
            var group = provider.AddToGroupByType(principalId, "live-admins");

            return group;
        }

        public bool AddUsersToMeetingHostsGroup(AdobeConnectProvider provider, IEnumerable<string> principalIds)
        {
            var group = provider.AddToGroupByType(principalIds, "live-admins");

            return group;
        }

        /// <summary>
        /// The set default users.
        /// </summary>
        /// <param name="lmsCompany">
        /// The credentials.
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
            List<LmsUserDTO> users = this.GetLMSUsers(lmsCompany, meeting, lmsUserId, courseId, out error, extraData as LtiParamDTO);

            IEnumerable<Principal> principalCache = GetAllPrincipals(lmsCompany, provider, users);
            IEnumerable<LmsUser> lmsUsers = this.LmsUserModel.GetByCompanyLms(lmsCompany.Id, users);

            ProcessUsersInAC(lmsCompany, provider, meetingScoId, users, principalCache, lmsUsers, true);
        }

        private void ProcessUsersInAC(LmsCompany lmsCompany, AdobeConnectProvider provider, 
            string meetingScoId, List<LmsUserDTO> users, 
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

                //
                // TODO: do we need this FORCE?
                // YES: !principal.PrincipalId.Equals(lmsUser.PrincipalId) - we use it to refresh PrincipalId
                //
                var principal = this.GetOrCreatePrincipal2(provider, login, email, u.GetFirstName(), u.GetLastName(), lmsCompany, principalCache);

                if (principal != null)
                {
                    // TRICK: we can filter by 'UserId' - cause we sync it in 'getUsersByLmsCompanyId' SP
                    string id = u.lti_id ?? u.id;
                    var lmsUser = lmsUsers.FirstOrDefault(x => x.UserId == id);

                    if (lmsUser == null || !principal.PrincipalId.Equals(lmsUser.PrincipalId))
                    {
                        if (lmsUser == null)
                        {
                            lmsUser = new LmsUser
                            {
                                LmsCompany = lmsCompany,
                                UserId = id,
                                Username = login,
                            };
                        }

                        lmsUser.PrincipalId = principal.PrincipalId;
                        this.LmsUserModel.RegisterSave(lmsUser);
                    }

                    // TODO: see http://help.adobe.com/en_US/connect/9.0/webservices/WS5b3ccc516d4fbf351e63e3d11a171ddf77-7fcb_SP1.html
                    this.SetLMSUserDefaultACPermissions2(provider, meetingScoId, u, principal.PrincipalId, meetingPermissions, hostPrincipals);
                }
            }

            // TRICK: do not move down to chunk part!
            AddUsersToMeetingHostsGroup(provider, hostPrincipals);

            foreach (var chunk in Chunk(meetingPermissions, 50))
            {
                var status = provider.UpdateScoPermissionForPrincipal(chunk);
                if (status.Code != StatusCodes.ok)
                {
                    if (reRunOnError)
                    {
                        IoC.Resolve<ILogger>().Error("UpdateScoPermissionForPrincipal - 1st try. Status.Code=" + status.Code.ToString());

                        IoC.Resolve<IPrincipalCache>().RecreatePrincipalCache(IoC.Resolve<LmsCompanyModel>().GetAll());
                        IEnumerable<Principal> refreshedPrincipalCache = GetAllPrincipals(lmsCompany, provider, users);
                        ProcessUsersInAC(lmsCompany, provider, meetingScoId, users, refreshedPrincipalCache, lmsUsers, false);
                        return;
                    }
                    else
                    {
                        IoC.Resolve<ILogger>().Error("UpdateScoPermissionForPrincipal - 2nd try. Status.Code=" + status.Code.ToString());
                        throw new InvalidOperationException("UpdateScoPermissionForPrincipal. Status.Code=" + status.Code.ToString());
                    }
                }
            }
        }

        private static IEnumerable<IEnumerable<T>> Chunk<T>(IEnumerable<T> source, int chunksize)
        {
            while (source.Any())
            {
                yield return source.Take(chunksize);
                source = source.Skip(chunksize);
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
        public Principal GetPrincipalByLoginOrEmail(AdobeConnectProvider provider, string login, string email, bool searchByEmailFirst)
        {
            Principal principal = null;

            if (searchByEmailFirst && !string.IsNullOrWhiteSpace(email))
            {
                var resultByEmail = provider.GetAllByEmail(email);
                if (!resultByEmail.Success)
                {
                    return null;
                }

                principal = resultByEmail.Return(x => x.Values, Enumerable.Empty<Principal>()).FirstOrDefault();
            }

            if (principal == null && !string.IsNullOrWhiteSpace(login))
            {
                var resultByLogin = provider.GetAllByLogin(login);
                if (!resultByLogin.Success)
                {
                    return null;
                }

                principal = resultByLogin.Return(x => x.Values, Enumerable.Empty<Principal>()).FirstOrDefault();
            }
            
            if (!searchByEmailFirst && principal == null && !string.IsNullOrWhiteSpace(email))
            {
                var resultByEmail = provider.GetAllByEmail(email);
                if (!resultByEmail.Success)
                {
                    return null;
                }

                principal = resultByEmail.Return(x => x.Values, Enumerable.Empty<Principal>()).FirstOrDefault();
            }

            return principal;
        }

        private Principal GetPrincipalByLoginOrEmail(IEnumerable<Principal> principalCache, string login, string email, bool searchByEmailFirst)
        {
            Principal principal = null;

            if (searchByEmailFirst && !string.IsNullOrWhiteSpace(email))
            {
                principal = principalCache.FirstOrDefault(p => p.Email.Equals(email, StringComparison.OrdinalIgnoreCase));
            }

            if ((principal == null) && !string.IsNullOrWhiteSpace(login))
            {
                principal = principalCache.FirstOrDefault(p => p.Login.Equals(login, StringComparison.OrdinalIgnoreCase));
            }

            if (!searchByEmailFirst && (principal == null) && !string.IsNullOrWhiteSpace(email))
            {
                principal = principalCache.FirstOrDefault(p => p.Email.Equals(email, StringComparison.OrdinalIgnoreCase));
            }

            return principal;
        }

        public Principal GetOrCreatePrincipal(
            AdobeConnectProvider provider,
            string login,
            string email,
            string firstName,
            string lastName,
            LmsCompany lmsCompany)
        {
            bool searchByEmailFirst = lmsCompany.ACUsesEmailAsLogin.GetValueOrDefault();
            bool denyUserCreation = lmsCompany.DenyACUserCreation;
            var principal = this.GetPrincipalByLoginOrEmail(provider, login, email, searchByEmailFirst);

            if (principal == null && !denyUserCreation)
            {
                var setup = new PrincipalSetup
                {
                    Email = string.IsNullOrWhiteSpace(email) ? null : email,
                    FirstName = firstName,
                    LastName = lastName,
                    Name = login,
                    Login = login,
                    Type = PrincipalTypes.user,
                };

                PrincipalResult pu = provider.PrincipalUpdate(setup);

                // TODO: review and add
                //if (!pu.Success)
                //{
                //    throw new InvalidOperationException("AC.PrincipalUpdate error", pu.Status.UnderlyingExceptionInfo);
                //}

                if (pu.Principal != null)
                {
                    principal = pu.Principal;
                }
            }

            return principal;
        }

        public Principal GetOrCreatePrincipal2(
            AdobeConnectProvider provider,
            string login,
            string email,
            string firstName,
            string lastName,
            LmsCompany lmsCompany,
            IEnumerable<Principal> principalCache)
        {
            bool searchByEmailFirst = lmsCompany.ACUsesEmailAsLogin.GetValueOrDefault();
            bool denyUserCreation = lmsCompany.DenyACUserCreation;

            Principal principal = null;
            if (principalCache != null)
            {
                principal = GetPrincipalByLoginOrEmail(principalCache, login, email, searchByEmailFirst);
            }

            if (principal == null)
            {
                principal = GetPrincipalByLoginOrEmail(provider, login, email, searchByEmailFirst);
            }

            if (!denyUserCreation && (principal == null))
            {
                var setup = new PrincipalSetup
                {
                    Email = string.IsNullOrWhiteSpace(email) ? null : email,
                    FirstName = firstName,
                    LastName = lastName,
                    Name = login,
                    Login = login,
                    Type = PrincipalTypes.user,
                };

                PrincipalResult pu = provider.PrincipalUpdate(setup);

                // TODO: review and add
                //if (!pu.Success)
                //{
                //    throw new InvalidOperationException("AC.PrincipalUpdate error", pu.Status.UnderlyingExceptionInfo);
                //}

                if (pu.Principal != null)
                {
                    principal = pu.Principal;
                }
            }

            return principal;
        }

        #endregion

        #region Private Methods

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
        //private bool IsUserSynched(IEnumerable<PermissionInfo> enrollments, LmsUserDTO lmsUser, AdobeConnectProvider provider)
        //{
        //    bool isFound = false;
        //    foreach (var host in enrollments)
        //    {
        //        if (this.LmsUserIsAcUser(lmsUser, host, provider))
        //        {
        //            lmsUser.ac_id = host.PrincipalId;
        //            lmsUser.ac_role = this.GetRoleString(host.PermissionId);
        //            isFound = true;
        //            break;
        //        }
        //    }

        //    if (!isFound)
        //    {
        //        return false;
        //    }

        //    return true;
        //}

        ///// <summary>
        ///// The get role string.
        ///// </summary>
        ///// <param name="permissionId">
        ///// The permission id.
        ///// </param>
        ///// <returns>
        ///// The <see cref="string"/>.
        ///// </returns>
        //private string GetRoleString(PermissionId permissionId)
        //{
        //    switch (permissionId)
        //    {
        //        case PermissionId.host:
        //            return "Host";
        //        case PermissionId.mini_host:
        //            return "Presenter";
        //        case PermissionId.view:
        //            return "Participant";
        //    }

        //    return "Unknown";
        //}

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
        //private bool IsParticipantSynched(List<LmsUserDTO> lmsUsers, PermissionInfo participant, AdobeConnectProvider provider)
        //{
        //    bool found = false;
        //    foreach (var lmsUser in lmsUsers)
        //    {
        //        if (this.LmsUserIsAcUser(lmsUser, participant, provider))
        //        {
        //            found = true;
        //        }
        //    }

        //    return found;
        //}

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
        //private bool LmsUserIsAcUser(LmsUserDTO lmsUser, PermissionInfo participant, AdobeConnectProvider provider)
        //{
        //    string email = lmsUser.GetEmail(), login = lmsUser.GetLogin();
        //    var isACUser = participant.Login != null && ((email != null && email.Equals(participant.Login, StringComparison.OrdinalIgnoreCase))
        //           || (login != null && login.Equals(participant.Login, StringComparison.OrdinalIgnoreCase)));
        //    if (isACUser)
        //    {
        //        return true;
        //    }
        //    var principal = provider.GetOneByPrincipalId(participant.PrincipalId);
        //    if (principal.Success)
        //    {
        //        isACUser = principal.PrincipalInfo.Principal.Email.Equals(email)
        //                   || principal.PrincipalInfo.Principal.Email.Equals(login);
        //    }

        //    return isACUser;
        //}

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

            foreach (var groupid in groupIds)
            {
                var groupPrincipals = provider.GetGroupUsers(groupid);
                principals.AddRange(groupPrincipals.Values);
            }

            return principals;
        }

        public List<LmsUserDTO> GetLMSUsers(LmsCompany credentials, LmsCourseMeeting meeting, string lmsUserId, int courseId, out string error, LtiParamDTO extraData = null, bool forceUpdate = false)
        {
            switch (credentials.LmsProvider.ShortName.ToLowerInvariant())
            {
                case LmsProviderNames.Canvas:
                    error = null;
                    return this.GetCanvasUsers(credentials, lmsUserId, courseId);
                case LmsProviderNames.BrainHoney:
                    return this.GetBrainHoneyUsers(credentials, courseId, out error, extraData);
                case LmsProviderNames.Blackboard:
                    return this.GetBlackBoardUsers(credentials, meeting, courseId, out error, forceUpdate);
                case LmsProviderNames.Moodle:
                    return this.GetMoodleUsers(credentials, courseId, out error);
                case LmsProviderNames.Sakai:
                    return this.GetSakaiUsers(credentials, extraData as LtiParamDTO, out error);
                case LmsProviderNames.Desire2Learn:
                    return this.GetDesire2LearnUsers(credentials, extraData as LtiParamDTO, lmsUserId, out error);
            }

            error = null;
            return new List<LmsUserDTO>();
        }

        /// <summary>
        /// The get canvas users.
        /// </summary>
        /// <param name="credentials">
        /// The credentials.
        /// </param>
        /// <param name="canvasUserId">
        /// The canvas User Id.
        /// </param>
        /// <param name="canvasCourseId">
        /// The canvas course id.
        /// </param>
        /// <returns>
        /// The <see cref="List{LmsUserDTO}"/>.
        /// </returns>
        private List<LmsUserDTO> GetCanvasUsers(LmsCompany credentials, string canvasUserId, int canvasCourseId)
        {
            var lmsUser = this.LmsUserModel.GetOneByUserIdAndCompanyLms(canvasUserId, credentials.Id).Value;
            var token = lmsUser.Return(
                    u => u.Token,
                    string.Empty);

            // probably it makes sence to call this method with company admin's token - for getting emails within one call
            List<LmsUserDTO> users = canvasApi.GetUsersForCourse(
                credentials.LmsDomain,
                token,
                canvasCourseId);

            // emails are now included in the above api call
            // leaving code below for old-style support for getting user email (for example for students)
            if (users.Any(x => string.IsNullOrEmpty(x.primary_email)))
            {
                var adminCourseUsers =
                    users.Where(u => u.lms_role.ToUpper().Equals("TEACHER") || u.lms_role.ToUpper().Equals("TA"))
                        .Select(u => u.id)
                        .Distinct();
                var adminCourseTokens =
                    adminCourseUsers.Select(u => this.LmsUserModel.GetOneByUserIdAndCompanyLms(u, credentials.Id).Value)
                        .Where(v => v != null)
                        .Select(v => v.Token)
                        .Where(t => t != null)
                        .ToList();
                if (!adminCourseTokens.Contains(token))
                {
                    adminCourseTokens.Add(token);
                }
                if (credentials.AdminUser != null && credentials.AdminUser.Token != null
                    && !adminCourseTokens.Contains(credentials.AdminUser.Token))
                {
                    adminCourseTokens.Add(credentials.AdminUser.Token);
                }

                foreach (var user in users)
                {
                    if (string.IsNullOrEmpty(user.primary_email))
                    {
                        // todo: investigate cases(except when role=student) when API does not return emails and probably remove this code
                        logger.InfoFormat("[Canvas GetUsers] Api did not return email for user with id={}", user.id);
                        foreach (var adminToken in adminCourseTokens)
                        {
                            if (!string.IsNullOrEmpty(user.primary_email))
                            {
                                break;
                            }
                            canvasApi.AddMoreDetailsForUser(credentials.LmsDomain, adminToken, user);
                        }
                    }
                }
            }

            return GroupUsers(users);
        }

        /// <summary>
        /// The get brain honey users.
        /// </summary>
        /// <param name="credentials">
        /// The credentials.
        /// </param>
        /// <param name="brainHoneyCourseId">
        /// The brain honey course id.
        /// </param>
        /// <param name="error">
        /// The error.
        /// </param>
        /// <param name="extraData">
        /// The extra Data.
        /// </param>
        /// <returns>
        /// The <see cref="List{LmsUserDTO}"/>.
        /// </returns>
        private List<LmsUserDTO> GetBrainHoneyUsers(LmsCompany credentials, int brainHoneyCourseId, out string error, object extraData)
        {
            //Session session = extraData == null ? null : (Session)extraData;
            List<LmsUserDTO> users = this.dlapApi.GetUsersForCourse(credentials, brainHoneyCourseId, out error, extraData);
            return GroupUsers(users);
        }

        private List<LmsUserDTO> GetDesire2LearnUsers(LmsCompany lmsCompany, LtiParamDTO param, string lmsUserId, out string error)
        {
            error = null; //todo: set when something is wrong
            LmsUser lmsUser = lmsCompany.AdminUser;
            if (lmsUser == null)
            {
                logger.WarnFormat("[GetD2LUsers] AdminUser is not set for LmsCompany with id={0}", lmsCompany.Id);
                lmsUser = LmsUserModel.GetOneByUserIdAndCompanyLms(lmsUserId, lmsCompany.Id).Value;
                if(lmsUser == null)
                    return new List<LmsUserDTO>();
            }

            if (string.IsNullOrEmpty(lmsUser.Token))
            {
                logger.WarnFormat("[GetD2LUsers]: Token does not exist for LmsUser with id={0}", lmsUser.Id);
                return new List<LmsUserDTO>();
            }

            var tokens = lmsUser.Token.Split(' ');
            var d2lService = IoC.Resolve<IDesire2LearnApiService>();

            //get course users list
            var classlistEnrollments = d2lService.GetApiObjects<List<ClasslistUser>>(
                tokens[0], tokens[1], param.lms_domain,
                String.Format(d2lService.EnrollmentsClasslistUrlFormat, (string)settings.D2LApiVersion, param.context_id));

            //get enrollments - this information contains user roles
            var enrollmentsList = new List<OrgUnitUser>();
            PagedResultSet<OrgUnitUser> enrollments = null;
            do
            {
                enrollments = d2lService.GetApiObjects<PagedResultSet<OrgUnitUser>>(tokens[0], tokens[1], param.lms_domain,
                    String.Format(d2lService.EnrollmentsUrlFormat, (string)settings.D2LApiVersion, param.context_id) +
                    (enrollments != null ? "?bookmark=" + enrollments.PagingInfo.Bookmark : string.Empty));
                if (enrollments == null || enrollments.Items == null)
                {
                    error = "Incorrect API call or returned data. Please contact site administrator";
                    logger.Error("[D2L Enrollments]: Object returned from API has null value");
                    return new List<LmsUserDTO>();
                }

                enrollmentsList.AddRange(enrollments.Items);
            }
            while (enrollments.PagingInfo.HasMoreItems);

            //mapping to LmsUserDTO
            var result = new List<LmsUserDTO>();
            if (classlistEnrollments != null)
            {
                // current user is admin and not enrolled to this course -> add him to user list
                if (classlistEnrollments.All(x => x.Identifier != lmsUserId))
                {
                    var currentUserInfo = d2lService.GetApiObjects<WhoAmIUser>(tokens[0], tokens[1], param.lms_domain,
                        String.Format(d2lService.WhoAmIUrlFormat, (string)settings.D2LApiVersion));
                    if (currentUserInfo != null)
                    {
                        classlistEnrollments.Add(new ClasslistUser
                        {
                            Identifier = currentUserInfo.Identifier,
                            Username = currentUserInfo.UniqueName,
                            DisplayName = currentUserInfo.FirstName + " " + currentUserInfo.LastName
                        });
                    }
                }

                foreach (var enrollment in classlistEnrollments)
                {
                    var userInfo = enrollmentsList.FirstOrDefault(e => e.User.Identifier == enrollment.Identifier);
                    var user = new LmsUserDTO
                    {
                        id = enrollment.Identifier,
                        login_id = enrollment.Username,
                        name = enrollment.DisplayName,
                        primary_email = enrollment.Email,
                        lms_role = userInfo != null ? userInfo.Role.Name : "Unknown"
                    };
                    result.Add(user);
                }
            }

            return result;
        }

        /// <summary>
        /// The get brain honey users.
        /// </summary>
        /// <param name="credentials">
        /// The credentials.
        /// </param>
        /// <param name="param">
        /// The parameter.
        /// </param>
        /// <param name="error">
        /// The error.
        /// </param>
        /// <returns>
        /// The <see cref="List{LmsUserDTO}"/>.
        /// </returns>
        private List<LmsUserDTO> GetSakaiUsers(LmsCompany credentials, LtiParamDTO param, out string error)
        {
            if (param != null)
            {
                var users = this.lti2Api.GetUsersForCourse(
                    credentials,
                    param.ext_ims_lis_memberships_url ?? param.ext_ims_lti_tool_setting_url,
                    param.ext_ims_lis_memberships_id,
                    out error);
                return GroupUsers(users);
            }

            error = "extra data is not set";
            return new List<LmsUserDTO>();
        }

        /// <summary>
        /// The get brain honey users.
        /// </summary>
        /// <param name="credentials">
        /// The credentials.
        /// </param>
        /// <param name="moodleCourseId">
        /// The Moodle course id.
        /// </param>
        /// <param name="error">
        /// The error.
        /// </param>
        /// <returns>
        /// The <see cref="List{LmsUserDTO}"/>.
        /// </returns>
        private List<LmsUserDTO> GetMoodleUsers(LmsCompany credentials, int moodleCourseId, out string error)
        {
            var users = this.moodleApi.GetUsersForCourse(credentials, moodleCourseId, out error);
            return GroupUsers(users);
        }

        private List<LmsUserDTO> GetBlackBoardUsers(LmsCompany credentials, LmsCourseMeeting meeting, int blackBoardCourseId, out string error, bool forceUpdate = false)
        {
            var timeout = TimeSpan.Parse((string)this.settings.UserCacheValidTimeout);
            var key = credentials.LmsDomain + ".course." + blackBoardCourseId;
            error = null;
            List<LmsUserDTO> cachedUsers = CheckCachedUsers(meeting, forceUpdate, timeout);
            if (cachedUsers == null)
            {
                var lockMe = GetLocker(key);
                lock (lockMe)
                {
                    if (meeting != null)
                    {
                        this.LmsCourseMeetingModel.Refresh(ref meeting);
                    }

                    cachedUsers = CheckCachedUsers(meeting, forceUpdate, timeout);
                    if (cachedUsers == null)
                    {
                        WebserviceWrapper client = null;
                        var users = this.soapApi.GetUsersForCourse(
                            credentials,
                            blackBoardCourseId,
                            out error,
                            ref client);

                        if ((users.Count == 0) && error.Return(x => x.IndexOf("ACCESS DENIED", StringComparison.InvariantCultureIgnoreCase) >= 0, false))
                        {
                            IoC.Resolve<ILogger>().Warn("GetBlackBoardUsers.AccessDenied. " + error);
                            // NOTE: set to null to re-create session.
                            client = null;
                            users = this.soapApi.GetUsersForCourse(
                                credentials,
                                blackBoardCourseId,
                                out error,
                                ref client);
                        }

                        // TODO: try to call logout
                        //client.logout();

                        if (string.IsNullOrWhiteSpace(error) && (meeting != null))
                        {
                            meeting.AddedToCache = DateTime.Now;
                            meeting.CachedUsers = JsonConvert.SerializeObject(users);
                            this.LmsCourseMeetingModel.RegisterSave(meeting, true);
                        }
                        else if ((users.Count == 0) && error.Return(x => x.IndexOf("ACCESS DENIED", StringComparison.InvariantCultureIgnoreCase) >= 0, false))
                        {
                            users = CheckCachedUsers(meeting, false, timeout) ?? new List<LmsUserDTO>();
                        }

                        cachedUsers = users;
                    }
                }
            }

            return GroupUsers(cachedUsers);
        }

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
        /// The get locker.
        /// </summary>
        /// <param name="lockerKey">
        /// The locker key.
        /// </param>
        /// <returns>
        /// The <see cref="object"/>.
        /// </returns>
        private static object GetLocker(string lockerKey)
        {
            if (!locker.ContainsKey(lockerKey))
            {
                lock (locker)
                {
                    if (!locker.ContainsKey(lockerKey))
                    {
                        locker.Add(lockerKey, new object());
                    }
                }
            }

            return locker[lockerKey];
        }

        /// <summary>
        /// The group users.
        /// </summary>
        /// <param name="users">
        /// The users.
        /// </param>
        /// <returns>
        /// The <see cref="List{LmsUserDTO}"/>.
        /// </returns>
        private static List<LmsUserDTO> GroupUsers(List<LmsUserDTO> users)
        {
            if (users != null && users.Any())
            {
                var order = new List<string>
                                {
                                    "owner",
                                    "author",
                                    "course builder",
                                    "teacher",
                                    "instructor",
                                    "teaching assistant",
                                    "ta",
                                    "designer",
                                    "student",
                                    "learner",
                                    "reader",
                                    "guest"
                                };
                users = users.GroupBy(u => u.id).Select(
                    ug =>
                    {
                        foreach (var orderRole in order)
                        {
                            string role = orderRole;
                            var userDTO =
                                ug.FirstOrDefault(u => u.lms_role.Equals(role, StringComparison.OrdinalIgnoreCase));
                            if (userDTO != null)
                            {
                                return userDTO;
                            }
                        }

                        return ug.First();
                    }).ToList();

                return users;
            }

            return new List<LmsUserDTO>();
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
            var groupPrincipalList = values.Where(x => x.HasChildren).Select(v => v.PrincipalId);
            if (groupPrincipalList.Any())
            {
                var groupValues = GetGroupPrincipals(provider, groupPrincipalList);
                foreach (var g in groupValues)
                {
                    if (alreadyAdded.Contains(g.PrincipalId))
                    {
                        continue;
                    }

                    values.Add(new PermissionInfo { PrincipalId = g.PrincipalId, Name = g.Name, Login = g.Login, IsPrimary = g.IsPrimary });
                    nonEditable.Add(g.PrincipalId);
                    alreadyAdded.Add(g.PrincipalId);
                }
            }

            return values;
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
            var allEnrollments = provider.GetAllMeetingEnrollments(meetingSco);
            var hostsResult =
                allEnrollments.Values.Return(
                    x => x.Where(v => v.PermissionId == PermissionId.host).ToList(),
                    new List<PermissionInfo>());
            var presentersResult =
                allEnrollments.Values.Return(
                    x => x.Where(v => v.PermissionId == PermissionId.mini_host).ToList(),
                    new List<PermissionInfo>());
            var participantsResult =
                allEnrollments.Values.Return(
                    x => x.Where(v => v.PermissionId == PermissionId.view).ToList(),
                    new List<PermissionInfo>());
            foreach (var g in hostsResult)
            {
                alreadyAdded.Add(g.PrincipalId);
            }

            foreach (var g in presentersResult)
            {
                alreadyAdded.Add(g.PrincipalId);
            }

            foreach (var g in participantsResult)
            {
                alreadyAdded.Add(g.PrincipalId);
            }

            nonEditable = nonEditable ?? new HashSet<string>();

            hosts = ProcessACMeetingAttendees(nonEditable, provider, hostsResult, alreadyAdded);
            presenters = ProcessACMeetingAttendees(nonEditable, provider, presentersResult, alreadyAdded);
            participants = ProcessACMeetingAttendees(nonEditable, provider, participantsResult, alreadyAdded);
        }

        private IEnumerable<Principal> GetAllPrincipals(LmsCompany lmsCompany, AdobeConnectProvider provider, List<LmsUserDTO> users)
        {
            string cacheMode = settings.Lti_AcUserCache_Mode as string;
            
            if (string.IsNullOrEmpty(cacheMode) || cacheMode.Equals("DISABLED", StringComparison.OrdinalIgnoreCase))
            {
                return null;
            }

            if (cacheMode.Equals("DATABASE", StringComparison.OrdinalIgnoreCase))
            {
                try
                {
                    return IoC.Resolve<AcCachePrincipalModel>().GetByLmsCompany(lmsCompany.Id, users).Select(source => new Principal
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
                    }).ToList();
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
                                throw new InvalidOperationException("UsersSetup.GetAllPrincipals.CONNECT error", result.Status.UnderlyingExceptionInfo);
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

        #endregion

    }

}
