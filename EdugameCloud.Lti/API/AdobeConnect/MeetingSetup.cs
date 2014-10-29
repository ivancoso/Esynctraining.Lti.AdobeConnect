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
    /// The meeting setup.
    /// </summary>
    public class MeetingSetup
    {
        #region Properties

        /// <summary>
        /// Gets the canvas course meeting model.
        /// </summary>
        private LmsCourseMeetingModel LmsCourseMeetingModel
        {
            get
            {
                return IoC.Resolve<LmsCourseMeetingModel>();
            }
        }

        /// <summary>
        /// Gets the company LMS model.
        /// </summary>
        private CompanyLmsModel СompanyLmsModel
        {
            get
            {
                return IoC.Resolve<CompanyLmsModel>();
            }
        }

        /// <summary>
        /// Gets the LMS user parameters model.
        /// </summary>
        private LmsUserParametersModel LmsUserParametersModel
        {
            get
            {
                return IoC.Resolve<LmsUserParametersModel>();
            }
        }

        /// <summary>
        /// Gets the LMS user parameters.
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
                return new MeetingDTO
                           {
                               id = "0", 
                               connect_server = credentials.AcServer, 
                               is_editable = this.CanEdit(param)
                           };
            }

            ScoInfoResult result = provider.GetScoInfo(meeting.ScoId);
            if (!result.Success || result.ScoInfo == null)
            {
                this.LmsCourseMeetingModel.RegisterDelete(meeting);
                this.LmsCourseMeetingModel.Flush();
                return new MeetingDTO
                           {
                               id = "0", 
                               connect_server = credentials.AcServer, 
                               is_editable = this.CanEdit(param)
                           };
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
            var apiUrl = credentials.AcServer + (credentials.AcServer.EndsWith("/") ? string.Empty : "/");

            apiUrl = apiUrl.EndsWith("api/xml/", StringComparison.OrdinalIgnoreCase) ? apiUrl.TrimEnd('/') : apiUrl + "api/xml";

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
        /// <param name="acUsers">
        /// The ac users.
        /// </param>
        /// <returns>
        /// The <see cref="List{LmsUserDTO}"/>.
        /// </returns>
        [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1305:FieldNamesMustNotUseHungarianNotation", Justification = "Reviewed. Suppression is OK here.")]
        public List<LmsUserDTO> GetUsers(
            CompanyLms credentials, 
            AdobeConnectProvider provider, 
            LtiParamDTO param, 
            IEnumerable<Principal> acUsers = null)
        {
            List<LmsUserDTO> users = this.GetLMSUsers(credentials, param.course_id);

            LmsCourseMeeting meeting = this.LmsCourseMeetingModel.GetOneByCourseId(credentials.Id, param.course_id).Value;
            if (meeting == null)
            {
                return users;
            }

            List<PermissionInfo> hosts, participants, presenters;
            this.GetMeetingAttendees(provider, meeting.ScoId, out hosts, out presenters, out participants);

            if (acUsers == null)
            {
                acUsers = provider.GetAllPrincipals().Values;
            }

            if (acUsers == null)
            {
                return users;
            }

            var adobeConnectUsers = acUsers.ToList();

            foreach (LmsUserDTO user in users)
            {
                string email = user.Email, login = user.Login;
                Principal acUser = adobeConnectUsers.FirstOrDefault(ac => login != null && ac.Login == login);
                if (acUser == null)
                {
                    acUser = adobeConnectUsers.FirstOrDefault(ac => email != null && ac.Email == email);
                }

                if (acUser == null)
                {
                    continue;
                }

                user.ac_id = acUser.PrincipalId;

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

            foreach (var permissionInfo in hosts.Where(h => !h.HasChildren))
            {
                users.Add(new LmsUserDTO { ac_id = permissionInfo.PrincipalId, name = permissionInfo.Name, ac_role = "Host" });
            }

            foreach (var permissionInfo in presenters.Where(h => !h.HasChildren))
            {
                users.Add(new LmsUserDTO { ac_id = permissionInfo.PrincipalId, name = permissionInfo.Name, ac_role = "Presenter" });
            }

            foreach (var permissionInfo in participants.Where(h => !h.HasChildren))
            {
                users.Add(new LmsUserDTO { ac_id = permissionInfo.PrincipalId, name = permissionInfo.Name, ac_role = "Participant" });
            }

            return users;
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

            string email = param.lis_person_contact_email_primary, login = param.custom_canvas_user_login_id;

            string password = email != credentials.AcUsername
                                  ? Membership.GeneratePassword(8, 2)
                                  : credentials.AcPassword;

            var adobeConnectUsers = provider.GetAllPrincipals().Values;

            Principal registeredUser = adobeConnectUsers.FirstOrDefault(ac => (email != null && ac.Email == email) || (login != null && ac.Login == login));

            if (registeredUser != null)
            {
                breezeToken = this.LoginIntoAC(credentials, param, registeredUser,  email, login, password, provider);
                this.SaveLMSUserParameters(param, credentials, registeredUser.PrincipalId);
            }
            else
            {
                return param.launch_presentation_return_url;
            }

            return meetingUrl + "?session=" + breezeToken;
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
        public void SaveLMSUserParameters(int lmsCourseId, CompanyLms lmsCompany, int lmsUserId, string adobeConnectUserId)
        {
            var lmsUserParameters = this.LmsUserParametersModel.GetOneForLogin(adobeConnectUserId, lmsCompany.AcServer, lmsUserId).Value;
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

            string email = param.lis_person_contact_email_primary, login = param.custom_canvas_user_login_id;

            string password = email != credentials.AcUsername
                                  ? Membership.GeneratePassword(8, 2)
                                  : credentials.AcPassword;

            IEnumerable<Principal> acUsers = provider.GetAllPrincipals().Values;

            Principal registeredUser =
                acUsers.FirstOrDefault(
                    ac => (email != null && ac.Email == email) || (login != null && ac.Login == login));

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
                   + (credentials.AcServer != null && credentials.AcServer.Last() == '/' ? string.Empty : "/") + recordingUrl
                   + "?session=" + breezeToken;
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

            if (!result.Values.Any(v => v.ScoId == recordingId))
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
                ?? new LmsCourseMeeting
                       {
                           CompanyLms = credentials,
                           CourseId = param.course_id
                       };

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

                this.SetDefaultUsers(credentials, provider, meeting.CourseId, result.ScoInfo.ScoId);

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
            var permission = provider.GetScoPublicAccessPermissions(result.ScoInfo.ScoId).Values.Return(x => x.ToList(), new List<PermissionInfo>());

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
            var ltiProvider = credentials.LmsProvider.LmsProviderName;
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
            LmsCourseMeeting meeting =
                this.LmsCourseMeetingModel.GetOneByCourseId(credentials.Id, param.course_id).Value;
            if (meeting == null)
            {
                return this.GetUsers(credentials, provider, param);
            }

            var adobeConnectUsers = provider.GetAllPrincipals().Values.With(x => x.ToList());

            if (user.ac_id == null || user.ac_id == "0")
            {
                string email = user.Email, login = user.Login;

                Principal principal = adobeConnectUsers.FirstOrDefault(ac => login != null && ac.Login == login);
                if (principal == null)
                {
                    principal = adobeConnectUsers.FirstOrDefault(ac => email != null && ac.Email == email);
                }

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

            return this.GetUsers(credentials, provider, param, adobeConnectUsers);
        }

        #endregion

        #region Methods

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
        /// <param name="meetingSco">
        /// The meeting SCO.
        /// </param>
        /// <param name="email">
        /// The email.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        private bool CanJoin(AdobeConnectProvider provider, string meetingSco, string email)
        {
            PrincipalCollectionResult registeredUser = provider.GetAllByEmail(HttpUtility.UrlEncode(email));

            if (registeredUser.Success && registeredUser.Values != null)
            {
                List<PermissionInfo> hosts, presenters, participants;
                this.GetMeetingAttendees(provider, meetingSco, out hosts, out presenters, out participants);

                // ReSharper disable once LoopCanBeConvertedToQuery
                foreach (Principal user in registeredUser.Values)
                {
                    if (hosts.Any(h => h.PrincipalId == user.PrincipalId)
                        || presenters.Any(p => p.PrincipalId == user.PrincipalId)
                        || participants.Any(p => p.PrincipalId == user.PrincipalId))
                    {
                        return true;
                    }
                }
            }

            return false;
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

            CourseAPI.CreateAnnouncement(
                credentials.LmsDomain, 
                credentials.AdminUser.Token,
                param.course_id, 
                string.Format("A new Adobe Connect room was created for course {0}", param.context_title),
                string.Format("Meeting \"{0}\" will start {1} at {2}. Its duration will be {3}. You can join it in your <a href='{4}'>Adobe Connect Conference section</a>.", name, startDate, startTime, duration, param.referer ?? string.Empty));
        }

        /// <summary>
        /// The get canvas users.
        /// </summary>
        /// <param name="credentials">
        /// The credentials.
        /// </param>
        /// <param name="courseId">
        /// The course id.
        /// </param>
        /// <returns>
        /// The <see cref="List{LmsUserDTO}"/>.
        /// </returns>
        private List<LmsUserDTO> GetLMSUsers(CompanyLms credentials, int courseId)
        {
            switch (credentials.LmsProvider.LmsProviderName.ToLowerInvariant())
            {
                case LmsProviderNames.Canvas:
                    return this.GetCanvasUsers(credentials, courseId);
                case LmsProviderNames.BrainHoney:
                    return this.GetBrainHoneyUsers(credentials, courseId);
            }

            return new List<LmsUserDTO>();
        }

        /// <summary>
        /// The get canvas users.
        /// </summary>
        /// <param name="credentials">
        /// The credentials.
        /// </param>
        /// <param name="canvasCourseId">
        /// The canvas course id.
        /// </param>
        /// <returns>
        /// The <see cref="List{LmsUserDTO}"/>.
        /// </returns>
        private List<LmsUserDTO> GetCanvasUsers(CompanyLms credentials, int canvasCourseId)
        {
            List<LmsUserDTO> users = CourseAPI.GetUsersForCourse(
                credentials.LmsDomain,
                credentials.AdminUser.Token,
                canvasCourseId);
            users = users.GroupBy(u => u.id).Select(
                ug =>
                {
                    var teacher = ug.FirstOrDefault(u => u.canvas_role.Equals("teacher", StringComparison.OrdinalIgnoreCase));
                    if (teacher != null)
                    {
                        return teacher;
                    }

                    teacher = ug.FirstOrDefault(u => u.canvas_role.Equals("ta", StringComparison.OrdinalIgnoreCase));
                    if (teacher != null)
                    {
                        return teacher;
                    }

                    teacher = ug.FirstOrDefault(u => u.canvas_role.Equals("designer", StringComparison.OrdinalIgnoreCase));
                    if (teacher != null)
                    {
                        return teacher;
                    }

                    teacher = ug.FirstOrDefault(u => u.canvas_role.Equals("student", StringComparison.OrdinalIgnoreCase));
                    return teacher ?? ug.First();
                }).ToList();

            return users;
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
            //// todo implement API call
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
            presenters = presentersResult.Values.Return(x => x.ToList(),  new List<PermissionInfo>());
            participants = participantsResult.Values.Return(x => x.ToList(), new List<PermissionInfo>());
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
            var ret = new MeetingDTO
                          {
                              id = result.ScoId,
                              ac_room_url = result.UrlPath.Trim("/".ToCharArray()),
                              name =
                                  result.Name.Substring(
                                      bracketIndex < 0 || (bracketIndex + 2 > result.Name.Length)
                                          ? 0
                                          : bracketIndex + 2),
                              summary = result.Description,
                              template = result.SourceScoId,
                              start_date = result.BeginDate.ToString("yyyy-MM-dd"),
                              start_time = result.BeginDate.ToString("h:mm tt", CultureInfo.InvariantCulture),
                              duration = (result.EndDate - result.BeginDate).ToString(@"h\:mm"),
                              connect_server = credentials.AcServer,
                              access_level =
                                  permission != null && (permissionInfo = permission.FirstOrDefault()) != null
                                      ? permissionInfo.PermissionId.ToString()
                                      : string.Empty,
                              can_join = this.CanJoin(provider, result.ScoId, param.lis_person_contact_email_primary),
                              is_editable = this.CanEdit(param)
                          };
            return ret;
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
        /// <param name="canvasCourseId">
        /// The canvas course id.
        /// </param>
        /// <param name="meetingScoId">
        /// The meeting SCO id.
        /// </param>
        private void SetDefaultUsers(
            CompanyLms credentials, 
            AdobeConnectProvider provider, 
            int canvasCourseId, 
            string meetingScoId)
        {
            List<LmsUserDTO> users = this.GetLMSUsers(credentials, canvasCourseId);

            var acUsers = provider.GetAllPrincipals().Values.Return(x => x.ToList(), new List<Principal>());

            foreach (LmsUserDTO u in users)
            {
                string email = u.Email, login = u.Login;
                Principal acUser = acUsers.FirstOrDefault(ac => login != null && ac.Login == login);
                if (acUser == null)
                {
                    acUser = acUsers.FirstOrDefault(ac => email != null && ac.Email == email);
                }

                if (acUser == null)
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
                        acUser = res.Principal;
                    }
                }

                var permission = MeetingPermissionId.view;
                string role = u.canvas_role != null ? u.canvas_role.ToLower() : string.Empty;

                if (role.Contains("teacher"))
                {
                    permission = MeetingPermissionId.host;
                }
                else if (role.Contains("ta") || role.Contains("designer"))
                {
                    permission = MeetingPermissionId.mini_host;
                }

                if (acUser != null)
                {
                    provider.UpdateScoPermissionForPrincipal(
                        meetingScoId, 
                        acUser.PrincipalId, 
                        permission);
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