namespace EdugameCloud.WCFService
{
    using System;
    using System.Configuration;
    using System.Reflection;
    using System.Web;
    using System.Web.Configuration;

    using Castle.Facilities.Logging;
    using Castle.Facilities.WcfIntegration;
    using Castle.MicroKernel.Registration;
    using Castle.Windsor;

    using EdugameCloud.Core.Business.Models;
    using EdugameCloud.Core.Converters;
    using EdugameCloud.Lti.API;
    using EdugameCloud.Lti.API.AdobeConnect;
    using EdugameCloud.Lti.Business.Models;
    using EdugameCloud.Persistence;
    using EdugameCloud.Persistence.Extensions;
    using EdugameCloud.WCFService.Converters;
    using EdugameCloud.WCFService.Providers;

    using Esynctraining.Core.Business;
    using Esynctraining.Core.Business.Models;
    using Esynctraining.Core.Providers;
    using Esynctraining.Core.Providers.Mailer;

    using FluentValidation;

    using NHibernate;

    using Configuration = NHibernate.Cfg.Configuration;

    /// <summary>
    ///     The DI config.
    /// </summary>
    // ReSharper disable once InconsistentNaming
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
            container.Register(
                Classes.FromAssemblyNamed("EdugameCloud.WCFService")
                    .BasedOn(typeof(IValidator<>))
                    .WithService.Base()
                    .LifestyleTransient());
            container.Register(
                Classes.FromAssemblyNamed("EdugameCloud.WCFService")
                    .BasedOn(typeof(BaseConverter<,>))
                    .WithService.Base()
                    .LifestyleTransient());

            container.Register(
                Types.FromAssemblyNamed("EdugameCloud.WCFService")
                    .Pick()
                    .If(Component.IsInNamespace("EdugameCloud.WCFService"))
                    .Unless(type => !type.Name.EndsWith("Service"))
                    .WithService.FirstInterface()
                    .LifestylePerWcfOperation());

            container.Register(Component.For<AuthenticationModel>().LifeStyle.PerWcfOperation());

            container.Register(Component.For<FluentConfiguration>().LifeStyle.Singleton);
            container.Register(Component.For<Configuration>().LifeStyle.Singleton.Activator<NHibernateConfigurationActivator>());
            container.Register(Component.For<ISessionFactory>().LifeStyle.Singleton.Activator<NHibernateSessionFactoryActivator>());
            container.Register(Component.For<ISessionSource>().ImplementedBy<NHibernateSessionSource>().LifeStyle.PerWcfOperationIncludingWebOrb());
            container.Register(Component.For(typeof(IRepository<,>)).ImplementedBy(typeof(Repository<,>)).LifeStyle.Transient);
            container.Register(Component.For(typeof(RealTimeNotificationModel)).ImplementedBy(typeof(RealTimeNotificationModel)).LifeStyle.Transient);

            container.Register(Component.For<ApplicationSettingsProvider>().ImplementedBy<ApplicationSettingsProvider>()
                    .DynamicParameters((k, d) => d.Add("collection", WebConfigurationManager.AppSettings))
                    .DynamicParameters((k, d) => d.Add("globalizationSection", ConfigurationManager.GetSection("system.web/globalization") as GlobalizationSection)).LifeStyle.Singleton);

            container.Register(Component.For<HttpServerUtilityBase>().ImplementedBy<HttpServerUtilityWrapper>()
                    .DynamicParameters((k, d) => d.Insert("httpServerUtility", HttpContext.Current.Server))
                    .LifeStyle.Transient);

            container.Register(Component.For<ITemplateProvider>().ImplementedBy<TemplateProvider>().LifeStyle.Transient);
            container.Register(Component.For<IAttachmentsProvider>().ImplementedBy<AttachmentsProvider>().LifeStyle.Transient);

            container.Register(Classes.FromAssembly(egcCoreAssembly).Pick()
                    .If(Component.IsInNamespace(egcCoremodelsType.Namespace))
                    .WithService.Self()
                    .Configure(c => c.LifestyleTransient()));

            container.Register(Classes.FromAssembly(egcCoreAssembly)
                    .BasedOn(typeof(BaseConverter<,>))
                    .WithService.Base()
                    .LifestyleTransient());

            container.Register(Component.For<MailModel>().ImplementedBy<MailModel>().LifeStyle.Transient);

            container.AddFacility(new LoggingFacility(LoggerImplementation.Log4net, "log4net.cfg.xml"));
            container.Register(Component.For<IResourceProvider>().ImplementedBy<WcfResourceProvider>().Activator<ResourceProviderActivator>());
            
            RegisterLtiComponents(container);
        }

        #endregion

        #region Methods

        /// <summary>
        /// The register LTI components.
        /// </summary>
        /// <param name="container">
        /// The container.
        /// </param>
        private static void RegisterLtiComponents(IWindsorContainer container)
        {
            Type modelsType = typeof(CompanyLmsModel);
            Assembly ltiAssmebly = modelsType.Assembly;

            container.Register(Component.For<LmsFactory>().ImplementedBy<LmsFactory>());
            container.Register(
                Classes.FromAssembly(ltiAssmebly)
                    .Pick()
                    .If(Component.IsInNamespace(modelsType.Namespace))
                    .WithService.Self()
                    .Configure(c => c.LifestyleTransient()));
            container.Register(
                Classes.FromAssembly(ltiAssmebly).BasedOn(typeof(ILmsAPI)).WithServiceSelf().LifestyleTransient());
            container.Register(Component.For<QuizConverter>().ImplementedBy<QuizConverter>());
            container.Register(Component.For<QuizResultConverter>().ImplementedBy<QuizResultConverter>());

            container.Register(Component.For<UsersSetup>().ImplementedBy<UsersSetup>());
            container.Register(Component.For<MeetingSetup>().ImplementedBy<MeetingSetup>());
        }

        #endregion
    }
}