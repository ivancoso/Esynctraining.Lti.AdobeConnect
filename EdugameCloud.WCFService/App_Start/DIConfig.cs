namespace EdugameCloud.WCFService
{
    using System.Configuration;
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
    using Esynctraining.Core.Providers;
    using FluentValidation;
    using Lti;

    public static class DIConfig
    {
        public static void RegisterComponents(IWindsorContainer container)
        {
            container
                .Install(
                    new LoggerWindsorInstaller(),
                    new EdugameCloud.Core.Logging.LoggerWindsorInstaller(),
                    Castle.Windsor.Installer.Configuration.FromXml(new AssemblyResource("assembly://Esynctraining.Core/Esynctraining.Core.Windsor.xml")),
                    Castle.Windsor.Installer.Configuration.FromXml(new AssemblyResource("assembly://Esynctraining.Mail/Esynctraining.Mail.Windsor.xml")),
                    new NHibernateWindsorInstaller()
                )
                .AddFacility<WcfFacility>()
                //https://groups.google.com/forum/#!msg/castle-project-users/TewcYkiP_Uc/yLW4HrbSUJgJ
                .AddFacility<TypedFactoryFacility>()

                .Register(

                Component.For<ISessionSource>()
                .ImplementedBy<NHibernateSessionWebSource>()
                .LifeStyle.PerWcfOperation(),

                Classes.FromAssemblyNamed("EdugameCloud.WCFService")
                    .BasedOn(typeof(IValidator<>))
                    .WithService.Base().LifestyleTransient(),

                Classes.FromAssemblyNamed("EdugameCloud.WCFService")
                    .BasedOn(typeof(BaseConverter<,>))
                    .WithService.Base().LifestyleTransient(),

                Types.FromAssemblyNamed("EdugameCloud.WCFService")
                    .Pick()
                    .If(Component.IsInNamespace("EdugameCloud.WCFService"))
                    .Unless(type => !type.Name.EndsWith("Service"))
                    .WithService.FirstInterface()
                    .LifestylePerWcfOperation(),

                Classes.FromAssemblyNamed("EdugameCloud.WCFService").BasedOn(typeof(QuizResultConverter))
                    .WithServiceSelf().LifestyleTransient(),

                Component.For<ConverterFactory>().ImplementedBy<ConverterFactory>(),

                Component.For<ApplicationSettingsProvider>().ImplementedBy<ApplicationSettingsProvider>()
                    .DynamicParameters((k, d) => d.Add("collection", WebConfigurationManager.AppSettings))
                    .LifeStyle.Singleton,

                Component.For<IResourceProvider>()
                    .ImplementedBy<WcfResourceProvider>()
                    .Activator<ResourceProviderActivator>()
                )
                .RegisterEgcComponents();
            
            RegisterLtiComponents(container);
        }


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
                Castle.Windsor.Installer.Configuration.FromXml(new AssemblyResource("assembly://EdugameCloud.Lti.Haiku/EdugameCloud.Lti.Haiku.Windsor.xml")),
                Castle.Windsor.Installer.Configuration.FromXml(new AssemblyResource("assembly://EdugameCloud.Lti.Bridge/EdugameCloud.Lti.Bridge.Windsor.xml")),
                new LtiWindsorInstaller()
            );
            
            container.Register(Component.For<QuizConverter>().ImplementedBy<QuizConverter>());
        }

    }

}