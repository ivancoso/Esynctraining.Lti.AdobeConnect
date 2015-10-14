﻿using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using EdugameCloud.Lti.API;
using EdugameCloud.Lti.API.AdobeConnect;

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

            container.Register(Component.For<LmsFactory>().ImplementedBy<LmsFactory>());

            container.Register(Component.For<IAdobeConnectUserService>().ImplementedBy<AdobeConnectUserService>());
            container.Register(Component.For<ISynchronizationUserService>().ImplementedBy<SynchronizationUserService>());
            container.Register(Component.For<IAdobeConnectAccountService>().ImplementedBy<AdobeConnectAccountService>());
            container.Register(Component.For<IRecordingsService>().ImplementedBy<RecordingsService>());

            container.Register(Component.For<TestConnectionService>().ImplementedBy<TestConnectionService>());

            container.Register(Classes.FromAssemblyNamed("EdugameCloud.Lti").Pick()
                .If(Component.IsInNamespace("EdugameCloud.Lti.Controllers")).WithService.Self().LifestyleTransient());
        }

    }

}
