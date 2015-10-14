using System;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using EdugameCloud.Lti.API;
using EdugameCloud.Lti.Domain.Entities;

namespace EdugameCloud.Lti.BrainHoney
{
    public sealed class BrainHoneyWindsorInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.Register(Classes.FromThisAssembly().BasedOn(typeof(ILmsAPI)).WithServiceSelf().LifestyleTransient());

            container.Register(Component.For<EdugameCloud.Lti.API.BrainHoney.IBrainHoneyScheduling>().ImplementedBy<ShedulingHelper>());
            container.Register(Component.For<EdugameCloud.Lti.API.BrainHoney.IBrainHoneyApi>().ImplementedBy<DlapAPI>().Named("IBrainHoneyApi"));

            container.Register(Component.For<LmsUserServiceBase>().ImplementedBy<BrainHoneyLmsUserService>().Named(LmsProviderEnum.BrainHoney.ToString()));
        }

    }

}
