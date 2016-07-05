﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;
using System.Web.SessionState;
using Castle.Core.Resource;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using EdugameCloud.Core.Business;
using EdugameCloud.Persistence;
using Esynctraining.CastleLog4Net;
using Esynctraining.Core.Logging;
using Esynctraining.Core.Providers;
using Esynctraining.Core.Utils;
using Esynctraining.FluentValidation;
using Esynctraining.Mvc;
using Esynctraining.Windsor;
using FluentValidation;
using FluentValidation.Mvc;
//using EdugameCloud.MVC.ModelBinders;

namespace EdugameCloud.Lti.Host
{
    public class Global : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            var webformVE = ViewEngines.Engines.OfType<WebFormViewEngine>().FirstOrDefault();
            ViewEngines.Engines.Remove(webformVE);

            var container = new WindsorContainer();
            WindsorIoC.Initialize(container);

            container.RegisterComponents();
            RegisterComponentsWeb(container);
            container.Install(new LoggerWindsorInstaller());
            container.Install(new EdugameCloud.Core.Logging.LoggerWindsorInstaller());
            RegisterLtiComponents(container);
            //RegisterLocalComponents(container);

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
            //BundleConfig.RegisterBundles(BundleTable.Bundles);
            DefaultModelBinder.ResourceClassKey = "Errors";
            MvcHandler.DisableMvcResponseHeader = true;

            //string pathPropertiesPath = this.GetPathPropertiesPath();
            //container.Register(Component.For<FlexSettingsProvider>().ImplementedBy<FlexSettingsProvider>().DynamicParameters((k, d) => d.Add("collection", FlexSettingsProvider.ReadSettings(pathPropertiesPath))).LifeStyle.Singleton);
            AuthConfig.RegisterAuth(container.Resolve<ApplicationSettingsProvider>());
            
            // TRICK: remove all files on start
            CachePolicies.InvalidateCache();
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

        protected void Application_End()
        {
            WindsorIoC.Container.Dispose();
        }

        public static void RegisterComponentsWeb(IWindsorContainer container)
        {
            // NOTE: is in container.RegisterComponents();
            //container.Install(
            //    Castle.Windsor.Installer.Configuration.FromXml(new AssemblyResource("assembly://Esynctraining.Core/Esynctraining.Core.Windsor.xml"))
            //);

            container.Register(Component.For<ISessionSource>().ImplementedBy<NHibernateSessionWebSource>().LifeStyle.PerWebRequest);

            container.Register(Component.For<ApplicationSettingsProvider>().ImplementedBy<ApplicationSettingsProvider>()
                .DynamicParameters((k, d) => d.Add("collection", WebConfigurationManager.AppSettings))
                .DynamicParameters((k, d) => d.Add("globalizationSection", ConfigurationManager.GetSection("system.web/globalization") as GlobalizationSection)).LifeStyle.Singleton);


            //container.Register(Component.For<AuthenticationModel>().LifeStyle.PerWebRequest);

            //container.Register(Classes.FromAssemblyNamed("EdugameCloud.MVC").Pick().If(Component.IsInNamespace("EdugameCloud.MVC.Controllers")).WithService.Self().LifestyleTransient());
            //container.Register(Classes.FromAssemblyNamed("EdugameCloud.MVC").BasedOn(typeof(IValidator<>)).WithService.Base().LifestyleTransient());
            //container.Register(Classes.FromAssemblyNamed("EdugameCloud.Web").BasedOn(typeof(IValidator<>)).WithService.Base().LifestyleTransient());
        }

        private static void RegisterLtiComponents(WindsorContainer container)
        {
            container.Install(
                Castle.Windsor.Installer.Configuration.FromXml(new AssemblyResource("assembly://EdugameCloud.Lti.Moodle/EdugameCloud.Lti.Moodle.Windsor.xml")),
                Castle.Windsor.Installer.Configuration.FromXml(new AssemblyResource("assembly://EdugameCloud.Lti.Desire2Learn/EdugameCloud.Lti.Desire2Learn.Windsor.xml")),
                Castle.Windsor.Installer.Configuration.FromXml(new AssemblyResource("assembly://EdugameCloud.Lti.Canvas/EdugameCloud.Lti.Canvas.Windsor.xml")),
                Castle.Windsor.Installer.Configuration.FromXml(new AssemblyResource("assembly://EdugameCloud.Lti.BrainHoney/EdugameCloud.Lti.BrainHoney.Windsor.xml")),
                Castle.Windsor.Installer.Configuration.FromXml(new AssemblyResource("assembly://EdugameCloud.Lti.Blackboard/EdugameCloud.Lti.BlackBoard.Windsor.xml")),
                Castle.Windsor.Installer.Configuration.FromXml(new AssemblyResource("assembly://EdugameCloud.Lti.Sakai/EdugameCloud.Lti.Sakai.Windsor.xml"))
            );

            container.Install(new LtiWindsorInstaller());
            container.Install(new LtiMvcWindsorInstaller());
            container.Install(new TelephonyWindsorInstaller());

            container.Register(Classes.FromAssemblyNamed("EdugameCloud.Lti").BasedOn(typeof(IValidator<>)).WithService.Base().LifestyleTransient());
        }

        private static void SetControllerFactory(IWindsorContainer container)
        {
            var controllerFactory = new ServiceLocatorControllerFactory(new WindsorServiceLocator(container));
            ControllerBuilder.Current.SetControllerFactory(controllerFactory);
        }

    }

}