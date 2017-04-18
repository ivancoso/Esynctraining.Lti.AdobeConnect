namespace EdugameCloud.Lti.Host
{
    using System.Web.Mvc;
    using System.Web.Routing;
    using EdugameCloud.Lti.Core.Routes;

    public static class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            routes.IgnoreRoute("{*favicon}", new { favicon = @"(.*/)?favicon.ico(/.*)?" });
            routes.IgnoreRoute("{*robotstxt}", new { robotstxt = @"(.*/)?robots.txt(/.*)?" });


            routes.MapLowercaseRoute("meeting-attendance-report", "meeting-attendance-report", 
                new { area = "Reports", controller = "File", action = "MeetingAttendanceReport" });
            routes.MapLowercaseRoute("meeting-sessions-report", "meeting-sessions-report", 
                new { area = "Reports", controller = "File", action = "MeetingSessionsReport" });
            routes.MapLowercaseRoute("meeting-recordings-report", "meeting-recordings-report", 
                new { area = "Reports", controller = "File", action = "MeetingRecordingsReport" });


            //routes.MapLowercaseRoute("hashcode", "hash/{value}", new { area="", controller = "HashCode", action = "GetHashCode", value = "" });

            LtiRoutes.AppendTo2(routes);
        }

    }

}