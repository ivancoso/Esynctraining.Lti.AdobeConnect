using System.Web.Mvc;
using System.Web.Routing;

namespace EdugameCloud.Canvas
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "Default",
                url: "Lti",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
            routes.MapRoute(
                name: "getusers",
                url: "Lti/User/GetAll",
                defaults: new { controller = "Home", action="GetUsers" }
            );
            routes.MapRoute(
                name: "updateuser",
                url: "Lti/User/Update",
                defaults: new { controller = "Home", action = "UpdateUser" }
            );
            routes.MapRoute(
                name: "getmeeting",
                url: "Lti/Meeting/Get",
                defaults: new { controller = "Home", action = "GetMeeting" }
            );
            routes.MapRoute(
                name: "updatemeeting",
                url: "Lti/Meeting/Update",
                defaults: new { controller = "Home", action = "UpdateMeeting" }
            );
            routes.MapRoute(
                name: "joinmeeting",
                url: "Lti/Meeting/Join",
                defaults: new { controller = "Home", action = "JoinMeeting" }
            );
            routes.MapRoute(
                name: "getrecordings",
                url: "Lti/Recording/GetAll",
                defaults: new { controller = "Home", action = "GetRecordings" }
            );
            routes.MapRoute(
                name: "gettemplates",
                url: "Lti/Template/GetAll",
                defaults: new { controller = "Home", action = "GetTemplates" }
            );

            routes.MapRoute(
                name: "Default2",
                url: "",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
            routes.MapRoute(
                name: "getusers2",
                url: "User/GetAll",
                defaults: new { controller = "Home", action = "GetUsers" }
            );
            routes.MapRoute(
                name: "updateuser2",
                url: "User/Update",
                defaults: new { controller = "Home", action = "UpdateUser" }
            );
            routes.MapRoute(
                name: "getmeeting2",
                url: "Meeting/Get",
                defaults: new { controller = "Home", action = "GetMeeting" }
            );
            routes.MapRoute(
                name: "updatemeeting2",
                url: "Meeting/Update",
                defaults: new { controller = "Home", action = "UpdateMeeting" }
            );
            routes.MapRoute(
                name: "joinmeeting2",
                url: "Meeting/Join",
                defaults: new { controller = "Home", action = "JoinMeeting" }
            );
            routes.MapRoute(
                name: "getrecordings2",
                url: "Recording/GetAll",
                defaults: new { controller = "Home", action = "GetRecordings" }
            );
            routes.MapRoute(
                name: "gettemplates2",
                url: "Template/GetAll",
                defaults: new { controller = "Home", action = "GetTemplates" }
            );
        }
    }
}