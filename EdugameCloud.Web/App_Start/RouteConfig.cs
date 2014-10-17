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
            routes.MapLowercaseRoute("LtiOAuthCallback", "lti/{provider}-callback", new { controller = "Lti", action = "callback" });

            routes.MapLowercaseRoute(
                name: "DefaultLti",
                url: "Lti/{layout}",
                defaults: new { controller = "Lti", action = "Index", layout = UrlParameter.Optional }
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
                name: "deleterecording",
                url: "Lti/Recording/Delete/{id}",
                defaults: new { controller = "Lti", action = "DeleteRecording", id = UrlParameter.Optional }
            );
            routes.MapLowercaseRoute(
                name: "joinrecording",
                url: "Lti/Recording/Join/{recordingUrl}",
                defaults: new { controller = "Lti", action = "JoinRecording", recordingUrl = UrlParameter.Optional }
            );
            routes.MapLowercaseRoute(
                name: "gettemplates",
                url: "Lti/Template/GetAll",
                defaults: new { controller = "Lti", action = "GetTemplates" }
            );
        }
    }
}