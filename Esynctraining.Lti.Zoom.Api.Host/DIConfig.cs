

namespace Esynctraining.Lti.Zoom.Api.Host
{
    //internal static class DIConfig
    //{
    //    public static WindsorContainer ConfigureWindsor(IConfiguration Configuration)
    //    {
    //        var configurationSection = Configuration.GetSection("AppSettings");
    //        var collection = new NameValueCollection();
    //        foreach (var appSetting in configurationSection.GetChildren())
    //        {
    //            collection.Add(appSetting.Key, appSetting.Value);
    //        }

    //        //container.Register(Component.For<ApplicationSettingsProvider>().ImplementedBy<ApplicationSettingsProvider>()
    //        //    .DynamicParameters((k, d) => d.Add("collection", collection))
    //        //    .LifeStyle.Singleton);

    //        //container.Install(new LoggerWindsorInstaller());
    //        //container.Install(new EdugameCloud.Core.Logging.LoggerWindsorInstaller());

    //        RegisterLtiComponents(container);
    //        //todo: per client
    //        container.Register(Component.For<ZoomApiWrapper>().ImplementedBy<ZoomApiWrapper>().DynamicParameters(
    //                (k, d) => d.Add("options", new ZoomApiOptions
    //                {
    //                    ZoomApiKey = "zc69A0RpTBKIpQbxknTs1Q",
    //                    ZoomApiSecret = "kTWs9t1G0pPcS2CSZ31SysWu2VxFgzs2uATL"
    //                }))
    //            .LifeStyle.Singleton);
    //        /*
    //         IEGCEnabledMoodleApi moodleApi, IEGCEnabledBlackBoardApi blackboardApi, IEGCEnabledSakaiApi sakaiApi
    //         */
    //        container.Register(Component.For<IEGCEnabledMoodleApi>().ImplementedBy<FakeMoodleApi>().LifestyleSingleton());
    //        container.Register(Component.For<IEGCEnabledBlackBoardApi>().ImplementedBy<FakeBBApi>().LifestyleSingleton());
    //        container.Register(Component.For<IEGCEnabledSakaiApi>().ImplementedBy<FakeSakaiApi>().LifestyleSingleton());
    //        container.Register(Component.For<ZoomUserService>().ImplementedBy<ZoomUserService>().LifestyleSingleton());
    //        container.Register(Component.For<ZoomRecordingService>().ImplementedBy<ZoomRecordingService>().LifestyleSingleton());
    //        container.Register(Component.For<ZoomMeetingService>().ImplementedBy<ZoomMeetingService>().LifestyleSingleton());
    //        container.Register(Component.For<ZoomReportService>().ImplementedBy<ZoomReportService>().LifestyleSingleton());

    //        return container;
    //    }

    //    private static void RegisterLtiComponents(WindsorContainer container)
    //    {
    //        container.Install(
    //            Castle.Windsor.Installer.Configuration.FromXml(new AssemblyResource("assembly://EdugameCloud.Lti.Canvas/EdugameCloud.Lti.Canvas.Windsor.xml"))
    //        );

    //        container.Install(new LtiWindsorInstaller());

    //        container.Register(Classes.FromAssemblyNamed("EdugameCloud.Lti").BasedOn(typeof(IValidator<>)).WithService.Base().LifestyleTransient());
    //    }

    //}

}
