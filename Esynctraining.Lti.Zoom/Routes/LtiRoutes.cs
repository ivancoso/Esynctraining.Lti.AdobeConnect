using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;

namespace Esynctraining.Lti.Zoom.Routes
{
    public static class LtiRoutes
    {
        public static void AppendTo(IRouteBuilder routes)
        {
            routes.MapRoute("ImsOAuthLogin2", "ims-login", new { controller = RouteConstants.LtiControllerName, action = "ImsLogin" }); //todo: remove after certification

            routes.MapRoute("LtiOAuthLogin", "{provider}-login", new { controller = RouteConstants.LtiControllerName, action = "LoginWithProvider" });
            routes.MapRoute("LtiOAuthCallback", "oauth-callback", new { controller = RouteConstants.LtiControllerName, action = RouteConstants.AuthCallbackActionName });
            routes.MapRoute("JoinMeeting", "meetings/{meetingId}/join", new { controller = RouteConstants.LtiControllerName, action = RouteConstants.JoinMeetingActionName });
            routes.MapRoute("JoinMeetingMobile", "meetings/{meetingId}/joinmobile", new { controller = RouteConstants.LtiControllerName, action = RouteConstants.JoinMeetingMobileActionName });
            routes.MapRoute("ReportBySession", "reports/meetings/{meetingId}/by-sessions/download", new { controller = RouteConstants.ReportsControllerName, action = RouteConstants.DownloadReportActionName });
            routes.MapRoute("ReportBySessionDetails", "reports/meetings/{meetingId}/details/{meetingSessionId}/download", new { controller = RouteConstants.ReportsControllerName, action = RouteConstants.DownloadCsvReportActionName });
            routes.MapRoute("ImsOAuthLogin", "ims", new { controller = RouteConstants.LtiControllerName, action = "ImsLogin" });
            routes.MapRoute("Outcomes", "outcomes", new { controller = RouteConstants.LtiControllerName, action = "Outcomes" });
            routes.MapRoute("default", "{controller=Home}/{action=Index}/{id?}");
        }
    }

    public static class RouteConstants
    {
        public const string LtiControllerName = "Lti";
        public const string ReportsControllerName = "Reports";
        public const string AuthCallbackActionName = "AuthenticationCallback";
        public const string JoinMeetingActionName = "JoinMeeting";
        public const string JoinMeetingMobileActionName = "JoinMeetingMobile";
        public const string DownloadReportActionName = "DownloadReport";
        public const string DownloadCsvReportActionName = "DownloadReportSessionDetails";
    }
}
