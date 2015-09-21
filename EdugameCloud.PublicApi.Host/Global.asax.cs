using System;
using System.Web.Http;
using System.Web.Mvc;
using Castle.Core.Logging;
using Castle.Windsor;
using Esynctraining.Core.Utils;

namespace EdugameCloud.PublicApi.Host
{
    public class Global : System.Web.HttpApplication
    {
        protected void Application_Start(object sender, EventArgs e)
        {
            var container = new WindsorContainer();
            IoC.Initialize(container);
            DIConfig.RegisterComponents(container);

            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            //RouteConfig.RegisterRoutes(RouteTable.Routes);
            //BundleConfig.RegisterBundles(BundleTable.Bundles);            
        }

        protected void Application_Error(object sender, EventArgs e)
        {
            IoC.Resolve<ILogger>().Error("Unhandled exception: ", this.Server.GetLastError());
        }

    }

}