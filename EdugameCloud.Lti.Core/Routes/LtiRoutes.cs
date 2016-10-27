namespace EdugameCloud.Lti.Core.Routes
{
    using System.Web.Mvc;
    using System.Web.Routing;

    /// <summary>
    /// The LTI routes.
    /// </summary>
    public static class LtiRoutes
    {
        //public static void AppendTo(RouteCollection routes)
        //{
        //    routes.MapLowercaseRoute("LtiOAuthLogin", "lti/{provider}-login", new { controller = "Lti", action = "login" });
        //    routes.MapLowercaseRoute("LtiOAuthCallback", "lti/oauth-callback", new { controller = "Lti", action = "callback" });

        //    routes.MapLowercaseRoute("DefaultLti", "Lti", new { controller = "Lti", action = "Index" });
        //    routes.MapLowercaseRoute("DefaultLtitabbed", "Lti/tabbed", new { controller = "Lti", action = "Index" });
        //    routes.MapLowercaseRoute("UserParameters", "Lti/GetAuthenticationParameters", new { controller = "Lti", action = "GetAuthenticationParameters" });

        //    routes.MapLowercaseRoute("scheduled", "scheduled-actions/{action}", new { controller = "LtiSchedule" });
        //    routes.MapLowercaseRoute("getusers", "lti/users", new { controller = "Lti", action = "GetUsers" });
        //    routes.MapLowercaseRoute("updateuser", "lti/users/update", new { controller = "Lti", action = "UpdateUser" });
        //    routes.MapLowercaseRoute("removefromacmeeting", "lti/users/removefrommeeting", new { controller = "Lti", action = "RemoveFromAcMeeting" });
            
        //    routes.MapLowercaseRoute("checkpass", "lti/settings/checkpass", new { controller = "Lti", action = "CheckPasswordBeforeJoin" });
        //    routes.MapLowercaseRoute("savesettings", "lti/settings/save", new { controller = "Lti", action = "SaveSettings" });
        //    routes.MapLowercaseRoute("leavemeeting", "lti/Meeting/Leave", new { controller = "Lti", action = "LeaveMeeting" });
        //    routes.MapLowercaseRoute("setdefaults", "lti/Meeting/SetDefaultACRoles", new { controller = "Lti", action = "SetDefaultRolesForNonParticipants" });
        //    routes.MapLowercaseRoute("getmeetingattendance", "Lti/Meeting/Attendance", new { controller = "LtiReport", action = "GetAttendanceReport" });
        //    routes.MapLowercaseRoute("getmeetingsessions", "Lti/Meeting/Sessions", new { controller = "LtiReport", action = "GetSessionsReport" });
        //    routes.MapLowercaseRoute("getmeetingrecordings", "lti/meeting/reports/by-recordings", new { controller = "LtiReport", action = "GetRecordingsReport" });

        //    routes.MapLowercaseRoute("updatemeeting", "Lti/Meeting/Update", new { controller = "Lti", action = "UpdateMeeting" });
        //    routes.MapLowercaseRoute("updatemeetingreturnusers", "Lti/Meeting/UpdateAndReturnLmsUsers", new { controller = "Lti", action = "UpdateMeetingAndReturnLmsUsers" });
        //    routes.MapLowercaseRoute("deletemeeting", "Lti/Meeting/Delete", new { controller = "Lti", action = "DeleteMeeting" });
        //    routes.MapLowercaseRoute("joinmeeting", "Lti/Meeting/Join", new { controller = "Lti", action = "JoinMeeting" });
        //    routes.MapLowercaseRoute("joinmeetingmobile", "Lti/Meeting/JoinMobile", new { controller = "Lti", action = "JoinMeetingMobile" });

        //    routes.MapLowercaseRoute("getrecordings", "lti/recordings", new { controller = "LtiRecording", action = "GetRecordings" });
        //    routes.MapLowercaseRoute("editrecording2", "lti/recordings/edit/{id}", new { controller = "LtiRecording", action = "EditRecording", id = UrlParameter.Optional },
        //        new { httpMethod = new HttpMethodConstraint("POST") });
        //    routes.MapLowercaseRoute("deleterecording", "lti/recordings/delete/{id}", new { controller = "LtiRecording", action = "DeleteRecording", id = UrlParameter.Optional });

        //    routes.MapLowercaseRoute("converttoMP4", "lti/recordings/MakeMP4", new { controller = "LtiRecording", action = "ConvertToMP4" });

        //    routes.MapLowercaseRoute("cancelMP4Converting", "lti/recordings/CancelMP4Converting/{recordingId}", new { controller = "LtiRecording", action = "CancelMP4Converting" });
        //    routes.MapLowercaseRoute("joinrecording", "lti/recordings/join/{recordingUrl}", new { controller = "LtiRecording", action = "JoinRecording", recordingUrl = UrlParameter.Optional });
        //    routes.MapLowercaseRoute("editrecording", "lti/recordings/edit/{recordingId}", new { controller = "LtiRecording", action = "EditRecording", recordingId = UrlParameter.Optional });
        //    routes.MapLowercaseRoute("getrecordingflv", "lti/recordings/GetFlv/{recordingUrl}", new { controller = "LtiRecording", action = "GetRecordingFlv", recordingUrl = UrlParameter.Optional });
        //    routes.MapLowercaseRoute("updaterecording", "lti/recordings/share/{recordingUrl}", new { controller = "LtiRecording", action = "ShareRecording", recordingUrl = UrlParameter.Optional });
        //    routes.MapLowercaseRoute("publishrecording", "lti/recordings/publish", new { controller = "LtiRecording", action = "PublishRecording" });
        //    routes.MapLowercaseRoute("unpublishrecording", "lti/recordings/unpublish", new { controller = "LtiRecording", action = "UnpublishRecording" });

        //    routes.MapLowercaseRoute("gettemplates", "lti/templates", new { controller = "AcTemplate", action = "GetTemplates" });

        //    routes.MapLowercaseRoute("extjspage", "extjs/entry", new { controller = "Lti", action = "GetExtJsPage" });

        //    routes.MapLowercaseRoute("addNewAcUser", "lti/acNewUser", new { controller = "AcUser", action = "AddNewUser" });
        //    routes.MapLowercaseRoute("searchExistingAcUser", "lti/acSearchUser", new { controller = "AcUser", action = "SearchExistingUser" });

        //    routes.MapLowercaseRoute("searchExistingMeeting", "lti/acSearchMeeting", new { controller = "AcMeeting", action = "SearchExistingMeeting" });
        //    routes.MapLowercaseRoute("reuseAdobeConnectMeeting", "lti/useExistingMeeting", new { controller = "Lti", action = "ReuseExistedAdobeConnectMeeting" });
            
        //    routes.MapLowercaseRoute("register-proxy-tool", "lti/register-proxy-tool", new { controller = "LtiProxyTool", action = "register-proxy-tool" });


        //    routes.MapLowercaseRoute("seminarsAll", "lti/seminars", new { controller = "Seminar", action = "GetAll" });
        //    routes.MapLowercaseRoute("seminarsCreate", "lti/seminars/create", new { controller = "Seminar", action = "Create" });
        //    routes.MapLowercaseRoute("seminarsEdit", "lti/seminars/edit", new { controller = "Seminar", action = "Edit" });

        //    routes.MapLowercaseRoute("seminarsSessionCreate", "lti/seminars/sessions/create", new { controller = "Seminar", action = "SaveSeminarSession" });
        //    routes.MapLowercaseRoute("seminarsSessionEdit", "lti/seminars/sessions/edit", new { controller = "Seminar", action = "SaveSeminarSession" });
        //    routes.MapLowercaseRoute("seminarsSessionDelete", "lti/seminars/sessions/delete", new { controller = "Seminar", action = "DeleteSeminarSession" });
        //    routes.MapLowercaseRoute("calendarCreateBatch", "lti/calendar/createbatch", new { controller = "Calendar", action = "CreateBatch" });
        //    routes.MapLowercaseRoute("calendarGetEvents", "lti/calendar/getevents", new { controller = "Calendar", action = "GetEvents" });
        //    routes.MapLowercaseRoute("calendarCreateEvent", "lti/calendar/createevent", new { controller = "Calendar", action = "CreateEvent" });
        //    routes.MapLowercaseRoute("calendarSaveEvent", "lti/calendar/saveevent", new { controller = "Calendar", action = "SaveEvent" });
        //    routes.MapLowercaseRoute("calendarDeleteEvent", "lti/calendar/deleteevent", new { controller = "Calendar", action = "DeleteEvent" });

        //    routes.MapLowercaseRoute("DefaultLtiAction", "lti/{action}", new { controller = "Lti" });

        //}

        public static void AppendTo2(RouteCollection routes)
        {
            routes.MapLowercaseRoute("LtiOAuthLogin", "{provider}-login", new { controller = "Lti", action = "login" });
            routes.MapLowercaseRoute("LtiOAuthCallback", "oauth-callback", new { controller = "Lti", action = "callback" });
            routes.MapLowercaseRoute("ImsLogin", "ims", new { controller = "Lti", action = "ims" });
            routes.MapLowercaseRoute("Outcomes", "outcomes", new { controller = "Lti", action = "outcomes" });

            // ??? routes.MapLowercaseRoute("DefaultLti", "", new { controller = "Lti", action = "Index" });
            // ??? routes.MapLowercaseRoute("DefaultLtitabbed", "Lti/tabbed", new { controller = "Lti", action = "Index" });
            // ??? routes.MapLowercaseRoute("UserParameters", "Lti/GetAuthenticationParameters", new { controller = "Lti", action = "GetAuthenticationParameters" });

            routes.MapLowercaseRoute("scheduled", "scheduled-actions/{action}", new { controller = "LtiSchedule" });
            routes.MapLowercaseRoute("getusers", "users", new { controller = "Lti", action = "GetUsers" });
            routes.MapLowercaseRoute("updateuser", "users/update", new { controller = "Lti", action = "UpdateUser" });
            routes.MapLowercaseRoute("removefromacmeeting", "users/removefrommeeting", new { controller = "Lti", action = "RemoveFromAcMeeting" });

            routes.MapLowercaseRoute("checkpass", "settings/checkpass", new { controller = "Lti", action = "CheckPasswordBeforeJoin" });
            routes.MapLowercaseRoute("savesettings", "settings/save", new { controller = "Lti", action = "SaveSettings" });
            routes.MapLowercaseRoute("leavemeeting", "meeting/Leave", new { controller = "Lti", action = "LeaveMeeting" });
            routes.MapLowercaseRoute("setdefaults", "meeting/SetDefaultACRoles", new { controller = "Lti", action = "SetDefaultRolesForNonParticipants" });
            routes.MapLowercaseRoute("getmeetingattendance", "meeting/attendance", new { controller = "LtiReport", action = "GetAttendanceReport" });
            routes.MapLowercaseRoute("getmeetingsessions", "meeting/sessions", new { controller = "LtiReport", action = "GetSessionsReport" });
            routes.MapLowercaseRoute("getmeetingrecordings", "meeting/reports/by-recordings", new { controller = "LtiReport", action = "GetRecordingsReport" });

            routes.MapLowercaseRoute("updatemeeting", "meeting/update", new { controller = "Lti", action = "UpdateMeeting" });
            routes.MapLowercaseRoute("updatemeetingreturnusers", "meeting/UpdateAndReturnLmsUsers", new { controller = "Lti", action = "UpdateMeetingAndReturnLmsUsers" });
            routes.MapLowercaseRoute("deletemeeting", "meeting/delete", new { controller = "Lti", action = "DeleteMeeting" });
            routes.MapLowercaseRoute("joinmeeting", "meeting/join", new { controller = "Lti", action = "JoinMeeting" });
            routes.MapLowercaseRoute("joinmeetingmobile", "meeting/JoinMobile", new { controller = "Lti", action = "JoinMeetingMobile" });

            routes.MapLowercaseRoute("getrecordings", "recordings", new { controller = "LtiRecording", action = "GetRecordings" });
            routes.MapLowercaseRoute("editrecording2", "recordings/edit/{id}", new { controller = "LtiRecording", action = "EditRecording", id = UrlParameter.Optional },
                new { httpMethod = new HttpMethodConstraint("POST") });
            routes.MapLowercaseRoute("deleterecording", "recordings/delete/{id}", new { controller = "LtiRecording", action = "DeleteRecording", id = UrlParameter.Optional });

            routes.MapLowercaseRoute("converttoMP4", "recordings/MakeMP4", new { controller = "LtiRecording", action = "ConvertToMP4" });

            routes.MapLowercaseRoute("cancelMP4Converting", "recordings/CancelMP4Converting/{recordingId}", new { controller = "LtiRecording", action = "CancelMP4Converting" });
            routes.MapLowercaseRoute("joinrecording", "recordings/join/{recordingUrl}", new { controller = "LtiRecording", action = "JoinRecording", recordingUrl = UrlParameter.Optional });
            routes.MapLowercaseRoute("editrecording", "recordings/edit/{recordingId}", new { controller = "LtiRecording", action = "EditRecording", recordingId = UrlParameter.Optional });
            routes.MapLowercaseRoute("getrecordingflv", "recordings/GetFlv/{recordingUrl}", new { controller = "LtiRecording", action = "GetRecordingFlv", recordingUrl = UrlParameter.Optional });
            routes.MapLowercaseRoute("updaterecording", "recordings/share/{recordingUrl}", new { controller = "LtiRecording", action = "ShareRecording", recordingUrl = UrlParameter.Optional });
            routes.MapLowercaseRoute("publishrecording", "recordings/publish", new { controller = "LtiRecording", action = "PublishRecording" });
            routes.MapLowercaseRoute("unpublishrecording", "recordings/unpublish", new { controller = "LtiRecording", action = "UnpublishRecording" });

            routes.MapLowercaseRoute("gettemplates", "templates", new { controller = "AcTemplate", action = "GetTemplates" });

            routes.MapLowercaseRoute("extjspage", "extjs-entry", new { controller = "Lti", action = "GetExtJsPage" });

            routes.MapLowercaseRoute("addNewAcUser", "acNewUser", new { controller = "AcUser", action = "AddNewUser" });
            routes.MapLowercaseRoute("searchExistingAcUser", "acSearchUser", new { controller = "AcUser", action = "SearchExistingUser" });
            routes.MapLowercaseRoute("searchExistingMeeting", "acSearchMeeting", new { controller = "AcMeeting", action = "SearchExistingMeeting" });
            routes.MapLowercaseRoute("reuseAdobeConnectMeeting", "useExistingMeeting", new { controller = "Lti", action = "ReuseExistedAdobeConnectMeeting" });

            routes.MapLowercaseRoute("register-proxy-tool", "register-proxy-tool", new { controller = "LtiProxyTool", action = "register-proxy-tool" });
            
            routes.MapLowercaseRoute("seminarsAll", "seminars", new { controller = "Seminar", action = "GetAll" });
            routes.MapLowercaseRoute("seminarsCreate", "seminars/create", new { controller = "Seminar", action = "Create" });
            routes.MapLowercaseRoute("seminarsEdit", "seminars/edit", new { controller = "Seminar", action = "Edit" });
            routes.MapLowercaseRoute("seminarsSessionCreate", "seminars/sessions/create", new { controller = "Seminar", action = "SaveSeminarSession" });
            routes.MapLowercaseRoute("seminarsSessionEdit", "seminars/sessions/edit", new { controller = "Seminar", action = "SaveSeminarSession" });
            routes.MapLowercaseRoute("seminarsSessionDelete", "seminars/sessions/delete", new { controller = "Seminar", action = "DeleteSeminarSession" });

            routes.MapLowercaseRoute("calendarCreateBatch", "calendar/createbatch", new { controller = "Calendar", action = "CreateBatch" });
            routes.MapLowercaseRoute("calendarGetEvents", "calendar/getevents", new { controller = "Calendar", action = "GetEvents" });
            routes.MapLowercaseRoute("calendarCreateEvent", "calendar/createevent", new { controller = "Calendar", action = "CreateEvent" });
            routes.MapLowercaseRoute("calendarSaveEvent", "calendar/saveevent", new { controller = "Calendar", action = "SaveEvent" });
            routes.MapLowercaseRoute("calendarDeleteEvent", "calendar/deleteevent", new { controller = "Calendar", action = "DeleteEvent" });


            routes.MapLowercaseRoute("GetAudioProfiles", "GetAudioProfiles", new { controller = "Lti", action = "GetAudioProfiles" });

            //routes.MapLowercaseRoute("DefaultLtiAction", "{action}", new { controller = "Lti" });

        }

    }

}
