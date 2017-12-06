using Castle.Core.Resource;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using EdugameCloud.Core;
using EdugameCloud.Core.Business;
using EdugameCloud.Lti.API;
using EdugameCloud.Lti.API.AdobeConnect;
using EdugameCloud.Lti.Core.Business.MeetingNameFormatting;
using EdugameCloud.Lti.Telephony;
using Esynctraining.AdobeConnect.Api.MeetingReports;
using Esynctraining.Core.Caching;
using Esynctraining.Core.Json;
using Esynctraining.Json.Jil;

namespace EdugameCloud.Lti
{
    public sealed class LtiWindsorInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.Register(Classes.FromAssemblyNamed("EdugameCloud.Lti.Core").Pick()
                .If(Component.IsInNamespace("EdugameCloud.Lti.Core.Business.Models"))
                .WithService.Self().Configure(c => c.LifestyleTransient()));

            container.Register(
                Component.For<IMeetingSetup>().ImplementedBy<MeetingSetup>().Named("IMeetingSetup"),
                Component.For<MeetingSetup>().ImplementedBy<MeetingSetup>(),
                Component.For<IUsersSetup>().ImplementedBy<UsersSetup>().Named("IUsersSetup"),
                Component.For<UsersSetup>().ImplementedBy<UsersSetup>(),
                Component.For<IReportsService>().ImplementedBy<ReportsService>(),
                Component.For<ICache>().ImplementedBy<PersistantCacheWrapper>().LifestyleSingleton().Named(CachePolicies.Names.PersistantCache),
                Component.For<IBuildVersionProcessor>().ImplementedBy<BuildVersionProcessor>().LifestyleSingleton(),

                Component.For<LmsFactory>().ImplementedBy<LmsFactory>().LifestyleSingleton(),
                Component.For<IJsonSerializer>().ImplementedBy<JilSerializer>().LifestyleSingleton(),
                Component.For<IJsonDeserializer>().ImplementedBy<JilSerializer>().LifestyleSingleton(),
                Component.For<IMeetingNameFormatterFactory>().ImplementedBy<MeetingNameFormatterFactory>().LifestyleSingleton(),

                Component.For<IAdobeConnectUserService>().ImplementedBy<AdobeConnectUserService>().LifestyleSingleton(),
                Component.For<ISynchronizationUserService>().ImplementedBy<SynchronizationUserService>().LifeStyle.Transient,
                Component.For<IAdobeConnectAccountService>().ImplementedBy<AdobeConnectAccountService>().LifestyleSingleton(),
                Component.For<IRecordingsService>().ImplementedBy<RecordingsService>().LifeStyle.Transient,
                Component.For<IAudioProfilesService>().ImplementedBy<AudioProfilesService>().LifeStyle.Transient,
                Component.For<ISeminarService>().ImplementedBy<SeminarService>().LifestyleSingleton(),

                Component.For<TestConnectionService>().ImplementedBy<TestConnectionService>().LifestyleSingleton(),
                Component.For<LmsRoleService>().ImplementedBy<LmsRoleService>().LifestyleSingleton(),
                Component.For<LmsUserServiceBase>().ImplementedBy<ImsUserService>().Named("Ims").LifestyleSingleton()
                );
            
            container.Install(
                Castle.Windsor.Installer.Configuration.FromXml(new AssemblyResource("assembly://Esynctraining.AdobeConnect/Esynctraining.AdobeConnect.Windsor.xml"))
            );
        }
    }

    public sealed class LtiMvcWindsorInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.Register(Classes.FromAssemblyNamed("EdugameCloud.Lti").Pick()
                .If(Component.IsInNamespace("EdugameCloud.Lti.Controllers")).WithService.Self().LifestyleTransient());
        }

    }
    public sealed class TelephonyWindsorInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.Register(Component.For<ITelephonyProfileEngine>().ImplementedBy<MeetingOneEngine>().Named("MEETINGONE"));
            //container.Register(Component.For<ITelephonyProfileEngine>().ImplementedBy<ArkadinEngine>().Named("ARKADIN"));
        }

    }

}
