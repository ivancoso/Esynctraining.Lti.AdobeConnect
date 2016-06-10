namespace PDFAnnotation.Persistence
{
    using System.Configuration;
    using System.Web;
    using System.Web.Configuration;

    using Castle.Facilities.WcfIntegration;
    using Castle.MicroKernel.Registration;
    using Castle.Windsor;
    using Esynctraining.Core.Business.Models;
    using Esynctraining.Core.Providers;
    using Esynctraining.Mail;
    using Esynctraining.Persistence;

    using NHibernate;
    using PDFAnnotation.Core.Utils;

    using Configuration = NHibernate.Cfg.Configuration;
    using Esynctraining.NHibernate;
    using Esynctraining.Core.FullText;    
    
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
                container.Register(Component.For<AuthenticationModel>().ImplementedBy(typeof(AuthenticationModel)).LifeStyle.PerWcfOperation());
            }

            container.Register(Component.For<Configuration>().LifeStyle.Singleton.Activator<NHibernateConfigurationActivator>());
            container.Register(Component.For<ISessionFactory>().LifeStyle.Singleton.Activator<Esynctraining.Persistence.NHibernateSessionFactoryActivator>());

            if (console)
            {
                container.Register(Component.For<ISessionSource>().ImplementedBy<NHibernateSessionSource>().LifeStyle.Transient);   
            }
            else if (wcf)
            {
                container.Register(Component.For<ISessionSource>().ImplementedBy<NHibernateSessionSource>().LifeStyle.PerWcfOperation());   
            }
            else if (web)
            {
                container.Register(Component.For<ISessionSource>().ImplementedBy<NHibernateSessionWebSource>().LifeStyle.PerWebRequest);
            }

            container.Register(Component.For(typeof(IRepository<,>)).ImplementedBy(typeof(Repository<,>)).LifeStyle.Transient);
            
            if (!console)
            {
                container.Register(Component.For<ApplicationSettingsProvider>().ImplementedBy<ApplicationSettingsProvider>().DynamicParameters((k, d) => d.Add("collection", WebConfigurationManager.AppSettings)).DynamicParameters((k, d) => d.Add("globalizationSection", ConfigurationManager.GetSection("system.web/globalization") as GlobalizationSection)).LifeStyle.Singleton);
            }
            else
            {
            // ReSharper disable RedundantCast (The cast is actually needed here for castle windsor to know the type of null)
                container.Register(Component.For<ApplicationSettingsProvider>().ImplementedBy<ApplicationSettingsProvider>().DynamicParameters((k, d) => d.Add("collection", ConfigurationManager.AppSettings)).DynamicParameters((k, d) => d.Add("globalizationSection", (GlobalizationSection)null)).LifeStyle.Singleton);
            // ReSharper restore RedundantCast
            }

            container.Register(Component.For<HttpServerUtilityBase>().ImplementedBy<HttpServerUtilityWrapper>().DynamicParameters((k, d) => d.Insert("httpServerUtility", HttpContext.Current.Server)).LifeStyle.Transient);

            container.Register(Component.For<FullTextModel>().LifeStyle.Singleton);
            container.Register(Component.For<Pdf2SwfConverter>().LifeStyle.Singleton);
            container.Register(Types.FromAssemblyNamed("PDFAnnotation.Core").Pick().If(Component.IsInNamespace("PDFAnnotation.Core.Business.Models", true)).WithService.Self().Configure(c => c.LifestyleTransient()));
            container.Register(Types.FromAssemblyNamed("Esynctraining.Core").Pick().If(Component.IsInNamespace("Esynctraining.Core.Business.Models")).Unless(type => type == typeof(AuthenticationModel) || type == typeof(FullTextModel)).WithService.Self().Configure(c => c.LifestyleTransient()));
            
            if (web)
            {
                container.Register(Component.For<AuthenticationModel>().ImplementedBy(typeof(AuthenticationModel)).LifeStyle.PerWebRequest);
              //  container.Register(Component.For<XDocumentWrapper>().LifeStyle.Transient);
                
            }

          //  container.AddFacility(new LoggingFacility(LoggerImplementation.Log4net, "log4net.cfg.xml"));
        }

        #endregion
    }
}