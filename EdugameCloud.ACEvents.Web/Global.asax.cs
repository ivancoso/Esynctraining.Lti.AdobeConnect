using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using Esynctraining.CastleLog4Net;
using Esynctraining.Core.Logging;
using Esynctraining.Core.Utils;
using Esynctraining.Mvc;
using Esynctraining.Windsor;
using FluentValidation;

namespace EdugameCloud.ACEvents.Web
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            var container = new WindsorContainer();
            WindsorIoC.Initialize(container);
            DIConfig.RegisterComponents(container);

            container.Install(new ControllersInstaller("EdugameCloud.ACEvents.Web"));
            //container.Register(Classes.FromAssemblyNamed("EdugameCloud.MVC").Pick().If(Component.IsInNamespace("EdugameCloud.MVC.Controllers")).WithService.Self().LifestyleTransient());

            //container.Register(Classes.FromAssemblyNamed("EdugameCloud.MVC").BasedOn(typeof(IValidator<>)).WithService.Base().LifestyleTransient());
            //container.Register(Classes.FromAssemblyNamed("EdugameCloud.ACEvents.Web").BasedOn(typeof(IValidator<>)).WithService.Base().LifestyleTransient());
            //container.Register(Classes.FromThisAssembly().BasedOn<BaseController>().LifestylePerWebRequest());
            SetControllerFactory(container);
        }

        protected void Application_BeginRequest()
        {
            //for CORS
            if (Request.Headers.AllKeys.Contains("Origin") && Request.HttpMethod == "OPTIONS")
            {
                Response.Flush();
            }
        }

        public sealed class ControllersInstaller : IWindsorInstaller
        {
            private readonly string assemblyName;


            public ControllersInstaller(string assemblyName)
            {
                if (string.IsNullOrWhiteSpace(assemblyName))
                    throw new ArgumentException("Non-empty value expected", nameof(assemblyName));

                this.assemblyName = assemblyName;
            }

            public void Install(IWindsorContainer container, IConfigurationStore store)
            {
                container.Register(
                    Classes.FromAssemblyNamed(assemblyName).Pick()
                    .If(t => t.Name.EndsWith("Controller"))
                    .Configure(configurer => configurer.Named(configurer.Implementation.Name))
                    .LifestylePerWebRequest());
            }

        }

        private static void SetControllerFactory(IWindsorContainer container)
        {
            var controllerFactory = new ServiceLocatorControllerFactory(new WindsorServiceLocator(container));
            ControllerBuilder.Current.SetControllerFactory(controllerFactory);
        }

        /// <summary>
        /// The application error.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The event arguments.
        /// </param>
        protected void Application_Error(object sender, EventArgs e)
        {
            // Code that runs when an unhandled error occurs
            IoC.Resolve<ILogger>().Error("Unhandled exception: ", this.Server.GetLastError());
        }

       
    }
}
