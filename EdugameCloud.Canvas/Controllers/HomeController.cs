namespace EdugameCloud.Canvas.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Globalization;
    using System.Linq;
    using System.Web;
    using System.Web.Mvc;
    using System.Web.UI;

    using EdugameCloud.Canvas.CanvasAPI;
    using EdugameCloud.Canvas.CanvasAPI.DTO;
    using EdugameCloud.Canvas.ViewModels;
    using EdugameCloud.Core.Business.Models;
    using EdugameCloud.Core.Domain.Entities;

    using Esynctraining.AC.Provider;
    using Esynctraining.AC.Provider.DataObjects;
    using Esynctraining.AC.Provider.DataObjects.Results;
    using Esynctraining.AC.Provider.Entities;
    using Esynctraining.Core.Extensions;
    using Esynctraining.Core.Providers;
    using Esynctraining.Core.Utils;

    using FluentNHibernate.Conventions.AcceptanceCriteria;
    using FluentNHibernate.Visitors;

    using Microsoft.Ajax.Utilities;

    [RequireHttps]
    public partial class HomeController : Canvas.Controllers.BaseController
    {
        private bool IsDebug
        {
            get
            {
                return ConfigurationManager.AppSettings["IsDebug"] != null;
            }
        }

        private CanvasConnectCredentials Credentials
        {
            get
            {
                var creds = Session["Credentials"] as CanvasConnectCredentials;

                if (creds == null && IsDebug)
                {
                    creds = canvasConnectCredentialsModel.GetOneByDomain("canvas.instructure.com").Value;
                }

                if (creds == null)
                {
                    this.RedirectToError("No integration settings were set for your application.");
                    return null;
                }

                return creds;
            }
        }

        private HomeViewModel Model
        {
            get
            {
                var model = Session["Model"] as HomeViewModel;

                if (model == null && IsDebug)
                {
                    model = new HomeViewModel()
                            {
                                custom_canvas_course_id = 865831 ,
                                lis_person_contact_email_primary = "no-email@test.com"
                            };
                }

                if (model == null)
                {
                    this.RedirectToError("You are not logged in.");
                    return null;
                }

                return model;
            }
        }

        private void RedirectToError(string errorText)
        {
            Response.Clear();
            Response.Write(String.Format("{{ \"error\": \"{0}\" }}", errorText));
            Response.End();
        }

        private CanvasCourseMeetingModel CanvasCourseMeetingModel
        {
            get
            {
                return IoC.Resolve<CanvasCourseMeetingModel>();
            }
        }

        private CanvasConnectCredentialsModel canvasConnectCredentialsModel
        {
            get
            {
                return IoC.Resolve<CanvasConnectCredentialsModel>();
            }
        }

        public HomeController(ApplicationSettingsProvider settings)
            : base(settings)
        {

        }

        public HomeController() : base(null)
        {
            
        }

        private AdobeConnectProvider GetProvider()
        {
            var connectionDetails = new ConnectionDetails()
            {
                ServiceUrl = Credentials.ACDomain + (Credentials.ACDomain.EndsWith("/") ? string.Empty : "/")
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
            provider.Login(new UserCredentials(Credentials.ACUsername, Credentials.ACPassword));

            return provider;
        }
        /*
        private void SaveMeeting(AdobeConnectProvider provider)
        {
            var credentials = Session["Credentials"] as CanvasConnectCredentials;
            if (credentials == null)
                throw new Exception("No Adobe Connect credentials found");

            this.CanvasCourseMeetingModel.Flush();
            var currentMeeting = this.CanvasCourseMeetingModel.GetOneByCourseId()
            var currentMeetingScoId = currentMeeting != null ? currentMeeting.ScoId : string.Empty;
            model.MeetingScoId = currentMeetingScoId;

            if (!string.IsNullOrEmpty(currentMeetingScoId))
            {
                var currentMeetingSco = provider.GetScoContent(currentMeetingScoId).ScoContent;
                if (currentMeetingSco != null)
                {
                    model.MeetingName = currentMeetingSco.Name;
                    model.MeetingUrl =  (model.ACDomain.EndsWith("/") ? model.ACDomain.Substring(0, model.ACDomain.Length - 1) : model.ACDomain) 
                        + currentMeetingSco.UrlPath;
                }
            }

            var meetings = provider.GetContentsByScoId(model.Params["custom_ac_folder_sco_id"]);
            model.Meetings = meetings.Values.ToList(); 
        }

        */


        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")]
        public virtual void Index(HomeViewModel model)
        {
            var credentials = canvasConnectCredentialsModel.GetOneByDomain(model.custom_canvas_api_domain).Value;
            if (credentials != null)
            {
                Session["Model"] = model;
                Session["Credentials"] = credentials;
            }
            else if (!IsDebug)
            {
                this.RedirectToError("Cannot log in.");
                return;
            }
            
            Response.Redirect("/extjs/index.html"); //this.View("Index", model);
        }

        public virtual ActionResult JoinMeeting()
        {
            string breezeToken = string.Empty, meetingUrl = string.Empty;

            var provider = this.GetProvider();

            this.CanvasCourseMeetingModel.Flush();
            var currentMeeting = this.CanvasCourseMeetingModel.GetOneByCourseId(Credentials.Id, Model.custom_canvas_course_id).Value;

            var currentMeetingScoId = currentMeeting != null ? currentMeeting.ScoId : string.Empty;
            
            if (!string.IsNullOrEmpty(currentMeetingScoId))
            {
                var currentMeetingSco = provider.GetScoContent(currentMeetingScoId).ScoContent;
                if (currentMeetingSco != null)
                {
                    meetingUrl = (Credentials.ACDomain.EndsWith("/") ? Credentials.ACDomain.Substring(0, Credentials.ACDomain.Length - 1) : Credentials.ACDomain)
                        + currentMeetingSco.UrlPath;
                }
            }

            var email = Model.lis_person_contact_email_primary ?? "test@test.com";

            var password = email != Credentials.ACUsername ? 
                System.Web.Security.Membership.GeneratePassword(8, 2) : 
                Credentials.ACPassword;
                
            var registeredUser = provider.GetAllByEmail(HttpUtility.UrlEncode(email));

            if (registeredUser.Success && registeredUser.Values != null && registeredUser.Values.Any())
            {
                var principalId = registeredUser.Values.First().PrincipalId;
                if (email != Credentials.ACUsername)
                {
                    provider.PrincipalUpdatePassword(principalId, password);
                }

                provider.PrincipalUpdate(new PrincipalSetup()
                                         {
                                             PrincipalId = principalId,
                                             FirstName = Model.lis_person_name_given,
                                             LastName = Model.lis_person_name_family,
                                             Name = Model.lis_person_name_full
                                         });

                breezeToken = provider.Login(new UserCredentials(HttpUtility.UrlEncode(email), password)).Status.SessionInfo;
            }
            else
            {
                return Json(Model.launch_presentation_return_url);
            }

            return this.Redirect(meetingUrl + "?session=" + breezeToken);
        }
        

        [HttpPost]
        public JsonResult GetUsers()
        {
            var users = CourseAPI.GetUsersForCourse(Credentials.CanvasDomain, Credentials.CanvasToken, Model.custom_canvas_course_id);

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
            
            var meeting = this.CanvasCourseMeetingModel.GetOneByCourseId(Credentials.Id, Model.custom_canvas_course_id).Value;
            if (meeting == null)
            {
                return this.Json(users);
            }
            
            var provider = this.GetProvider();

            PermissionCollectionResult hosts = provider.GetMeetingHosts(meeting.ScoId),
                participants = provider.GetMeetingParticipants(meeting.ScoId),
                presenters = provider.GetMeetingPresenters(meeting.ScoId);

            foreach (var user in users)
            {
                if (user.login_id == null && user.email == null)
                {
                    //user.ac_role = "not_allowed";
                    continue;
                };

                var acUser = provider.GetAllByEmail(HttpUtility.UrlEncode(user.login_id ?? user.email));
                if (acUser.Values == null || !acUser.Values.Any())
                {
                    //user.ac_role = "not_allowed";
                    continue;
                }

                user.ac_id = acUser.Values.First().PrincipalId;

                if (hosts.Values != null && hosts.Values.Any(v => v.PrincipalId == user.ac_id))
                {
                    user.ac_role = "Host";
                    hosts.Values = hosts.Values.Where(v => v.PrincipalId != user.ac_id).ToList();
                }
                else if (presenters.Values != null && presenters.Values.Any(v => v.PrincipalId == user.ac_id))
                {
                    user.ac_role = "Presenter";
                    presenters.Values = presenters.Values.Where(v => v.PrincipalId != user.ac_id).ToList();
                }
                else if (participants.Values != null && participants.Values.Any(v => v.PrincipalId == user.ac_id))
                {
                    user.ac_role = "Participant";
                    participants.Values = participants.Values.Where(v => v.PrincipalId != user.ac_id).ToList();
                }
            }

            foreach (var u in hosts.Values)
            {
                users.Add(new UserDTO()
                          {
                              ac_id = u.PrincipalId,
                              name = u.Name,
                              ac_role = "Host"
                          });
            }

            foreach (var u in presenters.Values)
            {
                users.Add(new UserDTO()
                {
                    ac_id = u.PrincipalId,
                    name = u.Name,
                    ac_role = "Presenter"
                });
            }

            foreach (var u in participants.Values)
            {
                users.Add(new UserDTO()
                {
                    ac_id = u.PrincipalId,
                    name = u.Name,
                    ac_role = "Participant"
                });
            }


            return Json(users);
        }

        [HttpPost]
        public JsonResult UpdateUser(UserDTO user)
        {
            var provider = this.GetProvider();
            
            var meeting = this.CanvasCourseMeetingModel.GetOneByCourseId(Credentials.Id, Model.custom_canvas_course_id).Value;
            if (meeting == null)
            {
                return this.Json(user);
            }

            if (user.ac_id == null || user.ac_id == "0")
            {
                var acUser = provider.GetAllByEmail(HttpUtility.UrlEncode(user.login_id ?? user.email));
                if (acUser.Values == null || acUser.Values.Count() == 0)
                {
                    var setup = new PrincipalSetup()
                                {
                                    Email = user.login_id ?? user.email,
                                    FirstName = HttpUtility.UrlEncode(user.name),
                                    LastName = HttpUtility.UrlEncode(user.name),
                                    Name = HttpUtility.UrlEncode(user.name),
                                    Password = System.Web.Security.Membership.GeneratePassword(8, 2),
                                    Login = user.login_id ?? user.email
                                };
                    var pu = provider.PrincipalUpdate(setup);
                    if (pu.Principal != null)
                    {
                        user.ac_id = pu.Principal.PrincipalId;
                    }
                }
                else
                {
                    user.ac_id = acUser.Values.First().PrincipalId;
                }
                
            }

            if (user.ac_role == null)
            {
                provider.UpdateScoPermissionForPrincipal(meeting.ScoId, user.ac_id, MeetingPermissionId.remove);
                return Json(user);
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
            
            return Json(user);
        }

        [HttpPost]
        public JsonResult GetMeeting()
        {
            var provider = this.GetProvider();

            var meeting = this.GetMeeting(provider);

            return Json(meeting);
        }

        [HttpPost]
        public JsonResult GetRecordings()
        {
            var provider = this.GetProvider();

            var recordings = this.GetRecordings(Credentials, Model.custom_canvas_course_id, provider);

            return Json(recordings);
        }

        [HttpPost]
        public JsonResult GetTemplates()
        {
            var provider = this.GetProvider();

            var templates = this.GetTemplates(Credentials, Model.custom_canvas_course_id, provider);

            return Json(templates);
        }

        [HttpPost]
        public JsonResult UpdateMeeting(MeetingDTO meeting)
        {
            var provider = this.GetProvider();

            var ret = this.SaveMeeting(Credentials, Model.custom_canvas_course_id, provider, meeting);

            var canEdit = Model.roles != null
                          && (Model.roles.IndexOf("Instructor") > -1
                              || Model.roles.IndexOf("Administrator") > -1);
            
            ret.is_editable = canEdit;

            var registeredUser = provider.GetAllByEmail(HttpUtility.UrlEncode(Model.lis_person_contact_email_primary));
            if (registeredUser.Values != null && registeredUser.Values.Any())
            {
                var principalId = registeredUser.Values.First().PrincipalId;
                IEnumerable<PermissionInfo> hosts = provider.GetMeetingHosts(meeting.id).Values,
                    participants = provider.GetMeetingParticipants(meeting.id).Values,
                    presenters = provider.GetMeetingPresenters(meeting.id).Values;
                if ((hosts != null && hosts.Any(h => h.PrincipalId == principalId))
                    || (participants != null && participants.Any(h => h.PrincipalId == principalId))
                    || (presenters != null && presenters.Any(h => h.PrincipalId == principalId)))
                {
                    ret.can_join = true;
                }

            }

            return Json(ret);
        }

        private List<RecordingDTO> GetRecordings(CanvasConnectCredentials credentials, int courseId, AdobeConnectProvider provider)
        {
            var meeting = this.CanvasCourseMeetingModel.GetOneByCourseId(credentials.Id, courseId).Value;

            if (meeting == null) return null;

            var result = provider.GetMeetingRecordings(new string[] { meeting.ScoId });
            return result.Values.Select(v => new RecordingDTO()
                                             {
                                                 name = v.Name,
                                                 description = v.Description,
                                                 begin_date = v.BeginDate.ToString("MM-dd-yyyy h:mm:ss tt"),
                                                 end_date = v.EndDate.ToString("MM-dd-yyyy h:mm:ss tt"),
                                                 duration = v.Duration,
                                                 url = Credentials.ACDomain + v.UrlPath.Trim("/".ToCharArray())
                                             }).ToList();
        }

        private List<TemplateDTO> GetTemplates(CanvasConnectCredentials credentials, int courseId, AdobeConnectProvider provider)
        {
            var result = provider.GetContentsByScoId("11044");
            return result.Values.Select(v => new TemplateDTO()
            {
                id = v.ScoId,
                name = v.Name
            }).ToList();
        }

        private MeetingDTO GetMeeting(AdobeConnectProvider provider)
        {
            var meeting = this.CanvasCourseMeetingModel.GetOneByCourseId(Credentials.Id, Model.custom_canvas_course_id).Value;

            var canEdit = Model.roles != null
                          && (Model.roles.Contains("Instructor")
                              || Model.roles.Contains("Administrator"));

            if (meeting == null) return 
                new MeetingDTO()
                {
                    id = "0",
                    connect_server = Credentials.ACDomain,
                    is_editable = canEdit
                };

            var ret = this.GetMeetingDTOByScoId(meeting.ScoId, provider);

            if (ret.id == "0")
            {
                CanvasCourseMeetingModel.RegisterDelete(meeting);
                CanvasCourseMeetingModel.Flush();
            }
            else
            {
                var registeredUser = provider.GetAllByEmail(HttpUtility.UrlEncode(Model.lis_person_contact_email_primary));
                if (registeredUser.Values != null && registeredUser.Values.Any())
                {
                    var principalId = registeredUser.Values.First().PrincipalId;
                    IEnumerable<PermissionInfo> hosts = provider.GetMeetingHosts(meeting.ScoId).Values,
                        participants = provider.GetMeetingParticipants(meeting.ScoId).Values,
                        presenters = provider.GetMeetingPresenters(meeting.ScoId).Values;
                    if ((hosts != null && hosts.Any(h => h.PrincipalId == principalId))
                        || (participants != null && participants.Any(h => h.PrincipalId == principalId))
                        || (presenters != null && presenters.Any(h => h.PrincipalId == principalId)))
                    {
                        ret.can_join = true;
                    }

                }
            }

            ret.is_editable = canEdit;
            return ret;
        }

        private MeetingDTO GetMeetingDTOByScoInfo(ScoInfo result, IEnumerable<PermissionInfo> permission)
        {
            return new MeetingDTO()
            {
                id = result.ScoId,
                ac_room_url = result.UrlPath.Trim("/".ToCharArray()),
                name = result.Name.Substring(result.Name.IndexOf("]") < 0 ? 0 : result.Name.IndexOf("]") + 2),
                summary = result.Description,
                template = result.SourceScoId,
                start_date = result.BeginDate.ToString("yyyy-MM-dd"),
                start_time = result.BeginDate.ToString("h:mm tt", CultureInfo.InvariantCulture),
                duration = (result.EndDate - result.BeginDate).ToString(@"h\:mm"),
                connect_server = Credentials.ACDomain,
                access_level = permission != null && permission.FirstOrDefault() != null ?
                 permission.FirstOrDefault().PermissionId.ToString() : ""
            };
        }

        private MeetingDTO GetMeetingDTOByScoId(string scoId, AdobeConnectProvider provider)
        {
            var result = provider.GetScoInfo(scoId).ScoInfo;
            if (result == null) return
                new MeetingDTO()
                {
                    id = "0",
                    connect_server = Credentials.ACDomain
                };

            var permission = provider.GetScoPublicAccessPermissions(scoId).Values;
            return this.GetMeetingDTOByScoInfo(result, permission);
        }

        private void SetDefaultUsers(List<UserDTO> users, AdobeConnectProvider provider, string meetingScoId)
        {
            foreach (var ugroup in users.GroupBy(u => u.id))
            {
                var u = ugroup.First();
                if (u.login_id == null && u.email == null) continue;

                var acUser = provider.GetAllByEmail(HttpUtility.UrlEncode(u.login_id ?? u.email));
                if (acUser.Values.Count() == 0)
                {
                    var res = provider.PrincipalUpdate(
                        new PrincipalSetup()
                        {
                            Email = u.login_id ?? u.email,
                            FirstName = u.name,
                            LastName = u.name,
                            Password = System.Web.Security.Membership.GeneratePassword(8, 2),
                            Login = u.login_id ?? u.email
                        });
                    acUser = provider.GetAllByEmail(HttpUtility.UrlEncode(u.login_id ?? u.email));
                }

                var permission = MeetingPermissionId.view;
                var roles = ugroup.Where(ug => ug.canvas_role != null).Select(ug => ug.canvas_role.ToLower()).ToList();

                if (roles.Contains("teacher"))
                {
                    permission = MeetingPermissionId.host;
                }
                else if (roles.Contains("ta") || roles.Contains("designer"))
                {
                    permission = MeetingPermissionId.mini_host;
                }

                if (acUser.Values != null)
                {
                    foreach (var val in acUser.Values)
                    {
                        var res = provider.UpdateScoPermissionForPrincipal(
                            meetingScoId,
                            val.PrincipalId,
                            permission);                        
                    }
                }
            }
            
        }

        private MeetingDTO SaveMeeting(CanvasConnectCredentials credentials, int courseId, AdobeConnectProvider provider, MeetingDTO meetingDTO)
        {
            var meeting = this.CanvasCourseMeetingModel.GetOneByCourseId(credentials.Id, courseId).Value
                ?? new CanvasCourseMeeting() { CanvasConnectCredentialsId = credentials.Id, CourseId = courseId };
            
            var meetingItem = new MeetingUpdateItem(){ ScoId = meeting.ScoId };
            //meetingItem.DateBegin = DateTime.Now.ToString();
            //meetingItem.DateEnd = DateTime.Now.AddDays(1).ToString();
            meetingItem.Name = String.Format("{0} [{1}] {2}", DateTime.Now.ToString("MM.dd.yy "), Model.context_label, meetingDTO.name);
            meetingItem.Description = meetingDTO.summary;
            meetingItem.UrlPath = meetingDTO.ac_room_url;

            DateTime startDate, startTime, dateBegin;
            /*
            if (DateTime.TryParse(meetingDTO.start_date, out startDate)
                && DateTime.TryParse(meetingDTO.start_time, out startTime))
            */
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
            if (DateTime.TryParse(meetingDTO.start_date + " " + meetingDTO.start_time, out dateBegin))
            {
                /*
                var dateBegin = new DateTime(
                    startDate.Year,
                    startDate.Month,
                    startDate.Day,
                    startTime.Hour,
                    startTime.Minute,
                    startTime.Second);
                */
                meetingItem.DateBegin = dateBegin.ToString("yyyy-MM-ddTHH:mm:ssZ");
                TimeSpan duration;
                if (TimeSpan.TryParse(meetingDTO.duration, out duration))
                {
                    meetingItem.DateEnd = dateBegin.AddMinutes((int)duration.TotalMinutes).ToString("yyyy-MM-ddTHH:mm:ssZ");                    
                }
            }
            
            meetingItem.FolderId = credentials.ACScoId;
            meetingItem.Language = "en";
            meetingItem.Type = ScoType.meeting;
            meetingItem.SourceScoId = meetingDTO.template;
                
            var result = meeting.ScoId != null ?
                provider.UpdateSco(meetingItem) :
                provider.CreateSco(meetingItem);

            if (result.ScoInfo != null)
            {
                meetingDTO.id = result.ScoInfo.ScoId;
            }

            if (meetingItem.ScoId == null && result.ScoInfo != null)
            {
                meeting.ScoId = result.ScoInfo.ScoId;
                this.CanvasCourseMeetingModel.RegisterSave(meeting);

                var users = CourseAPI.GetUsersForCourse(Credentials.CanvasDomain, credentials.CanvasToken, courseId);
                this.SetDefaultUsers(users, provider, result.ScoInfo.ScoId);

                if (Credentials.CanvasDomain.IndexOf("canvas") > -1 && !String.IsNullOrEmpty(Model.context_title))
                {
                    var rets = CourseAPI.CreateAnnouncement(
                        Credentials.CanvasDomain,
                        Credentials.CanvasToken,
                        Model.custom_canvas_course_id,
                        String.Format("A new Adobe Connect room was created for course {0}", Model.context_title),
                        String.Format("Meeting \"{0}\" will start {1} at {2}. It's duration will be {3}. You can join it in your Adobe Connect Conference section.",
                            meetingDTO.name, oldStartDate, meetingDTO.start_time, meetingDTO.duration));
                }
            }

            if (result.ScoInfo != null)
            {
                var specialPermissionId = meetingDTO.access_level == "denied"
                    ? SpecialPermissionId.denied
                    : (meetingDTO.access_level == "view_hidden"
                        ? SpecialPermissionId.view_hidden
                        : SpecialPermissionId.remove);

                provider.UpdatePublicAccessPermissions(result.ScoInfo.ScoId, specialPermissionId);
                var permission = provider.GetScoPublicAccessPermissions(result.ScoInfo.ScoId).Values;

                return this.GetMeetingDTOByScoInfo(result.ScoInfo, permission);
            }
            else
            {
                return new MeetingDTO() { id = "0", connect_server = Credentials.ACDomain };
            }

        }
    }
}