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

namespace EdugameCloud.Lti
{
    public sealed class LtiWindsorInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.Register(Classes.FromAssemblyNamed("EdugameCloud.Lti.Core").Pick()
                .If(Component.IsInNamespace("EdugameCloud.Lti.Core.Business.Models")).WithService.Self().Configure(c => c.LifestyleTransient()));

            container.Register(Component.For<IMeetingSetup>().ImplementedBy<MeetingSetup>().Named("IMeetingSetup"));
            container.Register(Component.For<MeetingSetup>().ImplementedBy<MeetingSetup>());
            container.Register(Component.For<IUsersSetup>().ImplementedBy<UsersSetup>().Named("IUsersSetup"));
            container.Register(Component.For<UsersSetup>().ImplementedBy<UsersSetup>());
            container.Register(Component.For<IReportsService>().ImplementedBy<ReportsService>());
            container.Register(Component.For<ICache>().ImplementedBy<PersistantCacheWrapper>().LifestyleSingleton().Named(CachePolicies.Names.PersistantCache));
            container.Register(Component.For<IBuildVersionProcessor>().ImplementedBy<BuildVersionProcessor>().LifestyleSingleton());

            container.Register(Component.For<LmsFactory>().ImplementedBy<LmsFactory>());
            container.Register(Component.For<IJsonSerializer>().ImplementedBy<JilSerializer>());
            container.Register(Component.For<IMeetingNameFormatterFactory>().ImplementedBy<MeetingNameFormatterFactory>());

            container.Register(Component.For<IAdobeConnectUserService>().ImplementedBy<AdobeConnectUserService>());
            container.Register(Component.For<ISynchronizationUserService>().ImplementedBy<SynchronizationUserService>());
            container.Register(Component.For<IAdobeConnectAccountService>().ImplementedBy<AdobeConnectAccountService>());
            container.Register(Component.For<IRecordingsService>().ImplementedBy<RecordingsService>());
            container.Register(Component.For<IAudioProfilesService>().ImplementedBy<AudioProfilesService>());
            container.Register(Component.For<ISeminarService>().ImplementedBy<SeminarService>());

            container.Register(Component.For<TestConnectionService>().ImplementedBy<TestConnectionService>());
            container.Register(Component.For<LmsRoleService>().ImplementedBy<LmsRoleService>());
            container.Register(Component.For<LmsUserServiceBase>().ImplementedBy<ImsUserService>().Named("Ims"));
            
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
