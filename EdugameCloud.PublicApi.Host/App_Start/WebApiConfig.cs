using System.Web.Http;
using Microsoft.Owin.Security.OAuth;

namespace EdugameCloud.PublicApi.Host
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // New code
            config.EnableCors();

            // Web API configuration and services
            // Configure Web API to use only bearer token authentication.
            config.SuppressDefaultHostAuthentication();
            config.Filters.Add(new HostAuthenticationFilter(OAuthDefaults.AuthenticationType));

            // Web API routes
            config.MapHttpAttributeRoutes();
            
            //config.routes.maphttproute(
            //    name: "defaultapi",
            //    routetemplate: "api/{controller}/{id}",
            //    defaults: new { id = routeparameter.optional }
            //);
        }

    }

}