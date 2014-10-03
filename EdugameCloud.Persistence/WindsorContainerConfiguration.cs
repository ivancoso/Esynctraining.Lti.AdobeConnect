namespace EdugameCloud.Persistence
{
    using System.Configuration;
    using System.Web;
    using System.Web.Configuration;

    using Castle.Facilities.Logging;
    using Castle.Facilities.WcfIntegration;
    using Castle.MicroKernel.Registration;
    using Castle.Windsor;

    using EdugameCloud.Core.Authentication;
    using EdugameCloud.Core.Business;
    using EdugameCloud.Core.Business.Models;
    using EdugameCloud.Core.Converters;
    using EdugameCloud.Persistence.Extensions;

    using Esynctraining.Core.Business;
    using Esynctraining.Core.Business.Models;
    using Esynctraining.Core.Providers;
    using Esynctraining.Core.Providers.Mailer;
    using Esynctraining.Core.Wrappers;

    using FluentValidation;

    using ISpye.Persistence;

    using NHibernate;
    using Configuration = NHibernate.Cfg.Configuration;

    /// <summary>
    /// The windsor container configuration.
    /// </summary>
    public static class WindsorContainerConfiguration
    {
        #region Public Methods and Operators

        /// <summary>
        /// The register components.
        /// </summary>
        /// <param name="container">
        /// The container.
        /// </param>
        /// <param name="console">
        /// The console.
        /// </param>
        /// <param name="wcf">
        /// The wcf.
        /// </param>
        /// <param name="web">
        /// The web.
        /// </param>
        public static void RegisterComponents(this IWindsorContainer container, bool console = false, bool wcf = false, bool web = false)
        {
            if (wcf)
            {
                container.AddFacility<WcfFacility>();
                container.Register(Classes.FromAssemblyNamed("EdugameCloud.WCFService").BasedOn(typeof(IValidator<>)).WithService.Base().LifestyleTransient());
                container.Register(Component.For<AuthenticationModel>().LifeStyle.PerWcfOperation());
            }

            container.Register(Component.For<FluentConfiguration>().LifeStyle.Singleton);
            container.Register(Component.For<Configuration>().LifeStyle.Singleton.Activator<NHibernateConfigurationActivator>());
            container.Register(Component.For<ISessionFactory>().LifeStyle.Singleton.Activator<NHibernateSessionFactoryActivator>());

            if (console)
            {
                container.Register(Component.For<ISessionSource>().ImplementedBy<NHibernateSessionSource>().LifeStyle.Transient);   
            }
            else if (wcf)
            {
                container.Register(Component.For<ISessionSource>().ImplementedBy<NHibernateSessionSource>().LifeStyle.PerWcfOperationIncludingWebOrb());   
            }
            else if (web)
            {
                container.Register(Component.For<ISessionSource>().ImplementedBy<NHibernateSessionWebSource>().LifeStyle.PerWebRequest);
            }

            container.Register(Component.For(typeof(IRepository<,>)).ImplementedBy(typeof(Repository<,>)).LifeStyle.Transient);
            container.Register(Component.For(typeof(RTMPModel)).ImplementedBy(typeof(RTMPModel)).LifeStyle.Transient);
            

            if (!console)
            {
                container.Register(Component.For<ApplicationSettingsProvider>().ImplementedBy<ApplicationSettingsProvider>().DynamicParameters((k, d) => d.Add("collection", WebConfigurationManager.AppSettings))
                             .DynamicParameters((k, d) => d.Add("globalizationSection", ConfigurationManager.GetSection("system.web/globalization") as GlobalizationSection)).LifeStyle.Singleton);
            }
            else
            {
                container.Register(Component.For<ApplicationSettingsProvider>().ImplementedBy<ApplicationSettingsProvider>().DynamicParameters((k, d) => d.Add("collection", ConfigurationManager.AppSettings))
                             .DynamicParameters((k, d) => d.Add("globalizationSection", (GlobalizationSection)null)).LifeStyle.Singleton);
            }

            container.Register(Component.For<HttpServerUtilityBase>().ImplementedBy<HttpServerUtilityWrapper>().DynamicParameters((k, d) => d.Insert("httpServerUtility", HttpContext.Current.Server)).LifeStyle.Transient);

            container.Register(Component.For<ITemplateProvider>().ImplementedBy<TemplateProvider>().LifeStyle.Transient);
            container.Register(Component.For<IAttachmentsProvider>().ImplementedBy<AttachmentsProvider>().LifeStyle.Transient);

            container.Register(Classes.FromAssemblyNamed("EdugameCloud.Core").Pick().If(Component.IsInNamespace("EdugameCloud.Core.Business.Models")).WithService.Self().Configure(c => c.LifestyleTransient()));
            container.Register(Classes.FromAssemblyNamed("Esynctraining.Core").Pick().If(Component.IsInNamespace("Esynctraining.Core.Business.Models")).If(type => type != typeof(AuthenticationModel)).WithService.Self().Configure(c => c.LifestyleTransient()));
            container.Register(Classes.FromAssemblyNamed("EdugameCloud.Core").BasedOn(typeof(BaseConverter<,>)).WithService.Base().LifestyleTransient());

            if (web)
            {
                container.Register(Component.For<AuthenticationModel>().LifeStyle.PerWebRequest);
                container.Register(Component.For<XDocumentWrapper>().LifeStyle.Transient);
                container.Register(Classes.FromAssemblyNamed("EdugameCloud.MVC").Pick().If(Component.IsInNamespace("EdugameCloud.MVC.Controllers")).WithService.Self().LifestyleTransient());
                container.Register(Classes.FromAssemblyNamed("EdugameCloud.MVC").BasedOn(typeof(IValidator<>)).WithService.Base().LifestyleTransient());
                container.Register(Classes.FromAssemblyNamed("EdugameCloud.Web").BasedOn(typeof(IValidator<>)).WithService.Base().LifestyleTransient());
            }

            container.AddFacility(new LoggingFacility(LoggerImplementation.Log4net, "log4net.cfg.xml"));
        }

        #endregion
    }
}