using EdugameCloud.Lti.API.AdobeConnect;
using EdugameCloud.Lti.BlackBoard;
using EdugameCloud.Lti.Bridge;
using EdugameCloud.Lti.Desire2Learn;
using EdugameCloud.Lti.Haiku;
using EdugameCloud.Lti.Moodle;
using EdugameCloud.Lti.Sakai;
using Esynctraining.Lti.Lms.AgilixBuzz;
using Esynctraining.Lti.Lms.Canvas;
using Esynctraining.Lti.Lms.Common;
using Esynctraining.Lti.Lms.Common.API;
using Esynctraining.Lti.Lms.Common.API.AgilixBuzz;
using Esynctraining.Lti.Lms.Common.API.BlackBoard;
using Esynctraining.Lti.Lms.Common.API.Canvas;
using Esynctraining.Lti.Lms.Common.API.Desire2Learn;
using Esynctraining.Lti.Lms.Common.API.Haiku;
using Esynctraining.Lti.Lms.Common.API.Moodle;
using Esynctraining.Lti.Lms.Common.API.Sakai;
using Esynctraining.Lti.Lms.Common.API.Schoology;
using Esynctraining.Lti.Lms.Schoology;
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

namespace EdugameCloud.WCFService
{
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
            //moodle
            container.Register(Component.For<IMoodleApi>().ImplementedBy<MoodleApi>().LifeStyle.Singleton);
            container.Register(Component.For<IEGCEnabledMoodleApi>().ImplementedBy<EGCEnabledMoodleApi>().LifeStyle.Singleton);
            container.Register(Component.For<LmsUserServiceBase>().ImplementedBy<MoodleLmsUserService>().Named(LmsProviderEnum.Moodle.ToString()).LifeStyle.Singleton);

            //brightspace
            container.Register(Component.For<IDesire2LearnApiService>().ImplementedBy<Desire2LearnApiService>().LifeStyle.Singleton);
            container.Register(Component.For<LmsUserServiceBase>().ImplementedBy<Desire2LearnLmsUserService>().Named(LmsProviderEnum.Brightspace.ToString()).LifeStyle.Singleton);
            container.Register(Component.For<LmsUserServiceBase>().ImplementedBy<Desire2LearnLmsUserServiceSync>().Named(LmsProviderEnum.Brightspace.ToString() + "_Sync").LifeStyle.Singleton);

            //canvas
            container.Register(Component.For<ICanvasAPI>().ImplementedBy<CanvasAPI>().LifeStyle.Singleton);
            container.Register(Component.For<IEGCEnabledCanvasAPI>().ImplementedBy<EGCEnabledCanvasAPI>().LifeStyle.Singleton);
            container.Register(Component.For<LmsUserServiceBase>().ImplementedBy<CanvasLmsUserService>().Named(LmsProviderEnum.Canvas.ToString()).LifeStyle.Singleton);
            container.Register(Component.For<LmsCourseSectionsServiceBase>().ImplementedBy<CanvasLmsCourseSectionsService>().Named(LmsProviderEnum.Canvas.ToString() + "SectionsService").LifeStyle.Transient);
            container.Register(Component.For<LmsCalendarEventServiceBase>().ImplementedBy<CanvasCalendarEventService>().Named(LmsProviderEnum.Canvas.ToString() + "CalendarService").LifeStyle.Singleton);

            //agilixbuzz
            container.Register(Component.For<IAgilixBuzzApi>().ImplementedBy<DlapAPI>().Named("IAgilixBuzzApi").LifeStyle.Singleton);
            //container.Register(Component.For<IAgilixBuzzScheduling>().ImplementedBy<ShedulingHelper>().LifeStyle.Singleton);
            container.Register(Component.For<LmsUserServiceBase>().ImplementedBy<AgilixBuzzLmsUserService>().Named(LmsProviderEnum.AgilixBuzz.ToString()).LifeStyle.Singleton);
            container.Register(Component.For<DlapAPI>().ImplementedBy<DlapAPI>().LifeStyle.Singleton);

            //blackboard
            container.Register(Component.For<IBlackBoardApi>().ImplementedBy<SoapBlackBoardApi>().LifeStyle.Singleton);
            container.Register(Component.For<IEGCEnabledBlackBoardApi>().ImplementedBy<EGCEnabledBlackboardApi>().LifeStyle.Singleton);
            container.Register(Component.For<LmsUserServiceBase>().ImplementedBy<BlackboardLmsUserService>().Named(LmsProviderEnum.Blackboard.ToString()).LifeStyle.Singleton);

