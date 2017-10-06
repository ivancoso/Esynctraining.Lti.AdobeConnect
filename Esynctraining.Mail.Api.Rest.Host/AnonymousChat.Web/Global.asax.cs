namespace AnonymousChat.Web
{
    using System.Configuration;
    using System.Web;
    using System.Web.Configuration;

    using AnonymousChat.Web.Providers;

    using Castle.Facilities.Logging;
    using Castle.MicroKernel.Registration;
    using Castle.Windsor;

    using Esynctraining.Core.Business.Models;
    using Esynctraining.Core.Providers;
    using Esynctraining.Core.Providers.Mailer;
    using Esynctraining.Core.Utils;

    using IResourceProvider = Esynctraining.Core.Providers.IResourceProvider;

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
            IoC.Initialize(new WindsorContainer());
            RegisterLocalComponents(IoC.Container);
        }

        /// <summary>
        /// The register local components.
        /// </summary>
        /// <param name="container">
        /// The container.
        /// </param>
        private static void RegisterLocalComponents(IWindsorContainer container)
        {
            container.Register(Component.For<ApplicationSettingsProvider>().ImplementedBy<ApplicationSettingsProvider>().DynamicParameters((k, d) => d.Add("collection", WebConfigurationManager.AppSettings)).DynamicParameters((k, d) => d.Add("globalizationSection", ConfigurationManager.GetSection("system.web/globalization") as GlobalizationSection)).LifeStyle.Singleton);

            container.Register(Component.For<ITemplateProvider>().ImplementedBy<TemplateProvider>().LifeStyle.Transient);
            container.Register(Component.For<IAttachmentsProvider>().ImplementedBy<AttachmentsProvider>().LifeStyle.Transient);

            container.Register(Component.For<MailModel>().ImplementedBy<MailModel>().LifestyleTransient());

            container.AddFacility(new LoggingFacility(LoggerImplementation.Log4net, "log4net.cfg.xml"));
            container.Register(Component.For<IResourceProvider>().ImplementedBy<WebResourceProvider>().Activator<ResourceProviderActivator>());
        }
    }
}