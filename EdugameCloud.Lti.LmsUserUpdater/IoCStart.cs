using System.Collections.Specialized;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using EdugameCloud.Lti.API.AdobeConnect;
using EdugameCloud.Lti.BlackBoard;
using EdugameCloud.Lti.Bridge;
using EdugameCloud.Lti.Haiku;
using EdugameCloud.Lti.Moodle;
using EdugameCloud.Lti.Sakai;
using EdugameCloud.Persistence;
using Esynctraining.CastleLog4Net;
using Esynctraining.Core.Providers;
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
using Esynctraining.Lti.Lms.Desire2Learn;
using Esynctraining.Lti.Lms.Moodle;
using Esynctraining.Lti.Lms.Sakai;
using Esynctraining.Lti.Lms.Schoology;
using Esynctraining.Windsor;
using Microsoft.Extensions.Configuration;

namespace EdugameCloud.Lti.LmsUserUpdater
{
    internal static class IoCStart
    {
        public static void Init(IConfigurationRoot configuration)
        {
            var container = new WindsorContainer();
            WindsorIoC.Initialize(container);
            container.RegisterComponents();

            var configurationSection = configuration.GetSection("AppSettings");
            var collection = new NameValueCollection();
            foreach (var appSetting in configurationSection.GetChildren())
            {
                collection.Add(appSetting.Key, appSetting.Value);
            }

            container.Register(Component.For<ISessionSource>().ImplementedBy<NHibernateSessionSource>().LifeStyle.Singleton);

            container.Register(Component.For<ApplicationSettingsProvider>().ImplementedBy<ApplicationSettingsProvider>()
                    .DynamicParameters((k, d) => d.Add("collection", collection))
                    .LifeStyle.Singleton);

            container.Install(new LoggerWindsorInstaller());
            container.Install(new EdugameCloud.Core.Logging.LoggerWindsorInstaller());
            RegisterLtiComponents(container);
        }

        private static void RegisterLtiComponents(WindsorContainer container)
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
            container.Register(Component.For<LmsCourseSectionsServiceBase>().ImplementedBy<SakaiLmsCourseSectionsService>().LifeStyle.Transient);

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
            //                Castle.Windsor.Installer.Configuration.FromXml(new AssemblyResource("assembly://EdugameCloud.Lti.Bridge/EdugameCloud.Lti.Bridge.Windsor.xml"))
            //            );

            container.Install(new LtiWindsorInstaller());
        }

    }

}
