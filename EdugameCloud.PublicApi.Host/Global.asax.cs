using System;
using System.Web.Http;
using System.Web.Mvc;
using Esynctraining.Core.Logging;
using Castle.Windsor;
using Esynctraining.Core.Utils;
using Esynctraining.Windsor;

namespace EdugameCloud.PublicApi.Host
{
    public class Global : System.Web.HttpApplication
    {
        protected void Application_Start(object sender, EventArgs e)
        {
            var container = new WindsorContainer();
            WindsorIoC.Initialize(container);
            DIConfig.RegisterComponents(container);

            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            //RouteConfig.RegisterRoutes(RouteTable.Routes);
            //BundleConfig.RegisterBundles(BundleTable.Bundles);

            MvcHandler.DisableMvcResponseHeader = true;
        }

        protected void Application_Error(object sender, EventArgs e)
        {
            IoC.Resolve<ILogger>().Error("Unhandled exception: ", this.Server.GetLastError());
        }

        // source : http://stackoverflow.com/questions/1178831/remove-server-response-header-iis7
        protected void Application_PreSendRequestHeaders(object sender, EventArgs e)
        {
            // Remove the "Server" HTTP Header from response
            if (null != Response)
            {
                Response.Headers.Remove("Server");
            }
        }

    }

}