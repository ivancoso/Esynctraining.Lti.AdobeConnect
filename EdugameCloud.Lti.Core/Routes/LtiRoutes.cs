namespace EdugameCloud.Lti.Core.Routes
{
    using System.Web.Mvc;
    using System.Web.Routing;

    /// <summary>
    /// The LTI routes.
    /// </summary>
    public static class LtiRoutes
    {
        /// <summary>
        /// The append to.
        /// </summary>
        /// <param name="routes">
        /// The routes.
        /// </param>
        public static void AppendTo(RouteCollection routes)
        {
            routes.MapLowercaseRoute("LtiOAuthLogin", "lti/{provider}-login", new { controller = "Lti", action = "login" });
            routes.MapLowercaseRoute("LtiOAuthCallback", "lti/oauth-callback", new { controller = "Lti", action = "callback" });

            routes.MapLowercaseRoute("DefaultLti", "Lti", new { controller = "Lti", action = "Index" });
            routes.MapLowercaseRoute("DefaultLtitabbed", "Lti/tabbed", new { controller = "Lti", action = "Index" });
            routes.MapLowercaseRoute("UserParameters", "Lti/GetAuthenticationParameters", new { controller = "Lti", action = "GetAuthenticationParameters" });

            routes.MapLowercaseRoute("scheduled", "scheduled-actions/{action}", new { controller = "LtiSchedule" });
            routes.MapLowercaseRoute("getusers", "Lti/User/GetAll", new { controller = "Lti", action = "GetUsers" });
            routes.MapLowercaseRoute("updateuser", "Lti/User/Update", new { controller = "Lti", action = "UpdateUser" });
            routes.MapLowercaseRoute("removefromacmeeting", "lti/user/removefrommeeting", new { controller = "Lti", action = "RemoveFromAcMeeting" });
            
            routes.MapLowercaseRoute("checkpass", "Lti/Settings/CheckPass", new { controller = "Lti", action = "CheckPasswordBeforeJoin" });
            routes.MapLowercaseRoute("savesettings", "Lti/Settings/Save", new { controller = "Lti", action = "SaveSettings" });
            routes.MapLowercaseRoute("leavemeeting", "Lti/Meeting/Leave", new { controller = "Lti", action = "LeaveMeeting" });
            routes.MapLowercaseRoute("setdefaults", "Lti/Meeting/SetDefaultACRoles", new { controller = "Lti", action = "SetDefaultRolesForNonParticipants" });
            routes.MapLowercaseRoute("getmeetingattendance", "Lti/Meeting/Attendance", new { controller = "Lti", action = "GetAttendanceReport" });
            routes.MapLowercaseRoute("getmeetingsessions", "Lti/Meeting/Sessions", new { controller = "Lti", action = "GetSessionsReport" });
            routes.MapLowercaseRoute("updatemeeting", "Lti/Meeting/Update", new { controller = "Lti", action = "UpdateMeeting" });
            routes.MapLowercaseRoute("updatemeetingreturnusers", "Lti/Meeting/UpdateAndReturnLmsUsers", new { controller = "Lti", action = "UpdateMeetingAndReturnLmsUsers" });
            routes.MapLowercaseRoute("deletemeeting", "Lti/Meeting/Delete", new { controller = "Lti", action = "DeleteMeeting" });
            routes.MapLowercaseRoute("joinmeeting", "Lti/Meeting/Join", new { controller = "Lti", action = "JoinMeeting" });
            routes.MapLowercaseRoute("joinmeetingmobile", "Lti/Meeting/JoinMobile", new { controller = "Lti", action = "JoinMeetingMobile" });

            routes.MapLowercaseRoute("getrecordings", "Lti/Recording/GetAll", new { controller = "LtiRecording", action = "GetRecordings" });
            routes.MapLowercaseRoute("deleterecording", "Lti/Recording/Delete/{id}", new { controller = "LtiRecording", action = "DeleteRecording", id = UrlParameter.Optional });
            routes.MapLowercaseRoute("converttoMP4", "Lti/Recording/MakeMP4", new { controller = "LtiRecording", action = "ConvertToMP4" });
            routes.MapLowercaseRoute("cancelMP4Converting", "Lti/Recording/CancelMP4Converting/{recordingId}", new { controller = "LtiRecording", action = "CancelMP4Converting" });
            routes.MapLowercaseRoute("joinrecording", "Lti/Recording/Join/{recordingUrl}", new { controller = "LtiRecording", action = "JoinRecording", recordingUrl = UrlParameter.Optional });
            routes.MapLowercaseRoute("editrecording", "Lti/Recording/Edit/{recordingId}", new { controller = "LtiRecording", action = "EditRecording", recordingId = UrlParameter.Optional });
            routes.MapLowercaseRoute("getrecordingflv", "Lti/Recording/GetFlv/{recordingUrl}", new { controller = "LtiRecording", action = "GetRecordingFlv", recordingUrl = UrlParameter.Optional });
            routes.MapLowercaseRoute("updaterecording", "Lti/Recording/Share/{recordingUrl}", new { controller = "LtiRecording", action = "ShareRecording", recordingUrl = UrlParameter.Optional });
            routes.MapLowercaseRoute("publishrecording", "lti/recording/publish", new { controller = "LtiRecording", action = "PublishRecording" });
            routes.MapLowercaseRoute("unpublishrecording", "lti/recording/unpublish", new { controller = "LtiRecording", action = "UnpublishRecording" });

            routes.MapLowercaseRoute("gettemplates", "Lti/Template/GetAll", new { controller = "Lti", action = "GetTemplates" });

            routes.MapLowercaseRoute("extjspage", "extjs/entry", new { controller = "Lti", action = "GetExtJsPage" });

            routes.MapLowercaseRoute("addNewAcUser", "lti/acNewUser", new { controller = "AcUser", action = "AddNewUser" });
            routes.MapLowercaseRoute("searchExistingAcUser", "lti/acSearchUser", new { controller = "AcUser", action = "SearchExistingUser" });

            routes.MapLowercaseRoute("searchExistingMeeting", "lti/acSearchMeeting", new { controller = "AcMeeting", action = "SearchExistingMeeting" });
            routes.MapLowercaseRoute("reuseAdobeConnectMeeting", "lti/useExistingMeeting", new { controller = "Lti", action = "ReuseExistedAdobeConnectMeeting" });
            
            routes.MapLowercaseRoute("register-proxy-tool", "lti/register-proxy-tool", new { controller = "LtiProxyTool", action = "register-proxy-tool" });

            routes.MapLowercaseRoute("DefaultLtiAction", "Lti/{action}", new { controller = "Lti" });            
        }

    }

}
