namespace EdugameCloud.Lti.Web
{
    using System.Configuration;
    using System.Web.Configuration;
    using Castle.Facilities.Logging;
    using Castle.MicroKernel.Registration;
    using Castle.Windsor;
    using Esynctraining.Core.Business;
    using Esynctraining.Core.Providers;
    using Esynctraining.Persistence;
    using NHibernate;
    using Configuration = NHibernate.Cfg.Configuration;
    using FluentConfiguration = EdugameCloud.Lti.Persistence.FluentConfiguration;

    /// <summary>
    /// The windsor container configuration.
    /// </summary>
    public static class DiConfig
    {
        #region Public Methods and Operators
        /// <summary>
        /// The register components.
        /// </summary>
        /// <param name="container">
        /// The container.
        /// </param>
        public static void RegisterComponents(IWindsorContainer container)
        {
            container.Register(Component.For<Esynctraining.Persistence.FluentConfiguration>().ImplementedBy<FluentConfiguration>().LifeStyle.Singleton);
            container.Register(Component.For<Configuration>().LifeStyle.Singleton.Activator<NHibernateConfigurationActivator>());
            container.Register(Component.For<ISessionFactory>().LifeStyle.Singleton.Activator<NHibernateSessionFactoryActivator>());
            container.Register(Component.For<ISessionSource>().ImplementedBy<NHibernateSessionWebSource>().LifeStyle.PerWebRequest);

            container.Register(Component.For(typeof(IRepository<,>)).ImplementedBy(typeof(Repository<,>)).LifeStyle.Transient);

            container.Register(Component.For<ApplicationSettingsProvider>().ImplementedBy<ApplicationSettingsProvider>().DynamicParameters((k, d) => d.Add("collection", WebConfigurationManager.AppSettings))
                             .DynamicParameters((k, d) => d.Add("globalizationSection", ConfigurationManager.GetSection("system.web/globalization") as GlobalizationSection)).LifeStyle.Singleton);

            container.AddFacility(new LoggingFacility(LoggerImplementation.Log4net, "log4net.cfg.xml"));
        }

        #endregion
    }
}