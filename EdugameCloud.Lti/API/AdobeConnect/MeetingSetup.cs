namespace EdugameCloud.Lti.API.AdobeConnect
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;
    using System.Linq;
    using System.Web.Security;

    using EdugameCloud.Core.Business.Models;
    using EdugameCloud.Core.Domain.Entities;
    using EdugameCloud.Lti.API.BrainHoney;
    using EdugameCloud.Lti.API.Canvas;
    using EdugameCloud.Lti.DTO;

    using Esynctraining.AC.Provider;
    using Esynctraining.AC.Provider.DataObjects;
    using Esynctraining.AC.Provider.DataObjects.Results;
    using Esynctraining.AC.Provider.Entities;
    using Esynctraining.Core.Extensions;
    using Esynctraining.Core.Utils;

    using RestSharp.Contrib;

    /// <summary>
    ///     The meeting setup.
    /// </summary>
    public class MeetingSetup
    {
        #region Fields

        /// <summary>
        /// The DLAP API.
        /// </summary>
        private readonly DlapAPI dlapApi;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="MeetingSetup"/> class.
        /// </summary>
        /// <param name="dlapApi">
        /// The DLAP API.
        /// </param>
        public MeetingSetup(DlapAPI dlapApi)
        {
            this.dlapApi = dlapApi;
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
        /// <returns>
        /// The <see cref="MeetingDTO"/>.
        /// </returns>
        public MeetingDTO GetMeeting(CompanyLms credentials, AdobeConnectProvider provider, LtiParamDTO param)
        {
            LmsCourseMeeting meeting = this.LmsCourseMeetingModel.GetOneByCourseId(credentials.Id, param.course_id).Value;

            if (meeting == null)
            {
                return this.CreateEmptyMeetingResponse(credentials, param);
            }

            ScoInfoResult result = provider.GetScoInfo(meeting.ScoId);
            if (!result.Success || result.ScoInfo == null)
            {
                this.LmsCourseMeetingModel.RegisterDelete(meeting);
                this.LmsCourseMeetingModel.Flush();
                return this.CreateEmptyMeetingResponse(credentials, param);
            }

            IEnumerable<PermissionInfo> permission = provider.GetScoPublicAccessPermissions(meeting.ScoId).Values;

            MeetingDTO meetingDTO = this.GetMeetingDTOByScoInfo(
                credentials, 
                provider, 
                param, 
                result.ScoInfo, 
                permission);

            return meetingDTO;
        }

        /// <summary>
        /// The get provider.
        /// </summary>
        /// <param name="credentials">
        /// The credentials.
        /// </param>
        /// <returns>
        /// The <see cref="AdobeConnectProvider"/>.
        /// </returns>
        public AdobeConnectProvider GetProvider(CompanyLms credentials)
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
            provider.Login(new UserCredentials(credentials.AcUsername, credentials.AcPassword));

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
        public List<RecordingDTO> GetRecordings(CompanyLms credentials, AdobeConnectProvider provider, int courseId)
        {
            LmsCourseMeeting meeting = this.LmsCourseMeetingModel.GetOneByCourseId(credentials.Id, courseId).Value;

            if (meeting == null)
            {
                return null;
            }

            ScoContentCollectionResult result = provider.GetMeetingRecordings(new[] { meeting.ScoId });
            return
                result.Values.Select(
                    v =>
                    new RecordingDTO
                        {
                            id = v.ScoId, 
                            name = v.Name, 
                            description = v.Description, 
                            begin_date = v.BeginDate.ToString("MM-dd-yy h:mm:ss tt"), 
                            end_date = v.EndDate.ToString("MM-dd-yy h:mm:ss tt"), 
                            duration = v.Duration, 
                            url = "/Lti/Recording/Join/" + v.UrlPath.Trim("/".ToCharArray())
                        }).ToList();
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
        /// <returns>
        /// The <see cref="List{LmsUserDTO}"/>.
        /// </returns>
        [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1305:FieldNamesMustNotUseHungarianNotation", 
            Justification = "Reviewed. Suppression is OK here.")]
        public List<LmsUserDTO> GetUsers(CompanyLms credentials, AdobeConnectProvider provider, LtiParamDTO param)
        {
            List<LmsUserDTO> users = this.GetLMSUsers(credentials, param.lms_user_id, param.course_id);

            LmsCourseMeeting meeting = this.LmsCourseMeetingModel.GetOneByCourseId(credentials.Id, param.course_id).Value;
            if (meeting == null)
            {
                return users;
            }

            List<PermissionInfo> hosts, participants, presenters;
            this.GetMeetingAttendees(provider, meeting.ScoId, out hosts, out presenters, out participants);

            foreach (LmsUserDTO lmsuser in users)
            {
                LmsUserDTO user = lmsuser;
                string email = user.Email, login = user.Login;

                var principal = this.GetACUser(provider, login, email);

                if (principal == null)
                {
                    continue;
                }

                user.ac_id = principal.PrincipalId;
                
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
        public List<ACSessionDTO> GetSessionsReport(CompanyLms credentials, AdobeConnectProvider provider, LtiParamDTO param, int startIndex = 0, int limit = 0)
        {
            LmsCourseMeeting meeting = this.LmsCourseMeetingModel.GetOneByCourseId(credentials.Id, param.course_id).Value;

            if (meeting == null)
            {
                return new List<ACSessionDTO>();
            }

            return this.GetSessionsWithParticipants(meeting.ScoId, provider, startIndex, limit);
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
        public List<ACSessionParticipantDTO> GetAttendanceReport(CompanyLms credentials, AdobeConnectProvider provider, LtiParamDTO param, int startIndex = 0, int limit = 0)
        {
            LmsCourseMeeting meeting = this.LmsCourseMeetingModel.GetOneByCourseId(credentials.Id, param.course_id).Value;

            if (meeting == null)
            {
                return new List<ACSessionParticipantDTO>();
            }

            return this.GetAttendanceReport(meeting.ScoId, provider, startIndex, limit);
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
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public string JoinMeeting(CompanyLms credentials, LtiParamDTO param)
        {
            AdobeConnectProvider provider = this.GetProvider(credentials);

            string breezeToken, meetingUrl = string.Empty;

            this.LmsCourseMeetingModel.Flush();
            LmsCourseMeeting currentMeeting =
                this.LmsCourseMeetingModel.GetOneByCourseId(credentials.Id, param.course_id).Value;

            string currentMeetingScoId = currentMeeting != null ? currentMeeting.ScoId : string.Empty;

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

            string password = email != credentials.AcUsername
                                  ? Membership.GeneratePassword(8, 2)
                                  : credentials.AcPassword;

            Principal registeredUser = this.GetACUser(provider, login, email);

            if (registeredUser != null)
            {
                breezeToken = this.LoginIntoAC(credentials, param, registeredUser, email, login, password, provider);
                this.SaveLMSUserParameters(param, credentials, registeredUser.PrincipalId);
            }
            else
            {
                return param.launch_presentation_return_url;
            }

            return meetingUrl + "?session=" + breezeToken;
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
        /// <param name="recordingUrl">
        /// The recording url.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public string JoinRecording(CompanyLms credentials, LtiParamDTO param, string recordingUrl)
        {
            AdobeConnectProvider provider = this.GetProvider(credentials);

            string breezeToken;

            string email = param.lis_person_contact_email_primary, login = param.lms_user_login;

            string password = email != credentials.AcUsername
                                  ? Membership.GeneratePassword(8, 2)
                                  : credentials.AcPassword;

            Principal registeredUser = this.GetACUser(provider, login, email);

            if (registeredUser != null)
            {
                if (email != credentials.AcUsername)
                {
                    provider.PrincipalUpdatePassword(registeredUser.PrincipalId, password);
                }

                provider.PrincipalUpdate(
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

                LoginResult resultByLogin = provider.Login(new UserCredentials(HttpUtility.UrlEncode(login), password));
                if (resultByLogin.Success)
                {
                    breezeToken = resultByLogin.Status.SessionInfo;
                }
                else
                {
                    LoginResult resultByEmail =
                        provider.Login(new UserCredentials(HttpUtility.UrlEncode(email), password));
                    breezeToken = resultByEmail.Status.SessionInfo;
                }
            }
            else
            {
                return param.launch_presentation_return_url;
            }

            return credentials.AcServer
                   + (credentials.AcServer != null && credentials.AcServer.Last() == '/' ? string.Empty : "/")
                   + recordingUrl + "?session=" + breezeToken;
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
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool RemoveRecording(
            CompanyLms credentials, 
            AdobeConnectProvider provider, 
            int courseId, 
            string recordingId)
        {
            LmsCourseMeeting meeting = this.LmsCourseMeetingModel.GetOneByCourseId(credentials.Id, courseId).Value;

            if (meeting == null)
            {
                return false;
            }

            ScoContentCollectionResult result = provider.GetMeetingRecordings(new[] { meeting.ScoId });

            if (result.Values.All(v => v.ScoId != recordingId))
            {
                return false;
            }

            provider.DeleteSco(recordingId);
            return true;
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
        public void SaveLMSUserParameters(
            int lmsCourseId, 
            CompanyLms lmsCompany, 
            int lmsUserId, 
            string adobeConnectUserId)
        {
            LmsUserParameters lmsUserParameters =
                this.LmsUserParametersModel.GetOneForLogin(adobeConnectUserId, lmsCompany.AcServer, lmsCourseId).Value;
            if (lmsUserParameters == null)
            {
                lmsUserParameters = new LmsUserParameters
                                        {
                                            AcId = adobeConnectUserId, 
                                            Course = lmsCourseId, 
                                            CompanyLms = lmsCompany
                                        };
            }

            lmsUserParameters.LmsUser = this.LmsUserModel.GetOneByUserIdAndCompanyLms(lmsUserId, lmsCompany.Id).Value;
            this.LmsUserParametersModel.RegisterSave(lmsUserParameters);
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
        /// <returns>
        /// The <see cref="MeetingDTO"/>.
        /// </returns>
        public MeetingDTO SaveMeeting(
            CompanyLms credentials, 
            AdobeConnectProvider provider, 
            LtiParamDTO param, 
            MeetingDTO meetingDTO)
        {
            // fix meeting dto date & time
            if (meetingDTO.start_time.IndexOf(":", StringComparison.Ordinal) == 1)
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
            LmsCourseMeeting meeting =
                this.LmsCourseMeetingModel.GetOneByCourseId(credentials.Id, param.course_id).Value
                ?? new LmsCourseMeeting { CompanyLms = credentials, CourseId = param.course_id };

            var updateItem = new MeetingUpdateItem { ScoId = meeting.ScoId };

            this.SetMeetingUpateItemFields(
                meetingDTO, 
                updateItem, 
                credentials.ACScoId, 
                param.context_label ?? "nolabel", 
                param.course_id);

            ScoInfoResult result = meeting.ScoId != null
                                       ? provider.UpdateSco(updateItem)
                                       : provider.CreateSco(updateItem);

            if (!result.Success || result.ScoInfo == null)
            {
                // didn't save, load old saved meeting
                return this.GetMeeting(credentials, provider, param);
            }

            if (updateItem.ScoId == null)
            {
                // newly created meeting
                meeting.ScoId = result.ScoInfo.ScoId;
                this.LmsCourseMeetingModel.RegisterSave(meeting);

                this.SetDefaultUsers(credentials, provider, param.lms_user_id, meeting.CourseId, result.ScoInfo.ScoId);

                this.CreateAnnouncement(
                    credentials, 
                    param, 
                    meetingDTO.name, 
                    oldStartDate, 
                    meetingDTO.start_time, 
                    meetingDTO.duration);
            }

            SpecialPermissionId specialPermissionId = meetingDTO.access_level == "denied"
                                                          ? SpecialPermissionId.denied
                                                          : (meetingDTO.access_level == "view_hidden"
                                                                 ? SpecialPermissionId.view_hidden
                                                                 : SpecialPermissionId.remove);

            provider.UpdatePublicAccessPermissions(result.ScoInfo.ScoId, specialPermissionId);
            List<PermissionInfo> permission = provider.GetScoPublicAccessPermissions(result.ScoInfo.ScoId).Values.Return(x => x.ToList(), new List<PermissionInfo>());

            MeetingDTO updatedMeeting = this.GetMeetingDTOByScoInfo(
                credentials, 
                provider, 
                param, 
                result.ScoInfo, 
                permission);

            return updatedMeeting;
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
            string ltiProvider = credentials.LmsProvider.LmsProviderName;
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

            string canvasSco = null;
            if (!string.IsNullOrWhiteSpace(credentials.ACScoId))
            {
                ScoInfoResult canvasFolder = provider.GetScoInfo(credentials.ACScoId);
                if (canvasFolder.Success && canvasFolder.ScoInfo != null)
                {
                    canvasSco = canvasFolder.ScoInfo.ScoId;
                }
            }

            if (canvasSco == null)
            {
                ScoContentCollectionResult sharedMeetings = provider.GetContentsByType("meetings");
                if (sharedMeetings.ScoId != null && sharedMeetings.Values != null)
                {
                    string name = string.Format("{0} for {1}", ltiProvider, credentials.LmsDomain ?? string.Empty);
                    ScoContent existingFolder = sharedMeetings.Values.FirstOrDefault(v => v.Name == name && v.IsFolder);
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

            this.СompanyLmsModel.RegisterSave(credentials);
            this.СompanyLmsModel.Flush();
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
        /// <returns>
        /// The <see cref="List{LmsUserDTO}"/>.
        /// </returns>
        public List<LmsUserDTO> SetDefaultRolesForNonParticipants(
            CompanyLms credentials,
            AdobeConnectProvider provider,
            LtiParamDTO param)
        {
            LmsCourseMeeting meeting = this.LmsCourseMeetingModel.GetOneByCourseId(credentials.Id, param.course_id).Value;
            var users = this.GetUsers(credentials, provider, param);
            if (meeting == null)
            {
                return users;
            }

            List<PermissionInfo> hosts, participants, presenters;
            this.GetMeetingAttendees(provider, meeting.ScoId, out hosts, out presenters, out participants);

            foreach (var user in users)
            {
                if (!this.IsUserSynched(hosts, presenters, participants, user))
                {
                    if (user.ac_id == null || user.ac_id == "0")
                    {
                        string email = user.Email, login = user.Login;

                        this.UpdateUserACValues(provider, user, login, email);
                    }

                    this.SetLMSUserDefaultACPermissions(provider, meeting.ScoId, user, user.ac_id);
                }
            }

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
        /// <returns>
        /// The <see cref="List{LmsUserDTO}"/>.
        /// </returns>
        public List<LmsUserDTO> UpdateUser(
            CompanyLms credentials, 
            AdobeConnectProvider provider, 
            LtiParamDTO param, 
            LmsUserDTO user)
        {
            LmsCourseMeeting meeting = this.LmsCourseMeetingModel.GetOneByCourseId(credentials.Id, param.course_id).Value;
            if (meeting == null)
            {
                return this.GetUsers(credentials, provider, param);
            }

            if (user.ac_id == null || user.ac_id == "0")
            {
                string email = user.Email, login = user.Login;

                this.UpdateUserACValues(provider, user, login, email);
            }

            if (user.ac_role == null)
            {
                provider.UpdateScoPermissionForPrincipal(meeting.ScoId, user.ac_id, MeetingPermissionId.remove);
                return this.GetUsers(credentials, provider, param);
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

            provider.UpdateScoPermissionForPrincipal(meeting.ScoId, user.ac_id, permission);

            return this.GetUsers(credentials, provider, param);
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
                                    Email = user.Email,
                                    FirstName = user.FirstName,
                                    LastName = user.LastName,
                                    Name = user.name,
                                    Login = user.Login,
                                    Password = Membership.GeneratePassword(8, 2)
                                };
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

        #endregion

        #region Methods

        /// <summary>
        /// The create empty meeting response.
        /// </summary>
        /// <param name="credentials">
        /// The credentials.
        /// </param>
        /// <param name="param">
        /// The parameter.
        /// </param>
        /// <returns>
        /// The <see cref="MeetingDTO"/>.
        /// </returns>
        private MeetingDTO CreateEmptyMeetingResponse(CompanyLms credentials, LtiParamDTO param)
        {
            return new MeetingDTO
            {
                id = "0",
                connect_server = credentials.AcServer,
                is_editable = this.CanEdit(param),
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
            var principal = string.IsNullOrWhiteSpace(login)
                                   ? null
                                   : provider.GetAllByLogin(login).Return(x => x.Values, new List<Principal>()).FirstOrDefault();
            if (principal == null && !string.IsNullOrWhiteSpace(email))
            {
                principal = provider.GetAllByEmail(email).Return(x => x.Values, new List<Principal>()).FirstOrDefault();
            }

            return principal;
        }

        /// <summary>
        /// The can edit.
        /// </summary>
        /// <param name="param">
        /// The parameter.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        private bool CanEdit(LtiParamDTO param)
        {
            return param.roles != null && (param.roles.Contains("Instructor") || param.roles.Contains("Administrator"));
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
        /// <param name="lmsUserId">
        /// The lms User Id.
        /// </param>
        /// <param name="courseId">
        /// The course Id.
        /// </param>
        /// <param name="meetingSco">
        /// The meeting SCO.
        /// </param>
        /// <param name="email">
        /// The email.
        /// </param>
        /// <param name="login">
        /// The login.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        private Tuple<bool, bool> GetMeetingFlags(
            AdobeConnectProvider provider,
            CompanyLms credentials,
            int lmsUserId,
            int courseId,
            string meetingSco,
            string email,
            string login)
        {
            bool canJoin = false;
            bool areUsersSynched = true;
            var registeredUser = this.GetACUser(provider, HttpUtility.UrlEncode(login), HttpUtility.UrlEncode(email));

            if (registeredUser != null)
            {
                List<PermissionInfo> hosts, presenters, participants;
                this.GetMeetingAttendees(provider, meetingSco, out hosts, out presenters, out participants);

                if (hosts.Any(h => h.PrincipalId == registeredUser.PrincipalId)
                    || presenters.Any(p => p.PrincipalId == registeredUser.PrincipalId)
                    || participants.Any(p => p.PrincipalId == registeredUser.PrincipalId))
                {
                    canJoin = true;
                }

                areUsersSynched = this.AreUsersSynched(credentials, lmsUserId, courseId, hosts, presenters, participants);
            }

            return new Tuple<bool, bool>(canJoin, areUsersSynched);
        }

        /// <summary>
        /// The lms user is ac user.
        /// </summary>
        /// <param name="lmsUser">
        /// The lms user.
        /// </param>
        /// <param name="participant">
        /// The participant.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        private static bool LmsUserIsAcUser(LmsUserDTO lmsUser, PermissionInfo participant)
        {
            return participant.Login != null && ((lmsUser.primary_email != null && lmsUser.primary_email.Equals(participant.Login, StringComparison.OrdinalIgnoreCase))
                   || (lmsUser.login_id != null && lmsUser.login_id.Equals(participant.Login, StringComparison.OrdinalIgnoreCase)));
        }

        /// <summary>
        /// The are users synched.
        /// </summary>
        /// <param name="credentials">
        /// The credentials.
        /// </param>
        /// <param name="lmsUserId">
        /// The lms User Id.
        /// </param>
        /// <param name="courseId">
        /// The course id.
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
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        private bool AreUsersSynched(
            CompanyLms credentials,
            int lmsUserId,
            int courseId,
            List<PermissionInfo> hosts,
            List<PermissionInfo> presenters,
            List<PermissionInfo> participants)
        {
            var lmsUsers = this.GetLMSUsers(credentials, lmsUserId, courseId);
            foreach (var lmsUser in lmsUsers)
            {
                if (!IsUserSynched(hosts, presenters, participants, lmsUser))
                {
                    return false;
                }
            }

            return true;
        }

        private bool IsUserSynched(List<PermissionInfo> hosts, List<PermissionInfo> presenters, List<PermissionInfo> participants, LmsUserDTO lmsUser)
        {
            bool isFound = false;
            foreach (var host in hosts)
            {
                if (LmsUserIsAcUser(lmsUser, host))
                {
                    lmsUser.ac_id = host.PrincipalId;
                    lmsUser.ac_role = "Host";
                    isFound = true;
                    break;
                }
            }

            foreach (var presenter in presenters)
            {
                if (LmsUserIsAcUser(lmsUser, presenter))
                {
                    lmsUser.ac_id = presenter.PrincipalId;
                    lmsUser.ac_role = "Presenter";
                    isFound = true;
                    break;
                }
            }

            foreach (var participant in participants)
            {
                if (LmsUserIsAcUser(lmsUser, participant))
                {
                    lmsUser.ac_id = participant.PrincipalId;
                    lmsUser.ac_role = "Participant";
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

            switch (credentials.LmsProvider.LmsProviderName.ToLowerInvariant())
            {
                case LmsProviderNames.Canvas:
                    CourseAPI.CreateAnnouncement(
                        credentials.LmsDomain,
                        credentials.AdminUser.Token,
                        param.course_id,
                        announcementTitle,
                        announcementMessage);
                    break;
                case LmsProviderNames.BrainHoney:
                    string error;
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
        /// <param name="brainHoneyCourseId">
        /// The brain honey course id.
        /// </param>
        /// <returns>
        /// The <see cref="List{LmsUserDTO}"/>.
        /// </returns>
        private List<LmsUserDTO> GetBrainHoneyUsers(CompanyLms credentials, int brainHoneyCourseId)
        {
            string error;
            List<LmsUserDTO> users = this.dlapApi.GetUsersForCourse(credentials, brainHoneyCourseId, out error);
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
            var order = new List<string> { "owner", "author", "teacher", "ta", "designer", "student", "reader", };
            users = users.GroupBy(u => u.id).Select(
                ug =>
                    {
                        foreach (var orderRole in order)
                        {
                            string role = orderRole;
                            var userDTO = ug.FirstOrDefault(u => u.lms_role.Equals(role, StringComparison.OrdinalIgnoreCase));
                            if (userDTO != null)
                            {
                                return userDTO;
                            }
                        }

                        return ug.First();
                    }).ToList();

            return users;
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
        private List<LmsUserDTO> GetCanvasUsers(CompanyLms credentials, int canvasUserId, int canvasCourseId)
        {
            var lmsUser = this.LmsUserModel.GetOneByUserIdAndCompanyLms(canvasUserId, credentials.Id).Value;
            var token = lmsUser != null
                            ? lmsUser.Token
                            : (credentials.AdminUser != null ? credentials.AdminUser.Token : string.Empty);

            List<LmsUserDTO> users = CourseAPI.GetUsersForCourse(
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
        /// <param name="lmsUserId">
        /// The lms User Id.
        /// </param>
        /// <param name="courseId">
        /// The course id.
        /// </param>
        /// <returns>
        /// The <see cref="List{LmsUserDTO}"/>.
        /// </returns>
        private List<LmsUserDTO> GetLMSUsers(CompanyLms credentials, int lmsUserId, int courseId)
        {
            switch (credentials.LmsProvider.LmsProviderName.ToLowerInvariant())
            {
                case LmsProviderNames.Canvas:
                    return this.GetCanvasUsers(credentials, lmsUserId, courseId);
                case LmsProviderNames.BrainHoney:
                    return this.GetBrainHoneyUsers(credentials, courseId);
            }

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
        private void GetMeetingAttendees(
            AdobeConnectProvider provider, 
            string meetingSco, 
            out List<PermissionInfo> hosts, 
            out List<PermissionInfo> presenters, 
            out List<PermissionInfo> participants)
        {
            PermissionCollectionResult hostsResult = provider.GetMeetingHosts(meetingSco);
            PermissionCollectionResult presentersResult = provider.GetMeetingPresenters(meetingSco);
            PermissionCollectionResult participantsResult = provider.GetMeetingParticipants(meetingSco);
            hosts = hostsResult.Values.Return(x => x.ToList(), new List<PermissionInfo>());
            presenters = presentersResult.Values.Return(x => x.ToList(), new List<PermissionInfo>());
            participants = participantsResult.Values.Return(x => x.ToList(), new List<PermissionInfo>());
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
                                dateTimeLeft = us.DateEnd,
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
                                dateStarted = sco.DateCreated,
                                dateEnded = sco.DateEnd,
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
                                dateTimeLeft = us.DateEnd,
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
        /// <returns>
        /// The <see cref="MeetingDTO"/>.
        /// </returns>
        private MeetingDTO GetMeetingDTOByScoInfo(
            CompanyLms credentials, 
            AdobeConnectProvider provider, 
            LtiParamDTO param, 
            ScoInfo result, 
            IEnumerable<PermissionInfo> permission)
        {
            PermissionInfo permissionInfo;
            int bracketIndex = result.Name.IndexOf("]", StringComparison.Ordinal);
            var flags = this.GetMeetingFlags(provider, credentials, param.lms_user_id, param.course_id, result.ScoId, param.lis_person_contact_email_primary, param.lms_user_login);

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
                              connect_server = credentials.AcServer, 
                              access_level = permission != null && (permissionInfo = permission.FirstOrDefault()) != null ? permissionInfo.PermissionId.ToString() : string.Empty, 
                              can_join = flags.Item1, 
                              are_users_synched = flags.Item2,
                              is_editable = this.CanEdit(param)
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
            string email, 
            string login, 
            string password, 
            AdobeConnectProvider provider)
        {
            string breezeToken;
            if (email != credentials.AcUsername)
            {
                provider.PrincipalUpdatePassword(registeredUser.PrincipalId, password);
            }

            provider.PrincipalUpdate(
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

            LoginResult resultByLogin = provider.Login(new UserCredentials(HttpUtility.UrlEncode(login), password));
            if (resultByLogin.Success)
            {
                breezeToken = resultByLogin.Status.SessionInfo;
            }
            else
            {
                LoginResult resultByEmail = provider.Login(new UserCredentials(HttpUtility.UrlEncode(email), password));
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
            this.SaveLMSUserParameters(param.course_id, lmsCompany, param.lms_user_id, adobeConnectUserId);
        }

        /// <summary>
        /// The set default users.
        /// </summary>
        /// <param name="credentials">
        /// The credentials.
        /// </param>
        /// <param name="provider">
        /// The provider.
        /// </param>
        /// <param name="lmsUserId">
        /// The lms User Id.
        /// </param>
        /// <param name="courseId">
        /// The LMS course id.
        /// </param>
        /// <param name="meetingScoId">
        /// The meeting SCO id.
        /// </param>
        private void SetDefaultUsers(
            CompanyLms credentials, 
            AdobeConnectProvider provider, 
            int lmsUserId,
            int courseId, 
            string meetingScoId)
        {
            List<LmsUserDTO> users = this.GetLMSUsers(credentials, lmsUserId, courseId);


            foreach (LmsUserDTO u in users)
            {
                string email = u.Email, login = u.Login;
                var principal = this.GetACUser(provider, login, email);
                if (principal == null)
                {
                    PrincipalResult res =
                        provider.PrincipalUpdate(
                            new PrincipalSetup
                                {
                                    Email = u.Email, 
                                    FirstName = u.FirstName, 
                                    LastName = u.LastName, 
                                    Password = Membership.GeneratePassword(8, 2), 
                                    Login = u.Login, 
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
        /// <param name="principal">
        /// The principal.
        /// </param>
        private void SetLMSUserDefaultACPermissions(
            AdobeConnectProvider provider,
            string meetingScoId,
            LmsUserDTO u,
            string principalId)
        {
            var permission = MeetingPermissionId.view;
            u.ac_role = "Participant";
            string role = u.lms_role != null ? u.lms_role.ToLower() : string.Empty;
            if (role.Contains("teacher"))
            {
                permission = MeetingPermissionId.host;
                u.ac_role = "Host";
            }
            else if (role.Contains("ta") || role.Contains("designer") || role.Contains("author") || role.Contains("owner"))
            {
                u.ac_role = "Presenter";
                permission = MeetingPermissionId.mini_host;
            }

            if (!string.IsNullOrWhiteSpace(principalId))
            {
                provider.UpdateScoPermissionForPrincipal(meetingScoId, principalId, permission);
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
        /// <param name="contextLabel">
        /// The context label.
        /// </param>
        /// <param name="courseId">
        /// The course id.
        /// </param>
        private void SetMeetingUpateItemFields(
            MeetingDTO meetingDTO, 
            MeetingUpdateItem updateItem, 
            string folderSco, 
            string contextLabel, 
            int courseId)
        {
            updateItem.Name = string.Format(
                "{0} [{1}] {2}", 
                courseId, 
                contextLabel.Substring(0, Math.Min(contextLabel.Length, 10)), 
                meetingDTO.name);
            if (updateItem.Name.Length > 60)
            {
                updateItem.Name = updateItem.Name.Substring(0, 60);
            }

            updateItem.Description = meetingDTO.summary;
            updateItem.UrlPath = meetingDTO.ac_room_url;
            updateItem.FolderId = folderSco;
            updateItem.Language = "en";
            updateItem.Type = ScoType.meeting;
            updateItem.SourceScoId = meetingDTO.template;

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

        #endregion
    }
}