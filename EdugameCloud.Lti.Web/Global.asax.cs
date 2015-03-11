﻿using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using EdugameCloud.Lti.API;
using EdugameCloud.Lti.API.AdobeConnect;
using EdugameCloud.Lti.API.Desire2Learn;
using EdugameCloud.Lti.Web.Providers;
using Esynctraining.Core.Providers;
using Esynctraining.Core.Utils;

namespace EdugameCloud.Lti.Web
{
    /// <summary>
    /// The MVC application.
    /// </summary>
    public class MvcApplication : HttpApplication
    {
        /// <summary>
        /// The application_ end.
        /// </summary>
        protected void Application_End()
        {
            IoC.Container.Dispose();
        }

        /// <summary>
        /// The application_ start.
        /// </summary>
        protected void Application_Start()
        {
            var container = new WindsorContainer();
            IoC.Initialize(container);
            DiConfig.RegisterComponents(container);
            RegisterLtiComponents(container);
            SetControllerFactory(container);
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            AuthConfig.RegisterAuth(container.Resolve<ApplicationSettingsProvider>());
        }

        /// <summary>
        /// The register LTI components.
        /// </summary>
        /// <param name="container">
        /// The container.
        /// </param>
        private static void RegisterLtiComponents(WindsorContainer container)
        {
            container.Register(Classes.FromAssemblyNamed("EdugameCloud.Lti").Pick().If(Component.IsInNamespace("EdugameCloud.Lti.Business.Models")).WithService.Self().Configure(c => c.LifestyleTransient()));
            container.Register(Classes.FromAssemblyNamed("EdugameCloud.Lti").BasedOn(typeof(ILmsAPI)).WithServiceSelf().LifestyleTransient());
            container.Register(Component.For<MeetingSetup>().ImplementedBy<MeetingSetup>());
            container.Register(Component.For<UsersSetup>().ImplementedBy<UsersSetup>());
            container.Register(Classes.FromAssemblyNamed("EdugameCloud.Lti").Pick().If(Component.IsInNamespace("EdugameCloud.Lti.Controllers")).WithService.Self().LifestyleTransient());
            container.Register(Component.For<IDesire2LearnApiService>().ImplementedBy<Desire2LearnApiService>()
                .DynamicParameters((k, d) =>
                {
                    var settings = k.Resolve<ApplicationSettingsProvider>();
                    d["providerKey"] = ((dynamic)settings).D2LApiKey;
                    d["providerSecret"] = ((dynamic)settings).D2LApiSecret;
                }).LifestyleTransient());
        }

        /// <summary>
        /// The set controller factory.
        /// </summary>
        /// <param name="container">
        /// The container.
        /// </param>
        private static void SetControllerFactory(IWindsorContainer container)
        {
            var controllerFactory = new WindsorControllerFactory(container);
            ControllerBuilder.Current.SetControllerFactory(controllerFactory);
        }
    }
}