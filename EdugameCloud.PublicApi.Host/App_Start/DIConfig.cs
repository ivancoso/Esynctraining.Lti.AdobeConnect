using System;
using System.Configuration;
using System.Reflection;
using System.Web;
using System.Web.Configuration;
using Castle.Facilities.Logging;
using Castle.Facilities.TypedFactory;
using Castle.Facilities.WcfIntegration;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using EdugameCloud.Core.Business.Models;
using EdugameCloud.Core.Converters;
using EdugameCloud.Persistence;
using Esynctraining.Core.Business;
using Esynctraining.Core.Business.Models;
using Esynctraining.Core.Providers;
using Esynctraining.Core.Providers.Mailer;
using NHibernate;
using Configuration = NHibernate.Cfg.Configuration;

namespace EdugameCloud.PublicApi
{
    public class DIConfig
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
            Type egcCoremodelsType = typeof(ACSessionModel);
            Assembly egcCoreAssembly = egcCoremodelsType.Assembly;

            container.AddFacility<WcfFacility>();
            //https://groups.google.com/forum/#!msg/castle-project-users/TewcYkiP_Uc/yLW4HrbSUJgJ
            container.AddFacility<TypedFactoryFacility>();
            
            //container.Register(Component.For<AuthenticationModel>().LifeStyle.PerWcfOperation());

            container.Register(Component.For<FluentConfiguration>().LifeStyle.Singleton);
            container.Register(Component.For<Configuration>().LifeStyle.Singleton.Activator<NHibernateConfigurationActivator>());
            container.Register(Component.For<ISessionFactory>().LifeStyle.Singleton.Activator<NHibernateSessionFactoryActivator>());
            container.Register(Component.For<ISessionSource>().ImplementedBy<NHibernateSessionSource>().LifeStyle.PerWebRequest);
            container.Register(Component.For(typeof(IRepository<,>)).ImplementedBy(typeof(Repository<,>)).LifeStyle.Transient);
            //container.Register(Component.For(typeof(RealTimeNotificationModel)).ImplementedBy(typeof(RealTimeNotificationModel)).LifeStyle.Transient);

            container.Register(Component.For<ApplicationSettingsProvider>().ImplementedBy<ApplicationSettingsProvider>()
                    .DynamicParameters((k, d) => d.Add("collection", WebConfigurationManager.AppSettings))
                    .DynamicParameters((k, d) => d.Add("globalizationSection", ConfigurationManager.GetSection("system.web/globalization") as GlobalizationSection)).LifeStyle.Singleton);

            //container.Register(Component.For<HttpServerUtilityBase>().ImplementedBy<HttpServerUtilityWrapper>()
            //        .DynamicParameters((k, d) => d.Insert("httpServerUtility", HttpContext.Current.Server))
            //        .LifeStyle.Transient);

            //container.Register(Component.For<ITemplateProvider>().ImplementedBy<TemplateProvider>().LifeStyle.Transient);
            //container.Register(Component.For<IAttachmentsProvider>().ImplementedBy<AttachmentsProvider>().LifeStyle.Transient);

            container.Register(Classes.FromAssembly(egcCoreAssembly).Pick()
                    .If(Component.IsInNamespace(egcCoremodelsType.Namespace))
                    .WithService.Self()
                    .Configure(c => c.LifestyleTransient()));
            
            //container.Register(Component.For<MailModel>().ImplementedBy<MailModel>().LifeStyle.Transient);

            container.AddFacility(new LoggingFacility(LoggerImplementation.Log4net, "log4net.cfg.xml"));
            
        }

        #endregion
        
    }

}
