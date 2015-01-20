namespace EdugameCloud.Lti.API.AdobeConnect
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;
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
    using Esynctraining.AC.Provider.DataObjects;
    using Esynctraining.AC.Provider.DataObjects.Results;
    using Esynctraining.AC.Provider.Entities;
    using Esynctraining.Core.Extensions;
    using Esynctraining.Core.Providers;
    using Esynctraining.Core.Utils;
    using Newtonsoft.Json;

    /// <summary>
    ///     The meeting setup.
    /// </summary>
    public class MeetingSetup
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
        /// Initializes a new instance of the <see cref="MeetingSetup"/> class.
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
        public MeetingSetup(DlapAPI dlapApi, SoapAPI soapApi, MoodleAPI moodleApi, LTI2Api lti2Api, ApplicationSettingsProvider settings)
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
        /// Gets the office hours model.
        /// </summary>
        private OfficeHoursModel OfficeHoursModel
        {
            get
            {
                return IoC.Resolve<OfficeHoursModel>();
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

        /// <summary>
        ///     Gets the LMS user parameters model.
        /// </summary>
        private LmsUserParametersModel LmsUserParametersModel
        {
            get
            {
                return IoC.Resolve<LmsUserParametersModel>();
            }
        }

        /// <summary>
        ///     Gets the company LMS model.
        /// </summary>
        private CompanyLmsModel СompanyLmsModel
        {
            get
            {
                return IoC.Resolve<CompanyLmsModel>();
            }
        }

        #endregion

        #region Public Methods and Operators

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
        public void SetLMSUserDefaultACPermissions(
            AdobeConnectProvider provider,
            string meetingScoId,
            LmsUserDTO u,
            string principalId)
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

            if (!string.IsNullOrWhiteSpace(principalId)
                && !string.IsNullOrWhiteSpace(meetingScoId))
            {
                provider.UpdateScoPermissionForPrincipal(meetingScoId, principalId, permission);
                if (permission == MeetingPermissionId.host)
                {
                    provider.AddToGroupByType(principalId, "live-admins");
                }
            }
        }

        /// <summary>
        /// The delete meeting.
        /// </summary>
        /// <param name="credentials">
        /// The credentials.
        /// </param>
        /// <param name="provider">
        /// The provider.
        /// </param>
        /// <param name="param">
        /// The param.
        /// </param>
        /// <param name="scoId">
        /// The sco Id.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly",
            Justification = "Reviewed. Suppression is OK here.")]
        public object DeleteMeeting(
            CompanyLms credentials,
            AdobeConnectProvider provider,
            LtiParamDTO param,
            string scoId)
        {
            if (!credentials.CanRemoveMeeting.GetValueOrDefault())
            {
                return new { error = "Meeting deleting is disabled for this company lms." };
            }
            LmsCourseMeeting meeting = this.LmsCourseMeetingModel.GetOneByCourseAndScoId(credentials.Id, param.course_id, scoId).Value;
            if (meeting == null)
            {
                return new { error = "Meeting not found" };
            }

            if (meeting.LmsMeetingType == (int)LmsMeetingType.OfficeHours)
            {
                this.LmsCourseMeetingModel.RegisterDelete(meeting);
                return "true";
            }

            var result = provider.DeleteSco(meeting.GetMeetingScoId());
            if (result.Code == StatusCodes.ok)
            {
                return "true";
            }
            return new { error = result.InnerXml };
        }

        /// <summary>
        /// The get meeting.
        /// </summary>
        /// <param name="credentials">
        /// The credentials.
        /// </param>
        /// <param name="provider">
        /// The provider.
        /// </param>
        /// <param name="param">
        /// The parameter.
        /// </param>
        /// <param name="scoId">
        /// The sco Id.
        /// </param>
        /// <returns>
        /// The <see cref="MeetingDTO"/>.
        /// </returns>
        public MeetingDTO GetMeeting(CompanyLms credentials, AdobeConnectProvider provider, LtiParamDTO param, string scoId)
        {
            LmsCourseMeeting meeting = this.LmsCourseMeetingModel.GetOneByCourseAndScoId(credentials.Id, param.course_id, scoId).Value;

            if (meeting == null)
            {
                return this.CreateEmptyMeetingResponse(param);
            }

            ScoInfoResult result = provider.GetScoInfo(meeting.GetMeetingScoId());
            if (!result.Success || result.ScoInfo == null)
            {
                return this.CreateEmptyMeetingResponse(param);
            }

            IEnumerable<PermissionInfo> permission = provider.GetScoPublicAccessPermissions(meeting.GetMeetingScoId()).Values;

            MeetingDTO meetingDTO = this.GetMeetingDTOByScoInfo(
                credentials, 
                provider, 
                param, 
                result.ScoInfo, 
                permission,
                meeting);

            return meetingDTO;
        }

        /// <summary>
        /// The get meetings.
        /// </summary>
        /// <param name="credentials">
        /// The credentials.
        /// </param>
        /// <param name="provider">
        /// The provider.
        /// </param>
        /// <param name="param">
        /// The param.
        /// </param>
        /// <returns>
        /// The <see cref="MeetingDTO"/>.
        /// </returns>
        public object GetMeetings(CompanyLms credentials, AdobeConnectProvider provider, LtiParamDTO param)
        {
            var ret = new List<MeetingDTO>();
            
            var meetings = this.LmsCourseMeetingModel.GetAllByCourseId(credentials.Id, param.course_id);

            foreach (var meeting in meetings)
            {
                ScoInfoResult result = provider.GetScoInfo(meeting.GetMeetingScoId());
                if (!result.Success || result.ScoInfo == null)
                {
                    continue;
                }

                IEnumerable<PermissionInfo> permission = provider.GetScoPublicAccessPermissions(meeting.GetMeetingScoId()).Values;

                MeetingDTO meetingDTO = this.GetMeetingDTOByScoInfo(
                    credentials,
                    provider,
                    param,
                    result.ScoInfo,
                    permission,
                    meeting);
                ret.Add(meetingDTO);
            }
            if (!ret.Any(m => m.type == (int)LmsMeetingType.OfficeHours))
            {
                var meeting =
                    this.LmsCourseMeetingModel.GetOneByUserAndType(
                        credentials.Id,
                        param.lms_user_id,
                        (int)LmsMeetingType.OfficeHours).Value;
                if (meeting == null)
                {
                    var lmsUser = this.LmsUserModel.GetOneByUserIdAndCompanyLms(param.lms_user_id, credentials.Id).Value;
                    if (lmsUser != null)
                    {
                        var officeHours = this.OfficeHoursModel.GetByLmsUserId(lmsUser.Id).Value;
                        if (officeHours != null)
                        {
                            meeting = new LmsCourseMeeting()
                                          {
                                              OfficeHours = officeHours,
                                              LmsMeetingType = (int)LmsMeetingType.OfficeHours,
                                              CompanyLms = credentials,
                                              CourseId = param.course_id
                                          };
                        }
                    }
                }
                
                if (meeting != null)
                {
                    ScoInfoResult result = provider.GetScoInfo(meeting.GetMeetingScoId());
                    if (result.Success && result.ScoInfo != null)
                    {
                        IEnumerable<PermissionInfo> permission = provider.GetScoPublicAccessPermissions(meeting.GetMeetingScoId()).Values;

                        MeetingDTO meetingDTO = this.GetMeetingDTOByScoInfo(
                            credentials,
                            provider,
                            param,
                            result.ScoInfo,
                            permission,
                            meeting);
                        meetingDTO.is_disabled_for_this_course = true;
                        ret.Add(meetingDTO);
                    }
                }
            }
            
            return new
                       {
                           meetings = ret,
                           is_teacher = this.IsTeacher(param),
                           lms_provider_name = credentials.LmsProvider.LmsProviderName,
                           connect_server = credentials.AcServer.EndsWith("/") ? credentials.AcServer : credentials.AcServer + "/",
                           is_settings_visible = credentials.IsSettingsVisible.GetValueOrDefault(),
                           is_removable = credentials.CanRemoveMeeting.GetValueOrDefault(),
                           can_edit_meeting = credentials.CanEditMeeting.GetValueOrDefault(),
                           office_hours_enabled = credentials.EnableOfficeHours.GetValueOrDefault(),
                           study_groups_enabled = credentials.EnableStudyGroups.GetValueOrDefault()
                       };
        }

        /// <summary>
        /// The get provider.
        /// </summary>
        /// <param name="credentials">
        /// The credentials.
        /// </param>
        /// <param name="login">
        /// The login.
        /// </param>
        /// <returns>
        /// The <see cref="AdobeConnectProvider"/>.
        /// </returns>
        public AdobeConnectProvider GetProvider(CompanyLms credentials, bool login = true)
        {
            string apiUrl = credentials.AcServer + (credentials.AcServer.EndsWith("/") ? string.Empty : "/");

            apiUrl = apiUrl.EndsWith("api/xml/", StringComparison.OrdinalIgnoreCase)
                         ? apiUrl.TrimEnd('/')
                         : apiUrl + "api/xml";

            var connectionDetails = new ConnectionDetails
                                        {
                                            ServiceUrl = apiUrl, 
                                            EventMaxParticipants = 10, 
                                            Proxy =
                                                new ProxyCredentials
                                                    {
                                                        Domain = string.Empty, 
                                                        Login = string.Empty, 
                                                        Password = string.Empty, 
                                                        Url = string.Empty
                                                    }
                                        };
            var provider = new AdobeConnectProvider(connectionDetails);
            if (login)
            {
                provider.Login(new UserCredentials(credentials.AcUsername, credentials.AcPassword));
            }

            return provider;
        }

        /// <summary>
        /// The get recordings.
        /// </summary>
        /// <param name="credentials">
        /// The credentials.
        /// </param>
        /// <param name="provider">
        /// The provider.
        /// </param>
        /// <param name="courseId">
        /// The course id.
        /// </param>
        /// <returns>
        /// The <see cref="List{RecordingDTO}"/>.
        /// </returns>
        public List<RecordingDTO> GetRecordings(CompanyLms credentials, AdobeConnectProvider provider, int courseId, string scoId)
        {
            LmsCourseMeeting meeting = this.LmsCourseMeetingModel.GetOneByCourseAndScoId(credentials.Id, courseId, scoId).Value;

            if (meeting == null)
            {
                return new List<RecordingDTO>();
            }

            var result = provider.GetMeetingRecordings(meeting.GetMeetingScoId());
            var recordings = new List<RecordingDTO>();

            foreach (var v in result.Values)
            {
                var moreDetails = provider.GetScoPublicAccessPermissions(v.ScoId);
                bool isPublic = false;
                if (moreDetails.Success && moreDetails.Values.Any())
                {
                    isPublic = moreDetails.Values.First().PermissionId == PermissionId.view;
                }

                string passcode = provider.GetAclField(v.ScoId, AclFieldId.meeting_passcode).FieldValue;

                recordings.Add(new RecordingDTO
                        {
                            id = v.ScoId, 
                            name = v.Name, 
                            description = v.Description, 
                            begin_date = v.BeginDateLocal.ToString("MM/dd/yy h:mm:ss tt"), 
                            end_date = v.EndDateLocal.ToString("MM/dd/yy h:mm:ss tt"), 
                            duration = v.Duration, 
                            url = "/Lti/Recording/Join/" + v.UrlPath.Trim("/".ToCharArray()),
                            is_public = isPublic,
                            password = passcode
                        });
            }

            return recordings;
        }

        /// <summary>
        /// The update recording.
        /// </summary>
        /// <param name="credentials">
        /// The credentials.
        /// </param>
        /// <param name="provider">
        /// The provider.
        /// </param>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <param name="isPublic">
        /// The is public.
        /// </param>
        /// <param name="password">
        /// The password.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public string UpdateRecording(CompanyLms credentials, AdobeConnectProvider provider, string id, bool isPublic, string password)
        {
            var recording = provider.GetScoInfo(id).ScoInfo;

            if (recording == null)
            {
                return string.Empty;
            }

            // ReSharper disable UnusedVariable
            var accessResult = provider.UpdatePublicAccessPermissions(id, isPublic ? PermissionId.view : PermissionId.remove); 
            var passwordResult = provider.UpdateAclField(id, AclFieldId.meeting_passcode, password);
            // ReSharper restore UnusedVariable
            var recordingUrl = (credentials.AcServer.EndsWith("/")
                                        ? credentials.AcServer.Substring(0, credentials.AcServer.Length - 1)
                                        : credentials.AcServer) + recording.UrlPath;
            return recordingUrl;
        }

        /// <summary>
        /// The get templates.
        /// </summary>
        /// <param name="provider">
        /// The provider.
        /// </param>
        /// <param name="templateFolder">
        /// The template folder.
        /// </param>
        /// <returns>
        /// The <see cref="List{TemplateDTO}"/>.
        /// </returns>
        public List<TemplateDTO> GetTemplates(AdobeConnectProvider provider, string templateFolder)
        {
            ScoContentCollectionResult result = provider.GetContentsByScoId(templateFolder);
            if (result.Values == null)
            {
                return new List<TemplateDTO>();
            }

            return result.Values.Select(v => new TemplateDTO { id = v.ScoId, name = v.Name }).ToList();
        }

        /// <summary>
        /// The get users.
        /// </summary>
        /// <param name="credentials">
        /// The credentials.
        /// </param>
        /// <param name="provider">
        /// The provider.
        /// </param>
        /// <param name="param">
        /// The parameter.
        /// </param>
        /// <param name="scoId">
        /// The sco Id.
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
        public List<LmsUserDTO> GetUsers(CompanyLms credentials, AdobeConnectProvider provider, LtiParamDTO param, string scoId, out string error, bool forceUpdate = false)
        {
            LmsCourseMeeting meeting = this.LmsCourseMeetingModel.GetOneByCourseAndScoId(credentials.Id, param.course_id, scoId).Value;
            List<LmsUserDTO> users = this.GetLMSUsers(credentials, meeting, param.lms_user_id, param.course_id, out error, param, forceUpdate);

            if (meeting == null)
            {
                return users;
            }

            List<PermissionInfo> hosts, participants, presenters;
            var nonEditable = new HashSet<string>();
            this.GetMeetingAttendees(provider, meeting.GetMeetingScoId(), out hosts, out presenters, out participants, nonEditable);

            foreach (LmsUserDTO lmsuser in users)
            {
                LmsUserDTO user = lmsuser;
                string email = user.GetEmail(), login = user.GetLogin();

                var principal = this.GetACUser(provider, login, email);

                if (principal == null)
                {
                    continue;
                }

                user.ac_id = principal.PrincipalId;
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
        /// The get users.
        /// </summary>
        /// <param name="credentials">
        /// The credentials.
        /// </param>
        /// <param name="provider">
        /// The provider.
        /// </param>
        /// <param name="param">
        /// The parameter.
        /// </param>
        /// <param name="scoId">
        /// The sco Id.
        /// </param>
        /// <param name="startIndex">
        /// The start Index.
        /// </param>
        /// <param name="limit">
        /// The limit.
        /// </param>
        /// <returns>
        /// The <see cref="List{LmsUserDTO}"/>.
        /// </returns>
        [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1305:FieldNamesMustNotUseHungarianNotation",
            Justification = "Reviewed. Suppression is OK here.")]
        public List<ACSessionDTO> GetSessionsReport(CompanyLms credentials, AdobeConnectProvider provider, LtiParamDTO param, string scoId, int startIndex = 0, int limit = 0)
        {
            LmsCourseMeeting meeting = this.LmsCourseMeetingModel.GetOneByCourseAndScoId(credentials.Id, param.course_id, scoId).Value;

            if (meeting == null)
            {
                return new List<ACSessionDTO>();
            }

            return this.GetSessionsWithParticipants(meeting.GetMeetingScoId(), provider, startIndex, limit);
        }

        /// <summary>
        /// The get users.
        /// </summary>
        /// <param name="credentials">
        /// The credentials.
        /// </param>
        /// <param name="provider">
        /// The provider.
        /// </param>
        /// <param name="param">
        /// The parameter.
        /// </param>
        /// <param name="scoId">
        /// The sco Id.
        /// </param>
        /// <param name="startIndex">
        /// The start Index.
        /// </param>
        /// <param name="limit">
        /// The limit.
        /// </param>
        /// <returns>
        /// The <see cref="List{LmsUserDTO}"/>.
        /// </returns>
        [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1305:FieldNamesMustNotUseHungarianNotation",
            Justification = "Reviewed. Suppression is OK here.")]
        public List<ACSessionParticipantDTO> GetAttendanceReport(CompanyLms credentials, AdobeConnectProvider provider, LtiParamDTO param, string scoId, int startIndex = 0, int limit = 0)
        {
            LmsCourseMeeting meeting = this.LmsCourseMeetingModel.GetOneByCourseAndScoId(credentials.Id, param.course_id, scoId).Value;

            if (meeting == null)
            {
                return new List<ACSessionParticipantDTO>();
            }

            return this.GetAttendanceReport(meeting.GetMeetingScoId(), provider, startIndex, limit);
        }

        /// <summary>
        /// The join meeting.
        /// </summary>
        /// <param name="credentials">
        /// The credentials.
        /// </param>
        /// <param name="param">
        /// The parameter.
        /// </param>
        /// <param name="userSettings">
        /// The user settings.
        /// </param>
        /// <param name="scoId">
        /// The sco Id.
        /// </param>
        /// <param name="adobeConnectProvider">
        /// The adobe connect Provider.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public string JoinMeeting(CompanyLms credentials, LtiParamDTO param, LmsUserSettingsDTO userSettings, string scoId, AdobeConnectProvider adobeConnectProvider = null)
        {
            var connectionMode = (AcConnectionMode)userSettings.acConnectionMode;
            string breezeToken = string.Empty, meetingUrl = string.Empty;

            AdobeConnectProvider provider = adobeConnectProvider ?? this.GetProvider(credentials);

            this.LmsCourseMeetingModel.Flush();
            LmsCourseMeeting currentMeeting =
                this.LmsCourseMeetingModel.GetOneByCourseAndScoId(credentials.Id, param.course_id, scoId).Value;

            if (currentMeeting == null)
            {
                return "No meeting found";
            }

            string currentMeetingScoId = currentMeeting.GetMeetingScoId();

            if (!string.IsNullOrEmpty(currentMeetingScoId))
            {
                ScoContent currentMeetingSco = provider.GetScoContent(currentMeetingScoId).ScoContent;
                if (currentMeetingSco != null)
                {
                    meetingUrl = (credentials.AcServer.EndsWith("/")
                                        ? credentials.AcServer.Substring(0, credentials.AcServer.Length - 1)
                                        : credentials.AcServer) + currentMeetingSco.UrlPath;
                }
            }

            string email = param.lis_person_contact_email_primary, login = param.lms_user_login;

            var password = this.GetACPassword(credentials, userSettings, email, login);

            Principal registeredUser = this.GetACUser(provider, login, email);
            
            if (currentMeeting.LmsMeetingType == (int)LmsMeetingType.OfficeHours)
            {
                var userid = currentMeeting.OfficeHours != null && currentMeeting.OfficeHours.LmsUser != null
                                 ? currentMeeting.OfficeHours.LmsUser.UserId
                                 : string.Empty;
                this.EnrollToOfficeHours(
                    credentials,
                    param,
                    provider,
                    currentMeetingScoId,
                    ref registeredUser,
                    userSettings,
                    param.lms_user_id == userid);
            }

            if (registeredUser != null)
            {
                if (connectionMode != AcConnectionMode.DontOverwriteACPassword)
                {
                    breezeToken = this.LoginIntoAC(
                        credentials,
                        param,
                        registeredUser,
                        connectionMode,
                        email,
                        login,
                        password,
                        provider);
                }

                this.SaveLMSUserParameters(param, credentials, registeredUser.PrincipalId);
            }
            else
            {
                return JsonConvert.SerializeObject(new { error = string.Format("Cannot find Adobe Connect user with email {0} or login {1}", email, login) });
            }

            return string.Format("{0}?session={1}", meetingUrl, breezeToken ?? "null");
        }

        /// <summary>
        /// The leave meeting.
        /// </summary>
        /// <param name="credentials">
        /// The credentials.
        /// </param>
        /// <param name="param">
        /// The param.
        /// </param>
        /// <param name="userSettings">
        /// The user settings.
        /// </param>
        /// <param name="scoId">
        /// The sco id.
        /// </param>
        /// <param name="adobeConnectProvider">
        /// The adobe connect provider.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public object LeaveMeeting(CompanyLms credentials, LtiParamDTO param, string scoId, AdobeConnectProvider adobeConnectProvider = null)
        {
            AdobeConnectProvider provider = adobeConnectProvider ?? this.GetProvider(credentials);

            this.LmsCourseMeetingModel.Flush();
            LmsCourseMeeting currentMeeting =
                this.LmsCourseMeetingModel.GetOneByCourseAndScoId(credentials.Id, param.course_id, scoId).Value;

            if (currentMeeting == null)
            {
                return new { error = "No meeting found" };
            }

            if (currentMeeting.LmsMeetingType != (int)LmsMeetingType.StudyGroup)
            {
                return new { error = "The meeting is not a study group, you can only leave study groups." };
            }

            string currentMeetingScoId = currentMeeting.GetMeetingScoId();
            
            string email = param.lis_person_contact_email_primary, login = param.lms_user_login;

            Principal registeredUser = this.GetACUser(provider, login, email);

            if (registeredUser != null)
            {
                var result = provider.UpdateScoPermissionForPrincipal(
                    currentMeetingScoId,
                    registeredUser.PrincipalId,
                    MeetingPermissionId.denied);
                if (result.Code == StatusCodes.ok)
                {
                    return true;
                }
                return result;
            }
            else
            {
                return JsonConvert.SerializeObject(new { error = string.Format("Cannot find Adobe Connect user with email {0} or login {1}", email, login) });
            }
        }

        /// <summary>
        /// The join recording.
        /// </summary>
        /// <param name="credentials">
        /// The credentials.
        /// </param>
        /// <param name="param">
        /// The parameter.
        /// </param>
        /// <param name="userSettings">
        /// The user settings
        /// </param>
        /// <param name="recordingUrl">
        /// The recording url.
        /// </param>
        /// <param name="mode">
        /// The mode.
        /// </param>
        /// <param name="adobeConnectProvider">
        /// The adobe Connect Provider.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public string JoinRecording(CompanyLms credentials, LtiParamDTO param, LmsUserSettingsDTO userSettings, string recordingUrl, string mode = null, AdobeConnectProvider adobeConnectProvider = null)
        {
            var breezeToken = string.Empty;

            var connectionMode = (AcConnectionMode)userSettings.acConnectionMode;
            if (connectionMode == AcConnectionMode.Overwrite
                || connectionMode == AcConnectionMode.DontOverwriteLocalPassword)
            {
                AdobeConnectProvider provider = adobeConnectProvider ?? this.GetProvider(credentials);

                string email = param.lis_person_contact_email_primary, login = param.lms_user_login;

                var password = this.GetACPassword(credentials, userSettings, email, login);

                Principal registeredUser = this.GetACUser(provider, login, email);

                if (registeredUser != null)
                {
                    breezeToken = this.LoginIntoAC(credentials, param, registeredUser, connectionMode, email, login, password, provider);
                }
                else
                {
                    return param.launch_presentation_return_url;
                }
            }

            var baseUrl = credentials.AcServer
                          + (credentials.AcServer != null && credentials.AcServer.EndsWith(@"/") ? string.Empty : "/")
                          + recordingUrl;

            return string.Format(
                           "{0}?session={1}{2}",
                           baseUrl,
                           breezeToken ?? "null",
                           mode != null ? string.Format("&pbMode={0}", mode) : string.Empty);
        }

        /// <summary>
        /// The remove recording.
        /// </summary>
        /// <param name="credentials">
        /// The credentials.
        /// </param>
        /// <param name="provider">
        /// The provider.
        /// </param>
        /// <param name="courseId">
        /// The course id.
        /// </param>
        /// <param name="recordingId">
        /// The recording id.
        /// </param>
        /// <param name="scoId">
        /// The sco Id.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool RemoveRecording(
            CompanyLms credentials, 
            AdobeConnectProvider provider, 
            int courseId, 
            string recordingId,
            string scoId)
        {
            LmsCourseMeeting meeting = this.LmsCourseMeetingModel.GetOneByCourseAndScoId(credentials.Id, courseId, scoId).Value;

            if (meeting == null)
            {
                return false;
            }

            ScoContentCollectionResult result = provider.GetMeetingRecordings(new[] { meeting.GetMeetingScoId() });

            if (result.Values.All(v => v.ScoId != recordingId))
            {
                return false;
            }

            provider.DeleteSco(recordingId);
            return true;
        }

        /// <summary>
        /// The save meeting.
        /// </summary>
        /// <param name="credentials">
        /// The credentials.
        /// </param>
        /// <param name="provider">
        /// The provider.
        /// </param>
        /// <param name="param">
        /// The parameter.
        /// </param>
        /// <param name="meetingDTO">
        /// The meeting DTO.
        /// </param>
        /// <param name="extraData">
        /// The extra Data.
        /// </param>
        /// <returns>
        /// The <see cref="MeetingDTO"/>.
        /// </returns>
        public object SaveMeeting(
            CompanyLms credentials, 
            AdobeConnectProvider provider, 
            LtiParamDTO param, 
            MeetingDTO meetingDTO,
            object extraData = null)
        {
            // fix meeting dto date & time

            //// TODO change on PadLeft
            if (meetingDTO.start_time != null && meetingDTO.start_time.IndexOf(":", StringComparison.Ordinal) == 1)
            {
                meetingDTO.start_time = "0" + meetingDTO.start_time;
            }

            string oldStartDate = meetingDTO.start_date;

            if (meetingDTO.start_date != null)
            {
                meetingDTO.start_date = meetingDTO.start_date.Substring(6, 4) + "-"
                                        + meetingDTO.start_date.Substring(0, 5);
            }
            // end fix meeting dto

            var type = meetingDTO.type > 0 ? meetingDTO.type : (int)LmsMeetingType.Meeting;
            if (type == (int)LmsMeetingType.OfficeHours)
            {
                meetingDTO.name = param.lis_person_name_full + "'s Office Hours";
            }
            
            LmsCourseMeeting meeting = this.LmsCourseMeetingModel.GetOneByCourseAndScoId(credentials.Id, param.course_id, meetingDTO.id).Value;
            if (meeting == null && type == (int)LmsMeetingType.OfficeHours)
            {
                meeting =
                    this.LmsCourseMeetingModel.GetOneByCourseAndType(
                        credentials.Id,
                        param.course_id,
                        (int)LmsMeetingType.OfficeHours).Value;
            }
            if (meeting == null)
            {
                meeting = new LmsCourseMeeting
                       {
                           CompanyLms = credentials,
                           CourseId = param.course_id,
                           LmsMeetingType = type,
                           ScoId = type == (int)LmsMeetingType.OfficeHours ? meetingDTO.id : null
                       };                
            }

            bool isNewMeeting = false;
            var existingMeeting = provider.GetScoInfo(meeting.GetMeetingScoId());
            if (!existingMeeting.Success)
            {
                isNewMeeting = true;
            }

            var updateItem = new MeetingUpdateItem { ScoId = isNewMeeting ? null : meeting.GetMeetingScoId() };

            string email = param.lis_person_contact_email_primary, login = param.lms_user_login;

            var registeredUser = this.GetACUser(provider, login, email);
            if (type == (int)LmsMeetingType.StudyGroup)
            {
                this.AddUserToMeetingHostsGroup(provider, registeredUser.PrincipalId);
            }

            var meetingFolder = this.GetMeetingFolder(credentials, provider, registeredUser);

            this.SetMeetingUpateItemFields(
                meetingDTO,
                updateItem,
                meetingFolder,
                type == (int)LmsMeetingType.OfficeHours ? (param.course_id + " " + meeting.Id) : param.course_id.ToString(),
                isNewMeeting);

            ScoInfoResult result = isNewMeeting ? provider.CreateSco(updateItem) : provider.UpdateSco(updateItem);

            if (!result.Success || result.ScoInfo == null)
            {
                // didn't save, load old saved meeting
                return new { error = result.Status };
            }

            if (isNewMeeting)
            {
                // newly created meeting
                if (meeting.LmsMeetingType != (int)LmsMeetingType.OfficeHours)
                {
                    meeting.ScoId = result.ScoInfo.ScoId;
                }
                this.LmsCourseMeetingModel.RegisterSave(meeting);
                
                if (meeting.LmsMeetingType == (int)LmsMeetingType.Meeting)
                {
                    this.SetDefaultUsers(
                        credentials,
                        meeting,
                        provider,
                        param.lms_user_id,
                        meeting.CourseId,
                        result.ScoInfo.ScoId,
                        extraData ?? param);

                    this.CreateAnnouncement(
                        credentials,
                        param,
                        meetingDTO.name,
                        oldStartDate,
                        meetingDTO.start_time,
                        meetingDTO.duration);
                }
                else
                {
                    var acUser = this.GetACUser(provider, param.lms_user_login, param.lis_person_contact_email_primary);
                    if (acUser != null)
                    {
                        provider.UpdateScoPermissionForPrincipal(
                            result.ScoInfo.ScoId,
                            acUser.PrincipalId,
                            MeetingPermissionId.host);
                    }
                }
            }

            if (meeting.LmsMeetingType == (int)LmsMeetingType.OfficeHours)
            {
                var lmsUser = this.LmsUserModel.GetOneByUserIdAndCompanyLms(param.lms_user_id, credentials.Id).Value;
                if (lmsUser != null)
                {
                    var officeHous = this.OfficeHoursModel.GetByLmsUserId(lmsUser.Id).Value
                                     ?? new OfficeHours() { LmsUser = lmsUser };
                    officeHous.Hours = meetingDTO.office_hours;
                    officeHous.ScoId = meeting.ScoId = result.ScoInfo.ScoId;
                    
                    this.OfficeHoursModel.RegisterSave(officeHous);

                    meeting.OfficeHours = officeHous;
                    meeting.ScoId = null;
                    this.LmsCourseMeetingModel.RegisterSave(meeting);
                }
            }
            else if (meeting.LmsMeetingType == (int)LmsMeetingType.StudyGroup)
            {
                var lmsUser = this.LmsUserModel.GetOneByUserIdAndCompanyLms(param.lms_user_id, credentials.Id).Value;
                if (lmsUser != null)
                {
                    meeting.Owner = lmsUser;
                    this.LmsCourseMeetingModel.RegisterSave(meeting);
                }
            }

            SpecialPermissionId specialPermissionId = string.IsNullOrEmpty(meetingDTO.access_level)
                ? (meetingDTO.allow_guests ? SpecialPermissionId.remove : SpecialPermissionId.denied)
                : meetingDTO.access_level == "denied" ? SpecialPermissionId.denied
                                                      : (meetingDTO.access_level == "view_hidden"
                                                                 ? SpecialPermissionId.view_hidden
                                                                 : SpecialPermissionId.remove);

            provider.UpdatePublicAccessPermissions(result.ScoInfo.ScoId, specialPermissionId);
            List<PermissionInfo> permission =
                provider.GetScoPublicAccessPermissions(result.ScoInfo.ScoId)
                    .Values.Return(x => x.ToList(), new List<PermissionInfo>());

            MeetingDTO updatedMeeting = this.GetMeetingDTOByScoInfo(
                credentials,
                provider,
                param,
                result.ScoInfo,
                permission,
                meeting);

            return updatedMeeting;
        }

        /// <summary>
        /// The delete meeting.
        /// </summary>
        /// <param name="credentials">
        /// The credentials.
        /// </param>
        /// <param name="provider">
        /// The provider.
        /// </param>
        /// <param name="param">
        /// The parameter.
        /// </param>
        /// <param name="scoId">
        /// The sco Id.
        /// </param>
        /// <param name="error">
        /// The error.
        /// </param>
        /// <returns>
        /// The <see cref="MeetingDTO"/>.
        /// </returns>
        public List<string> DeleteMeeting(
            CompanyLms credentials,
            AdobeConnectProvider provider,
            LtiParamDTO param,
            string scoId,
            out string error)
        {
            error = null;
            var model = this.LmsCourseMeetingModel;
            LmsCourseMeeting meeting = model.GetOneByCourseAndScoId(credentials.Id, param.course_id, scoId).Value;

            if (meeting == null)
            {
                error = "Meeting not found";
                return new List<string>();
            }

            List<PermissionInfo> enrollments = this.GetMeetingAttendees(provider, meeting.GetMeetingScoId());
            provider.DeleteSco(meeting.GetMeetingScoId());
            model.RegisterDelete(meeting, true);
            
            return enrollments.Select(x => x.Login).ToList();
        }

        /// <summary>
        /// The setup folders.
        /// </summary>
        /// <param name="credentials">
        /// The credentials.
        /// </param>
        /// <param name="provider">
        /// The provider.
        /// </param>
        public void SetupFolders(CompanyLms credentials, AdobeConnectProvider provider)
        {
            this.SetupTemplateFolder(credentials, provider);
            
            this.СompanyLmsModel.RegisterSave(credentials);
            this.СompanyLmsModel.Flush();   
        }

        /// <summary>
        /// The get meeting folder.
        /// </summary>
        /// <param name="credentials">
        /// The credentials.
        /// </param>
        /// <param name="provider">
        /// The provider.
        /// </param>
        /// <param name="user">
        /// The user.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public string GetMeetingFolder(CompanyLms credentials, AdobeConnectProvider provider, Principal user)
        {
            string adobeConnectScoId = null;

            if (credentials.UseUserFolder.GetValueOrDefault() && user != null)
            {
                adobeConnectScoId = this.SetupUserMeetingsFolder(credentials, provider, user);
            }

            if (adobeConnectScoId == null)
            {
                this.SetupSharedMeetingsFolder(credentials, provider);
                this.СompanyLmsModel.RegisterSave(credentials);
                this.СompanyLmsModel.Flush();
                adobeConnectScoId = credentials.ACScoId;
            }

            return adobeConnectScoId;
        }

        /// <summary>
        /// The update user.
        /// </summary>
        /// <param name="credentials">
        /// The credentials.
        /// </param>
        /// <param name="provider">
        /// The provider.
        /// </param>
        /// <param name="param">
        /// The parameter.
        /// </param>
        /// <param name="scoId">
        /// The sco Id.
        /// </param>
        /// <param name="forceUpdate">
        /// The force update.
        /// </param>
        /// <returns>
        /// The <see cref="List{LmsUserDTO}"/>.
        /// </returns>
        public List<LmsUserDTO> SetDefaultRolesForNonParticipants(
            CompanyLms credentials,
            AdobeConnectProvider provider,
            LtiParamDTO param,
            string scoId,
            bool forceUpdate)
        {
            LmsCourseMeeting meeting = this.LmsCourseMeetingModel.GetOneByCourseAndScoId(credentials.Id, param.course_id, scoId).Value;
            string error;
            var users = this.GetUsers(credentials, provider, param, scoId, out error, forceUpdate);
            if (meeting == null)
            {
                return users;
            }

            List<PermissionInfo> enrollments = this.GetMeetingAttendees(provider, meeting.GetMeetingScoId());

            foreach (var user in users)
            {
                if (!this.IsUserSynched(enrollments, user))
                {
                    if (user.ac_id == null || user.ac_id == "0")
                    {
                        string email = user.GetEmail(), login = user.GetLogin();

                        this.UpdateUserACValues(provider, user, login, email);
                    }

                    if (user.is_editable)
                    {
                        this.SetLMSUserDefaultACPermissions(provider, meeting.GetMeetingScoId(), user, user.ac_id);
                    }
                }
            }

            users.RemoveAll(u => (string.IsNullOrWhiteSpace(u.id) || u.id.Equals("0")) 
                && (!string.IsNullOrWhiteSpace(u.ac_role) && u.ac_role.Equals("Remove")));

            return users;
        }

        /// <summary>
        /// The update user.
        /// </summary>
        /// <param name="credentials">
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
            CompanyLms credentials, 
            AdobeConnectProvider provider, 
            LtiParamDTO param, 
            LmsUserDTO user, 
            string scoId,
            out string error,
            bool skipReturningUsers = false)
        {
            error = null;
            LmsCourseMeeting meeting = this.LmsCourseMeetingModel.GetOneByCourseAndScoId(credentials.Id, param.course_id, scoId).Value;
            if (meeting == null)
            {
                return skipReturningUsers ? null : this.GetUsers(credentials, provider, param, scoId, out error);
            }

            if (user.ac_id == null || user.ac_id == "0")
            {
                string email = user.GetEmail(), login = user.GetLogin();

                this.UpdateUserACValues(provider, user, login, email);
            }

            if (user.ac_role == null)
            {
                provider.UpdateScoPermissionForPrincipal(meeting.GetMeetingScoId(), user.ac_id, MeetingPermissionId.remove);
                return skipReturningUsers ? null : this.GetUsers(credentials, provider, param, scoId, out error);
            }

            var permission = MeetingPermissionId.view;
            if (user.ac_role.ToLower() == "presenter")
            {
                permission = MeetingPermissionId.mini_host;
            }
            else if (user.ac_role.ToLower() == "host")
            {
                permission = MeetingPermissionId.host;
            }

            provider.UpdateScoPermissionForPrincipal(meeting.GetMeetingScoId(), user.ac_id, permission);
            if (permission == MeetingPermissionId.host)
            {
                provider.AddToGroupByType(user.ac_id, "live-admins");
            }

            return skipReturningUsers ? null : this.GetUsers(credentials, provider, param, scoId, out error);
        }

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
                var users = this.GetUsers(credentials, provider, param, null, out error);
                var user =
                    users.FirstOrDefault(
                        u => u.lti_id != null && u.lti_id.Equals(param.lms_user_id, StringComparison.InvariantCultureIgnoreCase));
                if (user != null)
                {
                    login = user.login_id;
                }
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// The enroll to office hours.
        /// </summary>
        /// <param name="credentials">
        /// The credentials.
        /// </param>
        /// <param name="param">
        /// The param.
        /// </param>
        /// <param name="provider">
        /// The provider.
        /// </param>
        /// <param name="meetingScoId">
        /// The meeting sco id.
        /// </param>
        /// <param name="registeredUser">
        /// The registered user.
        /// </param>
        /// <param name="userSettings">
        /// The user settings.
        /// </param>
        /// <param name="isOwner">
        /// The is owner.
        /// </param>
        private void EnrollToOfficeHours(CompanyLms credentials, LtiParamDTO param, AdobeConnectProvider provider, string meetingScoId, ref Principal registeredUser,
            LmsUserSettingsDTO userSettings, bool isOwner)
        {
            if (registeredUser == null)
            {
                var setup = new PrincipalSetup
                {
                    Email = param.lis_person_contact_email_primary,
                    FirstName = param.lis_person_name_given,
                    LastName = param.lis_person_name_family,
                    Name = param.lms_user_login,
                    Login = param.lms_user_login,
                    Password = this.GetACPassword(credentials, userSettings, param.lis_person_contact_email_primary, param.lms_user_login)
                };
                if (string.IsNullOrWhiteSpace(setup.Email))
                {
                    setup.Email = null;
                }
                PrincipalResult pu = provider.PrincipalUpdate(setup);
                if (pu.Principal != null)
                {
                    registeredUser = pu.Principal;
                }
            }

            if (registeredUser != null)
            {
                provider.UpdateScoPermissionForPrincipal(
                    meetingScoId,
                    registeredUser.PrincipalId,
                    isOwner ? MeetingPermissionId.host : MeetingPermissionId.view);
            }
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
        /// The update user ac values.
        /// </summary>
        /// <param name="provider">
        /// The provider.
        /// </param>
        /// <param name="user">
        /// The user.
        /// </param>
        /// <param name="login">
        /// The login.
        /// </param>
        /// <param name="email">
        /// The email.
        /// </param>
        private void UpdateUserACValues(AdobeConnectProvider provider, LmsUserDTO user, string login, string email)
        {
            var principal = this.GetACUser(provider, login, email);

            if (principal == null)
            {
                var setup = new PrincipalSetup
                {
                    Email = user.GetEmail(),
                    FirstName = user.GetFirstName(),
                    LastName = user.GetLastName(),
                    Name = user.name,
                    Login = user.GetLogin(),
                    Password = Membership.GeneratePassword(8, 2)
                };
                if (string.IsNullOrWhiteSpace(setup.Email))
                {
                    setup.Email = null;
                }
                PrincipalResult pu = provider.PrincipalUpdate(setup);
                if (pu.Principal != null)
                {
                    user.ac_id = pu.Principal.PrincipalId;
                }
            }
            else
            {
                user.ac_id = principal.PrincipalId;
            }
        }

        /// <summary>
        /// The save LMS user parameters.
        /// </summary>
        /// <param name="lmsCourseId">
        /// The LMS Course Id.
        /// </param>
        /// <param name="lmsCompany">
        /// The LMS Company.
        /// </param>
        /// <param name="lmsUserId">
        /// The LMS User Id.
        /// </param>
        /// <param name="adobeConnectUserId">
        /// The current user AC id.
        /// </param>
        /// <param name="courseName">
        /// The course Name.
        /// </param>
        /// <param name="userEmail">
        /// The user Email.
        /// </param>
        private void SaveLMSUserParameters(
            int lmsCourseId,
            CompanyLms lmsCompany,
            string lmsUserId,
            string adobeConnectUserId,
            string courseName,
            string userEmail)
        {
            LmsUserParameters lmsUserParameters = this.LmsUserParametersModel.GetOneByAcIdCourseIdAndCompanyLmsId(adobeConnectUserId, lmsCourseId, lmsCompany.Id).Value;

            if (lmsUserParameters == null)
            {
                lmsUserParameters = new LmsUserParameters
                {
                    AcId = adobeConnectUserId,
                    Course = lmsCourseId,
                    CompanyLms = lmsCompany
                };
            }

            lmsUserParameters.LastLoggedIn = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ssZ");
            lmsUserParameters.CourseName = courseName;
            lmsUserParameters.UserEmail = userEmail;
            lmsUserParameters.LmsUser = this.LmsUserModel.GetOneByUserIdAndCompanyLms(lmsUserId, lmsCompany.Id).Value;
            this.LmsUserParametersModel.RegisterSave(lmsUserParameters);
        }

        /// <summary>
        /// The get AC password.
        /// </summary>
        /// <param name="credentials">
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
        private string GetACPassword(CompanyLms credentials, LmsUserSettingsDTO userSettings, string email, string login)
        {
            var connectionMode = (AcConnectionMode)userSettings.acConnectionMode;
            switch (connectionMode)
            {
                case AcConnectionMode.Overwrite:
                    string password = credentials.AcUsername.Equals(email, StringComparison.OrdinalIgnoreCase)
                                        || credentials.AcUsername.Equals(login, StringComparison.OrdinalIgnoreCase)
                                          ? credentials.AcPassword
                                          : Membership.GeneratePassword(8, 2);
                    return password;
                case AcConnectionMode.DontOverwriteLocalPassword:
                    return userSettings.password;
                default:
                    return null;
            }
        }

        /// <summary>
        /// The create empty meeting response.
        /// </summary>
        /// <param name="param">
        /// The parameter.
        /// </param>
        /// <returns>
        /// The <see cref="MeetingDTO"/>.
        /// </returns>
        private MeetingDTO CreateEmptyMeetingResponse(LtiParamDTO param)
        {
            return new MeetingDTO
            {
                id = "0",
                is_editable = this.IsTeacher(param),
                are_users_synched = true
            };
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
        /// <returns>
        /// The <see cref="Principal"/>.
        /// </returns>
        private Principal GetACUser(AdobeConnectProvider provider, string login, string email)
        {
            var resultByLogin = provider.GetAllByLogin(login);
            if (!resultByLogin.Success)
            {
                return null;
            }

            var principal = string.IsNullOrWhiteSpace(login)
                                   ? null
                                   : resultByLogin.Return(x => x.Values, new List<Principal>()).FirstOrDefault();
            if (principal == null && !string.IsNullOrWhiteSpace(email))
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
        /// The is teacher.
        /// </summary>
        /// <param name="param">
        /// The param.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        private bool IsTeacher(LtiParamDTO param)
        {
            return param.roles != null && (param.roles.Contains("Instructor")
                || param.roles.Contains("Administrator")
                || param.roles.Contains("Course Director")
                || param.roles.Contains("CourseDirector"));
        }

        /// <summary>
        /// The can edit.
        /// </summary>
        /// <param name="param">
        /// The parameter.
        /// </param>
        /// <param name="meeting">
        /// The meeting.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        private bool CanEdit(LtiParamDTO param, LmsCourseMeeting meeting)
        {
            if (meeting == null || meeting.LmsMeetingType == null)
            {
                return false;
            }

            if (meeting.LmsMeetingType == (int)LmsMeetingType.OfficeHours)
            {
                return meeting.OfficeHours != null && meeting.OfficeHours.LmsUser != null
                        && meeting.OfficeHours.LmsUser.UserId != null
                       && meeting.OfficeHours.LmsUser.UserId.Equals(param.lms_user_id);
            }

            if (meeting.LmsMeetingType == (int)LmsMeetingType.StudyGroup)
            {
                return meeting.Owner != null && meeting.Owner.UserId != null && meeting.Owner.UserId.Equals(param.lms_user_id);
            }

            return this.IsTeacher(param);
        }

        /// <summary>
        /// The can join.
        /// </summary>
        /// <param name="provider">
        /// The provider.
        /// </param>
        /// <param name="credentials">
        /// The credentials.
        /// </param>
        /// <param name="meeting">
        /// The meeting.
        /// </param>
        /// <param name="param">
        /// The parameter.
        /// </param>
        /// <param name="meetingSco">
        /// The meeting SCO.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        private Tuple<bool, bool> GetMeetingFlags(
            AdobeConnectProvider provider,
            CompanyLms credentials,
            LmsCourseMeeting meeting,
            LtiParamDTO param,
            string meetingSco)
        {
            var lmsUserId = param.lms_user_id;
            var courseId = param.course_id;
            string email = param.lis_person_contact_email_primary, login = param.lms_user_login;

            bool canJoin = false;
            bool areUsersSynched = true;
            var registeredUser = this.GetACUser(provider, login, email);

            if (registeredUser != null)
            {
                var enrollments = this.GetMeetingAttendees(provider, meetingSco);

                if (enrollments.Any(h => h.PrincipalId == registeredUser.PrincipalId))
                {
                    canJoin = true;
                }

                areUsersSynched = this.AreUsersSynched(credentials, meeting, lmsUserId, courseId, enrollments, param);
            }

            return new Tuple<bool, bool>(canJoin, areUsersSynched);
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
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        private bool LmsUserIsAcUser(LmsUserDTO lmsUser, PermissionInfo participant)
        {
            string email = lmsUser.GetEmail(), login = lmsUser.GetLogin();
            return participant.Login != null && ((email != null && email.Equals(participant.Login, StringComparison.OrdinalIgnoreCase))
                   || (login != null && login.Equals(participant.Login, StringComparison.OrdinalIgnoreCase)));
        }

        /// <summary>
        /// The are users synched.
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
        /// <param name="enrollments">
        /// The enrollments.
        /// </param>
        /// <param name="param">
        /// The parameter.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        private bool AreUsersSynched(
            CompanyLms credentials,
            LmsCourseMeeting meeting,
            string lmsUserId,
            int courseId,
            List<PermissionInfo> enrollments,
            LtiParamDTO param)
        {
            string error;
            var lmsUsers = this.GetLMSUsers(credentials, meeting, lmsUserId, courseId, out error, param);
            foreach (var lmsUser in lmsUsers)
            {
                if (!this.IsUserSynched(enrollments, lmsUser))
                {
                    return false;
                }
            }

            foreach (var participant in enrollments)
            {
                if (!this.IsParticipantSynched(lmsUsers, participant))
                {
                    return false;
                }
            }

            return true;
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
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        private bool IsParticipantSynched(List<LmsUserDTO> lmsUsers, PermissionInfo participant)
        {
            bool found = false;
            foreach (var lmsUser in lmsUsers)
            {
                if (this.LmsUserIsAcUser(lmsUser, participant))
                {
                    found = true;
                }
            }

            return found;
        }

        /// <summary>
        /// The is user synched.
        /// </summary>
        /// <param name="enrollments">
        /// The enrollments.
        /// </param>
        /// <param name="lmsUser">
        /// The LMS user.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        private bool IsUserSynched(IEnumerable<PermissionInfo> enrollments, LmsUserDTO lmsUser)
        {
            bool isFound = false;
            foreach (var host in enrollments)
            {
                if (this.LmsUserIsAcUser(lmsUser, host))
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
        /// The create announcement.
        /// </summary>
        /// <param name="credentials">
        /// The credentials.
        /// </param>
        /// <param name="param">
        /// The parameter.
        /// </param>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <param name="startDate">
        /// The start date.
        /// </param>
        /// <param name="startTime">
        /// The start time.
        /// </param>
        /// <param name="duration">
        /// The duration.
        /// </param>
        private void CreateAnnouncement(
            CompanyLms credentials, 
            LtiParamDTO param, 
            string name, 
            string startDate, 
            string startTime, 
            string duration)
        {
            if (!credentials.ShowAnnouncements.GetValueOrDefault() || string.IsNullOrEmpty(param.context_title))
            {
                return;
            }

            var announcementTitle = string.Format(
                "A new Adobe Connect room was created for course {0}",
                param.context_title);
            const string AnnouncementMessagePattern = "Meeting \"{0}\" will start {1} at {2}. Its duration will be {3}. You can join it in your <a href='{4}'>Adobe Connect Conference section</a>.";
            var announcementMessage = string.Format(
                AnnouncementMessagePattern,
                name,
                startDate,
                startTime,
                duration,
                param.referer ?? string.Empty);

            switch (credentials.LmsProvider.ShortName.ToLowerInvariant())
            {
                case LmsProviderNames.Canvas:
                    var lmsUser = LmsUserModel.GetOneByUserIdAndCompanyLms(param.lms_user_id, credentials.Id).Value;
                    var token = lmsUser.Return(
                            u => u.Token,
                            credentials.AdminUser.Return(a => a.Token, string.Empty));
                    CanvasAPI.CreateAnnouncement(
                        credentials.LmsDomain,
                        token,
                        param.course_id,
                        announcementTitle,
                        announcementMessage);
                    break;
                case LmsProviderNames.BrainHoney:
                    // string error;
//                    this.dlapApi.CreateAnnouncement(
//                        credentials,
//                        param.course_id,
//                        announcementTitle,
//                        announcementMessage, 
//                        out error);
                    break;
            }
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
                    credentials.AdminUser.Return(a => a.Token, string.Empty));

            List<LmsUserDTO> users = EGCEnabledCanvasAPI.GetUsersForCourse(
                credentials.LmsDomain, 
                token, 
                canvasCourseId);
            return this.GroupUsers(users);
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
        private List<LmsUserDTO> GetLMSUsers(CompanyLms credentials, LmsCourseMeeting meeting, string lmsUserId, int courseId, out string error, object extraData = null, bool forceUpdate = false)
        {
            switch (credentials.LmsProvider.ShortName.ToLowerInvariant())
            {
                case LmsProviderNames.Canvas:
                    error = null;
                    return this.GetCanvasUsers(credentials, credentials.AdminUser != null ? credentials.AdminUser.UserId : lmsUserId, courseId);
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
            var hostsResult = provider.GetMeetingHosts(meetingSco);
            var presentersResult = provider.GetMeetingPresenters(meetingSco);
            var participantsResult = provider.GetMeetingParticipants(meetingSco);
            if (hostsResult.Values != null)
            {
                foreach (var g in hostsResult.Values)
                {
                    alreadyAdded.Add(g.PrincipalId);
                }
            }

            if (presentersResult.Values != null)
            {
                foreach (var g in presentersResult.Values)
                {
                    alreadyAdded.Add(g.PrincipalId);
                }
            }

            if (participantsResult.Values != null)
            {
                foreach (var g in participantsResult.Values)
                {
                    alreadyAdded.Add(g.PrincipalId);
                }
            }

            nonEditable = nonEditable ?? new HashSet<string>();

            hosts = this.ProcessACMeetingAttendees(nonEditable, provider, hostsResult, alreadyAdded);
            presenters = this.ProcessACMeetingAttendees(nonEditable, provider, presentersResult, alreadyAdded);
            participants = this.ProcessACMeetingAttendees(nonEditable, provider, participantsResult, alreadyAdded);
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
        private List<PermissionInfo> GetMeetingAttendees(
            AdobeConnectProvider provider,
            string meetingSco,
            HashSet<string> nonEditable = null)
        {
            var alreadyAdded = new HashSet<string>();
            var allMeetingEnrollments = provider.GetAllMeetingEnrollments(meetingSco);
            if (allMeetingEnrollments.Values != null)
            {
                foreach (var g in allMeetingEnrollments.Values)
                {
                    alreadyAdded.Add(g.PrincipalId);
                }
            }

            return this.ProcessACMeetingAttendees(nonEditable ?? new HashSet<string>(), provider, allMeetingEnrollments, alreadyAdded);
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
        /// <param name="result">
        /// The result.
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
            PermissionCollectionResult result,
            HashSet<string> alreadyAdded)
        {
            var values = result.Values.Return(x => x.ToList(), new List<PermissionInfo>());
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
        private bool AddUserToMeetingHostsGroup(AdobeConnectProvider provider, string principalId)
        {
            var group = provider.AddToGroupByType(principalId, "live-admins");
            
            return group;
        }

        /// <summary>
        /// The get attendance Report.
        /// </summary>
        /// <param name="meetingId">
        /// The meeting Id.
        /// </param>
        /// <param name="acp">
        /// The ACP.
        /// </param>
        /// <param name="startIndex">
        /// The start Index.
        /// </param>
        /// <param name="limit">
        /// The limit.
        /// </param>
        /// <returns>
        /// The <see cref="List{ACSessionParticipantDTO}"/>.
        /// </returns>
        private List<ACSessionParticipantDTO> GetAttendanceReport(string meetingId, AdobeConnectProvider acp, int startIndex = 0, int limit = 0)
        {
            try
            {
                {
                    var meetingAttendees = acp.ReportMettingAttendance(meetingId, startIndex, limit).Values.ToList();
                    return meetingAttendees.Select(
                            us =>
                            new ACSessionParticipantDTO
                            {
                                firstName = us.SessionName,
                                login = us.Login,
                                dateTimeEntered = us.DateCreated,
                                dateTimeLeft = us.DateEnd.FixACValue(),
                                durationInHours = (float)us.Duration.TotalHours,
                                transcriptId = int.Parse(us.TranscriptId)
                            }).OrderByDescending(x => x.dateTimeEntered).ToList();
                }
            }
            // ReSharper disable once EmptyGeneralCatchClause
            catch
            {
            }

            return new List<ACSessionParticipantDTO>();
        }

        /// <summary>
        /// Gets sessions with participants.
        /// </summary>
        /// <param name="meetingId">
        /// The meeting Id.
        /// </param>
        /// <param name="acp">
        /// The ACP.
        /// </param>
        /// <param name="startIndex">
        /// The start Index.
        /// </param>
        /// <param name="limit">
        /// The limit.
        /// </param>
        /// <returns>
        /// The <see cref="List{ACSessionDTO}"/>.
        /// </returns>
        private List<ACSessionDTO> GetSessionsWithParticipants(string meetingId, AdobeConnectProvider acp, int startIndex = 0, int limit = 0)
        {
            try
            {
                {
                    var meetingAttendees = acp.ReportMettingAttendance(meetingId).Values.ToList();
                    var userSessions = meetingAttendees.GroupBy(v => v.AssetId, v => v).ToDictionary(g => int.Parse(g.Key), g => g.ToList());

                    var sessions = acp.ReportMettingSessions(meetingId, startIndex, limit).Values.ToList();

                    var sessionList =
                        (from asset in userSessions.Keys.Except(sessions.ConvertAll(s => int.Parse(s.AssetId)))
                         let index =
                             sessions.Any(s => !string.IsNullOrEmpty(s.Version))
                                 ? sessions.Max(s => int.Parse(s.Version)) + 1
                                 : 0
                         select
                             new ACSessionDTO
                             {
                                 assetId = asset,
                                 sessionNumber = index,
                                 sessionName = index.ToString(CultureInfo.CurrentCulture)
                             }).ToList();
                    sessions.AddRange(sessionList.Select(s => new MeetingSession { AssetId = s.assetId.ToString(CultureInfo.CurrentCulture) }));

                    foreach (var sco in sessions)
                    {
                        var session = sessionList.FirstOrDefault(s => s.assetId == int.Parse(sco.AssetId));
                        if (null == session)
                        {
                            session = new ACSessionDTO
                            {
                                scoId = int.Parse(sco.ScoId),
                                assetId = int.Parse(sco.AssetId),
                                dateStarted = sco.DateCreated.FixACValue(),
                                dateEnded = sco.DateEnd.FixACValue(),
                                sessionNumber = int.Parse(sco.Version),
                                sessionName = sco.Version,
                                participants = new List<ACSessionParticipantDTO>()
                            };

                            sessionList.Add(session);
                        }

                        foreach (var us in userSessions[session.assetId])
                        {
                            var participant = new ACSessionParticipantDTO
                            {
                                firstName = us.SessionName,
                                login = us.Login,
                                dateTimeEntered = us.DateCreated,
                                dateTimeLeft = us.DateEnd.FixACValue(),
                                durationInHours =
                                    (float)us.Duration.TotalHours,
                                transcriptId = int.Parse(us.TranscriptId)
                            };

                            session.meetingName = us.ScoName;
                            session.participants.Add(participant);
                        }

                        if (!session.dateStarted.HasValue)
                        {
                            session.dateStarted = session.participants.Min(p => p.dateTimeEntered);
                        }
                    }

                    foreach (var session in sessionList)
                    {
                        var singleAttendance = session.participants.GroupBy(p => p.loginOrFullName)
                            .ToDictionary(g => g.Key, g => g.ToList());
                        foreach (var attendance in singleAttendance.Where(a => !string.IsNullOrWhiteSpace(a.Key) && a.Value.Count > 1))
                        {
                            attendance.Value.Skip(1).ToList().ForEach(p => session.participants.Remove(p));
                            var attendee = attendance.Value.First();
                            attendee.dateTimeEntered = attendance.Value.Min(p => p.dateTimeEntered);
                            attendee.dateTimeLeft = attendance.Value.Max(p => p.dateTimeLeft);
                            attendee.durationInHours = attendance.Value.Sum(p => p.durationInHours);
                        }
                    }

                   return sessionList.OrderBy(s => s.sessionNumber).ToList();
                }
            }
            // ReSharper disable once EmptyGeneralCatchClause
            catch 
            {
            }

            return new List<ACSessionDTO>();
        }

        /// <summary>
        /// The get meeting DTO by SCO info.
        /// </summary>
        /// <param name="credentials">
        /// The credentials.
        /// </param>
        /// <param name="provider">
        /// The provider.
        /// </param>
        /// <param name="param">
        /// The parameter.
        /// </param>
        /// <param name="result">
        /// The result.
        /// </param>
        /// <param name="permission">
        /// The permission.
        /// </param>
        /// <param name="type">
        /// The type.
        /// </param>
        /// <returns>
        /// The <see cref="MeetingDTO"/>.
        /// </returns>
        private MeetingDTO GetMeetingDTOByScoInfo(
            CompanyLms credentials, 
            AdobeConnectProvider provider, 
            LtiParamDTO param, 
            ScoInfo result, 
            IEnumerable<PermissionInfo> permission,
            LmsCourseMeeting lmsCourseMeeting)
        {
            int bracketIndex = result.Name.IndexOf("]", StringComparison.Ordinal);
            //var lmsCourseMeeting = this.LmsCourseMeetingModel.GetOneByCourseAndScoId(credentials.Id, param.course_id, result.ScoId).Value;
            var flags = this.GetMeetingFlags(provider, credentials, lmsCourseMeeting, param, result.ScoId);
            PermissionInfo permissionInfo = permission != null ? permission.FirstOrDefault() : null;
            string officeHoursString = null;
            var type = lmsCourseMeeting.LmsMeetingType.GetValueOrDefault();
            if (type == (int)LmsMeetingType.OfficeHours)
            {
                var lmsUser = this.LmsUserModel.GetOneByUserIdAndCompanyLms(param.lms_user_id, credentials.Id).Value;
                if (lmsUser != null)
                {
                    var officeHours = this.OfficeHoursModel.GetByLmsUserId(lmsUser.Id).Value;
                    if (officeHours != null)
                    {
                        officeHoursString = officeHours.Hours;
                    }
                }
            }

            var ret = new MeetingDTO
                          {
                              id = result.ScoId, 
                              ac_room_url = result.UrlPath.Trim("/".ToCharArray()), 
                              name = result.Name.Substring(bracketIndex < 0 || (bracketIndex + 2 > result.Name.Length) ? 0 : bracketIndex + 2), 
                              summary = result.Description, 
                              template = result.SourceScoId, 
                              start_date = result.BeginDate.ToString("yyyy-MM-dd"), 
                              start_time = result.BeginDate.ToString("h:mm tt", CultureInfo.InvariantCulture), 
                              duration = (result.EndDate - result.BeginDate).ToString(@"h\:mm"),
                              access_level = permissionInfo != null ? permissionInfo.PermissionId.ToString() : "remove",
                              allow_guests = permissionInfo == null || permissionInfo.PermissionId == PermissionId.remove,
                              can_join = (type == (int)LmsMeetingType.OfficeHours) || flags.Item1, 
                              are_users_synched = flags.Item2 || type != (int)LmsMeetingType.Meeting,
                              is_editable = this.CanEdit(param, lmsCourseMeeting),
                              type = type,
                              office_hours = officeHoursString
                          };
            return ret;
        }

        /// <summary>
        /// The login into AC.
        /// </summary>
        /// <param name="credentials">
        /// The credentials.
        /// </param>
        /// <param name="param">
        /// The parameter.
        /// </param>
        /// <param name="registeredUser">
        /// The registered user.
        /// </param>
        /// <param name="connectionMode">
        /// The connection Mode.
        /// </param>
        /// <param name="email">
        /// The email.
        /// </param>
        /// <param name="login">
        /// The login.
        /// </param>
        /// <param name="password">
        /// The password.
        /// </param>
        /// <param name="provider">
        /// The provider.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        private string LoginIntoAC(
            CompanyLms credentials, 
            LtiParamDTO param, 
            Principal registeredUser, 
            AcConnectionMode connectionMode,
            string email, 
            string login, 
            string password, 
            AdobeConnectProvider provider)
        {
            string breezeToken;
            if (connectionMode == AcConnectionMode.Overwrite 
                && !credentials.AcUsername.Equals(email, StringComparison.OrdinalIgnoreCase)
                && !credentials.AcUsername.Equals(login, StringComparison.OrdinalIgnoreCase))
            {
                var resetPasswordResult = provider.PrincipalUpdatePassword(registeredUser.PrincipalId, password);
            }

            var principalUpdateResult = provider.PrincipalUpdate(
                new PrincipalSetup
                    {
                        PrincipalId = registeredUser.PrincipalId, 
                        FirstName = param.lis_person_name_given, 
                        LastName = param.lis_person_name_family, 
                        Name = registeredUser.Name, 
                        Login = registeredUser.Login, 
                        Email = registeredUser.Email, 
                        HasChildren = registeredUser.HasChildren
                    });

            var userProvider = this.GetProvider(credentials, false); // separate provider for user not to lose admin logging in

            LoginResult resultByLogin = userProvider.Login(new UserCredentials(login, password));
            if (resultByLogin.Success)
            {
                breezeToken = resultByLogin.Status.SessionInfo;
            }
            else
            {
                LoginResult resultByEmail = userProvider.Login(new UserCredentials(email, password));
                breezeToken = resultByEmail.Status.SessionInfo;
            }
            
            return breezeToken;
        }

        /// <summary>
        /// The save LMS user parameters.
        /// </summary>
        /// <param name="param">
        /// The parameter.
        /// </param>
        /// <param name="lmsCompany">
        /// The LMS Company.
        /// </param>
        /// <param name="adobeConnectUserId">
        /// The current AC user SCO id.
        /// </param>
        private void SaveLMSUserParameters(LtiParamDTO param, CompanyLms lmsCompany, string adobeConnectUserId)
        {
            this.SaveLMSUserParameters(param.course_id, lmsCompany, param.lms_user_id, adobeConnectUserId, param.context_title, param.lis_person_contact_email_primary);
        }

        /// <summary>
        /// The set default users.
        /// </summary>
        /// <param name="credentials">
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
        private void SetDefaultUsers(
            CompanyLms credentials, 
            LmsCourseMeeting meeting,
            AdobeConnectProvider provider, 
            string lmsUserId,
            int courseId, 
            string meetingScoId,
            object extraData = null)
        {
            string error;
            List<LmsUserDTO> users = this.GetLMSUsers(credentials, meeting, lmsUserId, courseId, out error, extraData);

            foreach (LmsUserDTO u in users)
            {
                string email = u.GetEmail(), login = u.GetLogin();
                var principal = this.GetACUser(provider, login, email);
                if (principal == null)
                {
                    PrincipalResult res =
                        provider.PrincipalUpdate(
                            new PrincipalSetup
                                {
                                    Email = u.GetEmail(), 
                                    FirstName = u.GetFirstName(), 
                                    LastName = u.GetLastName(), 
                                    Password = Membership.GeneratePassword(8, 2), 
                                    Login = u.GetLogin(), 
                                    Type = PrincipalTypes.user
                                });
                    if (res.Success && res.Principal != null)
                    {
                        principal = res.Principal;
                    }
                }

                if (principal != null)
                {
                    this.SetLMSUserDefaultACPermissions(provider, meetingScoId, u, principal.PrincipalId);
                }
            }
        }

        /// <summary>
        /// The set meeting update item fields.
        /// </summary>
        /// <param name="meetingDTO">
        /// The meeting DTO.
        /// </param>
        /// <param name="updateItem">
        /// The update item.
        /// </param>
        /// <param name="folderSco">
        /// The folder SCO.
        /// </param>
        /// <param name="courseId">
        /// The course id.
        /// </param>
        /// <param name="isNew">
        /// The is New.
        /// </param>
        private void SetMeetingUpateItemFields(
            MeetingDTO meetingDTO, 
            MeetingUpdateItem updateItem, 
            string folderSco, 
            string courseId,
            bool isNew)
        {
            updateItem.Name = string.Format(
                "[{0}] {1}", 
                courseId,
                meetingDTO.name);
            if (updateItem.Name.Length > 60)
            {
                updateItem.Name = updateItem.Name.Substring(0, 60);
            }

            updateItem.Description = meetingDTO.summary;
            updateItem.FolderId = folderSco;
            updateItem.Language = "en";
            updateItem.Type = ScoType.meeting;
            
            if (isNew)
            {
                updateItem.SourceScoId = meetingDTO.template;
                updateItem.UrlPath = meetingDTO.ac_room_url;
            }

            if (meetingDTO.start_date == null || meetingDTO.start_time == null)
            {
                updateItem.DateBegin = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ssZ");
                updateItem.DateEnd = DateTime.Now.AddDays(1).ToString("yyyy-MM-ddTHH:mm:ssZ");
            }

            DateTime dateBegin;

            if (DateTime.TryParse(meetingDTO.start_date + " " + meetingDTO.start_time, out dateBegin))
            {
                updateItem.DateBegin = dateBegin.ToString("yyyy-MM-ddTHH:mm:ssZ");
                TimeSpan duration;
                if (TimeSpan.TryParse(meetingDTO.duration, out duration))
                {
                    updateItem.DateEnd =
                        dateBegin.AddMinutes((int)duration.TotalMinutes).ToString("yyyy-MM-ddTHH:mm:ssZ");
                }
            }
        }

        /// <summary>
        /// The setup template folder.
        /// </summary>
        /// <param name="credentials">
        /// The credentials.
        /// </param>
        /// <param name="provider">
        /// The provider.
        /// </param>
        private void SetupTemplateFolder(CompanyLms credentials, AdobeConnectProvider provider)
        {
            string templatesSco = null;
            if (!string.IsNullOrWhiteSpace(credentials.ACTemplateScoId))
            {
                ScoInfoResult templatesFolder = provider.GetScoInfo(credentials.ACTemplateScoId);
                if (templatesFolder.Success && templatesFolder.ScoInfo != null)
                {
                    templatesSco = templatesFolder.ScoInfo.ScoId;
                }
            }

            if (templatesSco == null)
            {
                ScoContentCollectionResult sharedTemplates = provider.GetContentsByType("shared-meeting-templates");
                if (sharedTemplates.ScoId != null)
                {
                    credentials.ACTemplateScoId = sharedTemplates.ScoId;
                }
            }
        }

        /// <summary>
        /// The setup shared meetings folder.
        /// </summary>
        /// <param name="credentials">
        /// The credentials.
        /// </param>
        /// <param name="provider">
        /// The provider.
        /// </param>
        private void SetupSharedMeetingsFolder(CompanyLms credentials, AdobeConnectProvider provider)
        {
            string ltiFolderSco = null;
            string name = credentials.UserFolderName ?? credentials.LmsProvider.LmsProviderName;

            if (!string.IsNullOrWhiteSpace(credentials.ACScoId))
            {
                ScoInfoResult canvasFolder = provider.GetScoInfo(credentials.ACScoId);
                if (canvasFolder.Success && canvasFolder.ScoInfo != null && canvasFolder.ScoInfo.Name.Equals(name))
                {
                    ltiFolderSco = canvasFolder.ScoInfo.ScoId;
                }
            }

            if (ltiFolderSco == null)
            {
                ScoContentCollectionResult sharedMeetings = provider.GetContentsByType("meetings");
                if (sharedMeetings.ScoId != null && sharedMeetings.Values != null)
                {
                    ScoContent existingFolder =
                        sharedMeetings.Values.FirstOrDefault(v => v.Name.Equals(name) && v.IsFolder);
                    if (existingFolder != null)
                    {
                        credentials.ACScoId = existingFolder.ScoId;
                    }
                    else
                    {
                        ScoInfoResult newFolder =
                            provider.CreateSco(
                                new FolderUpdateItem
                                {
                                    Name = name,
                                    FolderId = sharedMeetings.ScoId,
                                    Type = ScoType.folder
                                });
                        if (newFolder.Success && newFolder.ScoInfo != null)
                        {
                            credentials.ACScoId = newFolder.ScoInfo.ScoId;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// The setup user meetings folder.
        /// </summary>
        /// <param name="credentials">
        /// The credentials.
        /// </param>
        /// <param name="provider">
        /// The provider.
        /// </param>
        /// <param name="user">
        /// The user.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        private string SetupUserMeetingsFolder(CompanyLms credentials, AdobeConnectProvider provider, Principal user)
        {
            string ltiFolderSco = null;

            StatusInfo status;
            var shortcut = provider.GetShortcutByType("user-meetings", out status);
            List<ScoContent> userFolders = new List<ScoContent>();

            if (shortcut != null)
            {
                var userMeetings = provider.GetScoExpandedContentByName(shortcut.ScoId, user.Login);
                if (userMeetings != null && userMeetings.Values != null)
                {
                    userFolders.AddRange(userMeetings.Values);
                }

                userMeetings = provider.GetScoExpandedContentByName(shortcut.ScoId, user.Email);
                if (userMeetings != null && userMeetings.Values != null)
                {
                    userFolders.AddRange(userMeetings.Values);
                }                
            }
            
            if (userFolders.Count > 0)
            {
                ScoContent userFolder = userFolders.FirstOrDefault(uf => uf.Type.Equals("folder"));

                if (userFolder == null)
                {
                    return null;
                }

                string name = credentials.UserFolderName ?? credentials.LmsProvider.LmsProviderName;

                var existingFolder = provider.GetContentsByScoId(userFolder.ScoId).Values.FirstOrDefault(v => v.Name.Equals(name) && v.IsFolder);

                if (existingFolder != null)
                {
                    ltiFolderSco = existingFolder.ScoId;
                }
                else
                {
                    ScoInfoResult newFolder =
                        provider.CreateSco(
                            new FolderUpdateItem
                            {
                                Name = name,
                                FolderId = userFolder.ScoId,
                                Type = ScoType.folder
                            });
                    if (newFolder.Success && newFolder.ScoInfo != null)
                    {
                        ltiFolderSco = newFolder.ScoInfo.ScoId;
                    }
                }
            }

            return ltiFolderSco;
        }

        #endregion
    }
}