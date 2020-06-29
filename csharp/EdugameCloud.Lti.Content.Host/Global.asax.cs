using System;
using System.Configuration;
using System.Web.Configuration;
using System.Web.Http;
using System.Web.Http.Dispatcher;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using EdugameCloud.Persistence;
using Esynctraining.Core.Providers;
using Esynctraining.WebApi;
using Esynctraining.Windsor;

namespace EdugameCloud.Lti.Content.Host
{
    public class Global : System.Web.HttpApplication
    {
        protected void Application_End()
        {
            WindsorIoC.Container.Dispose();
        }
        
        protected void Application_Start()
        {
            var container = new WindsorContainer();
            WindsorIoC.Initialize(container);

            container.RegisterComponents();
            RegisterComponentsWeb(container);
            container.Install(new Esynctraining.CastleLog4Net.LoggerWindsorInstaller());
            container.Install(new EdugameCloud.Core.Logging.LoggerWindsorInstaller());
            RegisterLtiComponents(container);
            RegisterLocalComponents(container);

            GlobalConfiguration.Configure(WebApiConfig.Register);
            GlobalConfiguration.Configuration.Services.Replace(
                typeof(IHttpControllerActivator),
                new ServiceLocatorControllerActivator(new WindsorServiceLocator(container)));
        }

        public static void RegisterComponentsWeb(IWindsorContainer container)
        {
            //container.Install(
            //    Castle.Windsor.Installer.Configuration.FromXml(new AssemblyResource("assembly://Esynctraining.Mail/Esynctraining.Mail.Windsor.xml"))
            //);

            container.Register(Component.For<ISessionSource>().ImplementedBy<NHibernateSessionWebSource>().LifeStyle.PerWebRequest);

            container.Register(Component.For<ApplicationSettingsProvider>().ImplementedBy<ApplicationSettingsProvider>()
                .DynamicParameters((k, d) => d.Add("collection", ConfigurationManager.AppSettings))
                .DynamicParameters((k, d) => d.Add("globalizationSection", (GlobalizationSection)null)).LifeStyle.Singleton);
        }

        private static void RegisterLtiComponents(WindsorContainer container)
        {
            container.Install(new LtiWindsorInstaller());
        }

        private static void RegisterLocalComponents(IWindsorContainer container)
        {
            //string mp4ApiBaseAddress = (Esynctraining.Core.Utils.IoC.Resolve<ApplicationSettingsProvider>() as dynamic).MP4ApiBaseAddress as string;
            //container.Register(Component.For<TaskClient>()
            //       .ImplementedBy<TaskClient>()
            //       .DependsOn(Dependency.OnValue("baseApiAddress", mp4ApiBaseAddress))
            //       .LifeStyle.Singleton);
            
            container.Register(Classes.FromAssemblyNamed("EdugameCloud.Lti.Content.Host")
                .Pick()
                .If(Component.IsInNamespace("EdugameCloud.Lti.Content.Host.Controllers"))
                .WithService.Self()
                .LifestyleTransient());
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