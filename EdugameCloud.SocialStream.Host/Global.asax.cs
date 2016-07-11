using System;
using System.Configuration;
using System.Linq;
using System.Web.Configuration;
using System.Web.Mvc;
using System.Web.Routing;
using Castle.Core.Resource;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using EdugameCloud.Core.Business.Models;
using EdugameCloud.Core.Converters;
using EdugameCloud.Persistence;
using Esynctraining.CastleLog4Net;
using Esynctraining.Core.Logging;
using Esynctraining.Core.Providers;
using Esynctraining.Core.Utils;
using Esynctraining.FluentValidation;
using Esynctraining.Mvc;
using Esynctraining.Windsor;
using FluentValidation.Mvc;

namespace EdugameCloud.SocialStream.Host
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_End()
        {
            WindsorIoC.Container.Dispose();
        }

        /// <summary>
        /// The application_ start.
        /// </summary>
        protected void Application_Start()
        {
            var webformVE = ViewEngines.Engines.OfType<WebFormViewEngine>().FirstOrDefault();
            ViewEngines.Engines.Remove(webformVE);

            var container = new WindsorContainer();
            WindsorIoC.Initialize(container);

            container.RegisterComponents();
            RegisterComponentsWeb(container);
            container.Install(new LoggerWindsorInstaller());
            container.Install(new Esynctraining.Core.Logging.CastleLogger.LoggerWindsorInstaller());
            RegisterLocalComponents(container);

            SetControllerFactory(container);
            AreaRegistration.RegisterAllAreas();
            //var modelBinders = container.ResolveAll(typeof(BaseModelBinder));
            //foreach (var binder in modelBinders)
            //{
            //    var modelBinder = (BaseModelBinder)binder;
            //    if (modelBinder.BinderTypes != null)
            //    {
            //        foreach (var binderType in modelBinder.BinderTypes)
            //        {
            //            ModelBinders.Binders.Add(binderType, modelBinder);
            //        }
            //    }
            //}

            DataAnnotationsModelValidatorProvider.AddImplicitRequiredAttributeForValueTypes = false;
            ModelValidatorProviders.Providers.Add(new FluentValidationModelValidatorProvider(new WindsorValidatorFactory(new WindsorServiceLocator(container), IoC.Resolve<ILogger>())));
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);

            DefaultModelBinder.ResourceClassKey = "Errors";
            MvcHandler.DisableMvcResponseHeader = true;

            AuthConfig.RegisterAuth(container.Resolve<ApplicationSettingsProvider>());

        }

        public static void RegisterComponentsWeb(IWindsorContainer container)
        {
            container.Register(Component.For<ISessionSource>().ImplementedBy<NHibernateSessionWebSource>().LifeStyle.PerWebRequest);

            container.Register(Component.For<ApplicationSettingsProvider>().ImplementedBy<ApplicationSettingsProvider>()
                .DynamicParameters((k, d) => d.Add("collection", WebConfigurationManager.AppSettings))
                .DynamicParameters((k, d) => d.Add("globalizationSection", ConfigurationManager.GetSection("system.web/globalization") as GlobalizationSection)).LifeStyle.Singleton);


            container.Register(Classes.FromAssemblyNamed("EdugameCloud.SocialStream.Host").Pick()
                .If(Component.IsInNamespace("EdugameCloud.SocialStream.Host.Controllers"))
                .WithService.Self().LifestyleTransient());
        }
        
        /// <summary>
        /// The register local components.
        /// </summary>
        /// <param name="container">
        /// The container.
        /// </param>
        private static void RegisterLocalComponents(IWindsorContainer container)
        {
            //container.Register(
            //    Classes.FromAssemblyNamed("EdugameCloud.MVC")
            //        .BasedOn(typeof(BaseModelBinder))
            //        .WithService.Base()
            //        .LifestyleTransient());
            //container.Register(
            //    Component.For<IResourceProvider>()
            //        .ImplementedBy<EGCResourceProvider>()
            //        .Activator<ResourceProviderActivator>());
        }

        /// <summary>
        /// The set controller factory.
        /// </summary>
        /// <param name="container">
        /// The container.
        /// </param>
        private static void SetControllerFactory(IWindsorContainer container)
        {
            var controllerFactory = new ServiceLocatorControllerFactory(new WindsorServiceLocator(container));
            ControllerBuilder.Current.SetControllerFactory(controllerFactory);
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

        protected void Application_Error(object sender, EventArgs e)
        {
            // Code that runs when an unhandled error occurs
            IoC.Resolve<ILogger>().Error("Unhandled exception: ", this.Server.GetLastError());
        }

    }

    public static class WindsorContainerConfiguration
    {
        public static void RegisterComponents(this IWindsorContainer container)
        {
            container.Install(new NHibernateWindsorInstaller());

            container.Install(
                Castle.Windsor.Installer.Configuration.FromXml(new AssemblyResource("assembly://Esynctraining.Core/Esynctraining.Core.Windsor.xml"))
            );

            container.Register(Component.For(typeof(RealTimeNotificationModel)).ImplementedBy(typeof(RealTimeNotificationModel)).LifeStyle.Transient);

            container.RegisterEgcComponents();
        }

        public static void RegisterEgcComponents(this IWindsorContainer container)
        {
            container.Register(Classes.FromAssemblyNamed("EdugameCloud.Core").Pick()
                .If(Component.IsInNamespace("EdugameCloud.Core.Business.Models"))
                .WithService.Self()
                .Configure(c => c.LifestyleTransient()));

            container.Register(Classes.FromAssemblyNamed("EdugameCloud.Core").BasedOn(typeof(BaseConverter<,>)).WithService.Base().LifestyleTransient());
        }
        
    }

}
