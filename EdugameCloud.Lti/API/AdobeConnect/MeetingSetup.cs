using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using EdugameCloud.Lti.API.BlackBoard;
using EdugameCloud.Lti.API.Canvas;
using EdugameCloud.Lti.Business.Models;
using EdugameCloud.Lti.Domain.Entities;
using EdugameCloud.Lti.DTO;
using EdugameCloud.Lti.Extensions;
using Esynctraining.AC.Provider;
using Esynctraining.AC.Provider.DataObjects;
using Esynctraining.AC.Provider.DataObjects.Results;
using Esynctraining.AC.Provider.Entities;
using Esynctraining.Core.Extensions;
using Esynctraining.Core.Utils;
using Newtonsoft.Json;

namespace EdugameCloud.Lti.API.AdobeConnect
{
    /// <summary>
    ///     The meeting setup.
    /// </summary>
    public sealed class MeetingSetup
    {
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
        private LmsCompanyModel LmsСompanyModel
        {
            get
            {
                return IoC.Resolve<LmsCompanyModel>();
            }
        }

        /// <summary>
        /// Gets the users setup.
        /// </summary>
        private UsersSetup UsersSetup
        {
            get
            {
                return IoC.Resolve<UsersSetup>();
            }
        }

        private SoapAPI BlackboardApi
        {
            get
            {
                return IoC.Resolve<SoapAPI>();
            }
        }

        #endregion

        #region Public Methods and Operators

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
            LmsCompany credentials,
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

            if (meeting.LmsMeetingType == (int)LmsMeetingType.OfficeHours && meeting.OfficeHours != null)
            {
                var coursesWithThisOfficeHours = this.LmsCourseMeetingModel.GetAllByOfficeHoursId(
                    meeting.OfficeHours.Id);
                if (coursesWithThisOfficeHours.Any(c => c.Id != meeting.Id))
                {
                    this.LmsCourseMeetingModel.RegisterDelete(meeting);
                    return "true";
                }
            }

            this.LmsCourseMeetingModel.RegisterDelete(meeting);
            if (meeting.OfficeHours != null)
            {
                this.OfficeHoursModel.RegisterDelete(meeting.OfficeHours);                
            }

            var result = provider.DeleteSco(meeting.GetMeetingScoId());
            if (result.Code == StatusCodes.ok)
            {
                return "true";
            }

            return new { error = result.InnerXml };
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
        /// The parameter.
        /// </param>
        /// <returns>
        /// The <see cref="MeetingDTO"/>.
        /// </returns>
        public object GetMeetings(LmsCompany credentials, AdobeConnectProvider provider, LtiParamDTO param)
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

            //if (!ret.Any(m => m.type == (int)LmsMeetingType.Meeting))
            //{
               // var empty = this.CreateEmptyMeetingResponse(param);
               // ret.Add(empty);
            //}

            if (!ret.Any(m => m.type == (int)LmsMeetingType.OfficeHours))
            {
                var officeHoursMeetings =
                    this.LmsCourseMeetingModel.GetOneByUserAndType(credentials.Id, param.lms_user_id, (int)LmsMeetingType.OfficeHours).Value;

                if (officeHoursMeetings == null)
                {
                    var lmsUser = this.LmsUserModel.GetOneByUserIdAndCompanyLms(param.lms_user_id, credentials.Id).Value;
                    if (lmsUser != null)
                    {
                        var officeHours = this.OfficeHoursModel.GetByLmsUserId(lmsUser.Id).Value;
                        if (officeHours != null)
                        {
                            officeHoursMeetings = new LmsCourseMeeting
                            {
                                OfficeHours = officeHours,
                                LmsMeetingType = (int)LmsMeetingType.OfficeHours,
                                LmsCompany = credentials,
                                CourseId = param.course_id,
                            };
                        }
                    }
                }

                if (officeHoursMeetings != null)
                {
                    ScoInfoResult result = provider.GetScoInfo(officeHoursMeetings.GetMeetingScoId());
                    if (result.Success && result.ScoInfo != null)
                    {
                        IEnumerable<PermissionInfo> permission = provider.GetScoPublicAccessPermissions(officeHoursMeetings.GetMeetingScoId()).Values;

                        MeetingDTO meetingDTO = this.GetMeetingDTOByScoInfo(
                            credentials,
                            provider,
                            param,
                            result.ScoInfo,
                            permission,
                            officeHoursMeetings);
                        meetingDTO.is_disabled_for_this_course = true;
                        ret.Add(meetingDTO);
                    }
                }
            }
            
