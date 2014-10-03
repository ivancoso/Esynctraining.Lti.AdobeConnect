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

            routes.MapLowercaseRoute(
                name: "DefaultLti",
                url: "Lti",
                defaults: new { controller = "Lti", action = "Index", id = UrlParameter.Optional }
            );

            routes.MapLowercaseRoute("Default", "{action}", new { controller = "Home", action = "Admin" });


            routes.MapLowercaseRoute(
                name: "getusers",
                url: "Lti/User/GetAll",
                defaults: new { controller = "Lti", action = "GetUsers" }
            );
            routes.MapLowercaseRoute(
                name: "updateuser",
                url: "Lti/User/Update",
                defaults: new { controller = "Lti", action = "UpdateUser" }
            );
            routes.MapLowercaseRoute(
                name: "getmeeting",
                url: "Lti/Meeting/Get",
                defaults: new { controller = "Lti", action = "GetMeeting" }
            );
            routes.MapLowercaseRoute(
                name: "updatemeeting",
                url: "Lti/Meeting/Update",
                defaults: new { controller = "Lti", action = "UpdateMeeting" }
            );
            routes.MapLowercaseRoute(
                name: "joinmeeting",
                url: "Lti/Meeting/Join",
                defaults: new { controller = "Lti", action = "JoinMeeting" }
            );
            routes.MapLowercaseRoute(
                name: "getrecordings",
                url: "Lti/Recording/GetAll",
                defaults: new { controller = "Lti", action = "GetRecordings" }
            );
            routes.MapLowercaseRoute(
                name: "gettemplates",
                url: "Lti/Template/GetAll",
                defaults: new { controller = "Lti", action = "GetTemplates" }
            );
        }
    }
}