            //sakai
            container.Register(Component.For<LTI2Api>().ImplementedBy<LTI2Api>().LifeStyle.Singleton);
            container.Register(Component.For<LmsUserServiceBase>().ImplementedBy<SakaiLmsUserService>().Named(LmsProviderEnum.Sakai.ToString()).LifeStyle.Singleton);
            container.Register(Component.For<ISakaiApi>().ImplementedBy<SakaiApi>().Named("ISakaiApi").LifeStyle.Singleton);
            container.Register(Component.For<ICalendarExportService>().ImplementedBy<SakaiCalendarExportService>().Named(LmsProviderEnum.Sakai.ToString() + "CalendarExportService").LifeStyle.Singleton);
            container.Register(Component.For<IEGCEnabledSakaiApi>().ImplementedBy<SakaiApi>().Named("IEGCEnabledSakaiApi").LifeStyle.Singleton);
            container.Register(Component.For<LmsCourseSectionsServiceBase>().ImplementedBy<SakaiLmsCourseSectionsService>().Named(LmsProviderEnum.Sakai.ToString() + "SectionsService").LifeStyle.Transient);

            //bridge
            container.Register(Component.For<IBridgeApi>().ImplementedBy<BridgeApi>().LifeStyle.Transient);
            container.Register(Component.For<LmsUserServiceBase>().ImplementedBy<BridgeLmsUserService>().Named(LmsProviderEnum.Bridge.ToString()).LifeStyle.Transient);
            container.Register(Component.For<ICalendarExportService>().ImplementedBy<BridgeCalendarExportService>().Named(LmsProviderEnum.Bridge.ToString() + "CalendarExportService").LifeStyle.Transient);
            container.Register(Component.For<IMeetingSessionService>().ImplementedBy<BridgeMeetingSessionService>().Named(LmsProviderEnum.Bridge.ToString() + "SessionsService").LifeStyle.Transient);

            //schoology
            container.Register(Component.For<LmsUserServiceBase>().ImplementedBy<SchoologyLmsUserService>().Named(LmsProviderEnum.Schoology.ToString()).LifeStyle.Singleton);
            container.Register(Component.For<ISchoologyRestApiClient>().ImplementedBy<SchoologyRestApiClient>().LifeStyle.Singleton);

            //haiku
            container.Register(Component.For<IHaikuRestApiClient>().ImplementedBy<HaikuRestApiClient>().LifeStyle.Singleton);
            container.Register(Component.For<LmsUserServiceBase>().ImplementedBy<HaikuLmsUserService>().Named(LmsProviderEnum.Haiku.ToString()).LifeStyle.Singleton);
            container.Register(Component.For<LmsCourseSectionsServiceBase>().ImplementedBy<HaikuLmsCourseSectionsService>().Named(LmsProviderEnum.Haiku.ToString() + "SectionsService").LifeStyle.Transient);

//            container.Install(
//                Castle.Windsor.Installer.Configuration.FromXml(new AssemblyResource("assembly://EdugameCloud.Lti.Moodle/EdugameCloud.Lti.Moodle.Windsor.xml")),
//                Castle.Windsor.Installer.Configuration.FromXml(new AssemblyResource("assembly://EdugameCloud.Lti.Desire2Learn/EdugameCloud.Lti.Desire2Learn.Windsor.xml")),
//                Castle.Windsor.Installer.Configuration.FromXml(new AssemblyResource("assembly://EdugameCloud.Lti.Canvas/EdugameCloud.Lti.Canvas.Windsor.xml")),
//                Castle.Windsor.Installer.Configuration.FromXml(new AssemblyResource("assembly://EdugameCloud.Lti.AgilixBuzz/EdugameCloud.Lti.AgilixBuzz.Windsor.xml")),
//                Castle.Windsor.Installer.Configuration.FromXml(new AssemblyResource("assembly://EdugameCloud.Lti.Blackboard/EdugameCloud.Lti.BlackBoard.Windsor.xml")),
//                Castle.Windsor.Installer.Configuration.FromXml(new AssemblyResource("assembly://EdugameCloud.Lti.Sakai/EdugameCloud.Lti.Sakai.Windsor.xml")),
//                Castle.Windsor.Installer.Configuration.FromXml(new AssemblyResource("assembly://EdugameCloud.Lti.Schoology/EdugameCloud.Lti.Schoology.Windsor.xml")),
//                Castle.Windsor.Installer.Configuration.FromXml(new AssemblyResource("assembly://EdugameCloud.Lti.Haiku/EdugameCloud.Lti.Haiku.Windsor.xml")),
//                Castle.Windsor.Installer.Configuration.FromXml(new AssemblyResource("assembly://EdugameCloud.Lti.Bridge/EdugameCloud.Lti.Bridge.Windsor.xml")),
//                new LtiWindsorInstaller()
//            );
            
            container.Register(Component.For<QuizConverter>().ImplementedBy<QuizConverter>());
        }

    }

}