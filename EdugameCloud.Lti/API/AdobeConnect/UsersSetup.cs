namespace EdugameCloud.Lti.API.AdobeConnect
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Threading;
    using System.Web.Security;

    using BbWsClient;

    using EdugameCloud.Lti.API.BlackBoard;
    using EdugameCloud.Lti.API.BrainHoney;
    using EdugameCloud.Lti.API.Canvas;
    using EdugameCloud.Lti.API.Common;
    using EdugameCloud.Lti.API.Moodle;
    using EdugameCloud.Lti.Business.Models;
    using EdugameCloud.Lti.Domain.Entities;
    using EdugameCloud.Lti.DTO;
    using EdugameCloud.Lti.Extensions;

    using Esynctraining.AC.Provider;
    using Esynctraining.AC.Provider.DataObjects.Results;
    using Esynctraining.AC.Provider.Entities;
    using Esynctraining.Core.Extensions;
    using Esynctraining.Core.Providers;
    using Esynctraining.Core.Utils;

    using Newtonsoft.Json;

    /// <summary>
    /// The users setup.
    /// </summary>
    public class UsersSetup
    {
        #region Fields

        /// <summary>
        /// The locker.
        /// </summary>
        private static readonly Dictionary<string, object> locker = new Dictionary<string, object>();

        /// <summary>
        /// The DLAP API.
        /// </summary>
        private readonly DlapAPI dlapApi;

        /// <summary>
        /// The SOAP API.
        /// </summary>
        private readonly SoapAPI soapApi;

        /// <summary>
        /// The Moodle API.
        /// </summary>
        private readonly MoodleAPI moodleApi;

        /// <summary>
        /// The LTI 2 API.
        /// </summary>
        private readonly LTI2Api lti2Api;

        /// <summary>
        /// The settings.
        /// </summary>
        private readonly dynamic settings;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="UsersSetup"/> class.
        /// </summary>
        /// <param name="dlapApi">
        /// The DLAP API.
        /// </param>
        /// <param name="soapApi">
        /// The SOAP API.
        /// </param>
        /// <param name="moodleApi">
        /// The Moodle API.
        /// </param>
        /// <param name="lti2Api">
        /// The LTI 2 API.
        /// </param>
        /// <param name="settings">
        /// The settings.
        /// </param>
        public UsersSetup(DlapAPI dlapApi, SoapAPI soapApi, MoodleAPI moodleApi, LTI2Api lti2Api, ApplicationSettingsProvider settings)
        {
            this.dlapApi = dlapApi;
            this.soapApi = soapApi;
            this.moodleApi = moodleApi;
            this.lti2Api = lti2Api;
            this.settings = settings;
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

        /// <summary>
        /// The get param login and email.
        /// </summary>
        /// <param name="param">
        /// The param.
        /// </param>
        /// <param name="credentials">
        /// The credentials.
        /// </param>
        /// <param name="provider">
        /// The provider.
        /// </param>
        /// <param name="email">
        /// The email.
        /// </param>
        /// <param name="login">
        /// The login.
        /// </param>
        public void GetParamLoginAndEmail(
            LtiParamDTO param,
            CompanyLms credentials,
            AdobeConnectProvider provider,
            out string email,
            out string login)
        {
            email = param.lis_person_contact_email_primary;
            login = param.lms_user_login;
            if (string.IsNullOrEmpty(email) && string.IsNullOrEmpty(login))
            {
                string error;
                LmsCourseMeeting meeting = this.LmsCourseMeetingModel.GetOneByCourseAndScoId(credentials.Id, param.course_id, null).Value;
                List<LmsUserDTO> users = this.GetLMSUsers(credentials, meeting, param.lms_user_id, param.course_id, out error, param, true);
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
        /// The get canvas users.
        /// </summary>
        /// <param name="credentials">
        /// The credentials.
        /// </param>
        /// <param name="meeting">
        /// The meeting.
        /// </param>
        /// <param name="lmsUserId">
        /// The LMS User Id.
        /// </param>
        /// <param name="courseId">
        /// The course id.
        /// </param>
        /// <param name="error">
        /// The error.
        /// </param>
        /// <param name="extraData">
        /// The extra Data.
        /// </param>
        /// <param name="forceUpdate">
        /// The force Update.
        /// </param>
        /// <returns>
        /// The <see cref="List{LmsUserDTO}"/>.
        /// </returns>
        public List<LmsUserDTO> GetLMSUsers(CompanyLms credentials, LmsCourseMeeting meeting, string lmsUserId, int courseId, out string error, object extraData = null, bool forceUpdate = false)
        {
            switch (credentials.LmsProvider.ShortName.ToLowerInvariant())
            {
                case LmsProviderNames.Canvas:
                    error = null;
                    return this.GetCanvasUsers(credentials, lmsUserId, courseId);
                case LmsProviderNames.BrainHoney:
                    return this.GetBrainHoneyUsers(credentials, courseId, out error, extraData is Session ? extraData : null);
                case LmsProviderNames.Blackboard:
                    return this.GetBlackBoardUsers(credentials, meeting, courseId, out error, forceUpdate);
                case LmsProviderNames.Moodle:
                    return this.GetMoodleUsers(credentials, courseId, out error);
                case LmsProviderNames.Sakai:
                    return this.GetSakaiUsers(credentials, extraData is LtiParamDTO ? (LtiParamDTO)extraData : null, out error);
            }

            error = null;
            return new List<LmsUserDTO>();
        }

        /// <summary>
        /// The get users.
        /// </summary>
        /// <param name="companyLms">
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
        public List<LmsUserDTO> GetUsers(CompanyLms companyLms, AdobeConnectProvider provider, LtiParamDTO param, string scoId, out string error, bool forceUpdate = false)
        {
            LmsCourseMeeting meeting = this.LmsCourseMeetingModel.GetOneByCourseAndScoId(companyLms.Id, param.course_id, scoId).Value;
            List<LmsUserDTO> users = this.GetLMSUsers(companyLms, meeting, param.lms_user_id, param.course_id, out error, param, forceUpdate);

            if (meeting == null)
            {
                return users;
            }

            List<PermissionInfo> hosts, participants, presenters;
            var nonEditable = new HashSet<string>();
            this.GetMeetingAttendees(provider, meeting.GetMeetingScoId(), out hosts, out presenters, out participants, nonEditable);

            foreach (LmsUserDTO user in users)
            {
                string login = user.GetLogin(), email = user.GetEmail();
                var lmsUser = this.LmsUserModel.GetOneByUserIdAndCompanyLms(user.lti_id ?? user.id, companyLms.Id).Value
                     ?? new LmsUser()
                     {
                         CompanyLms = companyLms,
                         Username = login,
                         UserId = user.lti_id ?? user.id
                     };

                if (string.IsNullOrEmpty(lmsUser.PrincipalId))
                {
                    var principal = this.GetOrCreatePrincipal(provider, login, email, user.GetFirstName(), user.GetLastName(), companyLms.ACUsesEmailAsLogin.GetValueOrDefault());
                    if (principal != null)
                    {
                        lmsUser.PrincipalId = principal.PrincipalId;
                        this.LmsUserModel.RegisterSave(lmsUser);
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

            // ReSharper disable once LoopCanBeConvertedToQuery
            foreach (PermissionInfo permissionInfo in hosts.Where(h => !h.HasChildren))
            {
                users.Add(new LmsUserDTO { ac_id = permissionInfo.PrincipalId, name = permissionInfo.Name, ac_role = "Host" });
            }

            // ReSharper disable once LoopCanBeConvertedToQuery
            foreach (PermissionInfo permissionInfo in presenters.Where(h => !h.HasChildren))
            {
                users.Add(new LmsUserDTO
                {
                    ac_id = permissionInfo.PrincipalId,
                    name = permissionInfo.Name,
                    ac_role = "Presenter"
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
                        ac_role = "Participant"
                    });
            }

            return users;
        }

        /// <summary>
        /// The update user.
        /// </summary>
        /// <param name="companyLms">
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
            CompanyLms companyLms,
            AdobeConnectProvider provider,
            LtiParamDTO param,
            string scoId,
            bool forceUpdate)
        {
            LmsCourseMeeting meeting = this.LmsCourseMeetingModel.GetOneByCourseAndScoId(companyLms.Id, param.course_id, scoId).Value;
            string error;
       
            var users = this.GetLMSUsers(companyLms, meeting, param.lms_user_id, param.course_id, out error, param, forceUpdate);
            
            if (meeting == null)
            {
                return users;
            }
            var meetingSco = meeting.GetMeetingScoId();

            List<PermissionInfo> enrollments = this.GetMeetingAttendees(provider, meeting.GetMeetingScoId());

            var principalIds = new HashSet<string>(enrollments.Select(e => e.PrincipalId));

            foreach (var lmsUser in users)
            {
                string login = lmsUser.GetLogin(), email = lmsUser.GetEmail();
                var user = this.LmsUserModel.GetOneByUserIdOrUserNameOrEmailAndCompanyLms(
                    lmsUser.lti_id ?? lmsUser.id,
                    login,
                    email,
                    companyLms.Id) ?? new LmsUser()
                    {
                        CompanyLms = companyLms,
                        Username = login,
                        UserId = lmsUser.lti_id ?? lmsUser.id
                    };

                if (string.IsNullOrEmpty(user.PrincipalId))
                {
                    var principal = this.GetOrCreatePrincipal(provider, login, email, lmsUser.GetFirstName(), lmsUser.GetLastName(), companyLms.ACUsesEmailAsLogin.GetValueOrDefault());
                    if (principal != null)
                    {
                        user.PrincipalId = principal.PrincipalId;
                        this.LmsUserModel.RegisterSave(user);
                    }
                }

                var enrollment = enrollments.FirstOrDefault(e => e.PrincipalId.Equals(user.PrincipalId));

                if (enrollment != null)
                {
                    lmsUser.ac_id = user.PrincipalId;
                    lmsUser.ac_role = enrollment.PermissionId == PermissionId.host
                                          ? "Host"
                                          : (enrollment.PermissionId == PermissionId.mini_host
                                                 ? "Presenter"
                                                 : "Participant");
                    principalIds.Remove(user.PrincipalId);
                }
                else if (!string.IsNullOrEmpty(user.PrincipalId))
                {
                    this.SetLMSUserDefaultACPermissions(provider, meetingSco, lmsUser, user.PrincipalId);
                }
            }

            foreach (var principalId in principalIds)
            {
                provider.UpdateScoPermissionForPrincipal(meetingSco, principalId, MeetingPermissionId.remove);
            }

            return users;
        }

        /// <summary>
        /// The update user.
        /// </summary>
        /// <param name="companyLms">
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
            CompanyLms companyLms,
            AdobeConnectProvider provider,
            LtiParamDTO param,
            LmsUserDTO user,
            string scoId,
            out string error,
            bool skipReturningUsers = false)
        {
            error = null;
            LmsCourseMeeting meeting = this.LmsCourseMeetingModel.GetOneByCourseAndScoId(companyLms.Id, param.course_id, scoId).Value;
            if (meeting == null)
            {
                return skipReturningUsers ? null : this.GetUsers(companyLms, provider, param, scoId, out error);
            }

            if (user.ac_id == null)
            {
                string email = user.GetEmail(), login = user.GetLogin();

                var principal = this.GetOrCreatePrincipal(
                    provider,
                    login,
                    email,
                    user.GetFirstName(),
                    user.GetLastName(),
                    companyLms.ACUsesEmailAsLogin.GetValueOrDefault());

                if (principal != null)
                {
                    user.ac_id = principal.PrincipalId;
                }
            }

            if (user.ac_id == null)
            {
                return skipReturningUsers ? null : this.GetUsers(companyLms, provider, param, scoId, out error);
            }

            if (user.ac_role == null)
            {
                provider.UpdateScoPermissionForPrincipal(meeting.GetMeetingScoId(), user.ac_id, MeetingPermissionId.remove);
                return skipReturningUsers ? null : this.GetUsers(companyLms, provider, param, scoId, out error);
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

            return skipReturningUsers ? null : this.GetUsers(companyLms, provider, param, scoId, out error);
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

            if (role.Contains("teacher") || role.Contains("instructor") || role.Contains("owner"))
            {
                permission = MeetingPermissionId.host;
                u.ac_role = "Host";
            }
            else if (role.Contains("ta") || role.Contains("designer") || role.Contains("author") || role.Contains("teaching assistant") || role.Contains("course builder") || role.Contains("evaluator"))
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
            string meetingSco,
            HashSet<string> nonEditable = null)
        {
            var alreadyAdded = new HashSet<string>();
            var allMeetingEnrollments = provider.GetAllMeetingEnrollments(meetingSco);
            var allValues = allMeetingEnrollments.Values.Return(x => x.ToList(), new List<PermissionInfo>());
            foreach (var g in allValues)
            {
                alreadyAdded.Add(g.PrincipalId);
            }

            return this.ProcessACMeetingAttendees(
                nonEditable ?? new HashSet<string>(),
                provider,
                allValues,
                alreadyAdded);
        }

        /// <summary>
        /// The get AC password.
        /// </summary>
        /// <param name="companyLms">
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
        public string GetACPassword(CompanyLms companyLms, LmsUserSettingsDTO userSettings, string email, string login)
        {
            var connectionMode = (AcConnectionMode)userSettings.acConnectionMode;
            switch (connectionMode)
            {
                case AcConnectionMode.Overwrite:
                    string password = companyLms.AcUsername.Equals(email, StringComparison.OrdinalIgnoreCase)
                                        || companyLms.AcUsername.Equals(login, StringComparison.OrdinalIgnoreCase)
                                          ? companyLms.AcPassword
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

        /// <summary>
        /// The set default users.
        /// </summary>
        /// <param name="companyLms">
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
            CompanyLms companyLms,
            LmsCourseMeeting meeting,
            AdobeConnectProvider provider,
            string lmsUserId,
            int courseId,
            string meetingScoId,
            object extraData = null)
        {
            string error;
            List<LmsUserDTO> users = this.GetLMSUsers(companyLms, meeting, lmsUserId, courseId, out error, extraData);

            foreach (LmsUserDTO u in users)
            {
                string email = u.GetEmail(), login = u.GetLogin();
                var principal = this.GetOrCreatePrincipal(provider, login, email, u.GetFirstName(), u.GetLastName(), companyLms.ACUsesEmailAsLogin.GetValueOrDefault());

                if (principal != null)
                {
                    var lmsUser = this.LmsUserModel.GetOneByUserIdOrUserNameOrEmailAndCompanyLms(
                        u.lti_id ?? u.id,
                        login,
                        email,
                        companyLms.Id);
                    if (lmsUser == null || !principal.PrincipalId.Equals(lmsUser.PrincipalId))
                    {
                        if (lmsUser == null)
                        {
                            lmsUser = new LmsUser()
                                          {
                                              CompanyLms = companyLms,
                                              UserId = u.lti_id ?? u.id,
                                              Username = login
                                          };
                        }

                        lmsUser.PrincipalId = principal.PrincipalId;
                        this.LmsUserModel.RegisterSave(lmsUser);
                    }

                    this.SetLMSUserDefaultACPermissions(provider, meetingScoId, u, principal.PrincipalId);
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

                principal = resultByEmail.Return(x => x.Values, new List<Principal>()).FirstOrDefault();
            }

            if (principal == null && !string.IsNullOrWhiteSpace(login))
            {
                var resultByLogin = provider.GetAllByLogin(login);
                if (!resultByLogin.Success)
                {
                    return null;
                }

                principal = resultByLogin.Return(x => x.Values, new List<Principal>()).FirstOrDefault();
            }
            
            if (!searchByEmailFirst && principal == null && !string.IsNullOrWhiteSpace(email))
            {
                var resultByEmail = provider.GetAllByEmail(email);
                if (!resultByEmail.Success)
                {
                    return null;
                }

                principal = resultByEmail.Return(x => x.Values, new List<Principal>()).FirstOrDefault();
            }

            return principal;
        }

        /// <summary>
        /// The get or create ac user.
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
        /// <param name="searchByEmailFirst">
        /// The search By Email First.
        /// </param>
        /// <returns>
        /// The <see cref="Principal"/>.
        /// </returns>
        public Principal GetOrCreatePrincipal(
            AdobeConnectProvider provider,
            string login,
            string email,
            string firstName,
            string lastName,
            bool searchByEmailFirst)
        {
            var principal = this.GetPrincipalByLoginOrEmail(provider, login, email, searchByEmailFirst);

            if (principal == null)
            {
                var setup = new PrincipalSetup
                {
                    Email = string.IsNullOrWhiteSpace(email) ? null : email,
                    FirstName = firstName,
                    LastName = lastName,
                    Name = login,
                    Login = login,
                    Type = PrincipalTypes.user
                };

                PrincipalResult pu = provider.PrincipalUpdate(setup);

                if (pu.Principal != null)
                {
                    principal = pu.Principal;
                }
            }

            return principal;
        }

        #endregion


        #region Private Methods

        /// <summary>
        /// The is user synched.
        /// </summary>
        /// <param name="enrollments">
        /// The enrollments.
        /// </param>
        /// <param name="lmsUser">
        /// The LMS user.
        /// </param>
        /// <param name="provider">
        /// The provider.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        private bool IsUserSynched(IEnumerable<PermissionInfo> enrollments, LmsUserDTO lmsUser, AdobeConnectProvider provider)
        {
            bool isFound = false;
            foreach (var host in enrollments)
            {
                if (this.LmsUserIsAcUser(lmsUser, host, provider))
                {
                    lmsUser.ac_id = host.PrincipalId;
                    lmsUser.ac_role = this.GetRoleString(host.PermissionId);
                    isFound = true;
                    break;
                }
            }

            if (!isFound)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// The get role string.
        /// </summary>
        /// <param name="permissionId">
        /// The permission id.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        private string GetRoleString(PermissionId permissionId)
        {
            switch (permissionId)
            {
                case PermissionId.host:
                    return "Host";
                case PermissionId.mini_host:
                    return "Presenter";
                case PermissionId.view:
                    return "Participant";
            }

            return "Unknown";
        }

        /// <summary>
        /// The is participant synched.
        /// </summary>
        /// <param name="lmsUsers">
        /// The LMS users.
        /// </param>
        /// <param name="participant">
        /// The participant.
        /// </param>
        /// <param name="provider">
        /// The provider.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        private bool IsParticipantSynched(List<LmsUserDTO> lmsUsers, PermissionInfo participant, AdobeConnectProvider provider)
        {
            bool found = false;
            foreach (var lmsUser in lmsUsers)
            {
                if (this.LmsUserIsAcUser(lmsUser, participant, provider))
                {
                    found = true;
                }
            }

            return found;
        }

        /// <summary>
        /// The LMS user is AC user.
        /// </summary>
        /// <param name="lmsUser">
        /// The LMS user.
        /// </param>
        /// <param name="participant">
        /// The participant.
        /// </param>
        /// <param name="provider">
        /// The provider.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        private bool LmsUserIsAcUser(LmsUserDTO lmsUser, PermissionInfo participant, AdobeConnectProvider provider)
        {
            string email = lmsUser.GetEmail(), login = lmsUser.GetLogin();
            var isACUser = participant.Login != null && ((email != null && email.Equals(participant.Login, StringComparison.OrdinalIgnoreCase))
                   || (login != null && login.Equals(participant.Login, StringComparison.OrdinalIgnoreCase)));
            if (isACUser)
            {
                return true;
            }
            var principal = provider.GetOneByPrincipalId(participant.PrincipalId);
            if (principal.Success)
            {
                isACUser = principal.PrincipalInfo.Principal.Email.Equals(email)
                           || principal.PrincipalInfo.Principal.Email.Equals(login);
            }

            return isACUser;
        }

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
        private List<Principal> GetGroupPrincipals(AdobeConnectProvider provider, IEnumerable<string> groupIds)
        {
            var principals = new List<Principal>();

            foreach (var groupid in groupIds)
            {
                var groupPrincipals = provider.GetGroupUsers(groupid);
                principals.AddRange(groupPrincipals.Values);
            }

            return principals;
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
        private List<LmsUserDTO> GetCanvasUsers(CompanyLms credentials, string canvasUserId, int canvasCourseId)
        {
            var lmsUser = this.LmsUserModel.GetOneByUserIdAndCompanyLms(canvasUserId, credentials.Id).Value;
            var token = lmsUser.Return(
                    u => u.Token,
                    string.Empty);

            List<LmsUserDTO> users = EGCEnabledCanvasAPI.GetUsersForCourse(
                credentials.LmsDomain,
                token,
                canvasCourseId);

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
                foreach (var adminToken in adminCourseTokens)
                {
                    if (!string.IsNullOrEmpty(user.primary_email))
                    {
                        break;
                    }
                    CanvasAPI.AddMoreDetailsForUser(credentials.LmsDomain, adminToken, user);
                }
            }

            return this.GroupUsers(users);
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
        private List<LmsUserDTO> GetBrainHoneyUsers(CompanyLms credentials, int brainHoneyCourseId, out string error, object extraData = null)
        {
            Session session = extraData == null ? null : (Session)extraData;
            List<LmsUserDTO> users = this.dlapApi.GetUsersForCourse(credentials, brainHoneyCourseId, out error, session);
            return this.GroupUsers(users);
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
        private List<LmsUserDTO> GetSakaiUsers(CompanyLms credentials, LtiParamDTO param, out string error)
        {
            if (param != null)
            {
                var users = this.lti2Api.GetUsersForCourse(
                    credentials,
                    param.ext_ims_lis_memberships_url ?? param.ext_ims_lti_tool_setting_url,
                    param.ext_ims_lis_memberships_id,
                    out error);
                return this.GroupUsers(users);
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
        private List<LmsUserDTO> GetMoodleUsers(CompanyLms credentials, int moodleCourseId, out string error)
        {
            var users = this.moodleApi.GetUsersForCourse(credentials, moodleCourseId, out error);
            return this.GroupUsers(users);
        }

        /// <summary>
        /// The get brain honey users.
        /// </summary>
        /// <param name="credentials">
        /// The credentials.
        /// </param>
        /// <param name="meeting">
        /// The meeting.
        /// </param>
        /// <param name="blackBoardCourseId">
        /// The black board course id.
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
        private List<LmsUserDTO> GetBlackBoardUsers(CompanyLms credentials, LmsCourseMeeting meeting, int blackBoardCourseId, out string error, bool forceUpdate = false)
        {
            var timeout = TimeSpan.Parse((string)this.settings.UserCacheValidTimeout);
            var key = credentials.LmsDomain + ".course." + blackBoardCourseId;
            error = null;
            List<LmsUserDTO> cachedUsers = this.CheckCachedUsers(meeting, forceUpdate, timeout);
            if (cachedUsers == null)
            {
                var lockMe = GetLocker(key);
                lock (lockMe)
                {
                    if (meeting != null)
                    {
                        this.LmsCourseMeetingModel.Refresh(ref meeting);
                    }

                    cachedUsers = this.CheckCachedUsers(meeting, forceUpdate, timeout);
                    if (cachedUsers == null)
                    {
                        WebserviceWrapper client = null;
                        var users = this.soapApi.GetUsersForCourse(
                            credentials,
                            blackBoardCourseId,
                            out error,
                            ref client);
                        if (users.Count == 0 && error.Return(x => x.ToLowerInvariant().Contains("access denied"), false))
                        {
                            Thread.Sleep(TimeSpan.FromSeconds(10));
                            users = this.soapApi.GetUsersForCourse(
                                credentials,
                                blackBoardCourseId,
                                out error,
                                ref client);
                        }

                        if (string.IsNullOrWhiteSpace(error) && meeting != null)
                        {
                            meeting.AddedToCache = DateTime.Now;
                            meeting.CachedUsers = JsonConvert.SerializeObject(users);
                            this.LmsCourseMeetingModel.RegisterSave(meeting, true);
                        }
                        else if (users.Count == 0 && error.Return(x => x.ToLowerInvariant().Contains("access denied"), false))
                        {
                            users = this.CheckCachedUsers(meeting, false, timeout) ?? new List<LmsUserDTO>();
                        }

                        cachedUsers = users;
                    }
                }
            }

            return this.GroupUsers(cachedUsers);
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
        private List<LmsUserDTO> CheckCachedUsers(LmsCourseMeeting meeting, bool forceUpdate, TimeSpan timeout)
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
        private List<LmsUserDTO> GroupUsers(List<LmsUserDTO> users)
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
        private List<PermissionInfo> ProcessACMeetingAttendees(
            HashSet<string> nonEditable,
            AdobeConnectProvider provider,
            List<PermissionInfo> values,
            HashSet<string> alreadyAdded)
        {
            var groupValues = this.GetGroupPrincipals(provider, values.Where(x => x.HasChildren).Select(v => v.PrincipalId));
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
        private void GetMeetingAttendees(
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

            hosts = this.ProcessACMeetingAttendees(nonEditable, provider, hostsResult, alreadyAdded);
            presenters = this.ProcessACMeetingAttendees(nonEditable, provider, presentersResult, alreadyAdded);
            participants = this.ProcessACMeetingAttendees(nonEditable, provider, participantsResult, alreadyAdded);
        }

        #endregion
    }
}
