namespace EdugameCloud.Web.App_Start
{
    using System.Web.Mvc;
    using System.Web.Routing;

    using EdugameCloud.MVC.Routes;

    /// <summary>
    /// The route config.
    /// </summary>
    public class RouteConfig
    {
        /// <summary>
        /// The register routes.
        /// </summary>
        /// <param name="routes">
        /// The routes.
        /// </param>
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            routes.IgnoreRoute("{*favicon}", new { favicon = @"(.*/)?favicon.ico(/.*)?" });
            routes.MapLowercaseRoute("Public", "public/{fileName}", new { controller = "File", action = "Public" });
            routes.MapLowercaseRoute("OAuthLogin", "social/{provider}-login", new { controller = "Social", action = "login" });
            routes.MapLowercaseRoute("OAuthCallback", "social/{provider}-callback", new { controller = "Social", action = "callback" });
            routes.MapLowercaseRoute("SubscriptionCallback", "social/{provider}-realtime", new { controller = "Social", action = "realtime-callback" });
            routes.MapLowercaseRoute("File", "file/{action}", new { controller = "File" });
            routes.MapLowercaseRoute("LtiOAuthLogin", "lti/{provider}-login", new { controller = "Lti", action = "login" });
            routes.MapLowercaseRoute("LtiOAuthCallback", "lti/oauth-callback", new { controller = "Lti", action = "callback" });
            routes.MapLowercaseRoute("Default", "{action}", new { controller = "Home", action = "Admin" });
            routes.MapLowercaseRoute("DefaultLti", "Lti", new { controller = "Lti", action = "Index" });
            routes.MapLowercaseRoute("DefaultLtitabbed", "Lti/tabbed", new { controller = "Lti", action = "Index" });

            routes.MapLowercaseRoute("getusers", "Lti/User/GetAll", new { controller = "Lti", action = "GetUsers" });
            routes.MapLowercaseRoute("updateuser", "Lti/User/Update", new { controller = "Lti", action = "UpdateUser" });

            routes.MapLowercaseRoute("getsetting", "Lti/Settings/Get", new { controller = "Lti", action = "GetSettings" });
            routes.MapLowercaseRoute("checkpass", "Lti/Settings/CheckPass", new { controller = "Lti", action = "CheckPasswordBeforeJoin" });
            routes.MapLowercaseRoute("savesettings", "Lti/Settings/Save", new { controller = "Lti", action = "SaveSettings" });
            routes.MapLowercaseRoute("getmeeting", "Lti/Meeting/Get", new { controller = "Lti", action = "GetMeeting" });
            routes.MapLowercaseRoute("setdefaults", "Lti/Meeting/SetDefaultACRoles", new { controller = "Lti", action = "SetDefaultRolesForNonParticipants" });
            routes.MapLowercaseRoute("getmeetingattendance", "Lti/Meeting/Attendance", new { controller = "Lti", action = "GetAttendanceReport" });
            routes.MapLowercaseRoute("getmeetingsessions", "Lti/Meeting/Sessions", new { controller = "Lti", action = "GetSessionsReport" });
            routes.MapLowercaseRoute("updatemeeting", "Lti/Meeting/Update", new { controller = "Lti", action = "UpdateMeeting" });
            routes.MapLowercaseRoute("joinmeeting", "Lti/Meeting/Join", new { controller = "Lti", action = "JoinMeeting" }); 
            routes.MapLowercaseRoute("getrecordings", "Lti/Recording/GetAll", new { controller = "Lti", action = "GetRecordings" });
            routes.MapLowercaseRoute("deleterecording", "Lti/Recording/Delete/{id}", new { controller = "Lti", action = "DeleteRecording", id = UrlParameter.Optional });
            routes.MapLowercaseRoute("joinrecording", "Lti/Recording/Join/{recordingUrl}", new { controller = "Lti", action = "JoinRecording", recordingUrl = UrlParameter.Optional });
            routes.MapLowercaseRoute("editrecording", "Lti/Recording/Edit/{recordingId}", new { controller = "Lti", action = "EditRecording", recordingId = UrlParameter.Optional });
            routes.MapLowercaseRoute("getrecordingflv", "Lti/Recording/GetFlv/{recordingUrl}", new { controller = "Lti", action = "GetRecordingFlv", recordingUrl = UrlParameter.Optional });
            routes.MapLowercaseRoute("updaterecording", "Lti/Recording/Share/{recordingUrl}", new { controller = "Lti", action = "ShareRecording", recordingUrl = UrlParameter.Optional });
            routes.MapLowercaseRoute("gettemplates", "Lti/Template/GetAll", new { controller = "Lti", action = "GetTemplates" }); 
            routes.MapLowercaseRoute("DefaultLtiAction", "Lti/{action}", new { controller = "Lti" });
        }
    }
}