using EdugameCloud.Lti.API.AdobeConnect;

namespace EdugameCloud.WCFService
{
    using System;
    using System.Configuration;
    using System.Reflection;
    using System.Web.Configuration;
    using Castle.Core.Resource;
    using Castle.Facilities.TypedFactory;
    using Castle.Facilities.WcfIntegration;
    using Castle.MicroKernel.Registration;
    using Castle.Windsor;
    using EdugameCloud.Core.Converters;
    using EdugameCloud.Persistence;
    using EdugameCloud.WCFService.Converters;
    using EdugameCloud.WCFService.Providers;
    using Esynctraining.CastleLog4Net;
    //using Esynctraining.Core.Business.Models;
    using Esynctraining.Core.Providers;
    //using Esynctraining.Core.Wcf;
    using FluentValidation;
    using Lti;

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
            container.Install(new LoggerWindsorInstaller());
            container.Install(new EdugameCloud.Core.Logging.LoggerWindsorInstaller());

            container.Install(
                Castle.Windsor.Installer.Configuration.FromXml(new AssemblyResource("assembly://Esynctraining.Core/Esynctraining.Core.Windsor.xml")),
                Castle.Windsor.Installer.Configuration.FromXml(new AssemblyResource("assembly://Esynctraining.Mail/Esynctraining.Mail.Windsor.xml"))
            );
            
            container.Install(new NHibernateWindsorInstaller());

            // TRICK: not PerWcfOperationIncludingWebOrb
            container.Register(Component.For<ISessionSource>().ImplementedBy<NHibernateSessionWebSource>().LifeStyle.PerWcfOperation());
            
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

            //container.Register(Component.For<AuthenticationModel>().LifeStyle.PerWcfOperation());
            
            container.Register(Component.For<ApplicationSettingsProvider>().ImplementedBy<ApplicationSettingsProvider>()
                    .DynamicParameters((k, d) => d.Add("collection", WebConfigurationManager.AppSettings))
                    .DynamicParameters((k, d) => d.Add("globalizationSection", ConfigurationManager.GetSection("system.web/globalization") as GlobalizationSection)).LifeStyle.Singleton);

            container.RegisterEgcComponents();

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
            container.Install(
                Castle.Windsor.Installer.Configuration.FromXml(new AssemblyResource("assembly://EdugameCloud.Lti.Moodle/EdugameCloud.Lti.Moodle.Windsor.xml")),
                Castle.Windsor.Installer.Configuration.FromXml(new AssemblyResource("assembly://EdugameCloud.Lti.Desire2Learn/EdugameCloud.Lti.Desire2Learn.Windsor.xml")),
                Castle.Windsor.Installer.Configuration.FromXml(new AssemblyResource("assembly://EdugameCloud.Lti.Canvas/EdugameCloud.Lti.Canvas.Windsor.xml")),
                Castle.Windsor.Installer.Configuration.FromXml(new AssemblyResource("assembly://EdugameCloud.Lti.AgilixBuzz/EdugameCloud.Lti.AgilixBuzz.Windsor.xml")),
                Castle.Windsor.Installer.Configuration.FromXml(new AssemblyResource("assembly://EdugameCloud.Lti.Blackboard/EdugameCloud.Lti.BlackBoard.Windsor.xml")),
                Castle.Windsor.Installer.Configuration.FromXml(new AssemblyResource("assembly://EdugameCloud.Lti.Sakai/EdugameCloud.Lti.Sakai.Windsor.xml")),
                Castle.Windsor.Installer.Configuration.FromXml(new AssemblyResource("assembly://EdugameCloud.Lti.Schoology/EdugameCloud.Lti.Schoology.Windsor.xml")),
                Castle.Windsor.Installer.Configuration.FromXml(new AssemblyResource("assembly://EdugameCloud.Lti.Haiku/EdugameCloud.Lti.Haiku.Windsor.xml"))
            );

            container.Install(new LtiWindsorInstaller());

            //container.Register(Component.For<EdugameCloud.Lti.API.AdobeConnect.IPrincipalCache>().ImplementedBy<PrincipalCache>());
            
            container.Register(Component.For<QuizConverter>().ImplementedBy<QuizConverter>());
        }

        #endregion

    }

}