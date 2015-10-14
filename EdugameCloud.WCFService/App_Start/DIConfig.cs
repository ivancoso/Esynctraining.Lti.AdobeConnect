namespace EdugameCloud.WCFService
{
    using System;
    using System.Configuration;
    using System.Reflection;
    using System.Web;
    using System.Web.Configuration;
    using Castle.Facilities.TypedFactory;
    using Castle.Facilities.WcfIntegration;
    using Castle.MicroKernel.Registration;
    using Castle.Windsor;
    using EdugameCloud.Core.Business.Models;
    using EdugameCloud.Core.Converters;
    using Lti;
    using EdugameCloud.Lti.AdobeConnect.Caching;
    using EdugameCloud.Lti.Sakai;
    using EdugameCloud.Lti.BlackBoard;
    using EdugameCloud.Lti.BrainHoney;
    using EdugameCloud.Lti.Canvas;
    using EdugameCloud.Lti.Desire2Learn;
    using EdugameCloud.Lti.Moodle;
    using EdugameCloud.Persistence;
    using EdugameCloud.WCFService.Converters;
    using EdugameCloud.WCFService.Providers;
    using Esynctraining.Core.Business.Models;
    using Esynctraining.Core.Providers;
    using Esynctraining.Core.Providers.Mailer;
    using FluentValidation;
    using Core;
    using Esynctraining.CastleLog4Net;
    using Esynctraining.Core.Wcf;

    /// <summary>
    /// The DI config.
    /// </summary>
    // ReSharper disable once InconsistentNaming
    public static class DIConfig
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
            container.Install(new CoreWindsorInstaller());
            container.Install(new NHibernateWindsorInstaller());
            container.Install(new MailWindsorInstaller());

            container.Register(Component.For<ISessionSource>().ImplementedBy<NHibernateSessionSource>().LifeStyle.PerWcfOperationIncludingWebOrb());


            Type egcCoremodelsType = typeof(ACSessionModel);
            Assembly egcCoreAssembly = egcCoremodelsType.Assembly;

            container.AddFacility<WcfFacility>();
            //https://groups.google.com/forum/#!msg/castle-project-users/TewcYkiP_Uc/yLW4HrbSUJgJ
            container.AddFacility<TypedFactoryFacility>();

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

            container.Register(
                Classes.FromAssemblyNamed("EdugameCloud.WCFService").BasedOn(typeof(QuizResultConverter)).WithServiceSelf().LifestyleTransient());
            container.Register(Component.For<ConverterFactory>().ImplementedBy<ConverterFactory>());

            container.Register(Component.For<AuthenticationModel>().LifeStyle.PerWcfOperation());
            
            container.Register(Component.For<ApplicationSettingsProvider>().ImplementedBy<ApplicationSettingsProvider>()
                    .DynamicParameters((k, d) => d.Add("collection", WebConfigurationManager.AppSettings))
                    .DynamicParameters((k, d) => d.Add("globalizationSection", ConfigurationManager.GetSection("system.web/globalization") as GlobalizationSection)).LifeStyle.Singleton);

            container.Register(Component.For<HttpServerUtilityBase>().ImplementedBy<HttpServerUtilityWrapper>()
                    .DynamicParameters((k, d) => d.Insert("httpServerUtility", HttpContext.Current.Server))
                    .LifeStyle.Transient);
            
            container.Register(Classes.FromAssembly(egcCoreAssembly).Pick()
                    .If(Component.IsInNamespace(egcCoremodelsType.Namespace))
                    .WithService.Self()
                    .Configure(c => c.LifestyleTransient()));

            container.Register(Classes.FromAssembly(egcCoreAssembly)
                    .BasedOn(typeof(BaseConverter<,>))
                    .WithService.Base()
                    .LifestyleTransient());

            container.Install(new LoggerWindsorInstaller());
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
            container.Install(new MoodleWindsorInstaller());
            container.Install(new Desire2LearnWindsorInstaller());
            container.Install(new CanvasWindsorInstaller());
            container.Install(new BrainHoneyWindsorInstaller());
            container.Install(new BlackboardWindsorInstaller());
            container.Install(new SakaiWindsorInstaller());
            container.Install(new LtiWindsorInstaller());

            container.Register(Component.For<EdugameCloud.Lti.API.AdobeConnect.IPrincipalCache>().ImplementedBy<PrincipalCache>());
            
            container.Register(Component.For<QuizConverter>().ImplementedBy<QuizConverter>());
        }

        #endregion

    }

}