            return new
                       {
                           meetings = ret,
                           is_teacher = this.UsersSetup.IsTeacher(param),
                           lms_provider_name = credentials.LmsProvider.LmsProviderName,
                           connect_server = credentials.AcServer.EndsWith("/") ? credentials.AcServer : credentials.AcServer + "/",
                           is_settings_visible = credentials.IsSettingsVisible.GetValueOrDefault(),
                           is_removable = credentials.CanRemoveMeeting.GetValueOrDefault(),
                           can_edit_meeting = credentials.CanEditMeeting.GetValueOrDefault(),
                           office_hours_enabled = credentials.EnableOfficeHours.GetValueOrDefault(),
                           study_groups_enabled = credentials.EnableStudyGroups.GetValueOrDefault(),
                           course_meetings_enabled = credentials.EnableCourseMeetings.GetValueOrDefault() || param.is_course_meeting_enabled,
                           is_lms_help_visible = credentials.ShowLmsHelp.GetValueOrDefault(),
                           is_egc_help_visible = credentials.ShowEGCHelp.GetValueOrDefault(),
                           user_guide_link = !string.IsNullOrEmpty(credentials.LmsProvider.UserGuideFileUrl) ? credentials.LmsProvider.UserGuideFileUrl : 
                                string.Format("/content/lti-instructions/{0}.pdf", credentials.LmsProvider.ShortName)
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
        public AdobeConnectProvider GetProvider(LmsCompany credentials, bool login = true)
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
        /// <param name="scoId">
        /// The SCO Id.
        /// </param>
        /// <returns>
        /// The <see cref="List{RecordingDTO}"/>.
        /// </returns>
        public List<RecordingDTO> GetRecordings(LmsCompany credentials, AdobeConnectProvider provider, int courseId, string scoId)
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
        public string UpdateRecording(LmsCompany credentials, AdobeConnectProvider provider, string id, bool isPublic, string password)
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
        /// The SCO Id.
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
        public List<ACSessionDTO> GetSessionsReport(LmsCompany credentials, AdobeConnectProvider provider, LtiParamDTO param, string scoId, int startIndex = 0, int limit = 0)
        {
            LmsCourseMeeting meeting = this.LmsCourseMeetingModel.GetOneByCourseAndScoId(credentials.Id, param.course_id, scoId).Value;

            if (meeting == null)
            {
                return new List<ACSessionDTO>();
            }

            return GetSessionsWithParticipants(meeting.GetMeetingScoId(), provider, startIndex, limit);
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
        /// The SCO Id.
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
        public List<ACSessionParticipantDTO> GetAttendanceReport(LmsCompany credentials, AdobeConnectProvider provider, LtiParamDTO param, string scoId, int startIndex = 0, int limit = 0)
        {
            LmsCourseMeeting meeting = this.LmsCourseMeetingModel.GetOneByCourseAndScoId(credentials.Id, param.course_id, scoId).Value;

            if (meeting == null)
            {
                return new List<ACSessionParticipantDTO>();
            }

            return GetAttendanceReport(meeting.GetMeetingScoId(), provider, startIndex, limit);
        }

        /// <summary>
        /// The join meeting.
        /// </summary>
        /// <param name="lmsCompany">
        /// The credentials.
        /// </param>
        /// <param name="param">
        /// The parameter.
        /// </param>
        /// <param name="userSettings">
        /// The user settings.
        /// </param>
        /// <param name="scoId">
        /// The SCO Id.
        /// </param>
        /// <param name="breezeSession">
        /// The breeze Session.
        /// </param>
        /// <param name="adobeConnectProvider">
        /// The adobe connect Provider.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public object JoinMeeting(LmsCompany lmsCompany, LtiParamDTO param, LmsUserSettingsDTO userSettings, string scoId, ref string breezeSession, AdobeConnectProvider adobeConnectProvider = null)
        {
            this.LmsCourseMeetingModel.Flush();
            LmsCourseMeeting currentMeeting =
                this.LmsCourseMeetingModel.GetOneByCourseAndScoId(lmsCompany.Id, param.course_id, scoId).Value;

            if (currentMeeting == null)
            {
                return new { error = string.Format("No meeting for course {0} and sco id {1} found", param.course_id, scoId) };
            }

            string currentMeetingScoId = currentMeeting.GetMeetingScoId();
            AdobeConnectProvider provider = adobeConnectProvider ?? this.GetProvider(lmsCompany);
            var meetingUrl = string.Empty;

            if (!string.IsNullOrEmpty(currentMeetingScoId))
            {
                ScoContent currentMeetingSco = provider.GetScoContent(currentMeetingScoId).ScoContent;
                if (currentMeetingSco != null)
                {
                    meetingUrl = (lmsCompany.AcServer.EndsWith("/")
                                        ? lmsCompany.AcServer.Substring(0, lmsCompany.AcServer.Length - 1)
                                        : lmsCompany.AcServer) + currentMeetingSco.UrlPath;
                }
            }

            string email = param.lis_person_contact_email_primary, login = param.lms_user_login;
            var lmsUser = this.LmsUserModel.GetOneByUserIdAndCompanyLms(param.lms_user_id, lmsCompany.Id).Value;
            if (lmsUser == null)
            {
                return new { error = string.Format("No user with id {0} found in the database.", param.lms_user_id) };
            }

            var password = this.UsersSetup.GetACPassword(lmsCompany, userSettings, email, login);
            var principalInfo = lmsUser.PrincipalId != null ? provider.GetOneByPrincipalId(lmsUser.PrincipalId).PrincipalInfo : null;
            var registeredUser = principalInfo != null ? principalInfo.Principal : null;
            
            if (currentMeeting.LmsMeetingType == (int)LmsMeetingType.OfficeHours)
            {
                var userid = currentMeeting.OfficeHours != null && currentMeeting.OfficeHours.LmsUser != null
                                 ? currentMeeting.OfficeHours.LmsUser.UserId
                                 : string.Empty;
                var isOwner = userid.Equals(param.lms_user_id);

                provider.UpdateScoPermissionForPrincipal(
                    currentMeetingScoId,
                    registeredUser.With(x => x.PrincipalId),
                    isOwner ? MeetingPermissionId.host : MeetingPermissionId.view);
                if (isOwner)
                {
                    this.UsersSetup.AddUserToMeetingHostsGroup(provider, registeredUser.With(x => x.PrincipalId));
                }
            }

            var connectionMode = (AcConnectionMode)userSettings.acConnectionMode;
            var breezeToken = string.Empty;

            if (registeredUser != null)
            {
                if (connectionMode != AcConnectionMode.DontOverwriteACPassword)
                {
                    breezeToken = this.LoginIntoAC(
                        lmsCompany,
                        param,
                        registeredUser,
                        connectionMode,
                        email,
                        login,
                        password,
                        provider);
                }

                string wstoken = null;

                if (lmsCompany.LmsProvider.Id == (int)LmsProviderEnum.Blackboard)
                {
                    string error;
                    var users = this.UsersSetup.GetLMSUsers(
                        lmsCompany,
                        currentMeeting,
                        param.lms_user_id,
                        param.course_id,
                        out error,
                        param);
                    var currentUser = users.FirstOrDefault(u => u.lti_id == param.lms_user_id);
                    if (currentUser != null)
                    {
                        wstoken = currentUser.id;
                    }
                }

                this.SaveLMSUserParameters(param, lmsCompany, registeredUser.PrincipalId, wstoken);
            }
            else
            {
                return new
                            {
                                error =
                                    string.Format(
                                        "Cannot find Adobe Connect user with principal id {0} or email {1} or login {2}",
                                        lmsUser.PrincipalId ?? string.Empty,
                                        email,
                                        login)
                            };
            }

            breezeSession = breezeToken ?? string.Empty;
            return lmsCompany.LoginUsingCookie.GetValueOrDefault() ? meetingUrl : string.Format("{0}?session={1}", meetingUrl, breezeToken ?? "null");
        }

        /// <summary>
        /// The leave meeting.
        /// </summary>
        /// <param name="lmsCompany">
        /// The credentials.
        /// </param>
        /// <param name="param">
        /// The parameter.
        /// </param>
        /// <param name="scoId">
        /// The SCO id.
        /// </param>
        /// <param name="adobeConnectProvider">
        /// The adobe connect provider.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public object LeaveMeeting(LmsCompany lmsCompany, LtiParamDTO param, string scoId, AdobeConnectProvider adobeConnectProvider = null)
        {
            AdobeConnectProvider provider = adobeConnectProvider ?? this.GetProvider(lmsCompany);

            this.LmsCourseMeetingModel.Flush();
            LmsCourseMeeting currentMeeting = this.LmsCourseMeetingModel.GetOneByCourseAndScoId(lmsCompany.Id, param.course_id, scoId).Value;

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

            Principal registeredUser = this.UsersSetup.GetPrincipalByLoginOrEmail(provider, login, email, lmsCompany.ACUsesEmailAsLogin.GetValueOrDefault());

            if (registeredUser != null)
            {
                var result = provider.UpdateScoPermissionForPrincipal(
                    currentMeetingScoId,
                    registeredUser.PrincipalId,
                    MeetingPermissionId.remove);
                return result.Code == StatusCodes.ok ? (object)true : result;
            }

            return JsonConvert.SerializeObject(new { error = string.Format("Cannot find Adobe Connect user with email {0} or login {1}", email, login) });
        }

        /// <summary>
        /// The join recording.
        /// </summary>
        /// <param name="lmsCompany">
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
        public object JoinRecording(LmsCompany lmsCompany, LtiParamDTO param, LmsUserSettingsDTO userSettings, string recordingUrl, ref string breezeSession, string mode = null, AdobeConnectProvider adobeConnectProvider = null)
        {
            var breezeToken = string.Empty;

            var connectionMode = (AcConnectionMode)userSettings.acConnectionMode;
            if (connectionMode == AcConnectionMode.Overwrite
                || connectionMode == AcConnectionMode.DontOverwriteLocalPassword)
            {
                AdobeConnectProvider provider = adobeConnectProvider ?? this.GetProvider(lmsCompany);

                string email = param.lis_person_contact_email_primary, login = param.lms_user_login;

                var password = this.UsersSetup.GetACPassword(lmsCompany, userSettings, email, login);

                var lmsUser = this.LmsUserModel.GetOneByUserIdAndCompanyLms(param.lms_user_id, lmsCompany.Id).Value;
                if (lmsUser == null)
                {
                    return new { error = string.Format("No user with id {0} found in the database.", param.lms_user_id) };
                }

                var principalInfo = lmsUser.PrincipalId != null ? provider.GetOneByPrincipalId(lmsUser.PrincipalId).PrincipalInfo : null;
                var registeredUser = principalInfo != null ? principalInfo.Principal : null;

                if (registeredUser != null)
                {
                    breezeToken = this.LoginIntoAC(lmsCompany, param, registeredUser, connectionMode, email, login, password, provider);
                }
                else
                {
                    return new { error = string.Format("No user with principal id {0} found in Adobe Connect.", lmsUser.PrincipalId ?? string.Empty) };
                }
            }

            var baseUrl = lmsCompany.AcServer
                          + (lmsCompany.AcServer != null && lmsCompany.AcServer.EndsWith(@"/") ? string.Empty : "/")
                          + recordingUrl;

            breezeSession = breezeToken ?? string.Empty;

            return string.Format(
                           "{0}?{1}{2}{3}",
                           baseUrl,
                           mode != null ? string.Format("pbMode={0}", mode) : string.Empty,
                           mode != null && !lmsCompany.LoginUsingCookie.GetValueOrDefault() ? "&" : string.Empty,
                           !lmsCompany.LoginUsingCookie.GetValueOrDefault() ? "session=" + breezeToken : string.Empty);
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
        /// The SCO Id.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool RemoveRecording(
            LmsCompany credentials, 
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
        /// <param name="lmsCompany">
        /// The company LMS.
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
            LmsCompany lmsCompany, 
            AdobeConnectProvider provider, 
            LtiParamDTO param, 
            MeetingDTO meetingDTO,
            object extraData = null)
        {
            this.FixMeetingDTOFields(meetingDTO, param);
            
            var type = meetingDTO.type > 0 ? meetingDTO.type : (int)LmsMeetingType.Meeting;
            var lmsUser = this.LmsUserModel.GetOneByUserIdAndCompanyLms(param.lms_user_id, lmsCompany.Id).Value;
            if (lmsUser == null)
            {
                return new { error = string.Format("No lms user found with id={0} and companyLmsId={1}", param.lms_user_id, lmsCompany.Id) };                
            }

            var meeting = this.GetLmsCourseMeeting(lmsCompany, param.course_id, meetingDTO.id, type);
            var meetingSco = meeting.GetMeetingScoId();
            
            var officeHours = this.OfficeHoursModel.GetByLmsUserId(lmsUser.Id).Value;

            if (string.IsNullOrEmpty(meetingSco) && type == (int)LmsMeetingType.OfficeHours && officeHours != null)
            {    
                meetingSco = officeHours.ScoId;
                meetingDTO.id = meetingSco;
            }

            var existingMeeting = provider.GetScoInfo(meetingSco);

            var isNewMeeting = !existingMeeting.Success;
            
            var updateItem = new MeetingUpdateItem { ScoId = isNewMeeting ? null : meetingSco };

            var registeredUser = this.UsersSetup.GetPrincipalByLoginOrEmail(
                provider,
                param.lms_user_login,
                param.lis_person_contact_email_primary,
                lmsCompany.ACUsesEmailAsLogin.GetValueOrDefault());

            if (type == (int)LmsMeetingType.StudyGroup)
            {
                this.UsersSetup.AddUserToMeetingHostsGroup(provider, registeredUser.PrincipalId);
            }

            var meetingFolder = this.GetMeetingFolder(lmsCompany, provider, registeredUser);
            
            this.SetMeetingUpateItemFields(
                meetingDTO,
                updateItem,
                meetingFolder,
                type == (int)LmsMeetingType.OfficeHours ? lmsUser.Id.ToString(CultureInfo.InvariantCulture) : param.course_id.ToString(CultureInfo.InvariantCulture),
                isNewMeeting,
                lmsCompany.AddPrefixToMeetingName.GetValueOrDefault());

            ScoInfoResult result = isNewMeeting ? provider.CreateSco(updateItem) : provider.UpdateSco(updateItem);

            if (!result.Success || result.ScoInfo == null)
            {
                return new { error = result.Status };
            }

            if (isNewMeeting)
            {
                // newly created meeting
                if (meeting.LmsMeetingType != (int)LmsMeetingType.OfficeHours)
                {
                    meeting.ScoId = result.ScoInfo.ScoId;
                }
                
                if (meeting.LmsMeetingType == (int)LmsMeetingType.Meeting)
                {
                    this.UsersSetup.SetDefaultUsers(
                        lmsCompany,
                        meeting,
                        provider,
                        param.lms_user_id,
                        meeting.CourseId,
                        result.ScoInfo.ScoId,
                        extraData ?? param);

                    this.CreateAnnouncement(
                        lmsCompany,
                        param,
                        meetingDTO.name,
                        meetingDTO.start_date,
                        meetingDTO.start_time,
                        meetingDTO.duration);
                }
                else
                {
                    var user = this.UsersSetup.GetPrincipalByLoginOrEmail(provider, 
                        param.lms_user_login, 
                        param.lis_person_contact_email_primary, 
                        lmsCompany.ACUsesEmailAsLogin.GetValueOrDefault());
                    if (user != null)
                    {
                        provider.UpdateScoPermissionForPrincipal(
                            result.ScoInfo.ScoId,
                            user.PrincipalId,
                            MeetingPermissionId.host);
                        this.UsersSetup.AddUserToMeetingHostsGroup(provider, user.PrincipalId);
                    }
                }
            }

            if (meeting.LmsMeetingType == (int)LmsMeetingType.OfficeHours)
            {
                officeHours = officeHours ?? new OfficeHours { LmsUser = lmsUser };
                officeHours.Hours = meetingDTO.office_hours;
                officeHours.ScoId = meeting.ScoId = result.ScoInfo.ScoId;
                    
                this.OfficeHoursModel.RegisterSave(officeHours);

                meeting.OfficeHours = officeHours;
                meeting.ScoId = null;
            }
            else if (meeting.LmsMeetingType == (int)LmsMeetingType.StudyGroup)
            {
                meeting.Owner = lmsUser;
            }

            this.LmsCourseMeetingModel.RegisterSave(meeting);
            this.LmsCourseMeetingModel.Flush();

            SpecialPermissionId specialPermissionId = string.IsNullOrEmpty(meetingDTO.access_level)
                                                          ? (meetingDTO.allow_guests
                                                                 ? SpecialPermissionId.remove
                                                                 : SpecialPermissionId.denied)
                                                          : "denied".Equals(meetingDTO.access_level, StringComparison.OrdinalIgnoreCase)
                                                                ? SpecialPermissionId.denied
                                                                : ("view_hidden".Equals(meetingDTO.access_level, StringComparison.OrdinalIgnoreCase)
                                                                       ? SpecialPermissionId.view_hidden
                                                                       : SpecialPermissionId.remove);

            provider.UpdatePublicAccessPermissions(result.ScoInfo.ScoId, specialPermissionId);
            List<PermissionInfo> permission =
                provider.GetScoPublicAccessPermissions(result.ScoInfo.ScoId)
                    .Values.Return(x => x.ToList(), new List<PermissionInfo>());

            MeetingDTO updatedMeeting = this.GetMeetingDTOByScoInfo(
                lmsCompany,
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
        /// The SCO Id.
        /// </param>
        /// <param name="error">
        /// The error.
        /// </param>
        /// <returns>
        /// The <see cref="MeetingDTO"/>.
        /// </returns>
        public List<string> DeleteMeeting(
            LmsCompany credentials,
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

            List<PermissionInfo> enrollments = this.UsersSetup.GetMeetingAttendees(provider, meeting.GetMeetingScoId());
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
        public void SetupFolders(LmsCompany credentials, AdobeConnectProvider provider)
        {
            this.SetupTemplateFolder(credentials, provider);
            
            this.LmsСompanyModel.RegisterSave(credentials);
            this.LmsСompanyModel.Flush();   
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
        public string GetMeetingFolder(LmsCompany credentials, AdobeConnectProvider provider, Principal user)
        {
            string adobeConnectScoId = null;

            if (credentials.UseUserFolder.GetValueOrDefault() && user != null)
            {
                ////TODO Think about user folders + renaming directory
                adobeConnectScoId = this.SetupUserMeetingsFolder(credentials, provider, user);
            }

            if (adobeConnectScoId == null)
            {
                this.SetupSharedMeetingsFolder(credentials, provider);
                this.LmsСompanyModel.RegisterSave(credentials);
                this.LmsСompanyModel.Flush();
                adobeConnectScoId = credentials.ACScoId;
            }

            return adobeConnectScoId;
        }
        
        /// <summary>
        /// The get LMS parameters.
        /// </summary>
        /// <param name="acId">
        /// The AC id.
        /// </param>
        /// <param name="acDomain">
        /// The AC domain.
        /// </param>
        /// <param name="scoId">
        /// The SCO id.
        /// </param>
        /// <param name="error">
        /// The error.
        /// </param>
        /// <returns>
        /// The <see cref="LmsUserParametersDTO"/>.
        /// </returns>
        [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1305:FieldNamesMustNotUseHungarianNotation", Justification = "Reviewed. Suppression is OK here.")]
        public LmsUserParametersDTO GetLmsParameters(string acId, string acDomain, string scoId, ref string error)
        {
            LmsCourseMeetingModel.Flush();
            var courseMeetings = LmsCourseMeetingModel.GetAllByMeetingId(scoId);

            var serverCourseMeetings = courseMeetings.Where(
                cm =>
                {
                    var acServer = cm.Return(c => c.LmsCompany.Return(cl => cl.AcServer, null), null);
                    if (acServer == null)
                    {
                        return false;
                    }

                    if (acServer.EndsWith("/"))
                    {
                        acServer = acServer.Substring(0, acServer.Length - 1);
                    }

                    return acDomain.StartsWith(acServer, StringComparison.InvariantCultureIgnoreCase);
                })
                    .ToList();

            if (!serverCourseMeetings.Any())
            {
                error = "This meeting is not associated to any course";
                return null;
            }

            List<LmsUserParameters> paramList = new List<LmsUserParameters>();

            foreach (var courseMeeting in serverCourseMeetings)
            {
                if (courseMeeting.LmsCompany == null)
                {
                    continue;
                }

                var param = this.LmsUserParametersModel.GetOneByAcIdCourseIdAndCompanyLmsId(acId, courseMeeting.CourseId, courseMeeting.LmsCompany.Id).Value;
                if (param != null)
                {
                    paramList.Add(param);
                }
            }

            return paramList.Any() ? new LmsUserParametersDTO(paramList.OrderByDescending(p => p.LastLoggedIn ?? "0").First()) : null;
        }

        #endregion

        #region Methods

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
        /// <param name="userId">
        /// The user id.
        /// </param>
        private void SaveLMSUserParameters(
            int lmsCourseId,
            LmsCompany lmsCompany,
            string lmsUserId,
            string adobeConnectUserId,
            string courseName,
            string userEmail,
            string userId)
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

            lmsUserParameters.Wstoken = userId;
            lmsUserParameters.LastLoggedIn = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ssZ");
            lmsUserParameters.CourseName = courseName;
            lmsUserParameters.UserEmail = userEmail;
            lmsUserParameters.LmsUser = this.LmsUserModel.GetOneByUserIdAndCompanyLms(lmsUserId, lmsCompany.Id).Value;
            this.LmsUserParametersModel.RegisterSave(lmsUserParameters);
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

            return this.UsersSetup.IsTeacher(param);
        }

        /// <summary>
        /// The can join.
        /// </summary>
        /// <param name="provider">
        /// The provider.
        /// </param>
        /// <param name="lmsCompany">
        /// The company LMS.
        /// </param>
        /// <param name="type">
        /// The type.
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
        private bool CanJoin(
            AdobeConnectProvider provider,
            LmsCompany lmsCompany,
            int type,
            LtiParamDTO param,
            string meetingSco)
        {
            if (type == (int)LmsMeetingType.OfficeHours)
            {
                return true;
            }

            var lmsUser = this.LmsUserModel.GetOneByUserIdAndCompanyLms(param.lms_user_id, lmsCompany.Id).Value;
            
            // this method is called after the user has opened the application through LtiController, so there should already be Principal found and saved for the user.
            if (lmsUser == null || lmsUser.PrincipalId == null)
            {
                return false;
            }

            var enrollments = this.UsersSetup.GetMeetingAttendees(provider, meetingSco);
            
            return enrollments.Any(e => e.PrincipalId != null && e.PrincipalId.Equals(lmsUser.PrincipalId));
        }

        private void CreateAnnouncement(
            LmsCompany lmsCompany, 
            LtiParamDTO param, 
            string name, 
            string startDate, 
            string startTime, 
            string duration)
        {
            if (!lmsCompany.ShowAnnouncements.GetValueOrDefault() || string.IsNullOrEmpty(param.context_title))
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

            switch (lmsCompany.LmsProvider.ShortName.ToLowerInvariant())
            {
                case LmsProviderNames.Canvas:
                    var lmsUser = LmsUserModel.GetOneByUserIdAndCompanyLms(param.lms_user_id, lmsCompany.Id).Value;
                    var token = lmsUser.Return(
                            u => u.Token,
                            lmsCompany.AdminUser.Return(a => a.Token, string.Empty));
                    CanvasAPI.CreateAnnouncement(
                        lmsCompany.LmsDomain,
                        token,
                        param.course_id,
                        announcementTitle,
                        announcementMessage);
                    break;
                case LmsProviderNames.Blackboard:
                    BlackboardApi.CreateAnnouncement(param.course_id, lmsCompany, announcementTitle, announcementMessage);
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
        private static List<ACSessionParticipantDTO> GetAttendanceReport(string meetingId, AdobeConnectProvider acp, int startIndex = 0, int limit = 0)
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

        
        private static List<ACSessionDTO> GetSessionsWithParticipantsBySessionTime(string meetingId, List<MeetingAttendee> meetingAttendees, 
            AdobeConnectProvider acp, int startIndex = 0, int limit = 0)
        {
            List<MeetingSession> sessions = acp.ReportMettingSessions(meetingId, startIndex, limit).Values.ToList();
            var result = sessions.Select(sco => new ACSessionDTO
            {
                scoId = int.Parse(sco.ScoId),
                assetId = int.Parse(sco.AssetId),
                dateStarted = sco.DateCreated.FixACValue(),
                dateEnded = sco.DateEnd.FixACValue(),
                sessionNumber = int.Parse(sco.Version),
                sessionName = sco.Version,
                participants = new List<ACSessionParticipantDTO>()
            }).ToList();

            var unprocessedAttendees = new List<MeetingAttendee>();
            foreach (var attendee in meetingAttendees)
            {
                var session = result.FirstOrDefault(x => x.dateStarted <= attendee.DateCreated && (!x.dateEnded.HasValue || attendee.DateEnd <= x.dateEnded));
                if (session != null)
                {
                    session.participants.Add(
                        new ACSessionParticipantDTO
                        {
                            firstName = attendee.SessionName,
                            login = attendee.Login,
                            dateTimeEntered = attendee.DateCreated,
                            dateTimeLeft = attendee.DateEnd.FixACValue(),
                            durationInHours =
                                (float) attendee.Duration.TotalHours,
                            transcriptId = int.Parse(attendee.TranscriptId)
                        });
                    if (String.IsNullOrEmpty(session.meetingName))
                    {
                        session.meetingName = attendee.ScoName;
                    }
                }
                else
                {
                    unprocessedAttendees.Add(attendee);
                }
            }
            //unlikely possible case, need to check
            if (unprocessedAttendees.Count > 0)
            {
                var maxDate = result.Max(x => x.dateStarted);
                var ua = unprocessedAttendees.Where(x => x.DateCreated >= maxDate);
                if (ua.Any())
                {
                    var currentSessionNumber = result.Max(x => x.sessionNumber);
                    result.Add(new ACSessionDTO
                    {
                        sessionNumber = currentSessionNumber + 1,
                        sessionName = (currentSessionNumber + 1).ToString(CultureInfo.CurrentCulture),
                        dateStarted = ua.Min(x => x.DateCreated),
                        participants = ua.Select(attendee => new ACSessionParticipantDTO
                        {
                            firstName = attendee.SessionName,
                            login = attendee.Login,
                            dateTimeEntered = attendee.DateCreated,
                            dateTimeLeft = attendee.DateEnd.FixACValue(),
                            durationInHours =
                                (float)attendee.Duration.TotalHours,
                            transcriptId = int.Parse(attendee.TranscriptId)
                        }).ToList(),
                        meetingName = ua.First().ScoName
                    });
                }
            }

            GroupSessionParticipants(result);
            return result.OrderBy(s => s.sessionNumber).ToList();
        }

        private static List<ACSessionDTO> GetSessionsWithParticipants(string meetingId, AdobeConnectProvider acp, int startIndex = 0, int limit = 0)
        {
            try
            {
                List<MeetingAttendee> meetingAttendees = acp.ReportMettingAttendance(meetingId).Values.ToList();
                if (meetingAttendees.All(x => string.IsNullOrEmpty(x.AssetId)))
                {
                    //todo: we should not rely on AssetId parameter and probably use following method in all cases
                    return GetSessionsWithParticipantsBySessionTime(meetingId, meetingAttendees, acp, startIndex, limit);
                }

                //left previous version to avoid any possible errors
                var userSessions = meetingAttendees.Where(x => !string.IsNullOrEmpty(x.AssetId))
                    .GroupBy(v => v.AssetId, v => v)
                    .ToDictionary(g => int.Parse(g.Key), g => g.ToList());

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
                sessions.AddRange(
                    sessionList.Select(
                        s => new MeetingSession {AssetId = s.assetId.ToString(CultureInfo.CurrentCulture)}));

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
                                (float) us.Duration.TotalHours,
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

                GroupSessionParticipants(sessionList);

                return sessionList.OrderBy(s => s.sessionNumber).ToList();
            }
                // ReSharper disable once EmptyGeneralCatchClause
            catch
            {
            }

            return new List<ACSessionDTO>();
        }

        private static void GroupSessionParticipants(IEnumerable<ACSessionDTO> sessionList)
        {
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
        }

        /// <summary>
        /// The get meeting DTO by SCO info.
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
        /// <param name="result">
        /// The result.
        /// </param>
        /// <param name="permission">
        /// The permission.
        /// </param>
        /// <param name="lmsCourseMeeting">
        /// The LMS Course Meeting.
        /// </param>
        /// <returns>
        /// The <see cref="MeetingDTO"/>.
        /// </returns>
        private MeetingDTO GetMeetingDTOByScoInfo(
            LmsCompany lmsCompany, 
            AdobeConnectProvider provider, 
            LtiParamDTO param, 
            ScoInfo result, 
            IEnumerable<PermissionInfo> permission,
            LmsCourseMeeting lmsCourseMeeting)
        {
            bool isEditable = this.CanEdit(param, lmsCourseMeeting);
            var type = lmsCourseMeeting.LmsMeetingType.GetValueOrDefault();
            int bracketIndex = result.Name.IndexOf("]", StringComparison.Ordinal);
            var canJoin = this.CanJoin(provider, lmsCompany, type, param, result.ScoId);
            PermissionInfo permissionInfo = permission != null ? permission.FirstOrDefault() : null;
            string officeHoursString = null;
            
            if (type == (int)LmsMeetingType.OfficeHours)
            {
                var lmsUser = this.LmsUserModel.GetOneByUserIdAndCompanyLms(param.lms_user_id, lmsCompany.Id).Value;
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
                              can_join = canJoin,
                              is_editable = isEditable,
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
            LmsCompany credentials, 
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
                // ReSharper disable once UnusedVariable
                var resetPasswordResult = provider.PrincipalUpdatePassword(registeredUser.PrincipalId, password);
            }

            // ReSharper disable once UnusedVariable
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
                var resultByEmail = userProvider.Login(new UserCredentials(email, password));
                if (resultByEmail.Success)
                {
                    // ReSharper disable once RedundantAssignment
                    breezeToken = resultByEmail.Status.SessionInfo;                    
                }

                resultByLogin = userProvider.Login(new UserCredentials(registeredUser.Login, password));
                breezeToken = resultByLogin.Status.SessionInfo;
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
        private void SaveLMSUserParameters(LtiParamDTO param, LmsCompany lmsCompany, string adobeConnectUserId, string wstoken)
        {
            this.SaveLMSUserParameters(param.course_id, lmsCompany, param.lms_user_id, adobeConnectUserId, param.context_title, param.lis_person_contact_email_primary, wstoken);
        }

        /// <summary>
        /// The fix meeting DTO fields.
        /// </summary>
        /// <param name="meetingDTO">
        /// The meeting DTO.
        /// </param>
        /// <param name="param">
        /// The parameter.
        /// </param>
        private void FixMeetingDTOFields(MeetingDTO meetingDTO, LtiParamDTO param)
        {
            if (meetingDTO.start_time != null)
            {
                meetingDTO.start_time = meetingDTO.start_time.PadLeft(8, '0');
            }

            // ReSharper disable once UnusedVariable
            string oldStartDate = meetingDTO.start_date;

            if (meetingDTO.start_date != null)
            {
                meetingDTO.start_date = meetingDTO.start_date.Substring(6, 4) + "-"
                                        + meetingDTO.start_date.Substring(0, 5);
            }

            if (meetingDTO.type == (int)LmsMeetingType.OfficeHours)
            {
                meetingDTO.name = param.lis_person_name_full + "'s Office Hours";
            }
        }

        /// <summary>
        /// The get LMS course meeting.
        /// </summary>
        /// <param name="lmsCompany">
        /// The company LMS.
        /// </param>
        /// <param name="courseId">
        /// The course id.
        /// </param>
        /// <param name="scoId">
        /// The SCO id.
        /// </param>
        /// <param name="type">
        /// The type.
        /// </param>
        /// <returns>
        /// The <see cref="LmsCourseMeeting"/>.
        /// </returns>
        private LmsCourseMeeting GetLmsCourseMeeting(LmsCompany lmsCompany, int courseId, string scoId, int type)
        {
            LmsCourseMeeting meeting = this.LmsCourseMeetingModel.GetOneByCourseAndScoId(lmsCompany.Id, courseId, scoId).Value;
            if (meeting == null && type == (int)LmsMeetingType.OfficeHours)
            {
                meeting =
                    this.LmsCourseMeetingModel.GetOneByCourseAndType(
                        lmsCompany.Id,
                        courseId,
                        (int)LmsMeetingType.OfficeHours).Value;
            }

            if (meeting == null)
            {
                meeting = new LmsCourseMeeting
                {
                    LmsCompany = lmsCompany,
                    CourseId = courseId,
                    LmsMeetingType = type,
                    ScoId = type == (int)LmsMeetingType.OfficeHours ? scoId : null
                };
            }

            return meeting;
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
        /// <param name="addPrefix">
        /// The add prefix
        /// </param>
        private void SetMeetingUpateItemFields(
            MeetingDTO meetingDTO, 
            MeetingUpdateItem updateItem, 
            string folderSco, 
            string courseId,
            bool isNew,
            bool addPrefix)
        {
            updateItem.Name = string.Format(
                addPrefix ? "[{0}] {1}" : "{1}", 
                courseId,
                meetingDTO.name);
            updateItem.Name = updateItem.Name.TruncateIfMoreThen(60);

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
        private void SetupTemplateFolder(LmsCompany credentials, AdobeConnectProvider provider)
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
        private void SetupSharedMeetingsFolder(LmsCompany credentials, AdobeConnectProvider provider)
        {
            string ltiFolderSco = null;
            string name = credentials.UserFolderName ?? credentials.LmsProvider.LmsProviderName;
            name = name.TruncateIfMoreThen(60);
            if (!string.IsNullOrWhiteSpace(credentials.ACScoId))
            {
                ScoInfoResult canvasFolder = provider.GetScoInfo(credentials.ACScoId);
                if (canvasFolder.Success && canvasFolder.ScoInfo != null)
                {
                    if (canvasFolder.ScoInfo.Name.Equals(name, StringComparison.OrdinalIgnoreCase))
                    {
                        ltiFolderSco = canvasFolder.ScoInfo.ScoId;
                    }
                    else
                    {
                        ScoInfoResult updatedSco =
                            provider.UpdateSco(
                                new FolderUpdateItem
                                    {
                                        ScoId = canvasFolder.ScoInfo.ScoId,
                                        Name = name,
                                        FolderId = canvasFolder.ScoInfo.FolderId,
                                        Type = ScoType.folder
                                    });
                        if (updatedSco.Success && updatedSco.ScoInfo != null)
                        {
                            ltiFolderSco = updatedSco.ScoInfo.ScoId;
                        }
                    }
                }
            }

            if (ltiFolderSco == null)
            {
                ScoContentCollectionResult sharedMeetings = provider.GetContentsByType("meetings");
                if (sharedMeetings.ScoId != null && sharedMeetings.Values != null)
                {
                    ScoContent existingFolder = sharedMeetings.Values.FirstOrDefault(v => v.Name.Equals(name, StringComparison.OrdinalIgnoreCase) && v.IsFolder);
                    if (existingFolder != null)
                    {
                        credentials.ACScoId = existingFolder.ScoId;
                    }
                    else
                    {
                        ScoInfoResult newFolder = provider.CreateSco(new FolderUpdateItem { Name = name, FolderId = sharedMeetings.ScoId, Type = ScoType.folder });
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
        private string SetupUserMeetingsFolder(LmsCompany credentials, AdobeConnectProvider provider, Principal user)
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
                name = name.TruncateIfMoreThen(60);
                var existingFolder = provider.GetContentsByScoId(userFolder.ScoId).Values.FirstOrDefault(v => v.Name.Equals(name, StringComparison.OrdinalIgnoreCase) && v.IsFolder);

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