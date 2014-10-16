namespace EdugameCloud.Lti.API.AdobeConnect
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;

    using Castle.Windsor.Installer;

    using EdugameCloud.Core.Business.Models;
    using EdugameCloud.Core.Domain.Entities;
    using EdugameCloud.Lti.API.Canvas;
    using EdugameCloud.Lti.DTO;

    using Esynctraining.AC.Provider;
    using Esynctraining.AC.Provider.DataObjects;
    using Esynctraining.AC.Provider.DataObjects.Results;
    using Esynctraining.AC.Provider.Entities;
    using Esynctraining.Core.Utils;

    using log4net.Util;

    using NHibernate.Mapping;

    using RestSharp.Contrib;

    public class MeetingSetup
    {
        #region Properties

        private CanvasCourseMeetingModel canvasCourseMeetingModel
        {
            get
            {
                return IoC.Resolve<CanvasCourseMeetingModel>();
            }
        }

        private CompanyLmsModel companyLmsModel
        {
            get
            {
                return IoC.Resolve<CompanyLmsModel>();
            }
        }

        #endregion

        #region Public Methods

        public AdobeConnectProvider GetProvider(CompanyLms credentials)
        {
            var connectionDetails = new ConnectionDetails()
            {
                ServiceUrl = credentials.AcServer + (credentials.AcServer.EndsWith("/") ? string.Empty : "/")
                    + "api/xml",
                EventMaxParticipants = 10,
                Proxy = new ProxyCredentials()
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
        
        public List<UserDTO> GetUsers(CompanyLms credentials, AdobeConnectProvider provider, LtiParamDTO param,
            IEnumerable<Principal> acUsers = null)
        {
            var users = this.GetCanvasUsers(credentials, param.custom_canvas_course_id);
            
            var meeting = this.canvasCourseMeetingModel.GetOneByCourseId(credentials.Id, param.custom_canvas_course_id).Value;
            if (meeting == null)
            {
                return users;
            }
            
            IEnumerable<PermissionInfo> hosts, participants, presenters;
            this.GetMeetingAttendees(provider, meeting.ScoId, out hosts, out presenters, out participants);

            if (acUsers == null)
            {
                acUsers = provider.GetAllPrincipals().Values;
            }

            if (acUsers == null) return users;

            foreach (var user in users)
            {
                string email = user.Email, login = user.Login;
                var acUser = acUsers.FirstOrDefault(ac => login != null && ac.Login == login);
                if (acUser == null) acUser = acUsers.FirstOrDefault(ac => email != null && ac.Email == email);

                if (acUser == null) continue;
                
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

            foreach (var u in hosts.Where(h => !h.HasChildren))
            {
                users.Add(new UserDTO()
                {
                    ac_id = u.PrincipalId,
                    name = u.Name,
                    ac_role = "Host"
                });
            }

            foreach (var u in presenters.Where(h => !h.HasChildren))
            {
                users.Add(new UserDTO()
                {
                    ac_id = u.PrincipalId,
                    name = u.Name,
                    ac_role = "Presenter"
                });
            }

            foreach (var u in participants.Where(h => !h.HasChildren))
            {
                users.Add(new UserDTO()
                {
                    ac_id = u.PrincipalId,
                    name = u.Name,
                    ac_role = "Participant"
                });
            }

            return users;
        }

        public MeetingDTO SaveMeeting(
            CompanyLms credentials,
            AdobeConnectProvider provider,
            LtiParamDTO param,
            MeetingDTO meetingDTO)
        {
            // fix meeting dto date & time
            if (meetingDTO.start_time.IndexOf(":") == 1)
            {
                meetingDTO.start_time = "0" + meetingDTO.start_time;
            }

            var oldStartDate = meetingDTO.start_date;

            if (meetingDTO.start_date != null)
            {
                meetingDTO.start_date = meetingDTO.start_date.Substring(6, 4) + "-"
                                        + meetingDTO.start_date.Substring(0, 5);
            }
            //end fix meeting dto

            var meeting =
                this.canvasCourseMeetingModel.GetOneByCourseId(credentials.Id, param.custom_canvas_course_id).Value
                ?? new CanvasCourseMeeting()
                {
                    CanvasConnectCredentialsId = credentials.Id,
                    CourseId = param.custom_canvas_course_id
                };

            var updateItem = new MeetingUpdateItem() { ScoId = meeting.ScoId };

            this.SetMeetingUpateItemFields(meetingDTO, updateItem, credentials.ACScoId, param.context_label ?? "nolabel", param.custom_canvas_course_id);

            var result = meeting.ScoId != null ? provider.UpdateSco(updateItem) : provider.CreateSco(updateItem);

            if (!result.Success || result.ScoInfo == null) // didn't save, load old saved meeting
            {
                return this.GetMeeting(credentials, provider, param);
            }

            if (updateItem.ScoId == null) // newly created meeting
            {
                meeting.ScoId = result.ScoInfo.ScoId;
                this.canvasCourseMeetingModel.RegisterSave(meeting);

                this.SetDefaultUsers(credentials, provider, meeting.CourseId, result.ScoInfo.ScoId);

                this.CreateAnnouncement(
                    credentials,
                    param,
                    meetingDTO.name,
                    oldStartDate,
                    meetingDTO.start_time,
                    meetingDTO.duration);
            }

            var specialPermissionId = meetingDTO.access_level == "denied"
                ? SpecialPermissionId.denied
                : (meetingDTO.access_level == "view_hidden"
                    ? SpecialPermissionId.view_hidden
                    : SpecialPermissionId.remove);

            provider.UpdatePublicAccessPermissions(result.ScoInfo.ScoId, specialPermissionId);
            var permission = provider.GetScoPublicAccessPermissions(result.ScoInfo.ScoId).Values;

            var updatedMeeting = this.GetMeetingDTOByScoInfo(credentials, provider, param, result.ScoInfo, permission);

            return updatedMeeting;
        }


        public string JoinRecording(CompanyLms credentials, LtiParamDTO param, string recordingUrl)
        {
            var provider = this.GetProvider(credentials);

            string breezeToken = string.Empty;

            string email = param.lis_person_contact_email_primary, login = param.custom_canvas_user_login_id;

            var password = email != credentials.AcUsername ?
                System.Web.Security.Membership.GeneratePassword(8, 2) :
                credentials.AcPassword;

            var acUsers = provider.GetAllPrincipals().Values;

            var registeredUser = acUsers.FirstOrDefault(ac => (email != null && ac.Email == email) || (login != null && ac.Login == login));

            if (registeredUser != null)
            {
                if (email != credentials.AcUsername)
                {
                    provider.PrincipalUpdatePassword(registeredUser.PrincipalId, password);
                }

                var res = provider.PrincipalUpdate(new PrincipalSetup()
                {
                    PrincipalId = registeredUser.PrincipalId,
                    FirstName = param.lis_person_name_given,
                    LastName = param.lis_person_name_family,
                    Name = registeredUser.Name,
                    Login = registeredUser.Login,
                    Email = registeredUser.Email,
                    HasChildren = registeredUser.HasChildren
                });

                var resultByLogin = provider.Login(new UserCredentials(HttpUtility.UrlEncode(login), password));
                if (resultByLogin.Success)
                {
                    breezeToken = resultByLogin.Status.SessionInfo;
                }
                else
                {
                    var resultByEmail = provider.Login(new UserCredentials(HttpUtility.UrlEncode(email), password));
                    breezeToken = resultByEmail.Status.SessionInfo;
                }
            }
            else
            {
                return param.launch_presentation_return_url;
            }

            return credentials.AcServer + (credentials.AcServer != null && credentials.AcServer.Last() == '/' ? "" : "/") 
                + recordingUrl + "?session=" + breezeToken;
        }

        public string JoinMeeting(CompanyLms credentials, LtiParamDTO param)
        {
            var provider = this.GetProvider(credentials);
            
            string breezeToken = string.Empty, meetingUrl = string.Empty;
            
            this.canvasCourseMeetingModel.Flush();
            var currentMeeting = this.canvasCourseMeetingModel.GetOneByCourseId(credentials.Id, param.custom_canvas_course_id).Value;

            var currentMeetingScoId = currentMeeting != null ? currentMeeting.ScoId : string.Empty;

            if (!string.IsNullOrEmpty(currentMeetingScoId))
            {
                var currentMeetingSco = provider.GetScoContent(currentMeetingScoId).ScoContent;
                if (currentMeetingSco != null)
                {
                    meetingUrl = (credentials.AcServer.EndsWith("/") ? credentials.AcServer.Substring(0, credentials.AcServer.Length - 1) : credentials.AcServer)
                        + currentMeetingSco.UrlPath;
                }
            }

            string email = param.lis_person_contact_email_primary, login = param.custom_canvas_user_login_id;

            var password = email != credentials.AcUsername ?
                System.Web.Security.Membership.GeneratePassword(8, 2) :
                credentials.AcPassword;

            var acUsers = provider.GetAllPrincipals().Values;

            var registeredUser = acUsers.FirstOrDefault(ac => (email != null && ac.Email == email) || (login != null && ac.Login == login));

            if (registeredUser != null)
            {
                if (email != credentials.AcUsername)
                {
                    provider.PrincipalUpdatePassword(registeredUser.PrincipalId, password);
                }

                var res = provider.PrincipalUpdate(new PrincipalSetup()
                {
                    PrincipalId = registeredUser.PrincipalId,
                    FirstName = param.lis_person_name_given,
                    LastName = param.lis_person_name_family,
                    Name = registeredUser.Name,
                    Login = registeredUser.Login,
                    Email = registeredUser.Email,
                    HasChildren = registeredUser.HasChildren
                });

                var resultByLogin = provider.Login(new UserCredentials(HttpUtility.UrlEncode(login), password));
                if (resultByLogin.Success)
                {
                    breezeToken = resultByLogin.Status.SessionInfo;
                }
                else
                {
                    var resultByEmail = provider.Login(new UserCredentials(HttpUtility.UrlEncode(email), password));
                    breezeToken = resultByEmail.Status.SessionInfo;
                }
            }
            else
            {
                return param.launch_presentation_return_url;
            }

            return meetingUrl + "?session=" + breezeToken;
        }

        public MeetingDTO GetMeeting(CompanyLms credentials, AdobeConnectProvider provider, LtiParamDTO param)
        {
            var meeting = this.canvasCourseMeetingModel.GetOneByCourseId(credentials.Id, param.custom_canvas_course_id).Value;

            if (meeting == null) return
                new MeetingDTO()
                {
                    id = "0",
                    connect_server = credentials.AcServer,
                    is_editable = this.CanEdit(param)
                };

            var result = provider.GetScoInfo(meeting.ScoId);
            if (!result.Success || result.ScoInfo == null)
            {
                this.canvasCourseMeetingModel.RegisterDelete(meeting);
                this.canvasCourseMeetingModel.Flush();
                return
                    new MeetingDTO()
                    {
                        id = "0",
                        connect_server = credentials.AcServer,
                        is_editable = this.CanEdit(param)
                    };
            }

            var permission = provider.GetScoPublicAccessPermissions(meeting.ScoId).Values;

            var meetingDTO = this.GetMeetingDTOByScoInfo(credentials, provider, param, result.ScoInfo, permission);
            
            return meetingDTO;
        }

        public List<UserDTO> UpdateUser(CompanyLms credentials, AdobeConnectProvider provider, LtiParamDTO param, UserDTO user)
        {
            var meeting = this.canvasCourseMeetingModel.GetOneByCourseId(credentials.Id, param.custom_canvas_course_id).Value;
            if (meeting == null)
            {
                return this.GetUsers(credentials, provider, param);
            }

            var acUsers = provider.GetAllPrincipals().Values;

            if (user.ac_id == null || user.ac_id == "0")
            {
                string email = user.Email, login = user.Login;
                
                var acUser = acUsers.FirstOrDefault(ac => login != null && ac.Login == login);
                if (acUser == null) acUser = acUsers.FirstOrDefault(ac => email != null && ac.Email == email);

                if (acUser == null)
                {
                    var setup = new PrincipalSetup()
                    {
                        Email = user.Email,
                        FirstName = user.FirstName,
                        LastName = user.LastName,
                        Name = user.name,
                        Login = user.Login,
                        Password = System.Web.Security.Membership.GeneratePassword(8, 2)
                    };
                    var pu = provider.PrincipalUpdate(setup);
                    if (pu.Principal != null)
                    {
                        user.ac_id = pu.Principal.PrincipalId;
                    }
                }
                else
                {
                    user.ac_id = acUser.PrincipalId;
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

            var res = provider.UpdateScoPermissionForPrincipal(meeting.ScoId, user.ac_id, permission);

            return this.GetUsers(credentials, provider, param, acUsers);
        }

        public List<RecordingDTO> GetRecordings(CompanyLms credentials, AdobeConnectProvider provider, int courseId)
        {
            var meeting = this.canvasCourseMeetingModel.GetOneByCourseId(credentials.Id, courseId).Value;

            if (meeting == null) return null;

            var result = provider.GetMeetingRecordings(new string[] { meeting.ScoId });
            return result.Values.Select(v => new RecordingDTO()
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

        public bool RemoveRecording(CompanyLms credentials, AdobeConnectProvider provider, int courseId, string recordingId)
        {
            var meeting = this.canvasCourseMeetingModel.GetOneByCourseId(credentials.Id, courseId).Value;

            if (meeting == null) return false;

            var result = provider.GetMeetingRecordings(new string[] { meeting.ScoId });

            if (!result.Values.Any(v => v.ScoId == recordingId)) return false;

            provider.DeleteSco(recordingId);
            return true;
        }

        public void SetupFolders(CompanyLms credentials, AdobeConnectProvider provider)
        {
            string templatesSco = null;
            if (!string.IsNullOrWhiteSpace(credentials.ACTemplateScoId))
            {
                var templatesFolder = provider.GetScoInfo(credentials.ACTemplateScoId);
                if (templatesFolder.Success && templatesFolder.ScoInfo != null)
                {
                    templatesSco = templatesFolder.ScoInfo.ScoId;
                }
            }

            if (templatesSco == null)
            {
                var sharedTemplates = provider.GetContentsByType("shared-meeting-templates");
                if (sharedTemplates.ScoId != null)
                {
                    credentials.ACTemplateScoId = sharedTemplates.ScoId;
                }
            }

            string canvasSco = null;
            if (!string.IsNullOrWhiteSpace(credentials.ACScoId))
            {
                var canvasFolder = provider.GetScoInfo(credentials.ACScoId);
                if (canvasFolder.Success && canvasFolder.ScoInfo != null)
                {
                    canvasSco = canvasFolder.ScoInfo.ScoId;
                }
            }

            if (canvasSco == null)
            {
                var sharedMeetings = provider.GetContentsByType("meetings");
                if (sharedMeetings.ScoId != null && sharedMeetings.Values != null)
                {
                    var name = "Canvas for " + (credentials.LmsDomain ?? "");
                    var existingFolder = sharedMeetings.Values.FirstOrDefault(v => v.Name == name && v.IsFolder);
                    if (existingFolder != null)
                    {
                        credentials.ACScoId = existingFolder.ScoId;
                    }
                    else
                    {
                        var newFolder = provider.CreateSco(new FolderUpdateItem()
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

            companyLmsModel.RegisterSave(credentials);
            companyLmsModel.Flush();
        }

        public List<TemplateDTO> GetTemplates(AdobeConnectProvider provider, string templateFolder)
        {
            var result = provider.GetContentsByScoId(templateFolder);
            if (result.Values == null)
            {
                return new List<TemplateDTO>() {};
            }

            return result.Values.Select(v => new TemplateDTO()
            {
                id = v.ScoId,
                name = v.Name
            }).ToList();
        }

        #endregion

        #region Private Methods

        private List<UserDTO> GetCanvasUsers(CompanyLms credentials, int canvasCourseId)
        {
            var users = CourseAPI.GetUsersForCourse(credentials.LmsDomain, credentials.AdminUser.Token, canvasCourseId);
            users = users.GroupBy(u => u.id).Select(
                ug =>
                {
                    var teacher = ug.FirstOrDefault(u => u.canvas_role.ToLower() == "teacher");
                    if (teacher != null) return teacher;
                    teacher = ug.FirstOrDefault(u => u.canvas_role.ToLower() == "ta");
                    if (teacher != null) return teacher;
                    teacher = ug.FirstOrDefault(u => u.canvas_role.ToLower() == "designer");
                    if (teacher != null) return teacher;
                    teacher = ug.FirstOrDefault(u => u.canvas_role.ToLower() == "student");
                    if (teacher != null) return teacher;
                    return ug.First();
                }).ToList();

            return users;
        }

        private void GetMeetingAttendees(
            AdobeConnectProvider provider,
            string meetingSco,
            out IEnumerable<PermissionInfo> hosts,
            out IEnumerable<PermissionInfo> presenters,
            out IEnumerable<PermissionInfo> participants)
        {
            var hostsResult = provider.GetMeetingHosts(meetingSco);
            var presentersResult = provider.GetMeetingPresenters(meetingSco);
            var participantsResult = provider.GetMeetingParticipants(meetingSco);
            hosts = hostsResult.Values ?? new List<PermissionInfo>();
            presenters = presentersResult.Values ?? new List<PermissionInfo>();
            participants = participantsResult.Values ?? new List<PermissionInfo>();
        }

        private bool CanJoin(AdobeConnectProvider provider, string meetingSco, string email)
        {
            var registeredUser = provider.GetAllByEmail(HttpUtility.UrlEncode(email));
            
            if (registeredUser.Success && registeredUser.Values != null)
            {
                IEnumerable<PermissionInfo> hosts, presenters, participants;
                this.GetMeetingAttendees(provider, meetingSco, out hosts, out presenters, out participants);

                foreach (var user in registeredUser.Values)
                {
                    if (hosts.Any(h => h.PrincipalId == user.PrincipalId)
                        || presenters.Any(p => p.PrincipalId == user.PrincipalId)
                        || participants.Any(p => p.PrincipalId == user.PrincipalId)) return true;
                }
            }
            
            return false;
        }

        private bool CanEdit(LtiParamDTO param)
        {
            return param.roles != null
                          && (param.roles.Contains("Instructor")
                              || param.roles.Contains("Administrator"));
        }

        private void CreateAnnouncement(CompanyLms credentials, LtiParamDTO param, string name, 
            string startDate, string startTime, string duration)
        {
            if (credentials.LmsDomain.IndexOf("canvas") < 0 || String.IsNullOrEmpty(param.context_title)) return;

            var rets = CourseAPI.CreateAnnouncement(
                credentials.LmsDomain,
                credentials.AdminUser.Token,
                param.custom_canvas_course_id,
                String.Format("A new Adobe Connect room was created for course {0}", param.context_title),
                String.Format("Meeting \"{0}\" will start {1} at {2}. Its duration will be {3}. You can join it in your Adobe Connect Conference section.",
                    name, startDate, startTime, duration));
        }

        private void SetDefaultUsers(CompanyLms credentials, AdobeConnectProvider provider, int canvasCourseId, string meetingScoId)
        {
            var users = this.GetCanvasUsers(credentials, canvasCourseId);

            var acUsers = provider.GetAllPrincipals().Values;

            foreach (var u in users)
            {
                string email = u.Email, login = u.Login;
                var acUser = acUsers.FirstOrDefault(ac => login != null && ac.Login == login);
                if (acUser == null) acUser = acUsers.FirstOrDefault(ac => email != null && ac.Email == email);

                if (acUser == null)
                {
                    var res = provider.PrincipalUpdate(
                        new PrincipalSetup()
                        {
                            Email = u.Email,
                            FirstName = u.FirstName,
                            LastName = u.LastName,
                            Password = System.Web.Security.Membership.GeneratePassword(8, 2),
                            Login = u.Login,
                            Type = PrincipalTypes.user
                        });
                    if (res.Success && res.Principal != null)
                    {
                        acUser = res.Principal;
                    }
                }

                var permission = MeetingPermissionId.view;
                var role = u.canvas_role != null ? u.canvas_role.ToLower() : string.Empty;

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
                    var res = provider.UpdateScoPermissionForPrincipal(
                        meetingScoId,
                        acUser.PrincipalId,
                        permission);
                }
            }

        }

        private MeetingDTO GetMeetingDTOByScoInfo(CompanyLms credentials, AdobeConnectProvider provider, LtiParamDTO param,
            ScoInfo result, IEnumerable<PermissionInfo> permission)
        {
            var bracketIndex = result.Name.IndexOf("]");
            var ret = new MeetingDTO()
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
                access_level = permission != null && permission.FirstOrDefault() != null ?
                permission.FirstOrDefault().PermissionId.ToString() : ""
            };

            ret.can_join = this.CanJoin(
                provider,
                result.ScoId,
                param.lis_person_contact_email_primary);
            ret.is_editable = this.CanEdit(param);

            return ret;
        }

        private void SetMeetingUpateItemFields(MeetingDTO meetingDTO, MeetingUpdateItem updateItem, string folderSco, string contextLabel, int courseId)
        {
            updateItem.Name = String.Format("{0} [{1}] {2}", courseId, contextLabel.Substring(0, Math.Min(contextLabel.Length, 10)), meetingDTO.name);
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
                    updateItem.DateEnd = dateBegin.AddMinutes((int)duration.TotalMinutes).ToString("yyyy-MM-ddTHH:mm:ssZ");
                }
            }
        }

        #endregion
    }
}
